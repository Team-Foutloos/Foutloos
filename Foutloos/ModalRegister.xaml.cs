using System;
using System.Collections.Generic;
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

            //Add the events to the mousedown, both being to cancel the modal at this time.
            register.MouseDown += Button_Click;
            cancelRegister.MouseDown += Button_Click;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
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

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            licenseLength.Content = 5 - license.Text.Length;
        }

        private void PasswordRepeat_PasswordChanged(object sender, RoutedEventArgs e)
        {
            passwordRepeatLength.Content = 20 - passwordRepeat.Password.Length;
        }

        private void Password_PasswordChanged(object sender, RoutedEventArgs e)
        {
            passwordLength.Content = 20 - password.Password.Length;
        }
    }
}
