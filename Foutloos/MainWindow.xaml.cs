using Foutloos.Multiplayer;
using System.Configuration;
using System.Windows;

namespace Foutloos
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {

            InitializeComponent();
            this.Content = new HomeScreen();

            //Prohibit window from being rescaled
            MouseDoubleClick += (sender, args) =>
            {
                args.Handled = true;
            };
        }

        //Make sure a user leaves a room if he is in it so there won't be any trouble in the DB
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(!string.IsNullOrEmpty(ConfigurationManager.AppSettings["username"]))
            {
                Connection c = new Connection();
                c.leaveRoom();
            }
        }
    }
}
