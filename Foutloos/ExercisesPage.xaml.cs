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

        private List<string> difficultyData = new List<string>();
        private List<string> textData = new List<string>();
        private string tekst;       
        
        public ExercisesPage()
        {
            InitializeComponent();
            AddButton();

        } 
        
        private void AddButton()
        {
            Connection c = new Connection();
            DataTable dt = new DataTable();
            
            dt = c.PullData("SELECT * FROM Exercises");
                       

            int selected = 0;
            int amateur = 0;
            int normal = 0;
            int expert = 0;
            int finished = 0;            
            int amount = 0;
            
            foreach (DataRow row in dt.Rows)
            {
                string ID = row["exerciseID"].ToString();
                string text = row["text"].ToString();
                textData.Add(text);
                string difficulty = row["difficulty"].ToString();
                difficultyData.Add(difficulty);
                string done = row["finished"].ToString();

                if (difficulty == "1")
                {
                    amateur++;
                    amount++;
                }else
                if (difficulty == "2")
                {
                    normal++;
                    amount++;
                }
                else
                if (difficulty == "3")
                {
                    expert++;
                    amount++;
                }               
                
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
            calculate(amateur, "Grid_Amateur");
            calculate(normal, "Grid_Normal");
            calculate(expert, "Grid_Expert");
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

                b1.Click += B1_Click;

                Grid.SetColumn(b1, j + 1);

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
                }
                if (gridName == "Grid_Selected")
                {
                    Grid_Selected.Children.Add(b1);
                }
                if (gridName == "Grid_Amateur")
                {
                    Grid_Amateur.Children.Add(b1);
                }
                if (gridName == "Grid_Normal")
                {
                    Grid_Normal.Children.Add(b1);
                }
                if (gridName == "Grid_Expert")
                {
                    Grid_Expert.Children.Add(b1);
                }
                if (gridName == "Grid_Finished")
                {
                    Grid_Finished.Children.Add(b1);
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
        private void B1_Click(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;            

            for (int i = 0; i <= textData.Count; i++)
            {              
                if (b.Name.Equals($"E{i}"))
                {
                    this.Exercise.Text = $"Exercise {i+1}";
                    this.wpm_number.Content = "0";
                    this.cpm_number.Content = "0";
                    this.error_number.Content = "0";
                    this.Description.Content = textData[i];
                    this.tekst = textData[i];
                    this.level.Text = $"Level: {difficultyData[i]}";

                }
            }           

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.Content = new HomeScreen();
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

       

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

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

            if (Text.IsChecked == true)
            {
                Application.Current.MainWindow.Content = new Exercise(tekst);                
            }
            else
            {
                Application.Current.MainWindow.Content = new VoiceExercise(tekst);
            }

            
        }
        
    }
}
