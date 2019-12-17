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

           textToType = this.c.PullData($"SELECT sentence FROM RoomExercise WHERE roomID = {roomID} AND roomExerciseID = {exerciseID}").Rows[0][0].ToString();

            

            this.exerciseID = exerciseID;
            this.roomID = roomID;
            inputText.Inlines.Clear();
            for (int i = 0; i < textToType.Length; i++)
            {
                if (i == 0)
                {
                    inputText.Inlines.Add(new Run(textToType[i].ToString()) { Foreground = Brushes.Black, TextDecorations = TextDecorations.Underline });
                }
                else
                {
                    inputText.Inlines.Add(new Run(textToType[i].ToString()) { Foreground = Brushes.Black });
                }
            }

            timer.Interval = TimeSpan.FromMilliseconds(1);
            timer.Tick += Timer_Elapsed;
            timer.Start();

            Connection c = new Connection();
            DataTable dt = new DataTable();
            dt = c.PullData($"SELECT username, playerScore FROM Usertable U LEFT JOIN RoomPlayer RP ON U.userID = RP.userID WHERE U.userID IN (SELECT userID FROM RoomPlayer WHERE roomID = {roomID})");

            foreach(DataRow s in dt.Rows)
            {
                ListViewItem lvi = new ListViewItem();
                lvi.Content = $"{s.Field<string>(0)} ({s.Field<int>(1)} pt)";
                lvi.Focusable = false;
                namesList.Items.Add(lvi);
            }

        }

        private void Timer_Elapsed(object sender, EventArgs e)
        {
            if (!done)
            {
                timeMilliseconds++;
                timerTextBlock.Text = millisecondsToTime(timeMilliseconds);
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            window.TextInput += Window_TextInput;
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

            string oldText = typedText;
            typedText += keyChar;
            bool focussedLetter = true;

            if(!done && textToType[typedText.Length - 1] == keyChar)
            {
                inputText.Inlines.Clear();

                for (int i = 0; i < textToType.Length; i++)
                {

                    if (typedText.Length > i && textToType[i] == typedText[i])
                    {
                        inputText.Inlines.Add(new Run(textToType[i].ToString()) { Foreground = Brushes.Green });
                    }
                    else
                    {
                        if (focussedLetter)
                        {
                            inputText.Inlines.Add(new Run(textToType[i].ToString()) { Foreground = Brushes.Black, TextDecorations = TextDecorations.Underline });
                            focussedLetter = false;
                        }
                        else
                        {
                            inputText.Inlines.Add(new Run(textToType[i].ToString()) { Foreground = Brushes.Black });
                        }

                    }
                }

                if(textToType.Length == typedText.Length)
                {
                    //timer.Stop();
                    doneTextBlock.Inlines.Add($" {millisecondsToTime(timeMilliseconds)}");
                    doneTextBlock.Visibility = Visibility.Visible;
                    inputText.Opacity = 0.6;
                    done = true;
                    int roomResultID = c.ID("SELECT MAX(roomResultID) FROM RoomResult") + 1;
                    int userID = int.Parse(ConfigurationManager.AppSettings.Get("userID"));
                    c.insertInto($"INSERT INTO RoomResult (roomResultID, roomExerciseID, roomID, userID, time) VALUES ({roomResultID}, {this.exerciseID}, {this.roomID}, {userID}, {timeMilliseconds})");
                    new Thread(() =>
                    {
                        while (true)
                        {
                            int playerCount = c.ID($"SELECT COUNT(*) FROM RoomPlayer WHERE roomID = {this.roomID}");
                            int playersDone = c.ID($"SELECT COUNT(*) FROM RoomResult WHERE roomID = {this.roomID} AND roomExerciseID = {this.exerciseID}");

                            if (playerCount == playersDone)
                            {
                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    Application.Current.MainWindow.Content = new ScoreboardScreen(this.roomID, this.exerciseID);
                                    
                                });
                                Thread.CurrentThread.Abort();
                            }
                            Thread.Sleep(500);
                        }
                        
                    }).Start();
                }

            }
            else
            {
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
