using System;
using System.Diagnostics;
using System.Speech.Synthesis;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace Foutloos
{
    /// <summary>
    /// Interaction logic for VoiceExercise.xaml
    /// </summary>
    public partial class VoiceExercise : Page
    {
        SpeechSynthesizer synthesizer;
        Stopwatch stopwatch = new Stopwatch();
        Timer timer = new Timer();
        string testString = "This is the end. Hold your breath and count to ten.";
        string typedText = "";
        bool running = false;
        bool exercisStarted = false;


        public VoiceExercise()
        {
            InitializeComponent();

            synthesizer = new SpeechSynthesizer();
            synthesizer.SpeakCompleted += new EventHandler<SpeakCompletedEventArgs>(synthesizer_SpeakCompleted);
            timer.Interval = 1000;
            timer.Elapsed += Timer_Elapsed;

            foreach (InstalledVoice v in synthesizer.GetInstalledVoices())
            {
                comboVoice.Items.Add(v.VoiceInfo.Name + ". " + v.VoiceInfo.Culture);
            }

            comboRate.Items.Add("0,5");
            comboRate.Items.Add("1");
            comboRate.Items.Add("1,25");
            comboRate.SelectedIndex = 1;

            
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            timeLable.Content = stopwatch.Elapsed.ToString();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            window.KeyDown += Window_KeyDown; ;
            window.TextInput += HandleTextComposition;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
        }

        private void HandleTextComposition(object sender, TextCompositionEventArgs e)
        {
            Char keyChar;
            try
            {
                keyChar = (Char)System.Text.Encoding.ASCII.GetBytes(e.Text)[0];
            }
            catch
            {
                return;
            }
            
            if (keyChar == '\r' && !running)
            {
                startSpeaking();
                running = true;
                headLabel.Content = "Press enter to replay";
            }
            else
            {

               if(keyChar.Equals('\b'))
               {
                    if(typedText.Length > 0)
                    {
                        typedText = typedText.Remove(typedText.Length - 1);
                    }
                    
               }
               else
               {
                    if(typedText.Length < testString.Length)
                    {
                        typedText += e.Text;
                    }
                        
               }
                if (typedText.Length <= testString.Length)
                {
                    inputText.Text = "";
                    bool wrong = false;
                    for (int i = 0; i < typedText.Length; i++)
                    {
                        if (typedText[i] == testString[i] && wrong == false)
                        {
                            inputText.Inlines.Add(new Run(typedText[i].ToString()) { Foreground = Brushes.Green });
                        }
                        else
                        {
                            inputText.Inlines.Add(new Run(typedText[i].ToString()) { Foreground = Brushes.Red });
                            wrong = true;
                        }
                    }
                }

            }

        }

        private void synthesizer_SpeakCompleted(object sender, SpeakCompletedEventArgs e)
        {
            running = false;
            comboVoice.IsEnabled = true;
            comboRate.IsEnabled = true;
        }

        private void startSpeaking()
        {
            if (!exercisStarted)
            {
                timer.Start();
                stopwatch.Start();
                exercisStarted = true;
            }
                

            comboVoice.IsEnabled = false;
            comboRate.IsEnabled = false;
            if (comboVoice.SelectedItem != null)
                synthesizer.SelectVoice(comboVoice.SelectedItem.ToString().Split('.')[0]);
            synthesizer.Volume = Convert.ToInt32(sliderVolume.Value * 10);
            //synthesizer.Rate = (int)((double.Parse(comboRate.SelectedItem.ToString()) * 10) - 15);
            synthesizer.SelectVoiceByHints(VoiceGender.Male, VoiceAge.Senior);
            switch (synthesizer.State)
            {
                //if synthesizer is ready
                case SynthesizerState.Ready:
                    synthesizer.SpeakAsync(testString);
                    break;
                
            }
        }

        public void sliderVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                this.synthesizer.Volume = (int)sliderVolume.Value * 10;
            }
            catch
            {
                Console.WriteLine("Something went wrong setting the volume");
            }
        }

        private void ComboRate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                this.synthesizer.Rate = (int)comboRate.SelectedItem * 10 - 10;
            }
            catch
            {
                Console.WriteLine("Something went wrong setting the rate");
            }

            
        }

    }
}
