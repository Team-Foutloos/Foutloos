using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Foutloos
{
    /// <summary>
    /// Interaction logic for Results.xaml
    /// </summary>
    public partial class Results : Page
    {
        private Connection c = new Connection();
        private List<UserExerciseResult> exerciselist;
        private List<UserExerciseResult> ReverseExerciseList;

        private List<KeyValuePair<int, int>> wpm_line;
        private List<KeyValuePair<int, int>> cpm_line;
        private List<KeyValuePair<int, int>> mistake_line;
        public Results()
        {
            InitializeComponent();
            try
            {
                userNameWelcome.Content = userNameWelcome.Content.ToString() + ConfigurationManager.AppSettings["username"];
                FillUserStats();
                FillPieChart();
                FillListBox();
                FillColumnCharts("Exercise_#", 0, 0, 0);
                FillLineChart();
            }
            catch (Exception e)
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

            //Query to get the total amount of exercises the user has with additional packages
            DataTable getExercisesCount = c.PullData($"SELECT COUNT(*) FROM Exercise WHERE packageID IN (SELECT STRING_AGG(packageID, ',') FROM License L LEFT JOIN Usertable U ON L.userID = U.userID WHERE U.username = '{ConfigurationManager.AppSettings["username"]}' )");


            //Displaying the amount of different exercises done by the user against the max amount the user can do (the amount the user has in extra packages plus the 15 of the standard package)
            UniqueExerciseComp.Text = dt0.Rows[0][4].ToString() + "/" + ((int)getExercisesCount.Rows[0][0] + 15).ToString();

            DataTable dt2 = new DataTable();
            dt2 = c.PullData($"SELECT COUNT(*) FROM Result R RIGHT JOIN Usertable U ON R.userID = U.userID " +
                $"WHERE speech = 1 AND username = '{ConfigurationManager.AppSettings["username"]}'");
            TextSpeechRatio.Text = $"{Convert.ToInt32(ExercisesCompleted.Text) - Convert.ToInt32(dt2.Rows[0][0])}/{dt2.Rows[0][0].ToString()}";

            DataTable dt3 = new DataTable();
            dt3 = c.PullData($"SELECT difficulty, COUNT(difficulty) FROM Result R RIGHT JOIN Usertable U On R.userID = U.userID " +
                $"JOIN Exercise E ON E.exerciseID = R.exerciseID WHERE username = '{ConfigurationManager.AppSettings["username"]}'" +
                $" GROUP BY difficulty ORDER BY COUNT(difficulty) DESC");
            switch (dt3.Rows[0][0].ToString())
            {
                case "1":
                    FavoriteDifficulty.Text = "Amateur";
                    break;
                case "2":
                    FavoriteDifficulty.Text = "Normal";
                    break;
                case "3":
                    FavoriteDifficulty.Text = "Expert";
                    break;
                default:
                    FavoriteDifficulty.Text = "unknown";
                    break;
            }


        }
        private void FillPieChart()
        {
            DataTable dt = new DataTable();
            dt = c.PullData($"SELECT letter, SUM(count) FROM Result R RIGHT JOIN Usertable U On R.userID = U.userID " +
                $"JOIN Error E ON R.resultID = E.resultID WHERE username = '{ConfigurationManager.AppSettings["username"]}' " +
                $"GROUP BY letter ORDER BY SUM(count) DESC");
            List<KeyValuePair<string, int>> mistakes = new List<KeyValuePair<string, int>>();

            for (int i = 0; i < 8 && i < dt.Rows.Count; i++)
            {
                if (dt.Rows[i][0].ToString() != " ")
                {
                    mistakes.Add(new KeyValuePair<string, int>(dt.Rows[i][0].ToString(), Convert.ToInt32(dt.Rows[i][1])));
                }
                else
                {
                    mistakes.Add(new KeyValuePair<string, int>("␣", Convert.ToInt32(dt.Rows[i][1])));
                }

            }

            PieChart.DataContext = mistakes;
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
            ReverseExerciseList = new List<UserExerciseResult>();
            //Reverse list for recent exercise top
            for (int i = (exerciselist.Count() - 1); i >= 0; i--)
            {
                ReverseExerciseList.Add((UserExerciseResult)exerciselist[i]);
            }
            //ExerciseList in the xaml-part will use this list as ItemSource when binding
            ExerciseList.ItemsSource = ReverseExerciseList;
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
            wpm_line = new List<KeyValuePair<int, int>>();
            cpm_line = new List<KeyValuePair<int, int>>();
            mistake_line = new List<KeyValuePair<int, int>>();
            int counter = 1;
            foreach (UserExerciseResult result in exerciselist)
            {
                wpm_line.Add(new KeyValuePair<int, int>(counter, result.WPM));
                cpm_line.Add(new KeyValuePair<int, int>(counter, result.CPM));
                mistake_line.Add(new KeyValuePair<int, int>(counter, result.Mistakes));
                counter++;
            }
            LineWPM.DataContext = wpm_line;
            LineCPM.DataContext = cpm_line;
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

        private void WPM_Click(object sender, RoutedEventArgs e)
        {
            if (WPMButton.Content.Equals("Hide WPM"))
            {
                LineWPM.DataContext = null;
                WPMButton.Content = "Show WPM";
            }
            else
            {
                LineWPM.DataContext = wpm_line;
                WPMButton.Content = "Hide WPM";
            }
        }

        private void CPM_Click(object sender, RoutedEventArgs e)
        {
            if (CPMButton.Content.Equals("Hide CPM"))
            {
                LineCPM.DataContext = null;
                CPMButton.Content = "Show CPM";
            }
            else
            {
                LineCPM.DataContext = cpm_line;
                CPMButton.Content = "Hide CPM";
            }
        }

        private void Mistakes_Click(object sender, RoutedEventArgs e)
        {
            if (MistakesButton.Content.Equals("Hide Mistakes"))
            {
                LineMistakes.DataContext = null;
                MistakesButton.Content = "Show Mistakes";
            }
            else
            {
                LineMistakes.DataContext = mistake_line;
                MistakesButton.Content = "Hide Mistakes";
            }
        }
    }
}
