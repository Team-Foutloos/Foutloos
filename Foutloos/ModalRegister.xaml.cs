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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Foutloos
{
    /// <summary>
    /// Interaction logic for ModalLogin.xaml
    /// </summary>
    public partial class ModalRegister : Window
    {
        public ModalRegister()
        {
            InitializeComponent();
        }


        private void Password_TextChanged(object sender, KeyEventArgs e)
        {
            PasswordBox passwordBox = (PasswordBox)sender;
            if (passwordBox.Password.Length == 20 && e.Key != Key.Escape)
            {

                Storyboard myStoryboard = (Storyboard)passwordBox.Resources["TestStoryboard"];
                Storyboard.SetTarget(myStoryboard.Children.ElementAt(0) as DoubleAnimationUsingKeyFrames, passwordBox);
                myStoryboard.Begin();
            }
        }

        private void Username_TextChanged_1(object sender, KeyEventArgs e)
        {
            TextBox box = (TextBox)sender;
            if (box.Text.Length == 12 && e.Key != Key.Escape)
            {
                Storyboard myStoryboard = (Storyboard)box.Resources["TestStoryboard"];
                Storyboard.SetTarget(myStoryboard.Children.ElementAt(0) as DoubleAnimationUsingKeyFrames, box);
                myStoryboard.Begin();
            }
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox box = (TextBox)sender;
            if (box.Text.Length == 5 && e.Key != Key.Escape)
            {
                Storyboard myStoryboard = (Storyboard)box.Resources["TestStoryboard"];
                Storyboard.SetTarget(myStoryboard.Children.ElementAt(0) as DoubleAnimationUsingKeyFrames, box);
                myStoryboard.Begin();
            }
        }

        //These functions are responsible for the number of the counter next to the textfields.

        private void Username_TextChanged(object sender, TextChangedEventArgs e)
        {
            usernameLength.Content = 12 - username.Text.Length;
        }

        private void Password_PasswordChanged(object sender, RoutedEventArgs e)
        {
            passwordLength.Content = 20 - password.Password.Length;
        }


        private void Register_MouseDown(object sender, MouseButtonEventArgs e)
        {
            string errorMessage = "";


            if (username.Text.Length <= 5)
            {
                errorMessage += "Username is too short, ";
            }
            else if (password.Password.Length <= 8)
            {
                errorMessage += "Password is too short, ";
            }
            else if (password.Password.Length > 8 && !(password.Password.Equals(passwordRepeat.Password))){
                errorMessage += "Passwords don't match";
            }
            else if (license.Text.Length < 5)
            {
                errorMessage += "This license is not in use";
            }
            else
            {
                //query that is being executed and being shows in a Table in the application.
                string connectionstring = "Data Source=127.0.0.1,1433; User Id=sa;Password=Foutloos!; Initial Catalog=foutloos_db;";
                string CmdString = $"INSERT INTO Usertable VALUES (@username, @password, @license)";
                using (SqlConnection con = new SqlConnection(connectionstring))
                {
                    con.Open();
                    SqlCommand insCmd = new SqlCommand(CmdString, con);
                    // use sqlParameters to prevent sql injection!
                    insCmd.Parameters.AddWithValue("@username", username.Text);
                    insCmd.Parameters.AddWithValue("@password", password.Password);
                    insCmd.Parameters.AddWithValue("@license", license.Text);
                    int affectedRows = insCmd.ExecuteNonQuery();
                    MessageBox.Show(affectedRows + " rows inserted!");
                }
                ErrorMessage.Foreground = new SolidColorBrush(Colors.Green);
                errorMessage = "This account has succesfully been made!";
                this.Close();
            }

            ErrorMessage.Content = errorMessage;
        }

        private void CancelRegister_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }
    }
}
