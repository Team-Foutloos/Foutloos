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
        private Connection c = new Connection();
        private List<UserExerciseResult> exerciselist;
        public Results()
        {
            InitializeComponent();
            try
            {
                UsernameBlock.Text = ConfigurationManager.AppSettings["username"];
                FillUserStats();
                FillListBox();
                FillColumnCharts("Exercise_#", 0, 0, 0);
                FillLineChart();
            }
            catch(Exception e)
            {

            }
        }


        private void FillUserStats()
        {
            DataTable dt0 = new DataTable();
            
            dt0 = c.PullData($"SELECT COUNT(*), AVG(wpm), AVG(cpm), SUM(mistakes), COUNT(DISTINCT(exerciseID)) FROM Result R RIGHT JOIN Usertable U ON R.userID = U.userID WHERE username = '{ConfigurationManager.AppSettings["username"]}'");
            
            ExercisesCompleted.Text = dt0.Rows[0][0].ToString();
            AverageWPM.Text = dt0.Rows[0][1].ToString();
            AverageCPM.Text = dt0.Rows[0][2].ToString();
            TotalMistakes.Text = dt0.Rows[0][3].ToString();

            DataTable dt1 = new DataTable();
            dt1 = c.PullData($"SELECT COUNT(*) FROM Exercise");
            
            UniqueExerciseComp.Text = dt0.Rows[0][4].ToString() + "/" + dt1.Rows[0][0].ToString();
            TextSpeechRatio.Text = "" + "/" + "";
            FavoriteDifficulty.Text = "";

        }
        private void FillListBox()
        {
            DataTable dt = new DataTable();
            exerciselist = new List<UserExerciseResult>();

            dt = c.PullData($"SELECT R.exerciseID, mistakes, wpm, cpm, time, difficulty, speech " +
                $"FROM Result R RIGHT JOIN Usertable U ON R.userID = U.userID " +
                $"JOIN Exercise E ON R.exerciseID = E.exerciseID WHERE username = '{ConfigurationManager.AppSettings["username"]}'");

            //Fill Listbox
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                //Convert datarow into object
                UserExerciseResult result = new UserExerciseResult();
                result.Name = "Exercise " + dt.Rows[i]["exerciseID"].ToString();
                result.Mistakes = Convert.ToInt32(dt.Rows[i]["mistakes"]);
                result.WPM = Convert.ToInt32(dt.Rows[i]["wpm"]);
                result.CPM = Convert.ToInt32(dt.Rows[i]["cpm"]);
                //result.Mistakes = Convert.ToInt32(dt.Rows[i]["time"]);
                result.Difficulty = dt.Rows[i]["difficulty"].ToString();
                //change number into text
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
                //change number into text
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
                //Add object to list
                exerciselist.Add(result);
            }

            //ExerciseList in the xaml-part will use this list as ItemSource when binding
            ExerciseList.ItemsSource = exerciselist;
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
