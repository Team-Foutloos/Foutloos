using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        public Results()
        {
            InitializeComponent();
            FillListBox();
            FillColumnChart("Exercise_0",60,30);
        }

        
        //TODO: change mockdata for the UserResult Table in the database (Table name might be different)
        private void FillListBox()
        {
            List<UserExerciseResult> exerciselist = new List<UserExerciseResult>();
            exerciselist.Add(new UserExerciseResult() { Name = "Exercise 11", Difficulty = "Beginner", Type = "Text", WPM = 40, Mistakes = 6 });
            exerciselist.Add(new UserExerciseResult() { Name = "Exercise 2", Difficulty = "Beginner", Type = "Text", WPM = 50, Mistakes = 4 });
            exerciselist.Add(new UserExerciseResult() { Name = "Exercise 3", Difficulty = "Advanced", Type = "Speech", WPM = 20, Mistakes = 1});

            ExerciseList.ItemsSource = exerciselist;
        }

        private void FillColumnChart(string Name, int WPM, int Mistakes)
        {
            columnChart.Title = Name;
            List<KeyValuePair<string, int>> valueList = new List<KeyValuePair<string, int>>();
            valueList.Add(new KeyValuePair<string, int>("WPM", WPM));
            valueList.Add(new KeyValuePair<string, int>("Mistakes", Mistakes));
            columnChart.DataContext = valueList;

        }

        private void ExerciseList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = (ListBox)sender;
            var currentExercise = (UserExerciseResult)item.SelectedItem;
            FillColumnChart(currentExercise.Name, currentExercise.WPM, currentExercise.Mistakes);
        }

    }
}
