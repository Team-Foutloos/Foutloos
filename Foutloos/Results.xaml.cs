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
            fillListBox();
            showColumnChart();
        }

        private void fillListBox()
        {
            List<UserExerciseResult> exerciselist = new List<UserExerciseResult>();
            exerciselist.Add(new UserExerciseResult() { Name = "Exercise 11", Difficulty = "Beginner", Type = "Text", WPM = 40 });
            exerciselist.Add(new UserExerciseResult() { Name = "Exercise 2", Difficulty = "Beginner", Type = "Text", WPM = 50 });
            exerciselist.Add(new UserExerciseResult() { Name = "Exercise 3", Difficulty = "Advanced", Type = "Speech", WPM = 20 });

            ExerciseList.ItemsSource = exerciselist;
        }
        
        private void showColumnChart()
        {
           
            List<KeyValuePair<string, int>> valueList = new List<KeyValuePair<string, int>>();
            valueList.Add(new KeyValuePair<string, int>("You", 30));
            valueList.Add(new KeyValuePair<string, int>("Top", 60));
            valueList.Add(new KeyValuePair<string, int>("Average", 45));

            columnChart.DataContext = valueList;
        }

        
    }
}
