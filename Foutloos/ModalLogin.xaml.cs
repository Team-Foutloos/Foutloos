using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace Foutloos
{
    /// <summary>
    /// Interaction logic for ModalLogin.xaml
    /// </summary>
    public partial class ModalLogin : Window
    {
        public ModalLogin()
        {
            InitializeComponent();
        }

        private void Login_MouseDown(object sender, MouseButtonEventArgs e)
        {
            string error = "";
            if (username.Text.Length == 0)
            {
                error = "Please enter a username.";
            }
            else if (password.Password.Length == 0)
            {
                error = "Please enter a password.";
            }
            else
            {
                string hashedPassword = SecurePasswordHasher.Hash(password.Password);
                //query that is being executed and being shows in a Table in the application.
                string connectionstring = "Data Source=127.0.0.1,1433; User Id=sa;Password=Foutloos!; Initial Catalog=foutloos_db;";
                string CmdString = $"SELECT * FROM Usertable WHERE username = @username";
                using (SqlConnection con = new SqlConnection(connectionstring))
                {
                    con.Open();
                    SqlCommand insCmd = new SqlCommand(CmdString, con);
                    // use sqlParameters to prevent sql injection!
                    insCmd.Parameters.AddWithValue("@username", username.Text);
                    insCmd.Parameters.AddWithValue("@password", hashedPassword);
                    using (SqlDataReader reader = insCmd.ExecuteReader())
                    {
                        if (reader.Read() && SecurePasswordHasher.Verify(password.Password, (string) reader["password"]))
                        {
                            Console.WriteLine("Succes!");
                        }
                        else
                        {
                            Console.WriteLine("Nothing found.");
                        }
                    }
                    con.Close();
                }
                //The check with the database has to be implemented here.
                error = "Succesfull!";
                errorMessage.Foreground = new SolidColorBrush(Colors.Green);
                this.Close();
            }
            errorMessage.Content = error;
        }

        private void Cancel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }
    }
}
