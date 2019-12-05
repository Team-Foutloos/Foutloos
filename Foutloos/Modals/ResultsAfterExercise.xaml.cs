﻿using System;
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
using System.Windows.Shapes;

namespace Foutloos.Modals
{
    /// <summary>
    /// Interaction logic for ResultsAfterExercise.xaml
    /// </summary>
    public partial class ResultsAfterExercise : Window
    {
        private int wpm;
        private int cpm;
        private TimeSpan time;
        private int mistakes;
        private double accuracy;
        private List<int> cpmTimeList;
        private List<int> wpmTimeList;
        private Dictionary<char, int> mistakeLetters;

        public ResultsAfterExercise(int wpm, int cpm, int time, int mistakes, double accuracy, List<int> cpmTimeList, List<int> wpmTimeList, Dictionary<char, int> mistakeLetter)
        {
            InitializeComponent();
            this.wpm = wpm;
            this.cpm = cpm;
            this.mistakes = mistakes;
            this.time = TimeSpan.FromSeconds(time);
            this.accuracy = accuracy;
            this.cpmTimeList = cpmTimeList;
            this.wpmTimeList = wpmTimeList;
            this.mistakeLetters = mistakeLetter;

            wordspm_label.Content = wordspm_label.Content.ToString() + wpm;
            charspm_label.Content = charspm_label.Content.ToString() + cpm;
            time_label.Content = time_label.Content.ToString() + this.time.ToString("mm':'ss");
            error_label.Content = error_label.Content.ToString() + mistakes;
            accuracy_label.Content = accuracy_label.Content.ToString() + Math.Round(accuracy, 2) + "%";

            FillLineChart();
            FillColumnChart();
        }

        private void ThemedButton_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
        }

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

        private void ThemedButton_PreviewMouseDown_1(object sender, MouseButtonEventArgs e)
        {
            Application.Current.MainWindow.Content = new ExercisesPage();
            this.Close();
        }

        private void ThemedButton_PreviewMouseDown_2(object sender, MouseButtonEventArgs e)
        {
            Application.Current.MainWindow.Content = new Exercise("temptekst", false);
            this.Close();
        }
    }
}