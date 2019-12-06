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
            UsernameBlock.Text = ConfigurationManager.AppSettings["username"];
            FillListBox();
            FillColumnCharts("Exercise_0", 60, 240,30) ;
            FillLineChart();
        }

        
        //TODO: change mockdata for the UserResult Table in the database (Table name might be different)
        private void FillListBox()
        {

            Connection c = new Connection();
            DataTable dt = new DataTable();
            

            dt = c.PullData("SELECT * FROM Result R RIGHT JOIN Usertable U ON R.userID = U.userID WHERE username = " + ConfigurationManager.AppSettings["username"]);
            
            UserExerciseResult result = new UserExerciseResult();
            //result.Mistakes = Convert.ToInt32(dt.Rows[0]["mistakes"]);
            


            exerciselist = new List<UserExerciseResult>();
            exerciselist.Add(new UserExerciseResult() { Name = "Exercise 1", Difficulty = "Beginner", Type = "Text", WPM = 40, Mistakes = 6, CPM = 240 }) ;
            exerciselist.Add(new UserExerciseResult() { Name = "Exercise 2", Difficulty = "Beginner", Type = "Text", WPM = 45, Mistakes = 4, CPM = 270 });
            exerciselist.Add(new UserExerciseResult() { Name = "Exercise 7", Difficulty = "Advanced", Type = "Speech", WPM = 20, Mistakes = 1, CPM = 120});
            exerciselist.Add(new UserExerciseResult() { Name = "Exercise 3", Difficulty = "Beginner", Type = "Text", WPM = 60, Mistakes = 15, CPM = 360 });
            exerciselist.Add(new UserExerciseResult() { Name = "Exercise 4", Difficulty = "Beginner", Type = "Text", WPM = 55, Mistakes = 6, CPM = 345 });
            exerciselist.Add(new UserExerciseResult() { Name = "Exercise 5", Difficulty = "Beginner", Type = "Text", WPM = 60, Mistakes = 7, CPM = 365 });
            exerciselist.Add(new UserExerciseResult() { Name = "Exercise 6", Difficulty = "Advanced", Type = "Text", WPM = 40, Mistakes = 7, CPM = 300 });
            exerciselist.Add(new UserExerciseResult() { Name = "Exercise 8", Difficulty = "Advanced", Type = "Speech", WPM = 16, Mistakes = 1, CPM = 100 });
            exerciselist.Add(new UserExerciseResult() { Name = "Exercise 9", Difficulty = "Advanced", Type = "Text", WPM = 57, Mistakes = 20, CPM = 380 });
            exerciselist.Add(new UserExerciseResult() { Name = "Exercise 10", Difficulty = "Advanced", Type = "Text", WPM = 50, Mistakes = 7, CPM = 340 });

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
