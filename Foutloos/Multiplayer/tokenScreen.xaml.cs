using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Foutloos.Multiplayer
{
    /// <summary>
    /// Interaction logic for tokenScreen.xaml
    /// </summary>
    public partial class tokenScreen : Page
    {
        public tokenScreen()
        {
            InitializeComponent();
        }

        //When the user clicks the home button
        private void ThemedIconButton_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Application.Current.MainWindow.Content = new HomeScreen();
        }

        //When the user clicks the Join Room button
        private void ThemedButton_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Connection c = new Connection();



            if (c.PullData($"SELECT roomID from room WHERE roomtoken = '{token_textBox.Text}'").Rows.Count > 0)
            {
                Application.Current.MainWindow.Content = new lobbyScreen(token_textBox.Text);
            }
            else
                error_label.Visibility = Visibility.Visible;
        }

        //When the user clicks the Create room button
        private void CreateRoom_button_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Application.Current.MainWindow.Content = new lobbyScreen(true);
        }

        private void JoinRoom_button_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Application.Current.MainWindow.Content = new ServerBrowser();
        }
    }
}
