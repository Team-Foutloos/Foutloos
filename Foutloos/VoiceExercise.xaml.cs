using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Speech.Synthesis;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Foutloos
{
    /// <summary>
    /// Interaction logic for VoiceExercise.xaml
    /// </summary>
    public partial class VoiceExercise : Page
    {

        //Setting some variables that are used for the application.
        //Timer for displaying elapsed time and calculating the WPM/CPM
        DispatcherTimer timer = new DispatcherTimer();

        //The speech synthesizer for speaking the given sentences
        SpeechSynthesizer synthesizer;

        string dbString = "";
        int exerciseID;
        List<string> dbStringLeft;

        //The text displayed on the screen
        string typedText = "";

        //Bools for running (while the speech is being spoken) and
        //exerciseStarted (from first time typing till the sentence is completed).
        bool running = false;
        bool exerciseStarted = false;
        bool exerciseFinished = false;

        //Variable for the total amount of seconds that have elapsed
        int second;
        int secondOfLastLetter;

        string previousWord;

        //Variable for the amount of chars typed within a certain timespan.
        int typedKeys;
        int typedWords;

        //Array with all the speech speed levels
        double[] rateValues = { 0.5, 1, 1.25 };

        //Dictionary with all mistakes
        Dictionary<char, int> mistakes = new Dictionary<char, int>();

        //Making an array that saves the indexes of errors so they wont be counted twice
        List<int> mistakeIndex = new List<int>();

        double avgWPM;
        double avgCPM;

        //Add a list to save the wpm and time
        List<int> wpmTimeList = new List<int>() { 0 };
        List<int> cpmTimeList = new List<int>() { 0 };


        public VoiceExercise(string text, int exerciseID)
        {
            InitializeComponent();
            //Setting the exercise defaults
            this.dbString = text;
            this.exerciseID = exerciseID;
            this.dbStringLeft = dbString.Split(' ').ToList<string>();

            //Setting the speech synthesizer (system default text to speech object).
            synthesizer = new SpeechSynthesizer();
            //Adding an event to the synthesizer
            synthesizer.SpeakCompleted += new EventHandler<SpeakCompletedEventArgs>(synthesizer_SpeakCompleted);


            //Putting each voice/language installed on the users pc in a combobox for the user to select.
            foreach (InstalledVoice v in synthesizer.GetInstalledVoices())
            {
                comboVoice.Items.Add(v.VoiceInfo.Name + ". " + v.VoiceInfo.Culture);
            }
            //Setting the default value of the combobox
            comboVoice.SelectedIndex = 0;
            //Adding an event when a value has been selected in the combobox
            comboVoice.SelectionChanged += ComboVoice_SelectionChanged;

            //Adding the different speed levels in the app from the rateValues array
            foreach (double speed in rateValues)
            {
                comboRate.Items.Add($"{speed} X");
            }
            //Set the default selected speed
            comboRate.SelectedIndex = 1;

            //Setting the standard synthesizer speed to the default
            synthesizer.Rate = (int)(rateValues[comboRate.SelectedIndex] * 10) - 10;

            //Configuring the timer and adding an event
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;



        }

        //Adding a TextComposition event to the window.
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //Adding the Composition handler to get the users input at all times.
            var window = Window.GetWindow(this);
            window.TextInput += HandleTextComposition;
            //Keydown for the virtual keyboard
            window.KeyDown += Window_KeyDown;
            window.KeyUp += Window_KeyUp;
        }




        //This will be executed every time a key is pressed
        private void HandleTextComposition(object sender, TextCompositionEventArgs e)
        {
            //Creating a variable for the typed key
            Char keyChar;
            try
            {
                //Try getting the typed key into the char variable
                keyChar = (Char)System.Text.Encoding.ASCII.GetBytes(e.Text)[0];
            }
            catch
            {
                return;
            }

            if (keyChar == 9 || keyChar == 8)
            {
                return;
            }



            //If enter is pressed while the exercise isn't running yet the exercise will start
            if (keyChar == '\r' && !running && !exerciseFinished)
            {
                UIElement rootVisual = this.Content as UIElement;
                AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(rootVisual);
                if (rootVisual != null && adornerLayer != null)
                {
                    CustomTools.DarkenAdorner darkenAdorner = new CustomTools.DarkenAdorner(rootVisual);
                    adornerLayer.Add(darkenAdorner);

                    //Dialog will be opened when the user wan't to exit the exercise when it's not finished
                    Modals.Countdown countdown = new Modals.Countdown();
                    countdown.ShowDialog();
                    adornerLayer.Remove(darkenAdorner);
                }
                running = true;
                timer.Start();
                exerciseStarted = true;
                startSpeaking();

            }
            else if (exerciseStarted && !exerciseFinished && keyChar != '\r')
            {
                //Catching the backspace key and make it remove the last char from the variable wich holds the
                //written text. All other keys will be added to the same variable.
                string typedTextOld = typedText;

                if (typedText.Length < dbString.Length)
                {
                    typedText += e.Text;
                    typedKeys++;
                }


                //Displayig the typed text on the user's screen (this wil build up the whole sentence from scratch again everytime the text is updated)
                if (typedText.Length <= dbString.Length)
                {
                    ProgressBar.Maximum = dbString.Length;
                    ProgressBar.Value = 0;
                    //First clearing the textblock
                    inputText.Text = "";
                    //Setting a bool witch turns true when a typo was made by the user
                    bool wrong = false;


                    //Writing the text on the screen of the user char for char
                    for (int i = 0; i < typedText.Length; i++)
                    {


                        //If the text is correct it will be green. If there was a typo all text from
                        //the typo onwards will be red.
                        if ((typedText[i] == dbString[i] || ((dbString[i] == '\'' || dbString[i] == '`' || dbString[i] == '’') && (typedText[i] == '\'' || typedText[i] == '`' || typedText[i] == '’')) || ((dbString[i] == '"' || dbString[i] == '“' || dbString[i] == '”') && (typedText[i] == '"' || typedText[i] == '“' || typedText[i] == '”'))) && wrong == false)
                        {
                            secondOfLastLetter = second;



                            ProgressBar.Foreground = Brushes.DeepSkyBlue;
                            inputText.Inlines.Add(new Run(dbString[i].ToString()) { Foreground = Brushes.Green });
                            ProgressBar.Value++;
                        }
                        else if (keyChar != '\r')//Make sure an enter press doesn't count as an error (Enter is pressed to replay the speech)
                        {
                            ProgressBar.Foreground = Brushes.Red;
                            //Make sure a mistake isn't counted multiple times. 
                            if (!mistakeIndex.Contains(i) && !wrong)
                            {

                                //Check if mistake was made earlier (!wrong means it only gets the first char where the user goes wrong)
                                if (mistakes.ContainsKey(dbString[i]))
                                {
                                    //If the mistake was already made earlier the mistake will count up to the existing dictionary entry
                                    mistakes[dbString[i]]++;
                                }
                                else if (!mistakes.ContainsKey(dbString[i]))
                                {
                                    //If the mistake wasn't made earlier this wil add the mistake to the list with standard count 1   
                                    mistakes.Add(dbString[i], 1);
                                }
                                //Gettin the total amount of mistakes using a LINQ query
                                int mistakesNumber = mistakes.Values.Sum();
                                //Displaying the amount of errors on the screen
                                errorLable.Content = mistakesNumber;
                                //Adding the mistakes index so it wont be counted up when the text updates
                                mistakeIndex.Add(i);
                            }


                            typedText = typedTextOld;

                            //Flipping the wrong variable so all the text after the mistake is also shown in red
                            if (!wrong)
                            {
                                wrong = true;
                            }
                        }
                    }

                    if (!wrong && dbString.Length > typedText.Length && dbString[typedText.Length] == ' ')
                    {
                        startSpeaking();
                    }



                    //If the sentence is completed
                    if (typedText.Length == dbString.Length && !wrong)
                    {
                        //The timer stops
                        timer.Stop();
                        //With a LINQ query the amount of mistakes is calculated
                        int mistakesNumber = mistakes.Values.Sum();
                        //Some basic info will be displayed on the users screen
                        headLabel.Content = $"Done! Total time: {SecondsToTime(second)}\nNumber of mistakes: {mistakesNumber}";
                        //The progressbars color will be set to green
                        ProgressBar.Foreground = Brushes.Green;
                        exerciseFinished = true;


                        //This will add the results to the resultstable
                        if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["username"]))
                        {
                            //Make a new connectionclass
                            Connection c = new Connection();

                            //Getting the necessary IDs from the database
                            int userID = c.ID($"SELECT userID FROM Usertable WHERE username='{ConfigurationManager.AppSettings["username"]}'");
                            int resultID = 1;
                            resultID = (c.ID("SELECT Max(resultID) FROM Result")) + 1;
                            //The query to insert the result into the result table
                            string CmdString = $"INSERT INTO Result (resultID, mistakes, time, wpm, cpm, userID, exerciseID, speech) VALUES ({resultID}, {mistakesNumber}, {second}, {avgWPM}, {avgCPM}, {userID}, {exerciseID}, 1)";
                            //Executing the query
                            if (c.insertInto(CmdString))
                            {
                                //If the result has been added to the database, the Errors can be saved too
                                //For each keyValuePair in the dictionary the key will be added with the matching value
                                foreach (KeyValuePair<char, int> mistake in mistakes)
                                {
                                    //Getting the id for the new error
                                    int errorID = (c.ID("SELECT Max(errorID) FROM Error")) + 1;
                                    //Setting the query for adding the errors
                                    string insertMistakes = $"INSERT INTO Error (errorID, letter, count, resultID) VALUES ({errorID}, '{mistake.Key}', {mistake.Value}, {resultID})";
                                    //Inserting the error with the query above
                                    c.insertInto(insertMistakes);
                                }
                            }

                        }


                        //Show the results
                        UIElement rootVisual = this.Content as UIElement;
                        AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(rootVisual);
                        int wordspm;
                        int charspm;
                        double accuracy = ((((double)dbString.Length - (double)mistakesNumber) / (double)dbString.Length) * 100);



                        //If the seconds is higher then 0, divide by seconds.
                        if (second > 0)
                        {
                            wordspm = (typedWords * 60) / second;
                            charspm = (typedKeys * 60) / second;
                        }
                        else
                        {
                            wordspm = (typedWords * 60);
                            charspm = (typedKeys * 60);
                        }
                        Modals.ResultsAfterExercise results = new Modals.ResultsAfterExercise(wordspm, charspm, second, mistakesNumber, accuracy, cpmTimeList, wpmTimeList, mistakes, dbString, exerciseID);
                        if (rootVisual != null && adornerLayer != null)
                        {
                            CustomTools.DarkenAdorner darkenAdorner = new CustomTools.DarkenAdorner(rootVisual, 200);
                            adornerLayer.Add(darkenAdorner);
                            results.ShowDialog();
                        }
                    }
                }

            }


        }

        //Every second that the timer is enabled this will happen.
        private void Timer_Tick(object sender, EventArgs e)
        {

            //Adding a second every time the timer ticks
            second++;
            //Displaying the correct time to the user
            timeLable.Content = SecondsToTime(second);


            //Add wpm to the list
            wpmTimeList.Add((typedWords * 60) / second);

            //Add cpm to the list
            cpmTimeList.Add((typedKeys * 60) / second);

            //Calculating the typed keys per minute
            avgCPM = Math.Round((typedText.Length / (double)second) * 60);
            cpmLable.Content = avgCPM;

            //Calculate the amount of words and after calculating the average Words per minute
            string[] woorden = typedText.Split(' ');
            typedWords = woorden.Length;
            avgWPM = Math.Round((typedWords / (double)second) * 60);
            wpmLable.Content = avgWPM;

            //If the user hasn't typed the right letter in a while (three seconds) the correct word will be shown
            if (second > secondOfLastLetter + 3)
            {
                headLabel.Content = previousWord;
            }

        }



        //When calling this the speech will start playing and the exercise starts.
        private void startSpeaking()
        {
            try
            {
                //Saving the word to say
                string wordToSay = dbStringLeft[0];
                previousWord = wordToSay;
                //Removing the word from the list
                dbStringLeft.RemoveAt(0);

                //Start text to speech
                new Thread(() =>
                {

                    synthesizer.Speak(wordToSay);

                }).Start();
            }
            catch
            {

            }


        }

        //This is executed when the synthesiser is done.
        private void synthesizer_SpeakCompleted(object sender, SpeakCompletedEventArgs e)
        {
            //Speech isn't running anymore so setting it to false
            running = false;
            //Enabling the comboboxes again
            comboVoice.IsEnabled = true;
            comboRate.IsEnabled = true;

        }

        //Every time the volume is changed by the user this method runs.
        public void sliderVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                //Setting the volume of the speech to the selected value (User input sliderVolume)
                this.synthesizer.Volume = (int)sliderVolume.Value;
            }
            catch
            {
                Console.WriteLine("Something went wrong setting the volume");
            }
        }

        //Every time the voice is changed by the user this method runs.
        private void ComboVoice_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                //Setting the voice of the speech to the selected system voice. (User input comboVoice)
                synthesizer.SelectVoice(comboVoice.SelectedItem.ToString().Split('.')[0]);
            }
            catch
            {
                Console.WriteLine("Something went wrong setting the rate");
            }
            //After selecting a combobox the focus is on that box. Because of that typing doesn't work anymore.
            //To fix this a focus is set to the volume slider wich makes typing possible again.
            sliderVolume.Focus();
        }

        //Everytime the speed is changed by the user this method runs.
        private void ComboRate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                //Setting the speed of the speech to the selected value. (User input comboRate)
                this.synthesizer.Rate = (int)(rateValues[comboRate.SelectedIndex] * 10) - 10;
            }
            catch
            {
                Console.WriteLine("Something went wrong setting the rate");
            }
            //After selecting a combobox the focus is on that box. Because of that typing doesn't work anymore.
            //To fix this a focus is set to the volume slider wich makes typing possible again.
            sliderVolume.Focus();
        }

        //This method converts time in seconds to a radable miniute:second format
        private string SecondsToTime(int seconds)
        {
            //Converting the int seconds to a correct time notation
            TimeSpan result = TimeSpan.FromSeconds(seconds);
            //Writing the text to the users screen in the correct time notation
            return result.ToString("mm':'ss");
        }

        //Homebutton
        private void HomeBTN_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!exerciseFinished && exerciseStarted)
            {
                //Custom dialog will be opened when the user wan't to exit the exercise when it's not finishedUIElement rootVisual = this.Content as UIElement;
                UIElement rootVisual = this.Content as UIElement;
                AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(rootVisual);
                if (rootVisual != null && adornerLayer != null)
                {
                    CustomTools.DarkenAdorner darkenAdorner = new CustomTools.DarkenAdorner(rootVisual);
                    adornerLayer.Add(darkenAdorner);

                    //Dialog will be opened when the user wan't to exit the exercise when it's not finished
                    Modals.YesCancelModal result = new Modals.YesCancelModal(this.synthesizer);
                    result.ShowDialog();
                    adornerLayer.Remove(darkenAdorner);

                }
            }
            else
            {
                if (synthesizer.State == SynthesizerState.Speaking)
                {
                    synthesizer.Pause();
                }
                Application.Current.MainWindow.Content = new HomeScreen();
            }
        }


        //Virtual keyboard by Mark
        //Display pressed key on the virtual keyboard.
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {

            //Functionality Toggle
            //Check if toggle is true
            if (ToggleKeyboard.Toggled)
            {
                //Check which key is pressed
                if (e.Key == Key.D1)
                {
                    Key1_Back.Fill = Brushes.Black;
                }
                else if (e.Key == Key.D2)
                {
                    Key2_Back.Fill = Brushes.Black;
                }
                else if (e.Key == Key.D3)
                {
                    Key3_Back.Fill = Brushes.Black;
                }
                else if (e.Key == Key.D4)
                {
                    Key4_Back.Fill = Brushes.Black;
                }
                else if (e.Key == Key.D5)
                {
                    Key5_Back.Fill = Brushes.Black;
                }
                else if (e.Key == Key.D6)
                {
                    Key6_Back.Fill = Brushes.Black;
                }
                else if (e.Key == Key.D7)
                {
                    Key7_Back.Fill = Brushes.Black;
                }
                else if (e.Key == Key.D8)
                {
                    Key8_Back.Fill = Brushes.Black;
                }
                else if (e.Key == Key.D9)
                {
                    Key9_Back.Fill = Brushes.Black;
                }
                else if (e.Key == Key.D0)
                {
                    Key0_Back.Fill = Brushes.Black;
                }
                else if (e.Key == Key.OemMinus)
                {
                    KeyDash_Back.Fill = Brushes.Black;
                }
                else if (e.Key == Key.Q)
                {
                    Keyq_Back.Fill = Brushes.Black;
                }
                else if (e.Key == Key.W)
                {
                    Keyw_Back.Fill = Brushes.Black;
                }
                else if (e.Key == Key.E)
                {
                    Keye_Back.Fill = Brushes.Black;
                }
                else if (e.Key == Key.R)
                {
                    Keyr_Back.Fill = Brushes.Black;
                }
                else if (e.Key == Key.T)
                {
                    Keyt_Back.Fill = Brushes.Black;
                }
                else if (e.Key == Key.Y)
                {
                    Keyy_Back.Fill = Brushes.Black;
                }
                else if (e.Key == Key.U)
                {
                    Keyu_Back.Fill = Brushes.Black;
                }
                else if (e.Key == Key.I)
                {
                    Keyi_Back.Fill = Brushes.Black;
                }
                else if (e.Key == Key.O)
                {
                    Keyo_Back.Fill = Brushes.Black;
                }
                else if (e.Key == Key.P)
                {
                    Keyp_Back.Fill = Brushes.Black;
                }
                else if (e.Key == Key.A)
                {
                    Keya_Back.Fill = Brushes.Black;
                }
                else if (e.Key == Key.S)
                {
                    Keys_Back.Fill = Brushes.Black;
                }
                else if (e.Key == Key.D)
                {
                    Keyd_Back.Fill = Brushes.Black;
                }
                else if (e.Key == Key.F)
                {
                    Keyf_Back.Fill = Brushes.Black;
                }
                else if (e.Key == Key.G)
                {
                    Keyg_Back.Fill = Brushes.Black;
                }
                else if (e.Key == Key.H)
                {
                    Keyh_Back.Fill = Brushes.Black;
                }
                else if (e.Key == Key.J)
                {
                    Keyj_Back.Fill = Brushes.Black;
                }
                else if (e.Key == Key.K)
                {
                    Keyk_Back.Fill = Brushes.Black;
                }
                else if (e.Key == Key.L)
                {
                    Keyl_Back.Fill = Brushes.Black;
                }
                else if (e.Key == Key.Oem1)
                {
                    KeyColon_Back.Fill = Brushes.Black;
                }
                else if (e.Key == Key.Oem7)
                {
                    KeyAccolade_Back.Fill = Brushes.Black;
                }
                else if (e.Key == Key.LeftShift)
                {
                    KeyShift_Back.Fill = Brushes.Black;
                    Key1_Text.Text = "!";
                    Key2_Text.Text = "@";
                    Key3_Text.Text = "#";
                    Key4_Text.Text = "$";
                    Key5_Text.Text = "%";
                    Key6_Text.Text = "^";
                    Key7_Text.Text = "&";
                    Key8_Text.Text = "*";
                    Key9_Text.Text = "(";
                    Key0_Text.Text = ")";
                    KeyDash_Text.Text = "_";
                    KeyColon_Text.Text = ":";
                    KeyAccolade_Text.Text = "\"";
                    KeyComma_Text.Text = "<";
                    KeyDot_Text.Text = ">";
                    KeySlash_Text.Text = "?";
                }
                else if (e.Key == Key.Z)
                {
                    Keyz_Back.Fill = Brushes.Black;
                }
                else if (e.Key == Key.X)
                {
                    Keyx_Back.Fill = Brushes.Black;
                }
                else if (e.Key == Key.C)
                {
                    Keyc_Back.Fill = Brushes.Black;
                }
                else if (e.Key == Key.V)
                {
                    Keyv_Back.Fill = Brushes.Black;
                }
                else if (e.Key == Key.B)
                {
                    Keyb_Back.Fill = Brushes.Black;
                }
                else if (e.Key == Key.N)
                {
                    Keyn_Back.Fill = Brushes.Black;
                }
                else if (e.Key == Key.M)
                {
                    Keym_Back.Fill = Brushes.Black;
                }
                else if (e.Key == Key.OemComma)
                {
                    KeyComma_Back.Fill = Brushes.Black;
                }
                else if (e.Key == Key.OemPeriod)
                {
                    KeyDot_Back.Fill = Brushes.Black;
                }
                else if (e.Key == Key.Oem2)
                {
                    KeySlash_Back.Fill = Brushes.Black;
                }
                else if (e.Key == Key.LeftCtrl)
                {
                    KeyControl_Back.Fill = Brushes.Black;
                }
                else if (e.Key == Key.LeftAlt)
                {
                    KeyAlt_Back.Fill = Brushes.Black;
                }
                else if (e.Key == Key.Space)
                {
                    KeySpace_Back.Fill = Brushes.Black;
                }
            }
        }

        //Reset pressed key on virtual keyboard
        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            //Functionality Toggle
            //Check if toggle is true
            if (ToggleKeyboard.Toggled)
            {
                //Check which key is pressed
                if (e.Key == Key.D1)
                {
                    Key1_Back.Fill = Brushes.LightGray;
                }
                else if (e.Key == Key.D2)
                {
                    Key2_Back.Fill = Brushes.LightGray;
                }
                else if (e.Key == Key.D3)
                {
                    Key3_Back.Fill = Brushes.LightGray;
                }
                else if (e.Key == Key.D4)
                {
                    Key4_Back.Fill = Brushes.LightGray;
                }
                else if (e.Key == Key.D5)
                {
                    Key5_Back.Fill = Brushes.LightGray;
                }
                else if (e.Key == Key.D6)
                {
                    Key6_Back.Fill = Brushes.LightGray;
                }
                else if (e.Key == Key.D7)
                {
                    Key7_Back.Fill = Brushes.LightGray;
                }
                else if (e.Key == Key.D8)
                {
                    Key8_Back.Fill = Brushes.LightGray;
                }
                else if (e.Key == Key.D9)
                {
                    Key9_Back.Fill = Brushes.LightGray;
                }
                else if (e.Key == Key.D0)
                {
                    Key0_Back.Fill = Brushes.LightGray;
                }
                else if (e.Key == Key.OemMinus)
                {
                    KeyDash_Back.Fill = Brushes.LightGray;
                }
                else if (e.Key == Key.Q)
                {
                    Keyq_Back.Fill = Brushes.LightGray;
                }
                else if (e.Key == Key.W)
                {
                    Keyw_Back.Fill = Brushes.LightGray;
                }
                else if (e.Key == Key.E)
                {
                    Keye_Back.Fill = Brushes.LightGray;
                }
                else if (e.Key == Key.R)
                {
                    Keyr_Back.Fill = Brushes.LightGray;
                }
                else if (e.Key == Key.T)
                {
                    Keyt_Back.Fill = Brushes.LightGray;
                }
                else if (e.Key == Key.Y)
                {
                    Keyy_Back.Fill = Brushes.LightGray;
                }
                else if (e.Key == Key.U)
                {
                    Keyu_Back.Fill = Brushes.LightGray;
                }
                else if (e.Key == Key.I)
                {
                    Keyi_Back.Fill = Brushes.LightGray;
                }
                else if (e.Key == Key.O)
                {
                    Keyo_Back.Fill = Brushes.LightGray;
                }
                else if (e.Key == Key.P)
                {
                    Keyp_Back.Fill = Brushes.LightGray;
                }
                else if (e.Key == Key.A)
                {
                    Keya_Back.Fill = Brushes.LightGray;
                }
                else if (e.Key == Key.S)
                {
                    Keys_Back.Fill = Brushes.LightGray;
                }
                else if (e.Key == Key.D)
                {
                    Keyd_Back.Fill = Brushes.LightGray;
                }
                else if (e.Key == Key.F)
                {
                    Keyf_Back.Fill = Brushes.LightGray;
                }
                else if (e.Key == Key.G)
                {
                    Keyg_Back.Fill = Brushes.LightGray;
                }
                else if (e.Key == Key.H)
                {
                    Keyh_Back.Fill = Brushes.LightGray;
                }
                else if (e.Key == Key.J)
                {
                    Keyj_Back.Fill = Brushes.LightGray;
                }
                else if (e.Key == Key.K)
                {
                    Keyk_Back.Fill = Brushes.LightGray;
                }
                else if (e.Key == Key.L)
                {
                    Keyl_Back.Fill = Brushes.LightGray;
                }
                else if (e.Key == Key.Oem1)
                {
                    KeyColon_Back.Fill = Brushes.LightGray;
                }
                else if (e.Key == Key.Oem7)
                {
                    KeyAccolade_Back.Fill = Brushes.LightGray;
                }
                else if (e.Key == Key.LeftShift)
                {
                    KeyShift_Back.Fill = Brushes.LightGray;
                    Key1_Text.Text = "1";
                    Key2_Text.Text = "2";
                    Key3_Text.Text = "3";
                    Key4_Text.Text = "4";
                    Key5_Text.Text = "5";
                    Key6_Text.Text = "6";
                    Key7_Text.Text = "7";
                    Key8_Text.Text = "8";
                    Key9_Text.Text = "9";
                    Key0_Text.Text = "0";
                    KeyDash_Text.Text = "-";
                    KeyColon_Text.Text = ";";
                    KeyAccolade_Text.Text = "\'";
                    KeyComma_Text.Text = ",";
                    KeyDot_Text.Text = ".";
                    KeySlash_Text.Text = "/";
                }
                else if (e.Key == Key.Z)
                {
                    Keyz_Back.Fill = Brushes.LightGray;
                }
                else if (e.Key == Key.X)
                {
                    Keyx_Back.Fill = Brushes.LightGray;
                }
                else if (e.Key == Key.C)
                {
                    Keyc_Back.Fill = Brushes.LightGray;
                }
                else if (e.Key == Key.V)
                {
                    Keyv_Back.Fill = Brushes.LightGray;
                }
                else if (e.Key == Key.B)
                {
                    Keyb_Back.Fill = Brushes.LightGray;
                }
                else if (e.Key == Key.N)
                {
                    Keyn_Back.Fill = Brushes.LightGray;
                }
                else if (e.Key == Key.M)
                {
                    Keym_Back.Fill = Brushes.LightGray;
                }
                else if (e.Key == Key.OemComma)
                {
                    KeyComma_Back.Fill = Brushes.LightGray;
                }
                else if (e.Key == Key.OemPeriod)
                {
                    KeyDot_Back.Fill = Brushes.LightGray;
                }
                else if (e.Key == Key.Oem2)
                {
                    KeySlash_Back.Fill = Brushes.LightGray;
                }
                else if (e.Key == Key.LeftCtrl)
                {
                    KeyControl_Back.Fill = Brushes.LightGray;
                }
                else if (e.Key == Key.LeftAlt)
                {
                    KeyAlt_Back.Fill = Brushes.LightGray;
                }
                else if (e.Key == Key.Space)
                {
                    KeySpace_Back.Fill = Brushes.LightGray;
                }
            }
        }

        //Toggle the visibility of the keyboard
        private void ToggleKeyboard_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (ToggleKeyboard.Toggled)
            {
                VirtualKeyboard.Visibility = Visibility.Visible;
            }
            else
            {
                VirtualKeyboard.Visibility = Visibility.Hidden;
            }
        }
    }
}
