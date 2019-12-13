using System;
using System.Data;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace Foutloos.Multiplayer
{
    /// <summary>
    /// Interaction logic for GameScreen.xaml
    /// </summary>
    public partial class GameScreen : Page
    {
        //Set basic variables
        string textToType = "Doekoe";
        string typedText = "";
        bool done = false;

        Timer timer = new Timer();

        int timeMilliseconds;

        public GameScreen(int roomID)
        {
            InitializeComponent();
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

            timer.Interval = 1;
            timer.Elapsed += Timer_Elapsed;
            timer.Start();

            Connection c = new Connection();
            DataTable dt = new DataTable();
            dt = c.PullData($"SELECT username FROM Usertable WHERE userID = (SELECT userID FROM RoomPlayer WHERE roomID = {roomID})");

            foreach(DataRow s in dt.Rows)
            {
                ListViewItem lvi = new ListViewItem();
                lvi.Content = $"{s.Field<string>(0)} (0 pt)";
                lvi.Focusable = false;
                namesList.Items.Add(lvi);
            }

        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            timeMilliseconds++;
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
                    timer.Stop();
                    doneTextBlock.Inlines.Add($" {millisecondsToTime(timeMilliseconds)}");
                    doneTextBlock.Visibility = Visibility.Visible;
                    inputText.Opacity = 0.6;
                    done = true;
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
            TimeSpan result = TimeSpan.FromMilliseconds(milliseconds);
            //Writing the text to the users screen in the correct time notation
            return result.ToString("ss':'fff");
        }
    }
}
