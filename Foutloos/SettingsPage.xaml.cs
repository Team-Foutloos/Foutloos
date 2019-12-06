using System;
using System.Collections.Generic;
using System.Configuration;
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
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            InitializeComponent();
            this.loginUIchange();
        }



        public void loginUIchange()
        {
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["username"]))
            {
                Title.Content = $"Hello, {ConfigurationManager.AppSettings["username"]}!";
                gridloggedIn.Visibility = Visibility.Visible;
                gridloggedOut.Visibility = Visibility.Hidden;
                btnLogOut.IsEnabled = true;
            }
            else
            {
                Title.Content = $"Hello!";
                gridloggedIn.Visibility = Visibility.Hidden;
                gridloggedOut.Visibility = Visibility.Visible;
            }
        }

        private void BtnLogIn_Click(object sender, RoutedEventArgs e)
        {
            ShowModal(new Modals.ModalLogin());
        }
        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            ShowModal(new Modals.ModalRegister());
        }

        private void ShowModal<T>(T modal) where T : Window
        {
            UIElement rootVisual = this.Content as UIElement;
            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(rootVisual);
            if (rootVisual != null && adornerLayer != null)
            {
                CustomTools.DarkenAdorner darkenAdorner = new CustomTools.DarkenAdorner(rootVisual);
                adornerLayer.Add(darkenAdorner);
                modal.ShowDialog();
                adornerLayer.Remove(darkenAdorner);
            }
        }

        private void BtnLogOut_Click(object sender, RoutedEventArgs e)
        {
            ShowModal(new Modals.LogoutAreYouSure());
        }

        private void ThemedButton_HomeMouseDown(object sender, MouseButtonEventArgs e)
        {
            Application.Current.MainWindow.Content = new HomeScreen();
        }

        private void License_Click(object sender, RoutedEventArgs e)
        {
            License license = new License();

            if (licenseBox.Text != "")            {
                                
                license.insertLicense(licenseBox.Text);
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Pls fill in a licensKey");
            }
        }
    }
}
