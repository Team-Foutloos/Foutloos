using System;
using System.Collections.Generic;
using System.Configuration;
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

            //Collapse the tokenGrid so the player can't see it, only the owner can
            token_grid.Visibility = Visibility.Collapsed;

            //Collapse the button Start so the player can't see it, only the owner can
            startMatch_button.Visibility = Visibility.Collapsed;
        }

        //Listen to the database
        private void backgroundListener()
        {
            while (true)
            {
                players = c.PullData($"SELECT username FROM usertable u JOIN roomplayer r ON u.userID = r.userID WHERE r.roomID = {roomID} ");
                

                for (int i = 0; i < players.Rows.Count; i++)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        TextBlock playerName = (TextBlock)players_grid.Children[i];
                        playerName.Text = players.Rows[i]["username"].ToString();
                        playerName.Visibility = Visibility.Visible;
                    });
                }
                Thread.Sleep(1000);
            }
        }

        //Let the user join a room
        private void joinRoom()
        {
            //First get the roomID
            if (roomID == 0)
            {
                roomID = c.ID($"SELECT roomID from room where roomToken = '{tokenString}'");
            }
            c.insertInto($"INSERT INTO roomplayer (roomID, userID) VALUES ('{roomID}', '{ConfigurationManager.AppSettings["userID"]}')");
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
            //First get a unique roomID
            roomID = 1;
            roomID = (c.ID("SELECT Max(roomID) FROM room")) + 1;

            //Then create a new room in the room table
            c.insertInto($"INSERT INTO room (roomID, roomToken) VALUES ('{roomID}','{createTokenString()}')");

            //Add the token to the token_textblock
            token_textblock.Text = token_textblock.Text + tokenString;
            
        }

        private void leaveRoom()
        {
            c.insertInto($"DELETE FROM roomplayer WHERE userID = {ConfigurationManager.AppSettings["userID"]}");
            
        }


        //When the user clicks the leave button.
        private void ThemedIconButton_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            leaveRoom();
            Application.Current.MainWindow.Content = new tokenScreen();
        }
    }
}
