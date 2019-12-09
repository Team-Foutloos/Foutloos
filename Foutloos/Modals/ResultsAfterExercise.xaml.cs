using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
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
using System.Windows.Shapes;

namespace Foutloos.Modals
{
    /// <summary>
    /// Interaction logic for ResultsAfterExercise.xaml
    /// </summary>
    public partial class ResultsAfterExercise : Window
    {
        private Connection c;
        private int wpm;
        private int cpm;
        private TimeSpan time;
        private int mistakes;
        private double accuracy;
        private List<int> cpmTimeList;
        private List<int> wpmTimeList;
        private string exerciseText;
        private Dictionary<char, int> mistakeLetters;
        private int exerciseID;
        private bool specialChars;
        private bool isSpoken;

        public ResultsAfterExercise(int wpm, int cpm, int time, int mistakes, double accuracy, List<int> cpmTimeList, List<int> wpmTimeList, Dictionary<char, int> mistakeLetter, string exerciseText, int exerciseID)
        {
            InitializeComponent();
            UIChange();

            this.wpm = wpm;
            this.cpm = cpm;
            this.mistakes = mistakes;
            this.time = TimeSpan.FromSeconds(time);
            this.accuracy = accuracy;
            this.cpmTimeList = cpmTimeList;
            this.wpmTimeList = wpmTimeList;
            this.mistakeLetters = mistakeLetter;
            this.exerciseText = exerciseText;
            this.exerciseID = exerciseID;
            this.isSpoken = true;

            wordspm_label.Content = wordspm_label.Content.ToString() + wpm;
            charspm_label.Content = charspm_label.Content.ToString() + cpm;
            time_label.Content = time_label.Content.ToString() + this.time.ToString("mm':'ss");
            error_label.Content = error_label.Content.ToString() + mistakes;
            accuracy_label.Content = accuracy_label.Content.ToString() + Math.Round(accuracy, 2) + "%";
            stringExercise.Text = exerciseText;

            FillLineChart();
            FillColumnChart();
        }


        public ResultsAfterExercise(int wpm, int cpm, int time, int mistakes, double accuracy, List<int> cpmTimeList, List<int> wpmTimeList, Dictionary<char, int> mistakeLetter, string exerciseText, int exerciseID, bool specialChars)
        {
            InitializeComponent();
            UIChange();

            this.wpm = wpm;
            this.cpm = cpm;
            this.mistakes = mistakes;
            this.time = TimeSpan.FromSeconds(time);
            this.accuracy = accuracy;
            this.cpmTimeList = cpmTimeList;
            this.wpmTimeList = wpmTimeList;
            this.mistakeLetters = mistakeLetter;
            this.exerciseText = exerciseText;
            this.exerciseID = exerciseID;
            this.specialChars = specialChars;
            this.isSpoken = false;

            wordspm_label.Content = wordspm_label.Content.ToString() + wpm;
            charspm_label.Content = charspm_label.Content.ToString() + cpm;
            time_label.Content = time_label.Content.ToString() + this.time.ToString("mm':'ss");
            error_label.Content = error_label.Content.ToString() + mistakes;
            accuracy_label.Content = accuracy_label.Content.ToString() + Math.Round(accuracy, 2) + "%";
            stringExercise.Text = exerciseText;

            FillLineChart();
            FillColumnChart();
        }


        private void UIChange()
        {
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["username"]))
            {
                Connection c = new Connection();
                DataTable dt = new DataTable();

                previousResultsNoLogin_grid.Visibility = Visibility.Hidden;
                try
                {
                    dt = c.PullData($"SELECT R.exerciseID, mistakes, wpm, cpm, time, difficulty, speech " +
                        $"FROM Result R RIGHT JOIN Usertable U ON R.userID = U.userID " +
                        $"JOIN Exercise E ON R.exerciseID = E.exerciseID WHERE username = '{ConfigurationManager.AppSettings["username"]}' AND R.exerciseID = {exerciseID}");
                    previousResultsLogin_grid.Visibility = Visibility.Visible;
                }
                catch
                {
                    noPreviousResultsLogin_grid.Visibility = Visibility.Visible;
                }
            }
        }



        //Fill all the values of the column chart (the errors per letter chart)
        private void FillColumnChart()
        {
            List<KeyValuePair<string, int>> valueList = new List<KeyValuePair<string, int>>();
            foreach (var dictionaryItem in mistakeLetters)
            {
                valueList.Add(new KeyValuePair<string, int>(dictionaryItem.Key.ToString(), dictionaryItem.Value));
            }
            charError.DataContext = valueList;


            Style styleLegend = new Style { TargetType = typeof(Control) };
            styleLegend.Setters.Add(new Setter(Control.HeightProperty, 0d));


            charError.LegendStyle = styleLegend;
        }

        //Fill all the values of the line chart (the CPM and WPM chart)
        private void FillLineChart()
        {
            List<KeyValuePair<int, int>> wpm_line = new List<KeyValuePair<int, int>>();
            List<KeyValuePair<int, int>> cpm_line = new List<KeyValuePair<int, int>>();
            int counter = 0;
            foreach (int result in this.cpmTimeList)
            {
                cpm_line.Add(new KeyValuePair<int, int>(counter, result));
                counter++;
            }
            LineCPM.DataContext = cpm_line;

            counter = 0;
            foreach (int result in this.wpmTimeList)
            {
                wpm_line.Add(new KeyValuePair<int, int>(counter, result));
                counter++;
            }
            LineWPM.DataContext = wpm_line;

            Style styleLegend = new Style { TargetType = typeof(Control) };
            styleLegend.Setters.Add(new Setter(Control.WidthProperty, 0d));
            styleLegend.Setters.Add(new Setter(Control.HeightProperty, 0d));


            pointChart.LegendStyle = styleLegend;


            CPMchart.LegendStyle = styleLegend;
        }

        //If the user clicks the back key.
        private void ThemedButton_PreviewMouseDown_1(object sender, MouseButtonEventArgs e)
        {
            Application.Current.MainWindow.Content = new ExercisesPage();
            this.Close();
        }

        //If the user clicks te retry key.
        private void ThemedButton_PreviewMouseDown_2(object sender, MouseButtonEventArgs e)
        {
            if (this.isSpoken)
                Application.Current.MainWindow.Content = new VoiceExercise(exerciseText, exerciseID);
            else
                Application.Current.MainWindow.Content = new Exercise(exerciseText, specialChars, exerciseID);
            this.Close();
        }

        //If the user clicks the next key.
        private void ThemedButton_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
        }

        //If the user clicks the login key.
        private void ThemedButton_PreviewMouseDown_3(object sender, MouseButtonEventArgs e)
        {
            ShowModal(new ModalLogin());
        }

        //If the user clicks the register key
        private void ThemedButton_PreviewMouseDown_4(object sender, MouseButtonEventArgs e)
        {
            ShowModal(new ModalRegister());
        }


        //This function shows the modal, login or register modal with generic types.
        private void ShowModal<T>(T modal) where T : Window
        {
            modal.ShowDialog();
            UIChange();
        }
    }
}
