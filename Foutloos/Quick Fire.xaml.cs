using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using WpfAnimatedGif;

namespace Foutloos
{
    public partial class Quick_Fire : Page
    {
        //Exercise text
        private string exerciseText = "alpha bravo charlie delta echo foxtrot golf hotel india juliett kilo lima mike november oscar papa quebec romeo sierra tango uniform victor whiskey x-ray yankee zulu";
        //Exercise words left
        private Queue<String> exerciseQueueTextLeft = new Queue<string>();
        //Next word used for text to speech
        string exerciseNextWord = "";
        //Next word correct
        string exerciseNextWordCorrect = "";
        //Next word to be typed
        string exerciseNextWordLeft = "";
        //Game timer
        DispatcherTimer timer = new DispatcherTimer();
        //Create a bool to see if the exercise is started.
        bool exerciseStarted = false;
        //Boolean to determine if exercise is finished
        private bool exerciseFinished = false;
        //Variable for keeping track of the amount of words in an exercise and how many of them are finished
        int exerciseWordAmount;
        int exerciseWordAmountFinished = 0;
        //Boolean used for streak counter + counters for current and highest streak
        bool streak = true;
        //Boolean for when player can type
        bool canType = true;
        int greenValue = 255;
        int redValue = 0;
        int streakCounter = 0;
        int topStreak = 0;
        int correctCounter = 0;

        //TODO: add difficulty screen
        int difficultyReading;
        int difficultyTyping;
        //TODO: more time for longer words
        int wordlength;

        //animation stuff
        TranslateTransform trans;
        BitmapImage DrivingImage;
        BitmapImage CrashingImage;
        BitmapImage SafeImage;
        
        

        public Quick_Fire()
        {
            InitializeComponent();
            try
            {   // load gifs to change CarPicture later
                DrivingImage = new BitmapImage();
                DrivingImage.BeginInit();
                DrivingImage.UriSource = new Uri(@"/assets/Car.gif", UriKind.RelativeOrAbsolute);
                DrivingImage.EndInit();

                CrashingImage = new BitmapImage();
                CrashingImage.BeginInit();
                CrashingImage.UriSource = new Uri(@"/assets/ExplodingCar.gif", UriKind.RelativeOrAbsolute);
                CrashingImage.EndInit();

                SafeImage = new BitmapImage();
                SafeImage.BeginInit();
                SafeImage.UriSource = new Uri(@"/assets/testimage.gif", UriKind.RelativeOrAbsolute);
                SafeImage.EndInit();
            } catch (Exception)
            {
                System.Windows.Forms.MessageBox.Show("gif loading failed");
            }
            
            //Save every word of exercise in a queue
            string[] e = exerciseText.Split(' ');
            foreach (string s in e)
            {
                exerciseQueueTextLeft.Enqueue(s);
            }

            //Save first word in a variable
            exerciseNextWord = exerciseQueueTextLeft.Dequeue();
            exerciseNextWordLeft = exerciseNextWord;
            ExerciseWord_TextBlock.Text = exerciseNextWord;

            //Configuring the timer and adding an event
            timer.Interval = TimeSpan.FromSeconds(0.1);
            timer.Tick += OnTimedEvent;

            //Save amount of words of an exercise in a variable and set corresponding label
            exerciseWordAmount = e.Length;
            ExerciseWordCounter_Label.Content = $"{exerciseWordAmountFinished}/{exerciseWordAmount}";

            //Set progressbar maximum value
            int counter = 0;
            foreach (string s in e)
            {

                counter++;

            }
            ProgressBar.Maximum = counter;

            
            wordlength = e[0].Length;
        }

        private void FoutloosButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //Home button functionality
            Application.Current.MainWindow.Content = new HomeScreen();
        }

