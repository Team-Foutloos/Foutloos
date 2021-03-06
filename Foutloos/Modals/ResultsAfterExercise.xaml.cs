﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
        private int countAmount;
        private int mistakes;
        private double accuracy;
        private List<int> cpmTimeList;
        private List<int> wpmTimeList;
        private string exerciseText;
        private Dictionary<char, int> mistakeLetters;
        private int exerciseID;
        private bool specialChars;
        private bool generated;
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

        //
        public ResultsAfterExercise(int wpm, int cpm, int time, int mistakes, double accuracy, List<int> cpmTimeList, List<int> wpmTimeList, Dictionary<char, int> mistakeLetter, string exerciseText, int exerciseID, bool generated, int countAmount)
        {
            InitializeComponent();
            UIChange();

            this.wpm = wpm;
            this.cpm = cpm;
            this.mistakes = mistakes;
            this.time = TimeSpan.FromSeconds(time);
            this.countAmount = countAmount;
            this.accuracy = accuracy;
            this.cpmTimeList = cpmTimeList;
            this.wpmTimeList = wpmTimeList;
            this.mistakeLetters = mistakeLetter;
            this.exerciseText = exerciseText;
            this.exerciseID = exerciseID;
            this.generated = generated;
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
                    dt = c.PullData($"SELECT time, mistakes, wpm, cpm FROM result WHERE exerciseID = {exerciseID} AND userID = (SELECT userID from usertable WHERE username = '{ConfigurationManager.AppSettings["username"]}')");
                }
                catch
                {
                    noPreviousResultsLogin_grid.Visibility = Visibility.Visible;
                }
                if (dt != null && dt.Rows.Count > 1)
                {
                    prevError.Content = prevError.Content.ToString() + dt.Rows[dt.Rows.Count - 2]["mistakes"].ToString();
                    prevWPM.Content = prevWPM.Content.ToString() + dt.Rows[dt.Rows.Count - 2]["wpm"].ToString();
                    prevCPM.Content = prevCPM.Content.ToString() + dt.Rows[dt.Rows.Count - 2]["cpm"].ToString();
                    prevAcur.Content = prevAcur.Content.ToString() + dt.Rows[dt.Rows.Count - 2]["time"].ToString();

                    previousResultsLogin_grid.Visibility = Visibility.Visible;
                }
                else
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
                if (dictionaryItem.Key != ' ')
                {
                    valueList.Add(new KeyValuePair<string, int>(dictionaryItem.Key.ToString(), dictionaryItem.Value));
                }
                else
                {
                    valueList.Add(new KeyValuePair<string, int>("␣", dictionaryItem.Value));
                }

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
            else if (this.generated)
            {
                Exercise exercise = new Exercise(exerciseText, specialChars, exerciseID);
                exercise.SetCountdown(countAmount);
                Application.Current.MainWindow.Content = exercise;
            }
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
