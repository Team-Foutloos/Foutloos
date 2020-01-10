using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
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

namespace Foutloos.Multiplayer
{
    /// <summary>
    /// Interaction logic for endScreen.xaml
    /// </summary>
    public partial class endScreen : Page
    {
        private int roomID;
        private Connection c;

        public endScreen(int roomID)
        {
            this.roomID = roomID;
            c = new Connection();
            InitializeComponent();
            initScoreBoard();

        }

        private void initScoreBoard()
        {
            //Get the datatables.
            DataTable playerScoresTotal = c.PullData($"SELECT playerscore, SUM(time) AS time, username, t.userID FROM +" +
                                                    $"roomresult t JOIN Usertable U ON t.userID = u.userID JOIN roomplayer p ON p.userID = u.userID +" +
                                                    $" WHERE t.roomID={roomID} GROUP BY username, playerscore, t.userID ORDER BY playerscore DESC");

            for (int i = 0; i < 4; i++)
            {
                //Show the UI place
                Grid medalGrid = (Grid)scoreboardThisRound_grid.Children[i];
                TextBlock playerName = (TextBlock)medalGrid.Children[0];

                try
                {
                    playerName.Text = playerScoresTotal.Rows[i]["username"].ToString();
                    medalGrid.Visibility = Visibility.Visible;
                }
                catch (Exception e)
                {

                }
            }


            //Check how many players are in the room
            for (int i = 0; i < playerScoresTotal.Rows.Count; i++)
            {

                Grid playerGrid = new Grid { Width = 500, HorizontalAlignment = HorizontalAlignment.Center, };


                //Create all the column definitions
                ColumnDefinition column_stand = new ColumnDefinition();
                ColumnDefinition column_name = new ColumnDefinition();
                ColumnDefinition column_time = new ColumnDefinition();
                ColumnDefinition column_score = new ColumnDefinition();

                //Add the column to the grid
                playerGrid.ColumnDefinitions.Add(column_stand);
                playerGrid.ColumnDefinitions.Add(column_name);
                playerGrid.ColumnDefinitions.Add(column_time);
                playerGrid.ColumnDefinitions.Add(column_score);


                TextBlock pos = new TextBlock { Text = (i + 1).ToString(), FontSize = 20, VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center };
                TextBlock name = new TextBlock { Text = playerScoresTotal.Rows[i]["username"].ToString(), FontSize = 20, VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center };
                TextBlock time = new TextBlock { Text = TimeSpan.FromMilliseconds((int.Parse(playerScoresTotal.Rows[i]["time"].ToString())) * 10).ToString("ss':'fff").ToString(), FontSize = 20, VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center };
                TextBlock totalScore = new TextBlock { Text = playerScoresTotal.Rows[i]["playerscore"].ToString(), FontSize = 20, VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center };
                //Position
                playerGrid.Children.Add(pos);
                //Name
                playerGrid.Children.Add(name);
                //Time
                playerGrid.Children.Add(time);
                //TotalScore
                playerGrid.Children.Add(totalScore);

                Grid.SetColumn(pos, 0);
                Grid.SetColumn(name, 1);
                Grid.SetColumn(time, 2);
                Grid.SetColumn(totalScore, 3);




                player_listBox.Items.Add(playerGrid);

            }


            for (int i = 0; i < playerScoresTotal.Rows.Count; i++)
            {
                if (playerScoresTotal.Rows[i]["userID"].ToString().Equals(ConfigurationManager.AppSettings["userID"].ToString()))
                {
                    playercurrent.Text = playercurrent.Text.ToString() + (i + 1);
                }
            }
        }


        //When the user clicks the leave button.
        private void ThemedIconButton_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            c.leaveRoom();
            Application.Current.MainWindow.Content = new tokenScreen();
        }
    }
}
