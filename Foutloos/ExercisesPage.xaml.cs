﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
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
    /// Interaction logic for ExercisesPage.xaml
    /// </summary>
    public partial class ExercisesPage : Page
    {

        Connection c = new Connection();
        //private bool text = false;
        //private bool spoken = false;
      
        public ExercisesPage()
        {
            InitializeComponent();
            AddButton();

        }

        
        private void AddButton()
        {           

            //Connection con = new Connection();
            //List<List<string>> exercises = new List<List<string>>();
            //exercises = con.QueryDataExercisesTable("SELECT * FROM Exercises");
            //Console.WriteLine(exercises.Count);

            int exnum = 1;
            int amount = 6;
            int x = 1;
            int j = 0;
            int i = 0;


            //The standard left and top margin are added.
            Grid_All.ShowGridLines = false;
            Grid_All.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50) });
            Grid_All.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(50) });

            //The amount of columns is always the same. Therefore this piece of code adds them. 
            for (int h = 0; h < 4; h++)
            {
                Grid_All.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(252) });
                Grid_All.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(50) });
            }

            //The button gets added as frequently as needed. 
            for (int z = 0; z < amount; z++)
            {
                Button b1 = new Button();
                Label l1 = new Label();

                b1.Click += B1_Click;
                
                

                Grid.SetColumn(b1, j + 1);


                Grid.SetRow(b1, x);
                b1.Background = Brushes.White;
                b1.Name = $"E{i}";
                b1.Content = $"Excercise: {exnum}";
                b1.Foreground = Brushes.Black;
                b1.BorderBrush = Brushes.Black;
                //b1.BorderThickness
                Grid_All.Children.Add(b1);
                l1.Content = "test";
                

                //< Label Content = "Label" HorizontalAlignment = "Left" Margin = "0,174,0,0" Grid.Row = "1" VerticalAlignment = "Top" Grid.Column = "1" />

                //The position is always 1,1, 3,1, 5,1 etc. Therefore There is always 2 added for j.
                j += 2;
                i++;
                exnum++;

                //The moment that the amount of buttons placed with modulo 4 is equal to zero. X gets 2 added to it so that it continues on the next line.
                //j becomes zero again so that it start again at y positition 1. There is a check that it is not equal to 0 otherwise it already swaps y position before filling the x positions.
                if (i % 4 == 0 && i != 0)
                {
                    x += 2;
                    j = 0;
                }

            }

            //Control if the amount of buttons / 4 is equal to 1, 2, 3 etc. This is to indicate how many times more rows have to get added to the software.
            for (int row = 0; row < Math.Ceiling((double)amount / 4); row++)
            {
                Grid_All.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(200) });
                Grid_All.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50) });

            }

            
            


        }

        private void B1_Click(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;

            if (b.Name.Equals("E0"))
            {
                this.Exercise.Text = "Exercise 1";
            }
            if (b.Name.Equals("E1"))
            {
                this.Exercise.Text = "Exercise 2";
            }
            if (b.Name.Equals("E2"))
            {
                this.Exercise.Text = "Exercise 3";
            }
            if (b.Name.Equals("E3"))
            {
                this.Exercise.Text = "Exercise 4";
            }
            if (b.Name.Equals("E4"))
            {
                this.Exercise.Text = "Exercise 5";
            }
            if (b.Name.Equals("E5"))
            {
                this.Exercise.Text = "Exercise 6";
            }
            if (b.Name.Equals("E6"))
            {
                this.Exercise.Text = "Exercise 7";
            }
            if (b.Name.Equals("E7"))
            {
                this.Exercise.Text = "Exercise 8";
            }
            //else
            //{
            //    System.Windows.Forms.MessageBox.Show("Button 1 not working");
            //}
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.Content = new HomeScreen();
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        //private void check_radio()
        //{
        //    if (Text.IsChecked == true)
        //    {
        //        this.text = true;
        //    }
        //    if (Spoken.IsChecked == true)
        //    {
        //        this.spoken = true;
        //    }            
        //}

        //private void FillDataGrid()
        //{
            
        //    //query that is being executed and being shows in a Table in the application.
        //    List<List<object>> result = c.QueryDataExercisesTable("SELECT * FROM Exercises");
        //    string waardes = "";

        //    for (int i = 0; i < result.Count; i++)
        //    {
        //        for (int x = 0; x < result[i].Count; x++)
        //        {
        //            waardes += result[i][x] + " ";
        //        }
        //    }

            

            





        //}

        private void TabControl_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
