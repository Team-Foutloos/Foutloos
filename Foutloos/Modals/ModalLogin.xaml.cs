﻿using System;
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

namespace Foutloos
{
    /// <summary>
    /// Interaction logic for ModalLogin.xaml
    /// </summary>
    public partial class ModalLogin : Window
    {
        private HomeScreen owner;
        public ModalLogin(HomeScreen owner)
        {
            this.owner = owner;
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
            Timer t = new Timer(closeWindow, null, 1200, 1200);


        }

        private void closeWindow(object state)
        {
            this.Dispatcher.Invoke(() =>
            {                            
                //If a user logs in, change the UI of the homePage.
                owner.loginUIchange();
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
            else
            {
                string hashedPassword = SecurePasswordHasher.Hash(password.Password);
                //query that is being executed and being shows in a Table in the application.
                string connectionstring = "Data Source=127.0.0.1,1433; User Id=sa;Password=Foutloos!; Initial Catalog=foutloos_db;";
                string CmdString = $"SELECT * FROM Usertable WHERE username = @username";
                using (SqlConnection con = new SqlConnection(connectionstring))
                {
                    try
                    {
                        con.Open();
                        SqlCommand insCmd = new SqlCommand(CmdString, con);
                        // use sqlParameters to prevent sql injection!
                        insCmd.Parameters.AddWithValue("@username", username.Text);
                        insCmd.Parameters.AddWithValue("@password", hashedPassword);
                        using (SqlDataReader reader = insCmd.ExecuteReader())
                        {
                            if (reader.Read() && SecurePasswordHasher.Verify(password.Password, (string)reader["password"]))
                            {
                                ConfigurationManager.AppSettings["username"] = (string)reader["username"];

                                loadingScreen();
                            }
                            else
                            {
                                shakeTheBox();
                                error = "Username or Password incorrect!";
                            }
                        }
                    } catch(Exception f)
                    {
                        error = "Your computer is not connected to the internet.";
                    }
                    con.Close();
                }
            }
            errorMessage.Content = error;
        }

        private void Cancel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }
    }
}