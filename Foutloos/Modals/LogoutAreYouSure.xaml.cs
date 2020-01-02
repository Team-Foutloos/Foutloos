using System.Configuration;
using System.Windows;
using System.Windows.Input;

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
            Application.Current.MainWindow.Content = new HomeScreen();
            this.Close();
        }

        private void ThemedButton_PreviewMouseDown_1(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }
    }
}
