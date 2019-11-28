using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Speech.Synthesis;
using System.Timers;
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

        string dbString = "This is the end. Hold your breath and count to ten.";

        //The text displayed on the screen
        string typedText = "";

        //Bools for running (while the speech is being spoken) and
        //exerciseStarted (from first time typing till the sentence is completed).
        bool running = false;
        bool exerciseStarted = false;
        bool exerciseFinished = false;

        //Variable for the total amount of seconds that have elapsed
        int second;
        //Variable for the amount of chars typed within a certain timespan.
        int typedKeys;

        //Array with all the speech speed levels
        double[] rateValues = { 0.5, 1, 1.25 };

        //Dictionary with all mistakes
        Dictionary<char, int> mistakes = new Dictionary<char, int>();

        //Making an array that saves the indexes of errors so they wont be counted twice
        List<int> mistakeIndex = new List<int>();
        

        public VoiceExercise()
        {
            InitializeComponent();

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
            foreach(double speed in rateValues)
            {
                comboRate.Items.Add(speed);
            }
            //Set the default selected speed
            comboRate.SelectedIndex = 1;

            //Setting the standard synthesizer speed to the default
            synthesizer.Rate = (int)(rateValues[comboRate.SelectedIndex] * 10) - 15;

            //Configuring the timer and adding an event
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            
            
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

            //If enter is pressed while the exercise isn't running yet the exercise will start
            if (keyChar == '\r' && !running && !exerciseFinished)
            {
                startSpeaking();
                running = true;
                headLabel.Content = "Press enter to replay";
            }
            else if(exerciseStarted && !exerciseFinished && keyChar != '\r')
            {
                //Catching the backspace key and make it remove the last char from the variable wich holds the
                //written text. All other keys will be added to the same variable.
                if (keyChar.Equals('\b'))
                {
                    if (typedText.Length > 0)
                    {
                        typedText = typedText.Remove(typedText.Length - 1);
                    }
                }
                else
                {
                    if (typedText.Length < dbString.Length)
                    {
                        typedText += e.Text;
                        typedKeys++;
                    }
                }

                
                //Displayig the typed text on the user's screen (this wil build up the whole sentence from scratch again everytime the text is updated)
                if (typedText.Length <= dbString.Length)
                {
                    //First clearing the textblock
                    inputText.Text = "";
                    //Setting a bool witch turns true when a typo was made by the user
                    bool wrong = false;

                    //Writing the text on the screen of the user char for char
                    for (int i = 0; i < typedText.Length; i++)
                    {
                        //If the text is correct it will be green. If there was a typo all text from
                        //the typo onwards will be red.
                        if (typedText[i] == dbString[i] && wrong == false)
                        {
                            inputText.Inlines.Add(new Run(typedText[i].ToString()) { Foreground = Brushes.Green });
                        }
                        else if(keyChar != '\r')//Make sure an enter press doesn't count as an error (Enter is pressed to replay the speech)
                        {

                            //Make sure a mistake isn't counted multiple times. 
                            if(!mistakeIndex.Contains(i) && !wrong)
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
                            
                            //Making spaces in the wrong part of the text red underscores for better visibility
                            string wrongChar = typedText[i].ToString().Replace(' ', '_');
                            //Adding the wrong char to the text
                            inputText.Inlines.Add(new Run(wrongChar) { Foreground = Brushes.Red });

                            //Flipping the wrong variable so all the text after the mistake is also shown in red
                            if (!wrong)
                            {
                                wrong = true;
                            }
                        }
                    }



                    //If the sentence is completed
                    if (typedText.Length == dbString.Length && !wrong)
                    {
                        timer.Stop();
                        int mistakesNumber = mistakes.Values.Sum();
                        headLabel.Content = $"Done! Total time: {SecondsToTime(second)}\nNumber of mistakes: {mistakesNumber}";
                        exerciseFinished = true;
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

            //Calculating the typed keys per minute
            cpmLable.Content = Math.Round((typedKeys / (double)second) * 60);

            string[] woorden = typedText.Split(' ');
            wpmLable.Content = Math.Round((woorden.Length / (double)second) * 60);
            
        }

        //Adding a TextComposition event to the window.
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //Adding the Composition handler to get the users input at all times.
            var window = Window.GetWindow(this);
            window.TextInput += HandleTextComposition;
        }

        //When calling this the speech will start playing and the exercise starts.
        private void startSpeaking()
        {
            //Start the time if the exersice didn't begin yet.
            if (!exerciseStarted)
            {
                timer.Start();
                exerciseStarted = true;
            }


            //Disabling the comboboxes so no changes can be made while speeking.
            comboVoice.IsEnabled = false;
            comboRate.IsEnabled = false;

            //Starting the system speech
            switch (synthesizer.State)
            {
                //if synthesizer is ready
                case SynthesizerState.Ready:
                    synthesizer.SpeakAsync(dbString);
                    break;

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
                this.synthesizer.Volume = (int)sliderVolume.Value * 10;
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
                this.synthesizer.Rate = (int)(rateValues[comboRate.SelectedIndex] * 10) - 15;
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
            //If the speech is still playing, stopping the speech.
            if (synthesizer.State == SynthesizerState.Speaking)
            {
                synthesizer.Pause();
            }

            //Navigate back to the homescreen.
            HomeScreen.owner.Content = new HomeScreen(HomeScreen.owner);
        }
    }
}
