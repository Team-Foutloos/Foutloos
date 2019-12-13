using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
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
        Thread myThread;

        public ExercisesPage()
        {
            Modals.loadingModal loadingIndicator = new Modals.loadingModal();
            InitializeComponent();
            loadingIndicator.Show();
            AddButton();
            loadingIndicator.Close();
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

                exercises[((int)Int64.Parse(difficulty)) - 1].Add(row);
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

            calculate(3, "Grid_All", amount);
            //calculate(selected, "Grid_Selected");
            calculate(0, "Grid_Amateur", exercises[0].Count);
            calculate(1, "Grid_Normal", exercises[1].Count);
            calculate(2, "Grid_Expert", exercises[2].Count);
            calculate(3, "Grid_Finished", amount);


        }

        private void calculate(int difficulty, string gridName, int amount)
        {

            int exnum = 1;
            int x = 1;
            int j = 0;
            int i = 0;
            int scroll = 0;


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
                //Save the difficulty so that you can use it easily later
                int dif = (int)Int64.Parse(exercise["difficulty"].ToString()) - 1;


                //Create the main button.
                BorderButton button = new BorderButton(dif);
                Border borderButton = button.getButton();
                Grid borderGrid = (Grid) borderButton.Child;
                Image completedIcon = (Image) borderGrid.Children[2];
                TextBlock l1 = (TextBlock)borderGrid.Children[0];

                int Name = c.ID($"SELECT userID FROM userTable WHERE username = '{ConfigurationManager.AppSettings["username"]}'");


                DataTable finished = new DataTable();                
                finished = c.PullData($"SELECT * from Result join exercise on result.exerciseID = exercise.exerciseID where Result.userID = {Name} AND Result.exerciseID = {exercise["exerciseID"]}");

                if (finished.Rows.Count > 0)
                {
                    completedIcon.Visibility = Visibility.Visible;
                    scroll++;
                }
                else
                {
                    completedIcon.Visibility = Visibility.Hidden;
                }

                

                
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
                    if (finished.Rows.Count > 0)
                    {
                        Grid_Finished.Children.Add(borderButton);
                        borderButton.PreviewMouseDown += (sender, e) => B1_Click(sender, e, 4);
                        //The position is always 1,1, 3,1, 5,1 etc. Therefore There is always 2 added for j.
                        j += 2;
                        i++;
                        exnum++;
                    }
                }
                else
                {
                    //The position is always 1,1, 3,1, 5,1 etc. Therefore There is always 2 added for j.
                    j += 2;
                    i++;
                    exnum++;
                }
                
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
            }
            for (int row = 0; row < Math.Ceiling((double)scroll / 4); row++)
            {
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

                    //Gets all the data from the database relating to the exercises.
                    DataTable finished = new DataTable();
                    int Name = c.ID($"SELECT userID FROM userTable WHERE username = '{ConfigurationManager.AppSettings["username"]}'");
                    finished = c.PullData($"SELECT * from Result join exercise on result.exerciseID = exercise.exerciseID where Result.userID = {Name} AND Result.exerciseID = {exercise["exerciseID"]}");

                    //Tries to place the previous results if available.
                    try
                    {
                        this.wpm_number.Content = $"{finished.Rows[finished.Rows.Count-1][3]}";
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


        //Cecked als er geen letters in txtAmount zit.
        bool IsDigitsOnly(string str)
        {
            foreach (char c in str)
            {
                if (c < '0' || c > '9')
                    return false;
            }

            return true;
        }

        private void StartGeneratedExercise_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            string errorMsg;
            if(radioWord.IsChecked == true)
            {
                if (IsDigitsOnly(txtWords.Text) && txtWords.Text != "" && txtWords.Text != null)
                {
                    int amount = int.Parse(txtWords.Text);
                    if(amount < 250)
                    {
                        startupRandomText(amount);
                    }
                    else
                    {
                        errorMsg = "The amount of words can't be more then 250 words";
                        lblError.Content = errorMsg;
                    }
                }
                else {
                    
                    errorMsg = "Please use numbers to indicate the amount of words needed";
                    lblError.Content = errorMsg;
                }
            }
            else if(radioTime.IsChecked == true)
            {
               
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

        //Randomly generate a text based on the users flaws
        private void startupRandomText()
        {

            Connection c = new Connection();

            string exerciseText = "";

            DataTable mostMistakes = new DataTable();
            DataTable dt0 = new DataTable();

            mostMistakes = c.PullData("SELECT TOP 1 letter FROM Result R RIGHT JOIN Usertable U On R.userID = U.userID " +
                $"JOIN Error E ON R.resultID = E.resultID WHERE username = '{ConfigurationManager.AppSettings["username"]}' AND letter NOT LIKE '% %' " +
                $"GROUP BY letter ORDER BY SUM(count) DESC");

            dt0 = c.PullData($"SELECT * FROM dictionary WHERE list LIKE '%{mostMistakes.Rows[0]["letter"]}%'");
            Random rand = new Random();

            for (int i = 0; i < 20; i++)
            {
                exerciseText += dt0.Rows[rand.Next(0, dt0.Rows.Count)]["list"].ToString();
                if (i != 19)
                {
                    exerciseText += " ";
                }
            }
            Application.Current.MainWindow.Content = new Exercise(exerciseText, false, 999);
        }

        private void startupRandomText(int value)
        {
            Connection c = new Connection();

            string exerciseText = "";

            DataTable mostMistakes = new DataTable();
            DataTable dt0 = new DataTable();

            mostMistakes = c.PullData("SELECT TOP 1 letter FROM Result R RIGHT JOIN Usertable U On R.userID = U.userID " +
                $"JOIN Error E ON R.resultID = E.resultID WHERE username = '{ConfigurationManager.AppSettings["username"]}' AND letter NOT LIKE '% %' " +
                $"GROUP BY letter ORDER BY SUM(count) DESC");

            dt0 = c.PullData($"SELECT * FROM dictionary WHERE list LIKE '%{mostMistakes.Rows[0]["letter"]}%'");
            Random rand = new Random();

            for (int i = 0; i < value; i++)
            {
                exerciseText += dt0.Rows[rand.Next(0, dt0.Rows.Count)]["list"].ToString();
                if (i != value-1)
                {
                    exerciseText += " ";
                }
            }
            Application.Current.MainWindow.Content = new Exercise(exerciseText, false, 999);
        }

        //For the randomly generated exercise
        private void ThemedButton_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {


            Connection c = new Connection();

            string exerciseText = "";

            DataTable mostMistakes = new DataTable();
            DataTable dt0 = new DataTable();

            mostMistakes = c.PullData("SELECT TOP 1 letter FROM Result R RIGHT JOIN Usertable U On R.userID = U.userID " +
                $"JOIN Error E ON R.resultID = E.resultID WHERE username = '{ConfigurationManager.AppSettings["username"]}' AND letter NOT LIKE '% %' " +
                $"GROUP BY letter ORDER BY SUM(count) DESC");

            dt0 = c.PullData($"SELECT * FROM dictionary WHERE list LIKE '%{mostMistakes.Rows[0]["letter"]}%'");
            Random rand = new Random();

            for (int i = 0; i < 20; i++)
            {
                exerciseText += dt0.Rows[rand.Next(0, dt0.Rows.Count)]["list"].ToString();
                if (i != 19)
                {
                    exerciseText += " ";
                }
            }
            Application.Current.MainWindow.Content = new Exercise(exerciseText, false, 999);
        }

        private void a(object sender, RoutedEventArgs e)
        {
            if(radioWord.IsChecked == true)
            {
                txtWords.IsEnabled = true;
                cmbTime.IsEnabled = false;
                Console.WriteLine("a");
            }
            else if (radioTime.IsChecked == true)
            {
                cmbTime.IsEnabled = true;
                txtWords.IsEnabled = false;
                Console.WriteLine("b");
            }
        }
    }
}
