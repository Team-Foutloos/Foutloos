﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Foutloos
{
    /// <summary>
    /// Interaction logic for ExercisesPage.xaml
    /// </summary>
    public partial class ExercisesPage : Page
    {

        private string tekst;
        private int exerciseID;
        private int amount = 0;

        DataTable dt = new DataTable();
        private List<List<DataRow>> exercises = new List<List<DataRow>>();
        List<DataRow> amateurExercises = new List<DataRow>();
        List<DataRow> normalExercises = new List<DataRow>();
        List<DataRow> expertExercises = new List<DataRow>();
        List<DataRow> allExercises = new List<DataRow>();
        List<DataRow> finished = new List<DataRow>();
        List<DataRow> GO = new List<DataRow>();
        List<DataRow> C = new List<DataRow>();
        List<DataRow> SC = new List<DataRow>();
        List<DataRow> JKR = new List<DataRow>();
        Connection c = new Connection();
        private Border selectedBorderButton;
        private double selectedOpacity = .3;
        private double backgroundOpacity = .5;        

        //Imma test something
        private List<Grid> grid_list = new List<Grid>();
        private List<List<int>> grid_Margin = new List<List<int>>();



        public ExercisesPage()
        {
            Modals.loadingModal loadingIndicator = new Modals.loadingModal();
            InitializeComponent();
            loadingIndicator.Show();
            addGrids();
            loadingIndicator.Close();
        }       

        private void addGrids()
        {

            //standard package that always gets added.
            dt = c.PullData($"SELECT * FROM Exercise LEFT JOIN Package ON Exercise.exerciseID = Package.packageID WHERE Exercise.packageID in (select packageID from Usertable join License on Usertable.userID = license.userID where Usertable.username = '{ConfigurationManager.AppSettings["username"]}') OR Exercise.packageID=1");

            //Create all the lists of exercises and add them to the main list (exercises)
            //use a 2D list to save spacing in the grids
            
            //In this list a certain order is used, amateurExercises gets index 0, normal 1, expert 2 and all 3, 
            

            grid_list.Add(Grid_Amateur);
            grid_list.Add(Grid_Normal);
            grid_list.Add(Grid_Expert);
            grid_list.Add(Grid_GO);
            grid_list.Add(Grid_C);
            grid_list.Add(Grid_SC);
            grid_list.Add(Grid_JKR);
            grid_list.Add(Grid_All);
            grid_list.Add(Grid_Finished);



            for (int i = 0; i < grid_list.Count+2; i++)
            {
                grid_Margin.Add(new List<int>() { 1, 0, 0 });
            }
                       


            exercises.Add(amateurExercises);
            exercises.Add(normalExercises);
            exercises.Add(expertExercises);
            exercises.Add(allExercises);
            exercises.Add(finished);
            exercises.Add(GO);
            exercises.Add(C);
            exercises.Add(SC);
            exercises.Add(JKR);


            amount = 0;

            foreach (DataRow row in dt.Rows)
            {
                string ID = row["exerciseID"].ToString();
                string text = row["text"].ToString();
                string difficulty = row["difficulty"].ToString();
                

                exercises[((int)Int64.Parse(difficulty)) - 1].Add(row);
               
                exercises[3].Add(row);                
                amount++;

            }
            //The standard left and top margin are added for grid All.
            Grid_All.ShowGridLines = false;
            Grid_All.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50) });
            Grid_All.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(50) });

            //The standard left and top margin are added for grid Amateur.
            Grid_Amateur.ShowGridLines = false;
            Grid_Amateur.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50) });
            Grid_Amateur.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(50) });

            //The standard left and top margin are added for grid Normal.
            Grid_Normal.ShowGridLines = false;
            Grid_Normal.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50) });
            Grid_Normal.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(50) });

            //The standard left and top margin are added for grid Expert.
            Grid_Expert.ShowGridLines = false;
            Grid_Expert.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50) });
            Grid_Expert.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(50) });

            //The standard left and top margin are added for grid Finished.
            Grid_Finished.ShowGridLines = false;
            Grid_Finished.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50) });
            Grid_Finished.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(50) });

            //The standard left and top margin are added for grid Finished.
            Grid_GO.ShowGridLines = false;
            Grid_GO.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50) });
            Grid_GO.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(50) });

            //The standard left and top margin are added for grid C#.
            Grid_C.ShowGridLines = false;
            Grid_C.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50) });
            Grid_C.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(50) });

            //The standard left and top margin are added for grid Special Characters.
            Grid_SC.ShowGridLines = false;
            Grid_SC.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50) });
            Grid_SC.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(50) });

            //The standard left and top margin are added for grid JKR.
            Grid_JKR.ShowGridLines = false;
            Grid_JKR.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50) });
            Grid_JKR.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(50) });



            addDLC();

            calculateGrids(Grid_All, amount);
            calculateGrids(Grid_Amateur, c.getPackageCount(1, 1));
            calculateGrids(Grid_Normal, c.getPackageCount(1, 2));
            calculateGrids(Grid_Expert, c.getPackageCount(1, 3));
            
            
            addButton();
        }

        private void addDLC()
        {

            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("Username")))
            {
                Tab_Finished.Visibility = Visibility.Visible;
                Grid_Finished.Visibility = Visibility.Visible;
                int Name = c.ID($"SELECT userID FROM userTable WHERE username = '{ConfigurationManager.AppSettings["username"]}'");
                calculateGrids(Grid_Finished, c.getPackageCountFinished(Name));
            }


            try
            {
                List<int> packages = new List<int>();                

                //checks how many and which packages are connected to the logged in account.          
                packages = c.getPackages($"select packageID from Usertable join License on Usertable.userID = license.userID where Usertable.username = '{ConfigurationManager.AppSettings["username"]}'");

                //Adds the packages for all the packages available in the account.
                foreach (int i in packages)
                {                  


                    if (i == 2)
                    {
                        
                        Grid_GO.Visibility = Visibility.Visible;
                        Tab_GO.Visibility = Visibility.Visible;

                        calculateGrids(Grid_GO, c.getPackageCount(2));
                    }
                    if (i == 3)
                    {
                        Tab_C.Visibility = Visibility.Visible;
                        Grid_C.Visibility = Visibility.Visible;
                        
                        calculateGrids(Grid_C, c.getPackageCount(3));
                    }
                    if (i == 4)
                    {
                       
                        Grid_SC.Visibility = Visibility.Visible;
                        Tab_SC.Visibility = Visibility.Visible;
                        calculateGrids(Grid_SC, c.getPackageCount(4));
                    }
                    if (i == 5)
                    {
                        
                        Grid_JKR.Visibility = Visibility.Visible;
                        Tab_JKR.Visibility = Visibility.Visible;
                        calculateGrids(Grid_JKR, c.getPackageCount(5));
                    }
                    if (i == 7)
                    {
                        
                        Generated.Visibility = Visibility.Visible;
                    }
                    if (i == 9)
                    {

                        Tab_QuickFire.Visibility = Visibility.Visible;

                    }


                    foreach (DataRow row in dt.Rows)
                    {

                        if (i == 2)
                        {
                            exercises[5].Add(row);
                        }
                        if (i == 3)
                        {
                            exercises[6].Add(row);
                        }
                        if (i == 4)
                        {
                            exercises[7].Add(row);
                        }
                        if (i == 5)
                        {
                            exercises[8].Add(row);
                        }                    
                        
                    }
                }

            }
            catch (Exception e)
            {

            }
            
        }       

        private void calculateGrids(Grid exercise_grid, int amount)
        {          

            // The amount of columns is always the same.Therefore this piece of code adds them.
            for (int h = 0; h < 4; h++)
            {
                exercise_grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(252) });
                exercise_grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(50) });
            }        
            
            
            //Control if the amount of buttons / 4 is equal to 1, 2, 3 etc. This is to indicate how many times more rows have to get added to the software.
            for (int row = 0; row < Math.Ceiling((double)amount / 4); row++)
            {
                exercise_grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(200) });
                exercise_grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50) });
            }
        }

        private Border getButton(int dif, int buttonName, DataTable finished, int scroll, int exnum, string name, int packageID)
        {
            //Create the main button.
            BorderButton button = new BorderButton(packageID, dif);
            Border borderButton = button.getButton();
            Grid borderGrid = (Grid)borderButton.Child;
            Image completedIcon = (Image)borderGrid.Children[2];
            TextBlock l1 = (TextBlock)borderGrid.Children[0];

            if (packageID - 1 == 1)
            {
                l1.FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#Orwell");
            }
            if (packageID - 1 == 2)
            {
                l1.FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#Code");
            }
            if (packageID - 1 == 4)
            {
                l1.FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#Orwell");
            }


            borderButton.Name = $"E{buttonName}";

            //Add the mouseEnter and mouseLeave event to the borderButton;
            borderButton.MouseEnter += BorderButton_MouseEnter;
            borderButton.MouseLeave += BorderButton_MouseLeave;

            //iets met stackpanel
            l1.Text = $"{name}: {exnum}";



            if (finished.Rows.Count > 0)
            {
                completedIcon.Visibility = Visibility.Visible;
                scroll++;
            }
            else
            {
                completedIcon.Visibility = Visibility.Hidden;
            }

            borderButton.PreviewMouseDown += B1_Click;

            return borderButton;
        }


        private void addButton()
        {
            int buttonName = 0;
            int exnum = 1;
            int scroll = 0;


            //The button gets added as frequently as needed. 
            foreach (DataRow exercise in dt.Rows)
            {
                int marginID;
                //Save the difficulty so that you can use it easily later
                int dif = (int)Int64.Parse(exercise["difficulty"].ToString()) - 1;
                string name = exercise["source"].ToString();

                int Name = c.ID($"SELECT userID FROM userTable WHERE username = '{ConfigurationManager.AppSettings["username"]}'");
                DataTable finished = c.PullData($"SELECT * from Result join exercise on result.exerciseID = exercise.exerciseID where Result.userID = {Name} AND Result.exerciseID = {exercise["exerciseID"]}");

                Border borderButton = getButton(dif, buttonName, finished, scroll, exnum, name, int.Parse(exercise["packageID"].ToString()));
                Border borderButtonAll = getButton(dif, buttonName, finished, scroll, exnum, name, int.Parse(exercise["packageID"].ToString()));


                if (finished.Rows.Count > 0)
                {
                    Border finishedButton = getButton(dif, buttonName, finished, scroll, exnum, name, int.Parse(exercise["packageID"].ToString()));
                    Grid_Finished.Children.Add(finishedButton);
                    Grid.SetRow(finishedButton, grid_Margin[grid_Margin.Count - 1][0]);
                    Grid.SetColumn(finishedButton, grid_Margin[grid_Margin.Count - 1][2] + 1);

                    grid_Margin[grid_Margin.Count - 1][2] += 2;
                    grid_Margin[grid_Margin.Count - 1][1]++;

                    if (grid_Margin[grid_Margin.Count - 1][1] % 4 == 0 && grid_Margin[grid_Margin.Count - 1][1] != 0)
                    {
                        grid_Margin[grid_Margin.Count - 1][0] += 2;
                        grid_Margin[grid_Margin.Count - 1][2] = 0;
                    }

                }

                //Get the pack of the exercise
                if ((int.Parse(exercise["packageID"].ToString()) == 1))
                {
                    grid_list[int.Parse(exercise["difficulty"].ToString())-1].Children.Add(borderButton);
                    marginID = int.Parse(exercise["difficulty"].ToString()) - 1;
                }
                else
                {
                    grid_list[int.Parse(exercise["packageID"].ToString())+1].Children.Add(borderButton);
                    marginID = int.Parse(exercise["packageID"].ToString()) + 1;
                }


                Grid.SetRow(borderButton, grid_Margin[marginID][0]);
                Grid.SetColumn(borderButton, grid_Margin[marginID][2] + 1);


                Grid.SetRow(borderButtonAll, grid_Margin[grid_Margin.Count - 2][0]);
                Grid.SetColumn(borderButtonAll, grid_Margin[grid_Margin.Count - 2][2] + 1);


                Grid_All.Children.Add(borderButtonAll);


                grid_Margin[marginID][2] += 2;
                grid_Margin[marginID][1]++;

                //Do it for all the exercises as well
                grid_Margin[grid_Margin.Count - 2][2] += 2;
                grid_Margin[grid_Margin.Count - 2][1]++;

                exnum++;

                buttonName++;

                if (grid_Margin[grid_Margin.Count - 2][1] % 4 == 0 && grid_Margin[grid_Margin.Count - 2][1] != 0)
                {
                    grid_Margin[grid_Margin.Count - 2][0] += 2;
                    grid_Margin[grid_Margin.Count - 2][2] = 0;
                }


                if (grid_Margin[marginID][1] % 4 == 0 && grid_Margin[marginID][1] != 0)
                {
                    grid_Margin[marginID][0] += 2;
                    grid_Margin[marginID][2] = 0;
                }

            }

        }

        //If the mouse leaves the exercise
        private void BorderButton_MouseLeave(object sender, MouseEventArgs e)
        {
            Border selectedExercise = (Border)sender;

            if (selectedBorderButton != selectedExercise)
            {
                selectedExercise.Background.Opacity = backgroundOpacity;
            }
        }

        //If the mouse is over the exercise
        private void BorderButton_MouseEnter(object sender, MouseEventArgs e)
        {
            Border selectedExercise = (Border)sender;
            selectedExercise.Background.Opacity = selectedOpacity;
        }


        //This checks which buttons has been clicked.
        private void B1_Click(object sender, RoutedEventArgs e)
        {

            Border b = (Border)sender;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (b.Name.Equals($"E{i}"))
                {


                    //Make the previously selected border the right opacity again if its not this button.
                    if (selectedBorderButton != null && selectedBorderButton != b)
                    {
                        selectedBorderButton.Background.Opacity = backgroundOpacity;
                    }
                    selectedBorderButton = b;
                    exerciseDetails_grid.Visibility = Visibility.Visible;
                    Console.WriteLine(i);
                    DataRow exercise = dt.Rows[i];
                    this.Exercise.Text = $"Exercise {i + 1}";

                    //Gets all the data from the database relating to the exercises.
                    DataTable finished = new DataTable();
                    int Name = c.ID($"SELECT userID FROM userTable WHERE username = '{ConfigurationManager.AppSettings["username"]}'");
                    finished = c.PullData($"SELECT * from Result join exercise on result.exerciseID = exercise.exerciseID where Result.userID = {Name} AND Result.exerciseID = {exercise["exerciseID"]}");

                    //Tries to place the previous results if available.
                    try
                    {
                        this.wpm_number.Content = $"{finished.Rows[finished.Rows.Count - 1][3]}";
                        this.cpm_number.Content = $"{finished.Rows[finished.Rows.Count - 1][4]}";
                        this.error_number.Content = $"{finished.Rows[finished.Rows.Count - 1][1]}";
                    }
                    catch (IndexOutOfRangeException)
                    {
                        this.wpm_number.Content = $"0";
                        this.cpm_number.Content = $"0";
                        this.error_number.Content = $"0";
                    }

                    this.Description.Text = exercise["text"].ToString();
                    this.tekst = exercise["text"].ToString();
                    this.exerciseID = int.Parse(exercise["exerciseID"].ToString());
                    this.level.Text = $"Level: {exercise["difficulty"]}";


                    if ((int)Int64.Parse(exercise["difficulty"].ToString()) == 1)
                    {
                        this.level.Text = "Level: Amateur";
                    }
                    if ((int)Int64.Parse(exercise["difficulty"].ToString()) == 2)
                    {
                        this.level.Text = "Level: Normal";
                    }
                    if ((int)Int64.Parse(exercise["difficulty"].ToString()) == 3)
                    {
                        this.level.Text = "Level: Expert";
                    }
                    this.Origin.Content = exercise["source"];

                }
            }

        }

        private void timer_Tick(object sender, EventArgs e)
        {

        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }


        private void TabControl_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ThemedIconButton_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Application.Current.MainWindow.Content = new HomeScreen();
        }


        //Cecks if there are letters in the textbox.
        bool IsDigitsOnly(string str)
        {
            foreach (char c in str)
            {
                if (c < '0' || c > '9')
                    return false;
            }

            return true;
        }

        //The functionality for the start generated excersize button
        private void StartGeneratedExercise_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //Class that generates text based on the users flaws
            WordGenerator Generator = new WordGenerator();
            int limit = 75;
            string errorMsg;
            if(radioWord.IsChecked == true)
            {
                if (IsDigitsOnly(txtWords.Text) && txtWords.Text != "" && txtWords.Text != null)
                {
                    int amount = int.Parse(txtWords.Text);
                    if(amount <= 0)
                    {
                        errorMsg = $"The amount of words can't be negatif or zero";
                        lblError.Content = errorMsg;
                    }
                    else if(amount > limit)
                    {
                        errorMsg = $"The amount of words can't be more then {limit} words";
                        lblError.Content = errorMsg;
                    }
                    else if(amount <= limit)
                    {
                        string generated = Generator.startupRandomText(amount, false);
                        Application.Current.MainWindow.Content =  new Exercise(generated, false, 999);
                    }
                }
                else {
                    
                    errorMsg = "Please use numbers to indicate the amount of words needed";
                    lblError.Content = errorMsg;
                }
            }
            else if(radioTime.IsChecked == true)
            {
                string generated = Generator.startupRandomText();
                Exercise exercise = new Exercise(generated, false, 999);
                switch (cmbTime.SelectedIndex)
                {
                    case 0:
                        exercise.SetCountdown(30);
                        Application.Current.MainWindow.Content = exercise;
                        break;
                    case 1:
                        exercise.SetCountdown(60);
                        Application.Current.MainWindow.Content = exercise;
                        break;
                    case 2:
                        exercise.SetCountdown(180);
                        Application.Current.MainWindow.Content = exercise;
                        break;
                    case 3:
                        exercise.SetCountdown(300);
                        Application.Current.MainWindow.Content = exercise;
                        break;
                }
            }
            else
            {
                errorMsg = "Please select one of the options above";
                lblError.Content = errorMsg;
            }
        }

        private void StartExercise_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Text.IsChecked == true)
            {
                Application.Current.MainWindow.Content = new Exercise(tekst, specialChar.IsChecked.Value, this.exerciseID);
            }
            else
            {
                Application.Current.MainWindow.Content = new VoiceExercise(tekst, exerciseID);

            }
        }

        //Enables and Disables inputs specific to the radiobutton it serves 
        private void RadioSwitch(object sender, RoutedEventArgs e)
        {
            if(radioWord.IsChecked == true)
            {
                txtWords.IsEnabled = true;
                cmbTime.IsEnabled = false;
            }
            else if (radioTime.IsChecked == true)
            {
                cmbTime.IsEnabled = true;
                txtWords.IsEnabled = false;
            }
        }

        //starts quickfiremode
        int difficultyTyping = 2;
        int difficultyReading = 2;
        
        private void Retry_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Application.Current.MainWindow.Content = new Quick_Fire(difficultyTyping, difficultyReading);
        }

            private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            difficultyTyping = 1;
        }

        private void RadioButton_Checked_1(object sender, RoutedEventArgs e)
        {
            difficultyTyping = 2;
        }

        private void RadioButton_Checked_2(object sender, RoutedEventArgs e)
        {
            difficultyTyping = 4;
        }

        private void RadioButton_Checked_3(object sender, RoutedEventArgs e)
        {
            difficultyReading = 1;
        }

        private void RadioButton_Checked_4(object sender, RoutedEventArgs e)
        {
            difficultyReading = 2;
        }

        private void RadioButton_Checked_5(object sender, RoutedEventArgs e)
        {
            difficultyReading = 4;
        }
    }
}
