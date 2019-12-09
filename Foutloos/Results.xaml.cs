using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Foutloos
{
    /// <summary>
    /// Interaction logic for Results.xaml
    /// </summary>
    public partial class Results : Page
    {
        private List<UserExerciseResult> exerciselist;
        public Results()
        {
            InitializeComponent();
            try
            {
                UsernameBlock.Text = ConfigurationManager.AppSettings["username"];
                FillListBox();
                FillColumnCharts("Exercise_#", 0, 0, 0);
                FillLineChart();
            }
            catch(Exception e)
            {

            }
        }

        
        //TODO: change mockdata for the UserResult Table in the database (Table name might be different)
        private void FillListBox()
        {

            Connection c = new Connection();
            DataTable dt = new DataTable();
            exerciselist = new List<UserExerciseResult>();

            dt = c.PullData($"SELECT R.exerciseID, mistakes, wpm, cpm, time, difficulty, speech " +
                $"FROM Result R RIGHT JOIN Usertable U ON R.userID = U.userID " +
                $"JOIN Exercise E ON R.exerciseID = E.exerciseID WHERE username = '{ConfigurationManager.AppSettings["username"]}'");

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                UserExerciseResult result = new UserExerciseResult();
                result.Name = "Exercise " + dt.Rows[i]["exerciseID"].ToString();
                result.Mistakes = Convert.ToInt32(dt.Rows[i]["mistakes"]);
                result.WPM = Convert.ToInt32(dt.Rows[i]["wpm"]);
                result.CPM = Convert.ToInt32(dt.Rows[i]["cpm"]);
                //result.Mistakes = Convert.ToInt32(dt.Rows[i]["time"]);
                result.Difficulty = dt.Rows[i]["difficulty"].ToString();
                switch (result.Difficulty)
                {
                    case "1":
                        result.Difficulty = "Amateur";
                        break;
                    case "2":
                        result.Difficulty = "Normal";
                        break;
                    case "3":
                        result.Difficulty = "Expert";
                        break;
                    default:
                        result.Difficulty = "unknown";
                        break;
                }
                result.Type = dt.Rows[i]["speech"].ToString();
                switch (result.Type)
                {
                    case "0":
                        result.Type = "Text";
                        break;
                    case "1":
                        result.Type = "Speech";
                        break;
                    default:
                        result.Type = "Unknown";
                        break;
                }
                exerciselist.Add(result);
                System.Windows.Forms.MessageBox.Show(dt.Rows[i]["difficulty"].ToString());
            }


            
            /*exerciselist.Add(new UserExerciseResult() { Name = "Exercise 1", Difficulty = "Beginner", Type = "Text", WPM = 40, Mistakes = 6, CPM = 240 }) ;
            exerciselist.Add(new UserExerciseResult() { Name = "Exercise 2", Difficulty = "Beginner", Type = "Text", WPM = 45, Mistakes = 4, CPM = 270 });
            exerciselist.Add(new UserExerciseResult() { Name = "Exercise 7", Difficulty = "Advanced", Type = "Speech", WPM = 20, Mistakes = 1, CPM = 120});
            exerciselist.Add(new UserExerciseResult() { Name = "Exercise 3", Difficulty = "Beginner", Type = "Text", WPM = 60, Mistakes = 15, CPM = 360 });
            exerciselist.Add(new UserExerciseResult() { Name = "Exercise 4", Difficulty = "Beginner", Type = "Text", WPM = 55, Mistakes = 6, CPM = 345 });
            exerciselist.Add(new UserExerciseResult() { Name = "Exercise 5", Difficulty = "Beginner", Type = "Text", WPM = 60, Mistakes = 7, CPM = 365 });
            exerciselist.Add(new UserExerciseResult() { Name = "Exercise 6", Difficulty = "Advanced", Type = "Text", WPM = 40, Mistakes = 7, CPM = 300 });
            exerciselist.Add(new UserExerciseResult() { Name = "Exercise 8", Difficulty = "Advanced", Type = "Speech", WPM = 16, Mistakes = 1, CPM = 100 });
            exerciselist.Add(new UserExerciseResult() { Name = "Exercise 9", Difficulty = "Advanced", Type = "Text", WPM = 57, Mistakes = 20, CPM = 380 });
            exerciselist.Add(new UserExerciseResult() { Name = "Exercise 10", Difficulty = "Advanced", Type = "Text", WPM = 50, Mistakes = 7, CPM = 340 });*/

            ExerciseList.ItemsSource = exerciselist;
        }

        private void FillColumnChart(string Name, int WPM, int Mistakes)
        {
            ColumnChartTitle.Text = Name;
            List<KeyValuePair<string, int>> valueList = new List<KeyValuePair<string, int>>();
            valueList.Add(new KeyValuePair<string, int>("WPM", WPM));
            valueList.Add(new KeyValuePair<string, int>("Mistakes", Mistakes));
            columnChart.DataContext = valueList;
        }
        private void FillColumnCharts(string Name, int WPM, int CPM, int Mistakes)
        {
            ColumnChartTitle.Text = Name;
            //WPM column
            List<KeyValuePair<string, int>> valueList = new List<KeyValuePair<string, int>>();
            valueList.Add(new KeyValuePair<string, int>("WPM", WPM));
            columnChart.DataContext = valueList;
            //CPM column
            List<KeyValuePair<string, int>> valueList2 = new List<KeyValuePair<string, int>>();
            valueList2.Add(new KeyValuePair<string, int>("CPM", CPM));
            columnChartCPM.DataContext = valueList2;
            //Mistakes column
            List<KeyValuePair<string, int>> valueList3 = new List<KeyValuePair<string, int>>();
            valueList3.Add(new KeyValuePair<string, int>("Mistakes", Mistakes));
            columnChartMistakes.DataContext = valueList3;
        }
       
        private void FillLineChart()
        {
            List<KeyValuePair<int, int>> wpm_line = new List<KeyValuePair<int, int>>();
            List<KeyValuePair<int, int>> mistake_line = new List<KeyValuePair<int, int>>();
            int counter = 1;
            foreach(UserExerciseResult result in exerciselist)
            {
                wpm_line.Add(new KeyValuePair<int,int>(counter, result.WPM));
                mistake_line.Add(new KeyValuePair<int, int>(counter, result.Mistakes));
                counter++;
            }
            LineWPM.DataContext = wpm_line;
            LineMistakes.DataContext = mistake_line;
        }

        private void ExerciseList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = (ListBox)sender;
            var currentExercise = (UserExerciseResult)item.SelectedItem;
            FillColumnCharts(currentExercise.Name, currentExercise.WPM, currentExercise.CPM, currentExercise.Mistakes);
        }

        private void GoHome_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Application.Current.MainWindow.Content = new HomeScreen();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            FillLineChart();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            List<KeyValuePair<int, int>> wpm_line = new List<KeyValuePair<int, int>>();
            List<KeyValuePair<int, int>> mistake_line = new List<KeyValuePair<int, int>>();
            int counter = 1;
            foreach (UserExerciseResult result in exerciselist)
            {
                wpm_line.Add(new KeyValuePair<int, int>(counter, result.WPM));
                mistake_line.Add(new KeyValuePair<int, int>(counter, result.CPM));
                counter++;
            }
            LineWPM.DataContext = wpm_line;
            LineMistakes.DataContext = mistake_line;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            List<KeyValuePair<int, int>> wpm_line = new List<KeyValuePair<int, int>>();
            List<KeyValuePair<int, int>> mistake_line = new List<KeyValuePair<int, int>>();
            int counter = 1;
            foreach (UserExerciseResult result in exerciselist)
            {
                wpm_line.Add(new KeyValuePair<int, int>(counter, result.WPM));
                mistake_line.Add(new KeyValuePair<int, int>(counter, result.CPM));
                counter++;
            }
            LineWPM.DataContext = null;
            LineMistakes.DataContext = mistake_line;
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            List<KeyValuePair<int, int>> wpm_line = new List<KeyValuePair<int, int>>();
            List<KeyValuePair<int, int>> mistake_line = new List<KeyValuePair<int, int>>();
            int counter = 1;
            foreach (UserExerciseResult result in exerciselist)
            {
                wpm_line.Add(new KeyValuePair<int, int>(counter, result.WPM));
                mistake_line.Add(new KeyValuePair<int, int>(counter, result.WPM));
                counter++;
            }
            LineWPM.DataContext = null;
            LineMistakes.DataContext = mistake_line;
        }
    }
}
