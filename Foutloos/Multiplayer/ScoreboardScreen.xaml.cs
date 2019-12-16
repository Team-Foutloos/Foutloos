using System;
using System.Data;
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


        public ScoreboardScreen(int roomID, int exerciseID)
        {
            InitializeComponent();
            this.roomID = roomID;
            this.exerciseID = exerciseID;
            c = new Connection();
            StartCountdown(CountdownDisplay);
            initializeScoreBoard();

        }

        private void initializeScoreBoard()
        {
            players = c.PullData($"SELECT username FROM usertable u JOIN roomplayer r ON u.userID = r.userID WHERE r.roomID = {roomID} ");

            //Check how many players are in the room
            for (int i = 0; i < players.Rows.Count; i++)
            {
                //Show the UI place
                Grid medalGrid = (Grid) scoreboardThisRound_grid.Children[i];
                TextBlock playerName = (TextBlock)medalGrid.Children[0];
                playerName.Text = players.Rows[i]["username"].ToString();
                medalGrid.Visibility = Visibility.Visible;

                Grid scoreGrid = (Grid)scoreboardThisRound_grid.Children[4];

                Border scoreBorder = (Border)scoreGrid.Children[0];
                Grid scoreTexts = (Grid)scoreBorder.Child;


                //Set the score under the positions
                TextBlock playerPositionResult = (TextBlock)scoreTexts.Children[i*3];
                TextBlock playerNameResult = (TextBlock)scoreTexts.Children[i*3+1];
                TextBlock playerTimeResult = (TextBlock)scoreTexts.Children[i*3+2];

                playerPositionResult.Visibility = Visibility.Visible;
                playerNameResult.Visibility = Visibility.Visible;
                playerTimeResult.Visibility = Visibility.Visible;

                playerNameResult.Text = players.Rows[i]["username"].ToString();


                //Set the score for the overallPosition
                Grid overallRankings = (Grid)ranking_grid;

                TextBlock playerStanding = (TextBlock) overallRankings.Children[i];
                playerStanding.Visibility = Visibility.Visible;
                playerStanding.Text = players.Rows[i]["username"].ToString();


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

            var countdownStoryboard = new Storyboard();
            countdownStoryboard.Children.Add(countdownAnimation);
            countdownStoryboard.Completed += CountdownTimer_Completed;
            countdownStoryboard.Begin(this);
        }

        private void CountdownTimer_Completed(object sender, EventArgs e)
        {
            //Start the next exercise
            Application.Current.MainWindow.Content = new GameScreen(roomID, this.exerciseID + 1);
        }

        //When the user clicks the leave button.
        private void ThemedIconButton_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

            c.leaveRoom(roomID);
            Application.Current.MainWindow.Content = new tokenScreen();
        }

    }
}
