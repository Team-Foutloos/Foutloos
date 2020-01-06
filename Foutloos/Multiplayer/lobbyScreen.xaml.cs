using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
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
    /// Interaction logic for lobbyScreen.xaml
    /// </summary>
    public partial class lobbyScreen : Page
    {
        private bool isOwner = false;
        private Connection c;
        private DataTable players = new DataTable();
        private string tokenString;
        private int roomID;
        private Thread databaseListener;
        private System.Media.SoundPlayer player = new System.Media.SoundPlayer(Properties.Resources.lobby_music);


        //When a creator joins the lobby
        public lobbyScreen(bool isOwner)
        {
            InitializeComponent();

            this.isOwner = isOwner;
            c = new Connection();


            //Create and join the room;
            createRoom();
            joinRoom();

            //Listen to the database.
            databaseListener = new Thread(new ThreadStart(backgroundListener));
            databaseListener.IsBackground = true;
            databaseListener.Start();
            
        }


        //When a player joins the lobby
        public lobbyScreen(string tokenString)
        {
            InitializeComponent();

            c = new Connection();
            this.tokenString = tokenString;

            //Join the room with this userID;
            joinRoom();

            //Listen to the database.
            databaseListener = new Thread(new ThreadStart(backgroundListener));
            databaseListener.IsBackground = true;
            databaseListener.Start();

            //Collapse the button Start so the player can't see it, only the owner can, and show the motivating text
            share_textblock.Text = "Tell him to hurry up please, we cannot wait to see you beat him.";
            token_textblock.Text = "Waiting for the host to start!";

            //Change this
            startMatch_button.Visibility = Visibility.Collapsed;
            
        }

        //Listen to the database
        private void backgroundListener()
        {
            while (true)
            {
                players = c.PullData($"SELECT username FROM usertable u JOIN roomplayer r ON u.userID = r.userID WHERE r.roomID = {roomID} ");

                DataTable hasStarted = (c.PullData($"SELECT hasStarted FROM room WHERE roomID = {roomID}"));
                if ((bool)hasStarted.Rows[0]["hasStarted"])
                {

                    this.Dispatcher.Invoke(() =>
                    {

                        Application.Current.MainWindow.Content = new GameScreen(roomID, 0);
                        player.Stop();
                        databaseListener.Abort();
                    });
                }

                this.Dispatcher.Invoke(() =>
                {
                    player_listBox.Items.Clear();
                });

                for (int i = 0; i < players.Rows.Count; i++)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        TextBlock playerName = new TextBlock() { FontSize=20 };
                        playerName.Text = players.Rows[i]["username"].ToString();

                        player_listBox.Items.Add(playerName);
                    });
                }
                Thread.Sleep(1000);
            }
        }

        //Let the user join a room
        private void joinRoom()
        {
            //Start the music
            player.PlayLooping();

            //First get the roomID
            if (roomID == 0)
            {
                roomID = c.ID($"SELECT roomID from room where roomToken = '{tokenString}'");
            }
            if (c.PullData($"SELECT roomID from roomplayer WHERE roomID = '{roomID}' AND userID = '{ConfigurationManager.AppSettings["userID"]}'").Rows.Count == 0)
            c.insertInto($"INSERT INTO roomplayer (roomID, userID, playerScore) VALUES ('{roomID}', '{ConfigurationManager.AppSettings["userID"]}',0)");
        }

        //Create a tokenString that consists of letters.
        private string createTokenString()
        {
            Random rd = new Random();
            const string allowedChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";
            tokenString = "";

            for (int i = 0; i < 5; i++)
            {
                tokenString += allowedChars[rd.Next(0, allowedChars.Length)];
            }
            return tokenString;
        }


        private void createRoom()
        {
            //If the user is already in a room because the game wasn't closed properly this wil remove him from that game in order to make joining a new one possible
            c.leaveRoom();

            //First get a unique roomID
            roomID = 1;
            roomID = (c.ID("SELECT Max(roomID) FROM room")) + 1;

            //Then create a new room in the room table
            c.insertInto($"INSERT INTO room (roomID, roomToken, hasStarted) VALUES ('{roomID}','{createTokenString()}',0)");

            //Add the token to the token_textblock
            token_textblock.Text = token_textblock.Text + tokenString;


            //Add the exercises to the database
            createExercises();
        }

        private void createExercises()
        {
            //Get all the words from dictionary so you can put them in exercises later
            DataTable words = new DataTable();
            Random rand = new Random();
            words = c.PullData($"SELECT * FROM Dictionary");

            //Create 10 exercises and add them in a for loop
            for (int i = 0; i < 5; i++)
            {
                string exercise = "";

                for (int j = 0; j < 8; j++)
                {
                    exercise += words.Rows[rand.Next(0, words.Rows.Count)]["list"].ToString();

                    if (j != 7)
                    exercise += " ";
                }

                c.insertInto($"INSERT INTO roomExercise VALUES ({i},{roomID},'{exercise}')");



            }
        }



        //When the user clicks the leave button.
        private void ThemedIconButton_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            c.leaveRoom();
            Application.Current.MainWindow.Content = new tokenScreen();
            player.Stop();
            databaseListener.Abort();
        }

        private void StartMatch_button_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (c.getPackages($"SELECT userID from roomplayer WHERE roomID = {roomID}").Count > 1)
            {
                //Start the game
                c.insertInto($"UPDATE room SET hasStarted=1 WHERE roomID = {roomID}");
                Application.Current.MainWindow.Content = new GameScreen(roomID, 0);
                player.Stop();
                databaseListener.Abort();

            }
        }
    }
}
