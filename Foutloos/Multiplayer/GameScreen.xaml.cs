using System;
using System.Configuration;
using System.Data;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Foutloos.Multiplayer
{
    /// <summary>
    /// Interaction logic for GameScreen.xaml
    /// </summary>
    public partial class GameScreen : Page
    {
        //Set basic variables
        string textToType = "";
        string typedText = "";
        int exerciseID;
        int roomID;
        bool done = false;
       
        Connection c = new Connection();

        DispatcherTimer timer = new DispatcherTimer();

        int timeMilliseconds;

        public GameScreen(int roomID, int exerciseID)
        {
            InitializeComponent();


            //Getting the text that the user has to type
            textToType = this.c.PullData($"SELECT sentence FROM RoomExercise WHERE roomID = {roomID} AND roomExerciseID = {exerciseID}").Rows[0][0].ToString();

            
            //Setting the exercise and the room id
            this.exerciseID = exerciseID;
            this.roomID = roomID;
            inputText.Inlines.Clear();
            for (int i = 0; i < textToType.Length; i++)
            {
                //Displaying the sentence with indicator where the user is
                if (i == 0)
                {
                    
                    inputText.Inlines.Add(new Run(textToType[i].ToString()) { Foreground = Brushes.Black, TextDecorations = TextDecorations.Underline });
                }
                else
                {
                    inputText.Inlines.Add(new Run(textToType[i].ToString()) { Foreground = Brushes.Black });
                }
            }


            //Get all players and scores from the current room
            Connection c = new Connection();
            DataTable dt = new DataTable();
            dt = c.PullData($"SELECT username, playerScore FROM Usertable U LEFT JOIN RoomPlayer RP ON U.userID = RP.userID WHERE U.userID IN (SELECT userID FROM RoomPlayer WHERE roomID = {roomID})");

            //Displaying all players in the current room
            foreach(DataRow s in dt.Rows)
            {
                ListViewItem lvi = new ListViewItem();
                lvi.Content = $"{s.Field<string>(0)} ({s.Field<int>(1)} pt)";
                lvi.Focusable = false;
                namesList.Items.Add(lvi);
            }

            if (exerciseID != 0)
            {
                //Setting the timer and starting it
                timer.Interval = TimeSpan.FromMilliseconds(1);
                timer.Tick += Timer_Elapsed;
                timer.Start();
            }

        }

        private void Timer_Elapsed(object sender, EventArgs e)
        {
            //For every tick increase the time and update the timer textblock
            if (!done)
            {
                timeMilliseconds++;
                timerTextBlock.Text = millisecondsToTime(timeMilliseconds);
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //Adding the textcomposition event
            var window = Window.GetWindow(this);
            window.TextInput += Window_TextInput;

            //Show countdown overlay if the exerciseID == 0
            if (exerciseID == 0)
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

                    //Setting the timer and starting it
                    timer.Interval = TimeSpan.FromMilliseconds(1);
                    timer.Tick += Timer_Elapsed;
                    timer.Start();
                }
            }
        }

        private void Window_TextInput(object sender, TextCompositionEventArgs e)
        {
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

            //Storing the old text in case the textblock has to be reset
            string oldText = typedText;
            //Adding the typed char to typed text
            typedText += keyChar;
            bool focussedLetter = true;

            //Checking if the typed letter is correct
            if(!done && textToType[typedText.Length - 1] == keyChar)
            {
                //Clear the textblock
                inputText.Inlines.Clear();

                //Print the string letter for letter
                for (int i = 0; i < textToType.Length; i++)
                {


                    if (typedText.Length > i && textToType[i] == typedText[i])
                    {
                        //Typed letters hget green
                        inputText.Inlines.Add(new Run(textToType[i].ToString()) { Foreground = Brushes.Green });
                    }
                    else
                    {
                        //Letters that have to be typed
                        if (focussedLetter)
                        {
                            //Letter that has to be typed next (gets underlined)
                            inputText.Inlines.Add(new Run(textToType[i].ToString()) { Foreground = Brushes.Black, TextDecorations = TextDecorations.Underline });
                            focussedLetter = false;
                        }
                        else
                        {
                            //all other letters that have to be typed are just black
                            inputText.Inlines.Add(new Run(textToType[i].ToString()) { Foreground = Brushes.Black });
                        }

                    }
                }

                //If the sentence is correct
                if(textToType.Length == typedText.Length)
                {
                    //Stop the timer and display the time it took to fisnish the sentence
                    timer.Stop();
                    doneTextBlock.Inlines.Add($" {millisecondsToTime(timeMilliseconds)}");
                    doneTextBlock.Visibility = Visibility.Visible;
                    inputText.Opacity = 0.6;
                    done = true;
                    //Get the needed ids
                    int roomResultID = c.ID("SELECT MAX(roomResultID) FROM RoomResult") + 1;
                    int userID = int.Parse(ConfigurationManager.AppSettings.Get("userID"));
                    c.insertInto($"INSERT INTO RoomResult (roomResultID, roomExerciseID, roomID, userID, time) VALUES ({roomResultID}, {this.exerciseID}, {this.roomID}, {userID}, {timeMilliseconds})");
                    //Start a thread which checks every half second if everyone is done
                    //If everyone is done everyone will simultanously be lead to the scoreboard
                    new Thread(() =>
                    {
                        while (true)
                        {
                            //Getting the neccesary ids
                            int playerCount = c.ID($"SELECT COUNT(*) FROM RoomPlayer WHERE roomID = {this.roomID}");
                            int playersDone = c.ID($"SELECT COUNT(*) FROM RoomResult WHERE roomID = {this.roomID} AND roomExerciseID = {this.exerciseID}");

                            //If everyone is done
                            if (playerCount == playersDone)
                            {
                                //The user will be transfered to the scoreboard
                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    Application.Current.MainWindow.Content = new ScoreboardScreen(this.roomID, this.exerciseID);
                                    
                                });
                                //The current thread will be closed
                                Thread.CurrentThread.Abort();
                            }
                            //Sleep for 500 milliseconds so the database won't be overloaded
                            Thread.Sleep(500);
                        }
                        
                    }).Start();
                }

            }
            else
            {
                //Set typed text to old variable
                typedText = oldText;
            }

            

        }

        //This method converts time in seconds to a radable miniute:second format
        private string millisecondsToTime(int milliseconds)
        {
            //Converting the int seconds to a correct time notation
            TimeSpan result = TimeSpan.FromMilliseconds(milliseconds*10);
            //Writing the text to the users screen in the correct time notation
            return result.ToString("ss':'fff");
        }
    }
}
