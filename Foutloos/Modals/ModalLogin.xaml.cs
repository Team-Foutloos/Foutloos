using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace Foutloos.Modals
{
    public partial class ModalLogin : Window
    {
        private Timer t;
        public ModalLogin()
        {
            InitializeComponent();
        }

        //Use this function to shake the entire window
        private void shakeTheBox()
        {

            Storyboard myStoryboard = (Storyboard)modalLogin.Resources["shaking"];
            Storyboard.SetTarget(myStoryboard.Children.ElementAt(0) as DoubleAnimationUsingKeyFrames, modalLogin);
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
            t = new Timer(closeWindow, null, 1200, 1200);


        }

        private void closeWindow(object state)
        {
            this.Dispatcher.Invoke(() =>
            {
                this.Close();
            });

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
            else if (!(CustomTools.LoginFunctions.login(username.Text, password.Password))){
                shakeTheBox();
                error = "Username or Password incorrect!";
            }
            else
            {
                loadingScreen();
            }
            errorMessage.Content = error;
        }

        private void Cancel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

     
    }
}
