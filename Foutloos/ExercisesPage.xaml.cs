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
            int x = 1;

            for (int i = 0; i < 40; i+= 2)
            {
                       
                Button b1 = new Button();
                Grid.SetColumn(b1, i-1);
                Grid.SetRow(b1, x-1);
                b1.Background = Brushes.Gray;
                b1.Name = $"E{i}";
                //                    Grid_All.Children.Add(new Button() { Name = $"E{i}", Background = Brushes.Gray });                                
                Grid_All.Children.Add(b1);
                             
                if (i % 4 == 0)
                {
                    x+=2;
                }                                     
                
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            HomeScreen.owner.Content = new HomeScreen(HomeScreen.owner);
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

        private void FillDataGrid()
        {
            
            //query that is being executed and being shows in a Table in the application.
            List<List<object>> result = c.QueryDataExercisesTable("SELECT * FROM Exercises");
            string waardes = "";

            for (int i = 0; i < result.Count; i++)
            {
                for (int x = 0; x < result[i].Count; x++)
                {
                    waardes += result[i][x] + " ";
                }
            }

            

            





        }

        private void TabControl_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
