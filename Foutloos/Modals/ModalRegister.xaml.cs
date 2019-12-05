using System;
using System.Collections.Generic;
using System.Configuration;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WpfAnimatedGif;

namespace Foutloos.Modals
{
    /// <summary>
    /// Interaction logic for ModalLogin.xaml
    /// </summary>
    public partial class ModalRegister : Window
    {
        Connection c = new Connection();

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

        //Use this function to shake the entire window
        private void shakeTheBox()
        {

            Storyboard myStoryboard = (Storyboard)modalRegister.Resources["shaking"];
            Storyboard.SetTarget(myStoryboard.Children.ElementAt(0) as DoubleAnimationUsingKeyFrames, modalRegister);
            myStoryboard.Begin();
        }

        //Use this function to turn the entire window into a loading 'screen'
        private void loadingScreen()
        {
            //First shake the entire window
            shakeTheBox();
            //The loading progress bar.
            var storyboard = this.Resources["close"] as Storyboard;
            storyboard.Begin();

            //Let the loadingscreen load for 1,5 seconds.
            Timer t = new Timer(closeWindow, null, 1200, 1200);


        }

        //Close the application
        private void closeWindow(object state)
        {
            this.Dispatcher.Invoke(() =>
            {
                this.Close();
            });
        }

        private void Register_MouseDown(object sender, MouseButtonEventArgs e)
        {
            string errorMessage = "";
            Window box = modalRegister;

            if (username.Text.Length < 5)
            {
                errorMessage += "Username is too short";

                shakeTheBox();
               
            }
            else if (password.Password.Length < 8)
            {
                errorMessage += "Password is too short";
                shakeTheBox();
            }
            else if (password.Password.Length > 8 && !(password.Password.Equals(passwordRepeat.Password))){
                errorMessage += "Passwords do not match";
                shakeTheBox();
            }
            else if (license.Text.Length < 5)
            {
                errorMessage += "This license is not in use";
                shakeTheBox();
            }
            else
            {

                //First hash the password, this happens in the securepasswordhasher class.
                string hashedPassword = SecurePasswordHasher.Hash(password.Password);

                //Insert the user into the database.
                string CmdString = $"INSERT INTO usertable VALUES ('{username.Text}', '{hashedPassword}', '{license.Text}')";
                if (c.insertInto(CmdString))
                {
                    ConfigurationManager.AppSettings["username"] = username.Text;
                    loadingScreen();
                };

            }

            ErrorMessage.Content = errorMessage;
        }

        private void CancelRegister_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }
    }
}
