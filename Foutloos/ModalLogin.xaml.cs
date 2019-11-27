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