        private void OnTimedEvent(object sender, EventArgs e)
        {
            if (canType)
            {
                //typing flow
                if (TimeleftBar.Value > 0)
                {
                    TimeleftBar.Value -= 60 * difficultyTyping / (1 + wordlength / 1);//if changed also change the animation speed

                    if (TimeleftBar.Value > 90)
                    {
                        greenValue = 255;
                        redValue = (int)Math.Floor(255 - (TimeleftBar.Value / TimeleftBar.Maximum * 255 * 2));
                    }
                    else
                    {
                        greenValue = (int)Math.Floor(TimeleftBar.Value / TimeleftBar.Maximum * 255 * 2);
                    }
                    TimeleftBar.Foreground = new SolidColorBrush(Color.FromArgb(0xFF, (byte)redValue, (byte)greenValue, 0x00));
                }
                else
                {
                    if (TimeleftBar.Value != 0)
                        TimeleftBar.Value = 0;
                    canType = false;

                    //TODO: mistakes++
                    nextWord(false);

                }
            }
            else
            {
                //reading flow
                if (TimeleftBar.Foreground != Brushes.Gray)
                    TimeleftBar.Foreground = Brushes.Gray;
                if (TimeleftBar.Value < 180)
                    TimeleftBar.Value += 60 * difficultyReading / (4 + wordlength/8 );
                else
                {
                    if (TimeleftBar.Value != 180)
                        TimeleftBar.Value = 180;
                    canType = true;

                    //calculate animation speed, the + 0.2 is the first and the final 100'th millisecond
                    double animationspeed = 0.2 + 0.1 * Math.Ceiling( (double)(180 / (60 * difficultyTyping / (1 + wordlength / 1))) );

                    //animates image
                    ImageBehavior.SetAnimatedSource(CarPicture, DrivingImage);
                    ImageBehavior.SetRepeatBehavior(CarPicture, RepeatBehavior.Forever);

                    Vector offset = VisualTreeHelper.GetOffset(CarPicture);
                    var left = offset.X;
                    trans = new TranslateTransform();
                    CarPicture.RenderTransform = trans;
                    DoubleAnimation anim1 = new DoubleAnimation(0,230-left,TimeSpan.FromSeconds(animationspeed));
                    trans.BeginAnimation(TranslateTransform.XProperty, anim1);
                    
                    
                    //adds typing function from user
                    var window = Window.GetWindow(this);
                    window.TextInput += HandleTextComposition;
                }

            }




            //Stop timer when exercise is finished and update progressbar colour
            if (exerciseFinished)
            {
                timer.Stop();
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //Adding the Composition handler to get the users input at all times.
            var window = Window.GetWindow(this);
            window.TextInput += HandleTextComposition;
        }

        private void HandleTextComposition(object sender, TextCompositionEventArgs e)
        {
            //Enable timer when key is pressed
            if (!timer.IsEnabled)
            {
                timer.Start();
            }

            //Check for input when exercise is not finished
            if (!exerciseFinished)
            {
                if (e.Text == exerciseNextWordLeft.First().ToString())
                { //Users input is correct

                    ExerciseWord_TextBlock.Text = "";
                    ExerciseWord_TextBlock.Inlines.Add(new Run(exerciseNextWordCorrect) { Foreground = Brushes.LightGray });

                    exerciseNextWordCorrect += exerciseNextWordLeft.First();
                    exerciseNextWordLeft = exerciseNextWordLeft.Remove(0, 1);

                    ExerciseWord_TextBlock.Inlines.Add(new Run(exerciseNextWordCorrect.Last().ToString()) { Foreground = Brushes.LightGreen });
                    ExerciseWord_TextBlock.Inlines.Add(new Run(exerciseNextWordLeft));

                    //Check if the current word is fully typed
                    if (exerciseNextWordLeft.Length == 0)
                        nextWord(true);

                }
                else
                    nextWord(false);

            }
        }
        private void nextWord(bool TypedCorrectInTime)
        {
            

            //puts timerbased flow back to read-only
            canType = false;
            TimeleftBar.Value = 0;

            //removes typing function from user
            var window = Window.GetWindow(this);
            window.TextInput -= HandleTextComposition;

            //Update progressbar
            ProgressBar.Value++;


            if (TypedCorrectInTime)
            {
                ImageBehavior.SetAnimatedSource(CarPicture, SafeImage);
                ImageBehavior.SetRepeatBehavior(CarPicture, RepeatBehavior.Forever);
                correctCounter++;
                streakCounter++;
                if (topStreak < streakCounter)
                    topStreak = streakCounter;

                StreakCounter_Label.Content = streakCounter;
            }
            else //if you failed typing the word
            {
                //reset streak
                streakCounter = 0;
                StreakCounter_Label.Content = streakCounter;
                ImageBehavior.SetAnimatedSource(CarPicture, CrashingImage);
                ImageBehavior.SetRepeatBehavior(CarPicture, new RepeatBehavior(1));
            }

            //GENERAL CODE FOR NEXT WORD
            
            //Update word counter
            exerciseWordAmountFinished++;
            ExerciseWordCounter_Label.Content = $"{correctCounter}/{exerciseWordAmountFinished}";

            //Check if there are any words left in the exercise
            if (exerciseQueueTextLeft.Count > 0)
            {
                exerciseNextWord = exerciseQueueTextLeft.Dequeue();
                wordlength = exerciseNextWord.Length;
                exerciseNextWordCorrect = "";
                exerciseNextWordLeft = exerciseNextWord;
                ExerciseWord_TextBlock.Text = exerciseNextWord;
                
            }
            else
            {
                //Exercise is finished when there are no more words left
                exerciseFinished = true;
                ProgressBar.Foreground = Brushes.Green;
            }
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            difficultyTyping = 1;
        }

        private void RadioButton_Checked_1(object sender, RoutedEventArgs e)
        {
            difficultyTyping = 2;
        }

        private void RadioButton_Checked_2(object sender, RoutedEventArgs e)
        {
            difficultyTyping = 4;
        }

        private void RadioButton_Checked_3(object sender, RoutedEventArgs e)
        {
            difficultyReading = 1;
        }

        private void RadioButton_Checked_4(object sender, RoutedEventArgs e)
        {
            difficultyReading = 2;
        }

        private void RadioButton_Checked_5(object sender, RoutedEventArgs e)
        {
            difficultyReading = 4;
        }
    }
}