using System;
using System.Collections.Generic;
using System.Data;
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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Foutloos.Multiplayer
{
    /// <summary>
    /// Interaction logic for ServerBrowser.xaml
    /// </summary>
    public partial class ServerBrowser : Page
    {
        Connection c;
        DataTable data;
        Thread databaseListener;
        public ServerBrowser()
        {
            InitializeComponent();
            c = new Connection();

            //Listen to the database.
            databaseListener = new Thread(new ThreadStart(addServers));
            databaseListener.IsBackground = true;
            databaseListener.Start();
        }

        private void addServers()
        {
            while (true)
            {

                data = c.PullData("SELECT roomToken, count(userID) count from room r JOIN roomplayer p ON r.roomID = p.roomID GROUP BY roomtoken");
                this.Dispatcher.Invoke(() =>
                {

                    serverList.Items.Clear();
                    for (int i = 0; i < data.Rows.Count; i++)
                {
                        string roomToken = data.Rows[i]["roomToken"].ToString();
                        DockPanel serverGrid = new DockPanel { Background = Brushes.LightBlue, Width = 1380, HorizontalAlignment = HorizontalAlignment.Center };
                        serverList.Items.Remove(serverList.Items);
                        if (i % 2 == 0)
                        {
                            serverGrid.Background = Brushes.White;
                            serverGrid.MouseLeave += (sender, e) => change_background_back(sender, e, 0);
                        }
                        else
                        {
                            serverGrid.MouseLeave += (sender, e) => change_background_back(sender, e, 1);
                        }
                        serverGrid.MouseEnter += change_background;
                        serverGrid.MouseDown += (sender, e) => Item_Click(sender, e, roomToken);

                        serverGrid.Children.Add(new TextBlock { Text = roomToken, FontSize = 20, Margin = new Thickness(20, 0, 0, 0), VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Left });
                        serverGrid.Children.Add(new TextBlock { Text = data.Rows[i]["count"].ToString(), FontSize = 20, Margin = new Thickness(0, 0, 20, 0), VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Right });
                        serverList.Items.Add(serverGrid);
                    }
                });
                Thread.Sleep(5000);
            }
        }

        //When the user clicks the home button
        private void ThemedIconButton_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Application.Current.MainWindow.Content = new tokenScreen();
        }

        private void change_background(object sender, MouseEventArgs e)
        {
            //Change the background of the sender
            DockPanel x = (DockPanel) sender;
            x.Background = Brushes.DeepSkyBlue;
        }

        private void change_background_back(object sender, MouseEventArgs e, int color)
        {
            //Change the background of the sender back
            DockPanel x = (DockPanel)sender;
            if (color == 1)
                x.Background = Brushes.LightBlue;
            else
                x.Background = Brushes.White;
        }

        //When the user clicks the server button
        private void Item_Click(object sender, MouseButtonEventArgs e, string token)
        {
            Application.Current.MainWindow.Content = new lobbyScreen(token);
        }
    }
}
