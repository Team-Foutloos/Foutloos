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
        private Storyboard countdownStoryboard;


        public ScoreboardScreen(int roomID, int exerciseID)
        {
            InitializeComponent();
            exerciseTextBlock.Text = $"Exercise {exerciseID + 1} / 10";
            this.roomID = roomID;
            this.exerciseID = exerciseID;
            c = new Connection();
            if(exerciseID < 9)
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
            DataTable playerScoresTotal = c.PullData($"SELECT r.userID, U.username, time FROM roomplayer r JOIN Usertable U ON U.userID = r.userID LEFT JOIN RoomResult rr ON r.roomID = rr.roomID WHERE r.roomID = {roomID} AND rr.roomExerciseID = {exerciseID} GROUP BY U.userID ORDER BY rr.time ASC");




            //Check how many players are in the room
            for (int i = 0; i < playerScoresTotal.Rows.Count; i++)
            {
                DataTable playerScores = c.PullData($"SELECT time FROM RoomResult WHERE roomID = {roomID} AND userID = {playerScoresTotal.Rows[i]["userID"].ToString()} AND roomExerciseID = {exerciseID}");
                //Show the UI place
                Grid medalGrid = (Grid) scoreboardThisRound_grid.Children[i];
                TextBlock playerName = (TextBlock)medalGrid.Children[0];
                playerName.Text = playerScoresTotal.Rows[i]["username"].ToString();
                medalGrid.Visibility = Visibility.Visible;

                Grid scoreGrid = (Grid)scoreboardThisRound_grid.Children[4];

                Border scoreBorder = (Border)scoreGrid.Children[0];
                Grid scoreTexts = (Grid)scoreBorder.Child;


                //Set the score under the positions
                TextBlock playerPositionResult = (TextBlock)scoreTexts.Children[i*3];
                TextBlock playerNameResult = (TextBlock)scoreTexts.Children[i*3+1];
                TextBlock playerTimeResult = (TextBlock)scoreTexts.Children[i*3+2];

                playerTimeResult.Text = TimeSpan.FromMilliseconds((int.Parse(playerScores.Rows[0]["time"].ToString())) * 10 ).ToString("ss':'fff");

                playerPositionResult.Visibility = Visibility.Visible;
                playerNameResult.Visibility = Visibility.Visible;
                playerTimeResult.Visibility = Visibility.Visible;

                playerNameResult.Text = playerScoresTotal.Rows[i]["username"].ToString();

                //Set the standing for the player text
                playercurrent.Text = playercurrent.Text + playerScoresTotal.Rows[i]["username"].ToString();


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
            Application.Current.MainWindow.Content = new GameScreen(roomID, this.exerciseID + 1);
        }

        //When the user clicks the leave button.
        private void ThemedIconButton_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            countdownStoryboard.Stop(this);
            c.leaveRoom(roomID);
            Application.Current.MainWindow.Content = new tokenScreen();
        }

    }
}
