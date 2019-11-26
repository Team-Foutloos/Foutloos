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

        //private bool text = false;
        //private bool spoken = false;
      
        public ExercisesPage()
        {
            InitializeComponent();
            FillDataGrid();
            
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
            string connectionstring = "Data Source=127.0.0.1,1433; User Id=sa;Password=Foutloos!; Initial Catalog=foutloos_db;";
            string CmdString = string.Empty;
            using (SqlConnection con = new SqlConnection(connectionstring))
            {
                CmdString = "SELECT * FROM Usertable";
                SqlCommand cmd = new SqlCommand(CmdString, con);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("SQLdata");
                sda.Fill(dt);
                SQLdata.ItemsSource = dt.DefaultView;
            }
        }




    }
}
