﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
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
    public partial class Exercise : Page
    {
        Connection c = new Connection();
        //Exercise text
        private string exerciseText = "Die latijnse tekst komt me echt de neus uit.";
        //String used to determine which characters are left in the exercise
        private string exerciseStringLeft;
        //String used to save users correct input
        private string userInputCorrect = "";
        //Boolean to determine if exercise is finished
        private bool exerciseFinished = false;
        //Dictionary used to save mistakes made by the user
        private Dictionary<char, int> userMistakes = new Dictionary<char, int>();
        //Boolean used to make sure a mistake isn't added multiples times in a row
        private bool mistake = false;
        //Marges used for the users input textbox
        private Thickness userInput_WithoutKeyboard = new Thickness(183, 352, 183, 0);
        private Thickness userInput_WithKeyboard = new Thickness(183, 452, 183, 0);
        //Timer for displaying elapsed time and calculating the WPM/CPM
        DispatcherTimer timer = new DispatcherTimer();
        //A countdown timer for generated exercises
        DispatcherTimer timedExcer = new DispatcherTimer();
        DispatcherTimer timedcheck = new DispatcherTimer();
        //Bool to check if the countdown must be enabeld
        private bool enabletimedExcer = false;
        private int replayTimeValue;

        //Variable for the amount seconds that needs to be countdown
        private int counter = 0;
        //Variable for the total amount of seconds that have elapsed
        private int seconds = 0;
        //Boolean used to determine if the exercise countdown is running
        private bool counterStarted = false;
        //Boolean used to determine if the timer is running
        private bool timerStarted = false;
        //Int to keep track of characters per minute
        private int cpm = 0;
        //Int to keep track of words per minute
        private int wpm = 0;
        //Int to keep track of mistakes made
        private int mistakes = 0;
        //Object for text to speech
        SpeechSynthesizer synthesizer = new SpeechSynthesizer();
        //Marges for the text to speech
        Thickness textToSpeechKeyboardOn = new Thickness(812, 395, 184, 355);
        Thickness textToSpeechKeyboardOff = new Thickness(812, 295, 184, 455);
        //Next word used for text to speech
        string exerciseNextWord = "";
        //Boolean for when text to speech is active
        bool textToSpeechActive = false;
        //Create a bool to see if the exercise is started.
        bool exerciseStarted = false;
        //Add a list to save the wpm and time
        List<int> wpmTimeList = new List<int>() { 0 };
        List<int> cpmTimeList = new List<int>() { 0 };
        //Boolean for spellchecking special characters
        bool specialCharacters;
        int exerciseID;

        bool firstTime = true;

        public Exercise(string text, bool sc, int exerciseID)
        {
            InitializeComponent();
            this.exerciseID = exerciseID;

            //Set users focus on the users inputbox
            UserInput_TextBox.Focus();

            //Set language of keyboard for exercise
            //int lang = InputLanguage.InstalledInputLanguages.IndexOf(InputLanguage.CurrentInputLanguage);

            //Show exercise text on the page
            this.exerciseText = text;
            exerciseStringLeft = text;

            //Save bool for special characters
            specialCharacters = sc;

            //Show special character instructions when enabled
            if (specialCharacters)
            {
                char first = exerciseStringLeft.First();
                if (first > 220 && first < 8200)
                {
                    SpecialChar.Visibility = Visibility.Visible;
                    SpecialChar.ChangeText(exerciseStringLeft.First());
                }
            }

            //Set progressbar maximum value
            ProgressBar.Maximum = exerciseText.Length;

            //Show keyboard
            UserInput_TextBox.Margin = userInput_WithKeyboard;
            overlayTextBox.Margin = userInput_WithKeyboard;
            Test.Visibility = Visibility.Visible;

            //Save remaining exercise text into variable
            exerciseStringLeft = exerciseText;
            //Visualize exercise text
            Exercise_TextBlock.Text = exerciseText;

            //Configuring the timer and adding an event
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;

            //Configuring the countdown and adding an event
            timedExcer.Interval = TimeSpan.FromSeconds(1);
            timedcheck.Interval = TimeSpan.FromMilliseconds(100);
            timedExcer.Tick += Countdown_Tick;
            timedcheck.Tick += Check_Tick;

            //Change text to speech toggle te be turned off by default
            ToggleSpeech.Toggled = false;
            ToggleSpeech.Dot.Margin = new Thickness(-39, 0, 0, 0);
            ToggleSpeech.Back.Fill = new SolidColorBrush(Color.FromRgb(160, 160, 160));

            //Save next word of an exercise
            string[] temp = exerciseStringLeft.Split(' ');
            exerciseNextWord = temp.First();

            //Set standard speed of text to speech
            synthesizer.Rate = 3;

            //Putting each voice/language installed on the users pc in a combobox for the user to select.
            foreach (InstalledVoice v in synthesizer.GetInstalledVoices())
            {
                Voice_ComboBox.Items.Add(v.VoiceInfo.Name + ". " + v.VoiceInfo.Culture);
            }
            //Setting the default value of the combobox
            Voice_ComboBox.SelectedIndex = 0;
            //Adding an event when a value has been selected in the combobox
            Voice_ComboBox.SelectionChanged += Voice_ComboBox_SelectionChanged;

            //Change text shown on screen
            TextToSpeech.Visibility = Visibility.Visible;
            TextToSpeech.Content = "";
        }
        private void UserInput_TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            //If the exercise is not started yet, show the countdown and start the exercise.
            if (!exerciseStarted)
            {
                UIElement rootVisual = this.Content as UIElement;
                AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(rootVisual);
                if (rootVisual != null && adornerLayer != null)
                {
                    CustomTools.DarkenAdorner darkenAdorner = new CustomTools.DarkenAdorner(rootVisual, 200);
                    adornerLayer.Add(darkenAdorner);

                    //Dialog will be opened when the user enters a key
                    Modals.Countdown countdown = new Modals.Countdown();
                    countdown.ShowDialog();
                    adornerLayer.Remove(darkenAdorner);
                    overlayTextBox.Visibility = Visibility.Hidden;

                    //Activates the countdown timer for gerenerated exercises
                    if (enabletimedExcer)
                    {
                        if (!counterStarted)
                        {
                            timedExcer.Start();
                            timedcheck.Start();
                            counterStarted = true;
                        }
                    }
                }
                //Turn the exercisStarted to true, so that when the user returns from the modal, the exersice starts.
                exerciseStarted = true;

                //Disable the key pressed
                e.Handled = true;

                //Start text to speech if the toggle is enabled
                if (ToggleSpeech.Toggled)
                {
                    //Start text to speech
                    new Thread(() =>
                    {
                        textToSpeechActive = true;
                        synthesizer.Speak(exerciseNextWord);
                        textToSpeechActive = false;
                    }).Start();

                    //Show text to speech label and combobox
                    TextToSpeech.Visibility = Visibility.Visible;
                    Voice_ComboBox.Visibility = Visibility.Visible;
                }
                else
                {
                    TextToSpeech.Visibility = Visibility.Hidden;
                }
                TextToSpeech.Content = "Press enter to replay speech!";
            }
            else
            {
                //Start timer
                if (!timerStarted)
                {
                    timer.Start();
                    timerStarted = true;
                }

                //Functionality Toggle Keyboard
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

                //Set user's cursor to the end of line
                UserInput_TextBox.CaretIndex = UserInput_TextBox.Text.Length;

                //Disable the use of backspace
                if (e.Key == Key.Back)
                {
                    e.Handled = true;
                }

                //Disable the use of enter
                if (e.Key == Key.Enter)
                {
                    //Start text to speech
                    if (!textToSpeechActive && !exerciseFinished)
                    {
                        new Thread(() =>
                        {
                            textToSpeechActive = true;
                            synthesizer.Speak(exerciseNextWord);
                            textToSpeechActive = false;
                        }).Start();
                    }
                    e.Handled = true;
                }

                //Check if the exercise is finished
                if (!exerciseFinished)
                {
                    //Check if the user pressed the spacebar
                    if (e.Key == Key.Space)
                    {
                        //Update characters per minute
                        cpm++;

                        //Check if the next character of the exercise is spacebar
                        if (exerciseStringLeft.First() == 32)
                        {
                            //Used for saving user's mistakes
                            mistake = false;

                            //Update words per minute
                            wpm++;

                            //Update variable with next word of the exercise
                            string[] temp = exerciseStringLeft.Split(' ');
                            //Remove space as first word in string array
                            temp = temp.Skip(1).ToArray();
                            exerciseNextWord = temp.First();
                            //Start speech if the toggle is true
                            if (ToggleSpeech.Toggled)
                            {
                                new Thread(() =>
                                {
                                    textToSpeechActive = true;
                                    synthesizer.Speak(exerciseNextWord);
                                    textToSpeechActive = false;
                                }).Start();
                            }

                            //Visualize correct input
                            Exercise_TextBlock.Text = "";
                            Exercise_TextBlock.Inlines.Add(new Run(userInputCorrect) { Foreground = Brushes.LightGray });
                            Exercise_TextBlock.Inlines.Add(new Run(exerciseStringLeft.First().ToString()) { Foreground = Brushes.LightGreen });

                            //Update variables
                            userInputCorrect += exerciseStringLeft.First().ToString();
                            exerciseStringLeft = exerciseStringLeft.Remove(0, 1);

                            //Add remaining exercise text if there is any
                            if (exerciseStringLeft.Length > 0)
                            {
                                Exercise_TextBlock.Inlines.Add(new Run(exerciseStringLeft.First().ToString()) { Background = Brushes.Yellow });

                                //Show special character instructions when enabled
                                if (specialCharacters)
                                {
                                    //Move special character instruction bubble
                                    Point location = UserInput_TextBox.GetRectFromCharacterIndex(UserInput_TextBox.CaretIndex).TopLeft;
                                    SpecialChar.Margin = new Thickness(location.X + 150, location.Y, 0, 0);
                                    
                                    //Check for spellcheck special characters
                                    char nextChar = exerciseStringLeft.First();
                                    if (nextChar == 8217)
                                    {
                                        nextChar = (char)39;
                                    }
                                    else if (nextChar == 8220 || nextChar == 8221)
                                    {
                                        nextChar = (char)34;
                                    }
                                    else if (nextChar == 8211)
                                    {
                                        nextChar = (char)45;
                                    }

                                    if (nextChar > 220 && specialCharacters)
                                    {
                                        SpecialChar.Visibility = Visibility.Visible;
                                        SpecialChar.ChangeText(exerciseStringLeft.First());
                                    }
                                    else
                                    {
                                        SpecialChar.Visibility = Visibility.Hidden;
                                    }
                                }

                                //Add remaining text
                                Exercise_TextBlock.Inlines.Add(new Run(exerciseStringLeft.Remove(0, 1)));
                            }

                            //Set progressbar value
                            ProgressBar.Value++;
                        }
                        else
                        {
                            //Disable incorrect input to be shown in user's inputbox
                            e.Handled = true;

                            //Check if the next character of the exercise was a mistake, by the user, before
                            if (!mistake)
                            {
                                //Update mistakes counter
                                mistakes++;

                                //Update dictionary containing user's mistakes
                                try
                                {
                                    userMistakes.Add((char)32, 1);
                                }
                                catch (Exception)
                                {
                                    userMistakes[(char)32] += 1;
                                }
                                mistake = true;
                            }

                            //Visualize incorrect input
                            Exercise_TextBlock.Text = "";
                            Exercise_TextBlock.Inlines.Add(new Run(userInputCorrect) { Foreground = Brushes.LightGray });
                            Exercise_TextBlock.Inlines.Add(new Run(exerciseStringLeft.First().ToString()) { Foreground = Brushes.Red, Background = Brushes.Yellow });
                            Exercise_TextBlock.Inlines.Add(new Run(exerciseStringLeft.Remove(0, 1)));
                        }
                    }
                }
                else
                {
                    //Show exercise in light green when it's finished
                    Exercise_TextBlock.Text = "";
                    Exercise_TextBlock.Inlines.Add(new Run(userInputCorrect) { Foreground = Brushes.LightGreen });
                    e.Handled = true;
                }
            }
        }

        private void UserInput_TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            //Check if exercise is finished
            if (!exerciseFinished)
            {
                //Update characters per minute
                cpm++;

                //Check for spellcheck special characters
                char nextChar = exerciseStringLeft.First();
                if (nextChar == 8217)
                {
                    nextChar = (char)39;
                }
                else if (nextChar == 8220 || nextChar == 8221)
                {
                    nextChar = (char)34;
                }
                else if (nextChar == 8211)
                {
                    nextChar = (char)45;
                }
                string nextString = e.Text;
                //Change users input based on the next character
                if (!specialCharacters)
                {
                    if (exerciseStringLeft.First() == 252 && nextString == "u")
                    {
                        nextString = "ü";
                    }
                    else if (exerciseStringLeft.First() == 232 && nextString == "e")
                    {
                        nextString = "è";
                    }
                    else if (exerciseStringLeft.First() == 228 && nextString == "a")
                    {
                        nextString = "ä";
                    }
                    else if (exerciseStringLeft.First() == 224 && nextString == "a")
                    {
                        nextString = "à";
                    }
                    else if (nextChar == 235 && nextString == "e")
                    {
                        nextString = "ë";
                    }
                    else if (exerciseStringLeft.First() == 233 && nextString == "e")
                    {
                        nextString = "é";
                    }
                    else if (exerciseStringLeft.First() == 239 && nextString == "i")
                    {
                        nextString = "ï";
                    }
                    else if (exerciseStringLeft.First() == 236 && nextString == "i")
                    {
                        nextString = "ì";
                    }
                    else if (exerciseStringLeft.First() == 246 && nextString == "o")
                    {
                        nextString = "ö";
                    }
                    else if (exerciseStringLeft.First() == 242 && nextString == "o")
                    {
                        nextString = "ò";
                    }
                    else if (exerciseStringLeft.First() == 249 && nextString == "u")
                    {
                        nextString = "ù";
                    }
                    else if (exerciseStringLeft.First() == 225 && nextString == "a")
                    {
                        nextString = "á";
                    }
                    else if (exerciseStringLeft.First() == 237 && nextString == "i")
                    {
                        nextString = "í";
                    }
                    else if (exerciseStringLeft.First() == 243 && nextString == "o")
                    {
                        nextString = "ó";
                    }
                    else if (exerciseStringLeft.First() == 250 && nextString == "u")
                    {
                        nextString = "ú";
                    }
                }

                //Check if the user's input is correct
                if (nextString == nextChar.ToString())
                {
                    //Used for saving user's mistakes
                    mistake = false;

                    //Visualize correct input
                    Exercise_TextBlock.Text = "";
                    Exercise_TextBlock.Inlines.Add(new Run(userInputCorrect) { Foreground = Brushes.LightGray });
                    Exercise_TextBlock.Inlines.Add(new Run(exerciseStringLeft.First().ToString()) { Foreground = Brushes.LightGreen });

                    //Update variables
                    userInputCorrect += exerciseStringLeft.First().ToString();
                    exerciseStringLeft = exerciseStringLeft.Remove(0, 1);

                    //Change users inputbox
                    UserInput_TextBox.Text = "";
                    UserInput_TextBox.Text = userInputCorrect;
                    UserInput_TextBox.CaretIndex = userInputCorrect.Length;
                    e.Handled = true;

                    //Add remaining exercise text if there is any
                    if (exerciseStringLeft.Length > 0)
                    {
                        Exercise_TextBlock.Inlines.Add(new Run(exerciseStringLeft.First().ToString()) { Background = Brushes.Yellow });
                        Exercise_TextBlock.Inlines.Add(new Run(exerciseStringLeft.Remove(0, 1)));

                        //Move special character instruction bubble
                        Point location = UserInput_TextBox.GetRectFromCharacterIndex(UserInput_TextBox.CaretIndex).TopLeft;
                        SpecialChar.Margin = new Thickness(location.X + 150, location.Y, 0, 0);

                        //Check for special character
                        char first = exerciseStringLeft.First();
                        if (first == 8217)
                        {
                            first = (char)39;
                        }
                        else if (first == 8220 || first == 8221)
                        {
                            first = (char)34;
                        }
                        else if(first == 8211)
                        {
                            first = (char)45;
                        }
                        if (first > 220 && specialCharacters)
                        {
                            SpecialChar.Visibility = Visibility.Visible;
                            SpecialChar.ChangeText(exerciseStringLeft.First());
                        }
                        else
                        {
                            SpecialChar.Visibility = Visibility.Hidden;
                        }
                    }

                    //Set progressbar value
                    ProgressBar.Value++;

                    //Check if the exercise is finished
                    if (exerciseStringLeft.Length == 0)
                    {
                        //Update words per minute
                        wpm++;

                        //Change exercise text when exercise is finished
                        timer.Stop();
                        exerciseFinished = true;
                        Exercise_TextBlock.Text = "";
                        Exercise_TextBlock.Inlines.Add(new Run(userInputCorrect) { Foreground = Brushes.LightGreen });
                        SpecialChar.Visibility = Visibility.Hidden;

                        //Hide text to speech element
                        TextToSpeech.Visibility = Visibility.Hidden;

                        //Change progressbar when the exercise is finished
                        ProgressBar.Foreground = Brushes.Green;

                        //Show the results
                        UIElement rootVisual = this.Content as UIElement;
                        AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(rootVisual);
                        int wordspm;
                        int charspm;
                        double accuracy = ((((double)exerciseText.Length - (double)mistakes) / (double)exerciseText.Length) * 100);

                        //If the seconds is higher then 0, divide by seconds.
                        if (seconds > 0)
                        {
                            wordspm = (wpm * 60) / seconds;
                            charspm = (cpm * 60) / seconds;
                        }
                        else
                        {
                            wordspm = (wpm * 60);
                            charspm = (cpm * 60);
                        }

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
                            string CmdString = $"INSERT INTO Result (resultID, mistakes, time, wpm, cpm, userID, exerciseID, speech) VALUES ({resultID}, {mistakes}, {seconds}, {wordspm}, {charspm}, {userID}, {exerciseID}, 0)";
                            //Executing the query
                            if (c.insertInto(CmdString))
                            {
                                //If the result has been added to the database, the Errors can be saved too
                                //For each keyValuePair in the dictionary the key will be added with the matching value
                                foreach (KeyValuePair<char, int> mistake in userMistakes)
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

                        Modals.ResultsAfterExercise results = new Modals.ResultsAfterExercise(wordspm, charspm, seconds, mistakes, accuracy, cpmTimeList, wpmTimeList, userMistakes, exerciseText, exerciseID, specialCharacters);
                        if (rootVisual != null && adornerLayer != null)
                        {
                            CustomTools.DarkenAdorner darkenAdorner = new CustomTools.DarkenAdorner(rootVisual, 200);
                            adornerLayer.Add(darkenAdorner);
                            results.ShowDialog();
                        }


                    }
                }
                else
                {
                    //Disable incorrect input to be shown in user's inputbox
                    e.Handled = true;

                    //Check if the next character of the exercise was a mistake, by the user, before
                    if (!mistake)
                    {
                        //Update mistake counter
                        mistakes++;

                        //Update dictionary containing user's mistakes
                        try
                        {
                            userMistakes.Add(exerciseStringLeft.First(), 1);
                        }
                        catch (Exception)
                        {
                            userMistakes[exerciseStringLeft.First()] += 1;
                        }
                        mistake = true;
                    }

                    //Check if the next character in the exercise is a space
                    if (exerciseStringLeft.First() == 32)
                    {
                        //Visualize incorrect input (spacebar)
                        Exercise_TextBlock.Text = "";
                        Exercise_TextBlock.Inlines.Add(new Run(userInputCorrect) { Foreground = Brushes.LightGray });
                        Exercise_TextBlock.Inlines.Add(new Run(exerciseStringLeft.First().ToString()) { Background = Brushes.Red });
                        Exercise_TextBlock.Inlines.Add(new Run(exerciseStringLeft.Remove(0, 1)));
                    }
                    else
                    {
                        //Visualize incorrect input (non-spacebar)
                        Exercise_TextBlock.Text = "";
                        Exercise_TextBlock.Inlines.Add(new Run(userInputCorrect) { Foreground = Brushes.LightGray });
                        Exercise_TextBlock.Inlines.Add(new Run(exerciseStringLeft.First().ToString()) { Foreground = Brushes.Red, Background = Brushes.Yellow });
                        Exercise_TextBlock.Inlines.Add(new Run(exerciseStringLeft.Remove(0, 1)));

                    }
                }
            }
        }

        private void UserInput_TextBox_PreviewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            //Disable copy/cut/paste commands in user's textbox
            if (e.Command == ApplicationCommands.Copy || e.Command == ApplicationCommands.Cut || e.Command == ApplicationCommands.Paste)
            {
                e.Handled = true;
            }
        }

        //Combobox for voice changing functionality
        private void Voice_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                //Setting the voice of the speech to the selected system voice. (User input comboVoice)
                synthesizer.SelectVoice(Voice_ComboBox.SelectedItem.ToString().Split('.')[0]);
            }
            catch (Exception) { }

            //Start text to speech
            if (!textToSpeechActive && !exerciseFinished && exerciseStarted)
            {
                new Thread(() =>
                {
                    textToSpeechActive = true;
                    synthesizer.Speak(exerciseNextWord);
                    textToSpeechActive = false;
                }).Start();
            }

            //Set focus to user textinput box
            UserInput_TextBox.Focus();
        }

        //Keyboard toggle functionality
        private void ToggleKeyboard_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (ToggleKeyboard.Toggled)
            {
                //Change user input box margin and text to speech margin
                UserInput_TextBox.Margin = userInput_WithKeyboard;
                overlayTextBox.Margin = userInput_WithKeyboard;
                TextToSpeech.Margin = textToSpeechKeyboardOn;
                Test.Visibility = Visibility.Visible;
            }
            else
            {
                //Hide keyboard
                UserInput_TextBox.Margin = userInput_WithoutKeyboard;
                overlayTextBox.Margin = userInput_WithoutKeyboard;
                TextToSpeech.Margin = textToSpeechKeyboardOff;
                Test.Visibility = Visibility.Hidden;
            }

            //Set focus to user textinput box
            UserInput_TextBox.Focus();
        }

        //Speech toggle functionality
        private void ToggleSpeech_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //Change label and combobox visibility based on the toggle and exercise progress
            if (exerciseStarted)
            {
                if (ToggleSpeech.Toggled)
                {
                    TextToSpeech.Visibility = Visibility.Visible;
                    Voice_ComboBox.Visibility = Visibility.Visible;

                    //Start text to speech
                    new Thread(() =>
                    {
                        textToSpeechActive = true;
                        synthesizer.Speak(exerciseNextWord);
                        textToSpeechActive = false;
                    }).Start();
                }
                else
                {
                    TextToSpeech.Visibility = Visibility.Hidden;
                    Voice_ComboBox.Visibility = Visibility.Hidden;
                }
            }
            else
            {
                if (ToggleSpeech.Toggled)
                {
                    Voice_ComboBox.Visibility = Visibility.Visible;
                }
                else
                {
                    Voice_ComboBox.Visibility = Visibility.Hidden;
                }
            }

            //Set focus to user textinput box
            UserInput_TextBox.Focus();
        }

        private void UserInput_TextBox_PreviewKeyUp(object sender, KeyEventArgs e)
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

        //Home button functionality
        private void FoutloosButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            timedExcer.Stop();
            timer.Stop();
            //If the exercise started or isnt finished, the user needs to be sure.
            if (!exerciseFinished && exerciseStarted)
            {
                UIElement rootVisual = this.Content as UIElement;
                AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(rootVisual);
                if (rootVisual != null && adornerLayer != null)
                {
                    CustomTools.DarkenAdorner darkenAdorner = new CustomTools.DarkenAdorner(rootVisual);
                    adornerLayer.Add(darkenAdorner);

                    //Dialog will be opened when the user wan't to exit the exercise when it's not finished
                    Modals.YesCancelModal result = new Modals.YesCancelModal();
                    result.ShowDialog();
                    adornerLayer.Remove(darkenAdorner);
                }
            }
            //If the exercise if finished or hasnt started yet, the user can cancel out of it at any time.
            else
            {
                Application.Current.MainWindow.Content = new HomeScreen();
            }
        }


        //sets the how long the generated excersise last
        public void SetCountdown(int amount)
        {
            replayTimeValue = amount;
            counter = amount;
            enabletimedExcer = true;
        }

        private void Check_Tick(object sender, EventArgs e)
        {
            //value that will determine when extra text is needed

            int kicker = 75;
            if (ProgressBar.Value % kicker == 0)
                firstTime = true;
            //adds more text if the kickers is reached
            if (ProgressBar.Value % kicker == 50 && firstTime.Equals(true))
            {
                firstTime = false;
                DataTable mostMistakes = new DataTable();
                DataTable dt0 = new DataTable();
                //Pulls a list of words based on the letters you did wrong the most
                mostMistakes = c.PullData("SELECT TOP 1 letter FROM Result R RIGHT JOIN Usertable U On R.userID = U.userID " +
                    $"JOIN Error E ON R.resultID = E.resultID WHERE username = '{ConfigurationManager.AppSettings["username"]}' AND letter NOT LIKE '% %' " +
                    $"GROUP BY letter ORDER BY SUM(count) DESC");
                dt0 = c.PullData($"SELECT * FROM dictionary WHERE list LIKE '%{mostMistakes.Rows[0]["letter"]}%'");
                Random rand = new Random();

                //fills the exerciseText with a set amount of text
                exerciseStringLeft += " ";
                for (int i = 0; i < 20; i++)
                {
                    exerciseStringLeft += dt0.Rows[rand.Next(0, dt0.Rows.Count)]["list"].ToString();
                    if (i != 19)
                    {
                        exerciseStringLeft += " ";
                    }
                }

                ProgressBar.Maximum += exerciseStringLeft.Count();
                Scroller.ScrollToVerticalOffset(exerciseStringLeft.Length);

            }
        }
        //Countdown Event for the generated excersise
        private void Countdown_Tick(object sender, EventArgs e)
        {
          
            
            counter--;
            if (counter <= 0)
            {
                

                //stops all timers
                timedExcer.Stop();
                timedcheck.Stop();
                timer.Stop();

                //Update words per minute
                wpm++;

                //Change exercise text when exercise is finished
                exerciseFinished = true;
                Exercise_TextBlock.Text = "";
                Exercise_TextBlock.Inlines.Add(new Run(userInputCorrect) { Foreground = Brushes.LightGreen });
                SpecialChar.Visibility = Visibility.Hidden;

                //Hide text to speech element
                TextToSpeech.Visibility = Visibility.Hidden;

                //Change progressbar when the exercise is finished
                ProgressBar.Foreground = Brushes.Green;

                //Show the results
                UIElement rootVisual = this.Content as UIElement;
                AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(rootVisual);
                int wordspm;
                int charspm;
                double accuracy = ((((double)exerciseText.Length - (double)mistakes) / (double)exerciseText.Length) * 100);

                //If the seconds is higher then 0, divide by seconds.
                if (seconds > 0)
                {
                    wordspm = (wpm * 60) / seconds;
                    charspm = (cpm * 60) / seconds;
                }
                else
                {
                    wordspm = (wpm * 60);
                    charspm = (cpm * 60);
                }

                //This will add the results to the resultstable
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["username"]))
                {
                   
                    //Getting the necessary IDs from the database
                    int userID = c.ID($"SELECT userID FROM Usertable WHERE username='{ConfigurationManager.AppSettings["username"]}'");
                    int resultID = 1;
                    resultID = (c.ID("SELECT Max(resultID) FROM Result")) + 1;
                    //The query to insert the result into the result table
                    string CmdString = $"INSERT INTO Result (resultID, mistakes, time, wpm, cpm, userID, exerciseID, speech) VALUES ({resultID}, {mistakes}, {seconds}, {wordspm}, {charspm}, {userID}, {exerciseID}, 0)";
                    //Executing the query
                    if (c.insertInto(CmdString))
                    {
                        //If the result has been added to the database, the Errors can be saved too
                        //For each keyValuePair in the dictionary the key will be added with the matching value
                        foreach (KeyValuePair<char, int> mistake in userMistakes)
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



                Modals.ResultsAfterExercise results = new Modals.ResultsAfterExercise(wordspm, charspm, seconds, mistakes, accuracy, cpmTimeList, wpmTimeList, userMistakes, exerciseText, exerciseID, enabletimedExcer, replayTimeValue);
                if (rootVisual != null && adornerLayer != null)
                {
                    CustomTools.DarkenAdorner darkenAdorner = new CustomTools.DarkenAdorner(rootVisual, 200);
                    adornerLayer.Add(darkenAdorner);
                    results.ShowDialog();
                }
            }

        }

        //Timer functionality
        private void Timer_Tick(object sender, EventArgs e)
        {
            //Add one second to the counter
            seconds++;

            //Change timer
            TimeSpan time = TimeSpan.FromSeconds(seconds);
            Time.Content = time.ToString("mm':'ss");

            //Update characters per minute
            CPM.Content = Convert.ToString((cpm * 60) / seconds);
            //Add cpm to the list
            cpmTimeList.Add((cpm * 60) / seconds);


            //Update words per minute
            WPM.Content = Convert.ToString((wpm * 60) / seconds);
            //Add wpm to the list
            wpmTimeList.Add((wpm * 60) / seconds);

            //Update mistake counter
            Error.Content = Convert.ToString(mistakes);
        }
    }
}