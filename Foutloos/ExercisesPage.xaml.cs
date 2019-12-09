using System;
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

        private string tekst;
        private int exerciseID;
        private int amount = 0;
        
        private List<List<DataRow>> exercises = new List<List<DataRow>>();
        List<DataRow> amateurExercises = new List<DataRow>();
        List<DataRow> normalExercises = new List<DataRow>();
        List<DataRow> expertExercises = new List<DataRow>();
        List<DataRow> allExercises = new List<DataRow>();
        List<DataRow> finished = new List<DataRow>();
        Connection c = new Connection();
        private Border selectedBorderButton;
        private double selectedOpacity = .3;
        private double backgroundOpacity = .5;

        public ExercisesPage()
        {
            InitializeComponent();
            
            AddButton();

        } 

        private void AddDLC()
        {

            try
            {
                List<int> packages = new List<int>();
                DataTable dt = new DataTable();
                
                //checks how many and which packages are connected to the logged in account.
                packages = c.getPackages($"select packageID from Usertable join License on Usertable.userID = license.userID where Usertable.username = '{ConfigurationManager.AppSettings["username"]}'");

                //Adds the packages for all the packages available in the account.
                foreach (int i in packages)
                {
                    dt = c.PullData($"SELECT * FROM Exercise LEFT JOIN Package ON Exercise.exerciseID = Package.packageID WHERE Exercise.packageID = {i}");

                    exercises.Add(amateurExercises);
                    exercises.Add(normalExercises);
                    exercises.Add(expertExercises);
                    exercises.Add(allExercises);
                    exercises.Add(finished);
                                                         
                    foreach (DataRow row in dt.Rows)
                    {
                        string ID = row["exerciseID"].ToString();
                        string text = row["text"].ToString();
                        string difficulty = row["difficulty"].ToString();

                        exercises[((int)Int64.Parse(difficulty)) - 1].Add(row);
                        exercises[3].Add(row);
                        amount++;

                    }
                }
                
            }
            catch (Exception e)
            {
                
            }
            
        }
        
        private void AddButton()
        {            
            DataTable dt = new DataTable();
            
            //standard package that always gets added.
            dt = c.PullData($"SELECT * FROM Exercise LEFT JOIN Package ON Exercise.exerciseID = Package.packageID WHERE Exercise.packageID = 1");

            //Create all the lists of exercises and add them to the main list (exercises)
            //In this list a certain order is used, amateurExercises gets index 0, normal 1, expert 2 and all 3, 
            

            exercises.Add(amateurExercises);
            exercises.Add(normalExercises);
            exercises.Add(expertExercises);
            exercises.Add(allExercises);
            exercises.Add(finished);
                            
                      
            amount = 0;
            
            foreach (DataRow row in dt.Rows)
            {
                string ID = row["exerciseID"].ToString();
                string text = row["text"].ToString();
                string difficulty = row["difficulty"].ToString();

                exercises[((int)Int64.Parse(difficulty))-1].Add(row);
                exercises[3].Add(row);
                amount++;        
                
            }

            //The standard left and top margin are added for grid All.
            Grid_All.ShowGridLines = false;
            Grid_All.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50) });
            Grid_All.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(50) });

            ////The standard left and top margin are added for grid Selected for you.
            //Grid_Selected.ShowGridLines = false;
            //Grid_Selected.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50) });
            //Grid_Selected.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(50) });

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

            AddDLC();

            calculate(3, "Grid_All");
            //calculate(selected, "Grid_Selected");
            calculate(0, "Grid_Amateur");
            calculate(1, "Grid_Normal");
            calculate(2, "Grid_Expert");
            calculate(4, "Grid_Finished");
            


        }

        private void calculate(int difficulty, string gridName)
        {

            int exnum = 1;
            int x = 1;
            int j = 0;
            int i = 0;


            // The amount of columns is always the same.Therefore this piece of code adds them.
            for (int h = 0; h < 4; h++)
            {
                if (gridName == "Grid_All")
                {
                    Grid_All.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(252) });
                    Grid_All.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(50) });
                }
                //if (gridName == "Grid_Selected")
                //{
                //    Grid_Selected.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(252) });
                //    Grid_Selected.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(50) });
                //}
                if (gridName == "Grid_Amateur")
                {
                    Grid_Amateur.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(252) });
                    Grid_Amateur.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(50) });
                }
                if (gridName == "Grid_Normal")
                {
                    Grid_Normal.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(252) });
                    Grid_Normal.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(50) });
                }
                if (gridName == "Grid_Expert")
                {
                    Grid_Expert.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(252) });
                    Grid_Expert.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(50) });
                }
                if (gridName == "Grid_Finished")
                {
                    Grid_Finished.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(252) });
                    Grid_Finished.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(50) });
                }

            }


            //The button gets added as frequently as needed. 
            foreach (var exercise in exercises[difficulty])
            {
                TextBlock l1 = new TextBlock();
                Grid borderButtonGrid = new Grid() {Margin=new Thickness(10)};
                TextBlock level = new TextBlock();

                //Get the completed logo
                BitmapImage logo = new BitmapImage();
                logo.BeginInit();
                logo.UriSource = new Uri(@"/assets/tick.png", UriKind.RelativeOrAbsolute);
                logo.EndInit();

                Image completedIcon = new Image();
                completedIcon.Source = logo;
                completedIcon.Width = 40;
                completedIcon.Opacity = .4;
                Border borderButton = new Border();
                var availableColors = new List<Color>();

                //Save the difficulty so that you can use it easily laser
                int dif = (int)Int64.Parse(exercise["difficulty"].ToString()) - 1;

                //Set all the button colors
                var allColor = Color.FromRgb(0, 102, 255);
                var easyColor = Color.FromRgb(51, 204, 51);
                var mediumColor = Color.FromRgb(255, 153, 0);
                var hardColor = Color.FromRgb(204, 0, 0);
                //Put all of the colors in a list so they can be easily picked out later.
                availableColors.Add(easyColor);
                availableColors.Add(mediumColor);
                availableColors.Add(hardColor);


                //Set all the properties for the label.
                borderButton.Child = borderButtonGrid;

                //Set the properties for the children of the grid of the borderbutotn.
                borderButtonGrid.Children.Add(l1);
                borderButtonGrid.Children.Add(level);
                borderButtonGrid.Children.Add(completedIcon);


                borderButton.CornerRadius = new CornerRadius(10);
                borderButton.BorderBrush = new SolidColorBrush(availableColors[dif]) { Opacity = 1 };
                borderButton.BorderThickness = new Thickness(5);
                borderButton.Background = new SolidColorBrush(availableColors[dif]) { Opacity = backgroundOpacity };
                level.FontSize = 15;
                level.HorizontalAlignment = HorizontalAlignment.Left;
                level.VerticalAlignment = VerticalAlignment.Bottom;
                l1.HorizontalAlignment = HorizontalAlignment.Center;
                l1.VerticalAlignment = VerticalAlignment.Center;
                completedIcon.HorizontalAlignment = HorizontalAlignment.Right;
                completedIcon.VerticalAlignment = VerticalAlignment.Bottom;
                Grid.SetColumn(borderButton, j + 1);

                //Add the mouseEnter and mouseLeave event to the borderButton;
                borderButton.MouseEnter += BorderButton_MouseEnter;
                borderButton.MouseLeave += BorderButton_MouseLeave;

                
                    //Add the right color to the borders according to the level
                    borderButton.PreviewMouseDown += (sender, e) => B1_Click(sender, e, dif);

                //iets met stackpanel
                Grid.SetRow(borderButton, x);
                borderButton.Name = $"E{i}";
                l1.Text = $"Excercise: {exnum}";

                //Set the levelText
                switch (dif)
                {
                    case 0:
                        level.Text = "Amateur";
                        break;
                    case 1:
                        level.Text = "Normal";
                        break;
                    case 2:
                        level.Text = "Expert";
                        break;
                }

                //b1.BorderThickness

                //if (gridName == "Grid_Selected")
                //{
                //    Grid_Selected.Children.Add(b1);
                //    b1.Click += (sender, e) => B1_Click(sender, e, 5);
                //}
                if (gridName == "Grid_All")
                {
                    Grid_All.Children.Add(borderButton);
                    borderButton.PreviewMouseDown += (sender, e) => B1_Click(sender, e, 3);
                }
                if (gridName == "Grid_Amateur")
                {
                    Grid_Amateur.Children.Add(borderButton);
                }
                if (gridName == "Grid_Normal")
                {
                    Grid_Normal.Children.Add(borderButton);
                }
                if (gridName == "Grid_Expert")
                {
                    Grid_Expert.Children.Add(borderButton);
                }
                if (gridName == "Grid_Finished")
                {
                    Grid_Finished.Children.Add(borderButton);
                    borderButton.PreviewMouseDown += (sender, e) => B1_Click(sender, e, 4);
                }

                //if ((int)Int64.Parse(exercise[difficulty].ToString()) == 4)
                //{
                //    Grid_Finished.Children.Add(borderButton);
                //    borderButton.PreviewMouseDown += (sender, e) => B1_Click(sender, e, 4);
                //}

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
                if (gridName == "Grid_All")
                {
                    Grid_All.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(200) });
                    Grid_All.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50) });

                }
                //if (gridName == "Grid_Selected")
                //{
                //    Grid_Selected.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(200) });
                //    Grid_Selected.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50) });

                //}
                if (gridName == "Grid_Amateur")
                {
                    Grid_Amateur.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(200) });
                    Grid_Amateur.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50) });

                }
                if (gridName == "Grid_Normal")
                {
                    Grid_Normal.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(200) });
                    Grid_Normal.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50) });

                }
                if (gridName == "Grid_Expert")
                {
                    Grid_Expert.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(200) });
                    Grid_Expert.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50) });

                }
                if (gridName == "Grid_Finished")
                {
                    Grid_Finished.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(200) });
                    Grid_Finished.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50) });

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
        private void B1_Click(object sender, RoutedEventArgs e, int difficulty)
        {
            Border b = (Border)sender;
            for (int i = 0; i < exercises[difficulty].Count; i++)
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
                    DataRow exercise = exercises[difficulty][i];
                    this.Exercise.Text = $"Exercise {i+1}";
                    this.wpm_number.Content = "0";
                    this.cpm_number.Content = "0";
                    this.error_number.Content = "0";
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
    }
}
