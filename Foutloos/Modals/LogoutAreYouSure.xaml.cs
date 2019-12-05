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
using System.Windows.Shapes;

namespace Foutloos.Modals
{
    /// <summary>
    /// Interaction logic for LogoutAreYouSure.xaml
    /// </summary>
    public partial class LogoutAreYouSure : Window
    {
        public LogoutAreYouSure()
        {
            InitializeComponent();
        }

        private void ThemedButton_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ConfigurationManager.AppSettings["username"] = "";
            Application.Current.MainWindow.Content = new SettingsPage();
            this.Close();      
        }

        private void ThemedButton_PreviewMouseDown_1(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }
    }
}
