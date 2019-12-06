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
        private int amount;
        private List<List<DataRow>> exercises = new List<List<DataRow>>();

        public ExercisesPage()
        {
            InitializeComponent();
            AddButton();

        } 
        
        private void AddButton()
        {
            Connection c = new Connection();
            DataTable dt = new DataTable();
            
            dt = c.PullData("SELECT * FROM Exercise LEFT JOIN Package ON Exercise.exerciseID = Package.packageID WHERE Exercise.packageID = 1");


            //Create all the lists of exercises and add them to the main list (exercises)
            //In this list a certain order is used, amateurExercises gets index 0, normal 1, expert 2 and all 3, 
            List<DataRow> amateurExercises = new List<DataRow>();
            List<DataRow> normalExercises = new List<DataRow>();
            List<DataRow> expertExercises = new List<DataRow>();
            List<DataRow> allExercises = new List<DataRow>();

            exercises.Add(amateurExercises);
            exercises.Add(normalExercises);
            exercises.Add(expertExercises);
            exercises.Add(allExercises);

            int selected = 0;           
            int finished = 0;            
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

            //The standard left and top margin are added for grid Selected for you.
            Grid_Selected.ShowGridLines = false;
            Grid_Selected.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50) });
            Grid_Selected.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(50) });

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

            calculate(amount, "Grid_All");
            calculate(selected, "Grid_Selected");
            calculate(exercises[0].Count, "Grid_Amateur");
            calculate(exercises[1].Count, "Grid_Normal");
            calculate(exercises[2].Count, "Grid_Expert");
            calculate(finished, "Grid_Finished");


        }

        private void calculate(int amount, string gridName)
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
                if (gridName == "Grid_Selected")
                {
                    Grid_Selected.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(252) });
                    Grid_Selected.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(50) });
                }
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
            for (int z = 0; z < amount; z++)
            {
                Button b1 = new Button();
                Label l1 = new Label();                                


                Grid.SetColumn(b1, j + 1);

                
                //iets met stackpanel

                Grid.SetRow(b1, x);
                b1.Background = Brushes.White;
                b1.Name = $"E{i}";
                b1.Content = $"Excercise: {exnum}";
                b1.Foreground = Brushes.Black;
                b1.BorderBrush = Brushes.Black;
                //b1.BorderThickness



                if (gridName == "Grid_All")
                {
                    Grid_All.Children.Add(b1);
                    b1.Click += (sender, e ) => B1_Click(sender, e, 3);
                }
                if (gridName == "Grid_Selected")
                {
                    Grid_Selected.Children.Add(b1);
                    b1.Click += (sender, e) => B1_Click(sender, e, 5);
                }
                if (gridName == "Grid_Amateur")
                {
                    Grid_Amateur.Children.Add(b1);
                    b1.Click += (sender, e) => B1_Click(sender, e, 0);
                    b1.Background = Brushes.LightGreen;
                }
                if (gridName == "Grid_Normal")
                {
                    Grid_Normal.Children.Add(b1);
                    b1.Click += (sender, e) => B1_Click(sender, e, 1);
                    b1.Background = Brushes.DarkOrange;
                }
                if (gridName == "Grid_Expert")
                {
                    Grid_Expert.Children.Add(b1);
                    b1.Click += (sender, e) => B1_Click(sender, e, 2);
                    b1.Background = Brushes.Red;
                }
                if (gridName == "Grid_Finished")
                {
                    Grid_Finished.Children.Add(b1);
                    b1.Click += (sender, e) => B1_Click(sender, e, 4);
                }

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
                if (gridName == "Grid_Selected")
                {
                    Grid_Selected.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(200) });
                    Grid_Selected.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50) });

                }
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

        
        //This checks which buttons has been clicked.
        private void B1_Click(object sender, RoutedEventArgs e, int difficulty)
        {
            Button b = (Button)sender;
            for (int i = 0; i <= exercises[difficulty].Count; i++)
            {              
                if (b.Name.Equals($"E{i}"))
                {
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
                Application.Current.MainWindow.Content = new Exercise(tekst, true);
            }
            else
            {
                Application.Current.MainWindow.Content = new VoiceExercise(tekst, exerciseID);
            }
        }
    }
}
