using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace Foutloos
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : Page
    {

        private int userID;
        public SettingsPage()
        {
            InitializeComponent();
            this.loginUIchange();
            checkLicenses();
        }



        public void loginUIchange()
        {
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["username"]))
            {
                Title.Content = $"Hello, {ConfigurationManager.AppSettings["username"]}!";
                txtUsername.Text = ConfigurationManager.AppSettings["username"];
                userID = c.ID($"SELECT userID FROM Usertable WHERE username = '{ConfigurationManager.AppSettings["username"]}'");
                gridloggedIn.Visibility = Visibility.Visible;
                gridloggedOut.Visibility = Visibility.Hidden;
                btnLogOut.Visibility = Visibility.Visible;
            }
            else
            {
                Title.Content = $"Hello!";
                gridloggedIn.Visibility = Visibility.Hidden;
                gridloggedOut.Visibility = Visibility.Visible;
                btnLogOut.Visibility = Visibility.Hidden;
            }
        }

        private void BtnLogIn_Click(object sender, MouseButtonEventArgs e)
        {
            ShowModal(new Modals.ModalLogin());
            userID = c.ID($"SELECT userID FROM Usertable WHERE username = '{ConfigurationManager.AppSettings["username"]}'");
            Application.Current.MainWindow.Content = new SettingsPage();
        }
        private void BtnRegister_Click(object sender, MouseButtonEventArgs e)
        {
            ShowModal(new Modals.ModalRegister());
            Application.Current.MainWindow.Content = new SettingsPage();
        }

        private void ShowModal<T>(T modal) where T : Window
        {
            UIElement rootVisual = this.Content as UIElement;
            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(rootVisual);
            if (rootVisual != null && adornerLayer != null)
            {
                CustomTools.DarkenAdorner darkenAdorner = new CustomTools.DarkenAdorner(rootVisual);
                adornerLayer.Add(darkenAdorner);
                modal.ShowDialog();
                adornerLayer.Remove(darkenAdorner);
            }
        }

        //The logout button
        private void BtnLogOut_Click(object sender, MouseButtonEventArgs e)
        {
            ShowModal(new Modals.LogoutAreYouSure());
        }

        //The return to home button
        private void ThemedButton_HomeMouseDown(object sender, MouseButtonEventArgs e)
        {
            Application.Current.MainWindow.Content = new HomeScreen();
        }

        //Connector to the Database
        Connection c = new Connection();

        //The save button to save user/email adress changes
        private void BtnSave_Click(object sender, MouseButtonEventArgs e)
        {
            string errorMessage = "";

            if (txtUsername.Text.Length < 5)
            {
                errorMessage += "Username is too short";
            }
            else
            {
                System.Console.WriteLine(userID);
                string CmdString = $"UPDATE usertable set username = '{txtUsername.Text}' WHERE userID = {userID}";
                if (c.insertInto(CmdString))
                {
                    System.Windows.Forms.MessageBox.Show("Username succesfully changed");
                    ConfigurationManager.AppSettings["username"] = txtUsername.Text;
                    txtUsername.Text = ConfigurationManager.AppSettings["username"];
                    Application.Current.MainWindow.Content = new SettingsPage();

                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("werkt niet");
                }
            }
        }

        private void BtnChangePassword_Click(object sender, MouseButtonEventArgs e)
        {
            ShowModal(new Modals.ChangePassword(userID));
        }

        private void License_Click(object sender, MouseButtonEventArgs e)
        {
            License license = new License();

            if (licenseBox.Text != "")
            {
                license.insertLicense(licenseBox.Text);
                checkLicenses();
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Pls fill in a licenseKey");
            }
        }

        private void TxtUsername_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox box = (TextBox)sender;
            if (box.Text.Length == 12)
            {
                Storyboard myStoryboard = (Storyboard)box.Resources["TestStoryboard"];
                Storyboard.SetTarget(myStoryboard.Children.ElementAt(0) as DoubleAnimationUsingKeyFrames, box);
                myStoryboard.Begin();
            }
        }

        public void checkLicenses()
        {
            List<int> licenses = new List<int>();
            StringBuilder l = new StringBuilder();
            int Name = c.ID($"SELECT userID FROM userTable WHERE username = '{ConfigurationManager.AppSettings["username"]}'");
            licenses = c.getPackages($"SELECT packageID FROM license WHERE userID = {Name}");

            l.Append("STANDARD Pack\n");


            foreach (int license in licenses)
            {
                if (license == 2)
                {
                    l.Append("George Orwell Pack \n");
                }
                if (license == 3)
                {
                    l.Append("C# Pack \n");
                }
                if (license == 4)
                {
                    l.Append("Special Characters \n");
                }
                if (license == 5)
                {
                    l.Append("J.K. Rowling Pack \n");
                }
                
                if (license == 7)
                {
                    l.Append("Auto generating Pack \n");
                }
                if (license == 8)
                {
                    l.Append("Multiplayer Pack \n");
                }
                if (license == 9)
                {
                    l.Append("Quick Fire Pack");
                }

            }

            licensesContent.Text = l.ToString();
        }
    }
}
