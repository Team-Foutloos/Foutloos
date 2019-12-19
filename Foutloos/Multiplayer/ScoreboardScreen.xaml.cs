using System;
using System.Configuration;
using System.Data;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace Foutloos.Multiplayer
{
    /// <summary>
    /// Interaction logic for ScoreboardScreen.xaml
    /// </summary>
    public partial class ScoreboardScreen : Page
    {
        private int roomID;
        private int exerciseID;
        private Connection c;
        private DataTable players;
        private Storyboard countdownStoryboard;
        private bool alreadyLeft = false;


        public ScoreboardScreen(int roomID, int exerciseID)
        {
            InitializeComponent();
            exerciseTextBlock.Text = $"Exercise {exerciseID + 1} / 10";
            this.roomID = roomID;
            this.exerciseID = exerciseID;
            c = new Connection();
            if (exerciseID < 9)
            {
                StartCountdown(CountdownDisplay);
            }
            else
            {
                nextExerciseTextBlock.Text = "Game is finished!";
            }

            initializeScoreBoard();

        }

        private void initializeScoreBoard()
        {

            //Get the datatables.
            DataTable playerScoresTotal = c.PullData($"SELECT t.userID FROM roomresult t WHERE t.roomExerciseID={exerciseID} AND t.roomID={roomID} ORDER BY time ASC");

            //Add the score to the player
            if (playerScoresTotal.Rows[0]["userID"].ToString().Equals(ConfigurationManager.AppSettings["userID"].ToString()))
            {
                c.insertInto($"UPDATE roomplayer SET playerscore = playerscore + 5 WHERE userID = {int.Parse(ConfigurationManager.AppSettings["userID"].ToString())}");
            }
            else if (playerScoresTotal.Rows[1]["userID"].ToString().Equals(ConfigurationManager.AppSettings["userID"].ToString()))
            {
                c.insertInto($"UPDATE roomplayer SET playerscore = playerscore + 3 WHERE userID = {int.Parse(ConfigurationManager.AppSettings["userID"].ToString())}");
            }
            else if (playerScoresTotal.Rows[2]["userID"].ToString().Equals(ConfigurationManager.AppSettings["userID"].ToString()))
            {
                c.insertInto($"UPDATE roomplayer SET playerscore = playerscore + 1 WHERE userID = {int.Parse(ConfigurationManager.AppSettings["userID"].ToString())}");
            }

            Thread.Sleep(400);
             //Get the datatables.
            playerScoresTotal = c.PullData($"SELECT playerscore, username, t.userID, time FROM roomresult t JOIN Usertable U ON t.userID = u.userID JOIN roomplayer p ON p.userID = u.userID WHERE t.roomExerciseID={exerciseID} AND t.roomID={roomID} ORDER BY time ASC");

            
            DataTable playerStanding = c.PullData($"SELECT playerscore, userID from roomplayer WHERE roomID = {roomID} ORDER BY playerscore DESC");


            //Check how many players are in the room
            for (int i = 0; i < playerScoresTotal.Rows.Count; i++)
            {

                //Show the UI place
                Grid medalGrid = (Grid)scoreboardThisRound_grid.Children[i];
                TextBlock playerName = (TextBlock)medalGrid.Children[0];
                playerName.Text = playerScoresTotal.Rows[i]["username"].ToString();
                medalGrid.Visibility = Visibility.Visible;

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


            for (int i = 0; i < playerStanding.Rows.Count; i++)
            {
                if (playerStanding.Rows[i]["userID"].ToString().Equals(ConfigurationManager.AppSettings["userID"].ToString()))
                {
                    playercurrent.Text = playercurrent.Text.ToString() + (i + 1);
                }
            }
        }
        


        private void StartCountdown(FrameworkElement target)
        {
            var countdownAnimation = new StringAnimationUsingKeyFrames();

            for (var i = 5; i > 0; i--)
            {
                var keyTime = TimeSpan.FromSeconds(5 - i);
                var frame = new DiscreteStringKeyFrame(i.ToString(), KeyTime.FromTimeSpan(keyTime));
                countdownAnimation.KeyFrames.Add(frame);
            }
            countdownAnimation.KeyFrames.Add(new DiscreteStringKeyFrame(" ", KeyTime.FromTimeSpan(TimeSpan.FromSeconds(6))));
            Storyboard.SetTargetName(countdownAnimation, target.Name);
            Storyboard.SetTargetProperty(countdownAnimation, new PropertyPath(TextBlock.TextProperty));

            countdownStoryboard = new Storyboard();
            countdownStoryboard.Children.Add(countdownAnimation);
            countdownStoryboard.Completed += CountdownTimer_Completed;
            countdownStoryboard.Begin(this);
        }

        private void CountdownTimer_Completed(object sender, EventArgs e)
        {
            //Start the next exercise
            if (!alreadyLeft)
            Application.Current.MainWindow.Content = new GameScreen(roomID, this.exerciseID + 1);
        }

        //When the user clicks the leave button.
        private void ThemedIconButton_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            alreadyLeft = true;
            c.leaveRoom();
            Application.Current.MainWindow.Content = new tokenScreen();
        }

    }
}
