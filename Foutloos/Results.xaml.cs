﻿using System;
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
        private List<UserExerciseResult> exerciselist;
        public Results()
        {
            InitializeComponent();
            FillListBox();
            FillColumnChart("Exercise_0",60,30);
            FillLineChart();
        }

        
        //TODO: change mockdata for the UserResult Table in the database (Table name might be different)
        private void FillListBox()
        {
            exerciselist = new List<UserExerciseResult>();
            exerciselist.Add(new UserExerciseResult() { Name = "Exercise 1", Difficulty = "Beginner", Type = "Text", WPM = 40, Mistakes = 6 });
            exerciselist.Add(new UserExerciseResult() { Name = "Exercise 2", Difficulty = "Beginner", Type = "Text", WPM = 45, Mistakes = 4 });
            exerciselist.Add(new UserExerciseResult() { Name = "Exercise 7", Difficulty = "Advanced", Type = "Speech", WPM = 20, Mistakes = 1});
            exerciselist.Add(new UserExerciseResult() { Name = "Exercise 3", Difficulty = "Beginner", Type = "Text", WPM = 60, Mistakes = 15 });
            exerciselist.Add(new UserExerciseResult() { Name = "Exercise 4", Difficulty = "Beginner", Type = "Text", WPM = 55, Mistakes = 6 });
            exerciselist.Add(new UserExerciseResult() { Name = "Exercise 5", Difficulty = "Beginner", Type = "Text", WPM = 60, Mistakes = 7 });
            exerciselist.Add(new UserExerciseResult() { Name = "Exercise 6", Difficulty = "Advanced", Type = "Text", WPM = 40, Mistakes = 7 });
            exerciselist.Add(new UserExerciseResult() { Name = "Exercise 8", Difficulty = "Advanced", Type = "Speech", WPM = 16, Mistakes = 1 });
            exerciselist.Add(new UserExerciseResult() { Name = "Exercise 9", Difficulty = "Advanced", Type = "Text", WPM = 57, Mistakes = 20 });
            exerciselist.Add(new UserExerciseResult() { Name = "Exercise 10", Difficulty = "Advanced", Type = "Text", WPM = 50, Mistakes = 7 });

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
            FillColumnChart(currentExercise.Name, currentExercise.WPM, currentExercise.Mistakes);
        }

    }
}
