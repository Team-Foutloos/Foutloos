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
    public partial class SettingsAndProfilePage : Page
    {
        public SettingsAndProfilePage()
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
            ShowModal(new Modals.ModalLogin(this));
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
    }
}
