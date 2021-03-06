﻿using System.Configuration;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

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
            else if (password.Password.Length > 8 && !(password.Password.Equals(passwordRepeat.Password)))
            {
                errorMessage += "Passwords do not match";
                shakeTheBox();
            }
            else if (c.getPackages($"SELECT * FROM usertable WHERE username='{username.Text}'").Count > 0)
            {
                errorMessage += "This username is already taken";
                shakeTheBox();
            }
            else
            {

                //First hash the password, this happens in the securepasswordhasher class.
                string hashedPassword = SecurePasswordHasher.Hash(password.Password);
                int userID = (c.ID("SELECT Max(userID) FROM Usertable")) + 1;

                //Insert the user into the database.
                string CmdString = $"INSERT INTO usertable VALUES ('{userID}', '{username.Text}', '{hashedPassword}')";
                if (c.insertInto(CmdString))
                {
                    ConfigurationManager.AppSettings["username"] = username.Text;
                    ConfigurationManager.AppSettings["userID"] = userID.ToString();
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
