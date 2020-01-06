using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace Foutloos
{
    /// <summary>
    /// Interaction logic for HomeScreen.xaml
    /// </summary>
    public partial class HomeScreen : Page
    {
        Connection c = new Connection();

        public HomeScreen()
        {
            InitializeComponent();

            if (!c.checkConnection())
            {
                new Thread(() => {
                    MessageBox.Show("A working internet connection is required.", "No working internet connection");
                    Environment.Exit(0);
                }).Start();

            }


            //Update the UI.
            this.loginUIchange();

           



            //Add a listener to all the 'Buttons' (All exercises, login and register)

        }

        public void createExercisesGrid()
        {

            BoxGrid.Children.Clear();
            DataTable packages;
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("username")))
            {
                //Get the owned packages from the database and if there are less than four it will add some locked packages. With the NOT IN all packages that cant quick start are filtered out
                packages = c.PullData($"SELECT TOP 4 P.description, L.packageID FROM License L LEFT JOIN Package P ON L.packageID = P.packageID WHERE userID = {ConfigurationManager.AppSettings.Get("userID")} AND P.packageID NOT IN (1,7,8,9) ORDER BY NEWID()");
                packages.Columns.Add("owned", typeof(int));
                foreach (DataRow dr in packages.Rows)
                {
                    dr["owned"] = 1;
                }
                if (packages.Rows.Count < 4)
                {
                    int toBePulled = 4 - packages.Rows.Count;
                    DataTable randPackages = c.PullData($"SELECT TOP {toBePulled} description, packageID FROM Package WHERE PackageID NOT IN (SELECT packageID FROM License WHERE userID = {ConfigurationManager.AppSettings.Get("userID")}) AND packageID != 1 ORDER BY NEWID()");
                    randPackages.Columns.Add("owned", typeof(int));
                    foreach (DataRow dr in randPackages.Rows)
                    {
                         packages.Rows.Add(dr.ItemArray);
                    }
                }
            }
            else
            {
                packages = c.PullData("SELECT TOP 4 description, packageID FROM Package WHERE PackageID != 1 ORDER BY NEWID()");
            }


            //Going thru all the TextBlocs in the grid to add the hover events.
            for (int i = 0; i < packages.Rows.Count; i++)
            {
                //Setting a standard text to each TextBlock
                //Here will the random exercises from the database come.
                BorderButton button = new BorderButton();
                Border borderButton = button.getButton();
                Grid borderGrid = (Grid)borderButton.Child;
                Image completedIcon = (Image)borderGrid.Children[1];
                TextBlock l1 = (TextBlock)borderGrid.Children[0];
                borderButton.Name = $"B{packages.Rows[i]["packageID"]}";
                borderButton.Margin = new Thickness(5);
                l1.TextWrapping = TextWrapping.Wrap;
                l1.FontSize = 17;
                l1.FontWeight = FontWeights.Bold;
                l1.Foreground = Brushes.White;
                l1.Text = packages.Rows[i]["description"].ToString();

                //Adding the events
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("username")) && packages.Rows[i]["owned"].ToString() == "1")
                {
                    
                    borderButton.Background = new SolidColorBrush(Color.FromRgb(51, 204, 51));
                    borderButton.BorderBrush = new SolidColorBrush(Color.FromRgb(51, 204, 51));
                    borderButton.Background.Opacity = 0.7;

                    completedIcon.Visibility = Visibility.Hidden;
                    borderButton.MouseDown += BorderButton_MouseDown;
                    borderButton.MouseEnter += OnBoxEnter;
                    borderButton.MouseLeave += OnBoxLeave;

                }

                Grid.SetColumn(button.getButton(), i);
                BoxGrid.Children.Add(button.getButton());
            }
        }

        //When a user clicks on the box, the exercise starts.
        private void BorderButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            
            int packageID = int.Parse(((Border)sender).Name.Substring(1));
            DataTable exercise = c.PullData($"SELECT TOP 1 * FROM Exercise WHERE packageID = {packageID} ORDER BY NEWID()");
            Application.Current.MainWindow.Content = new Exercise(exercise.Rows[0][1].ToString(), false, int.Parse(exercise.Rows[0][0].ToString()));
        }

        //Change things when a user logs in.
        public void loginUIchange()
        {
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["username"]))
            {
                settingsBtn.DynamicTextIcon = "Account";
                setButtonIcon("accountIconWhite.png");
                Title.Content = $"Welcome {ConfigurationManager.AppSettings["username"]}";
                ButtonRowAccount.Visibility = Visibility.Collapsed;
                seeProgressBtn.Visibility = Visibility.Visible;
                createExercisesGrid();
            }
            else
            {
                settingsBtn.DynamicTextIcon = "Settings";
                setButtonIcon("settingsWhite.png");
                seeProgressBtn.Visibility = Visibility.Collapsed;
                createExercisesGrid();
            }

            if (c.getPackages($"SELECT licenseID FROM license WHERE packageID = 8 AND userID = {ConfigurationManager.AppSettings.Get("userID")}").Count > 0)
            {
                MultiplayerGrid.Visibility = Visibility.Visible;
            }
        }

        public void setButtonIcon(string name)
        {
            //BitmapImage source = new BitmapImage();
            settingsBtn.DynamicIcon = BitmapFrame.Create(new Uri($"pack://application:,,,/assets/{name}"));
        }


        //When the mouse enters an Exercise box this happens
        private void OnBoxEnter(object sender, EventArgs e)
        {
            DoubleAnimation fade = new DoubleAnimation();

            Border button = (Border)sender;
            fade.From = .7;
            fade.To = .3;
            fade.Duration = new Duration(TimeSpan.FromMilliseconds(200));
            button.Background.BeginAnimation(SolidColorBrush.OpacityProperty, fade);


        }

        //End of mouse hover (Reset to begin values)
        private void OnBoxLeave(object sender, EventArgs e)
        {
            DoubleAnimation fade = new DoubleAnimation();

            Border button = (Border)sender;
            fade.From = .3;
            fade.To = .7;
            fade.Duration = new Duration(TimeSpan.FromMilliseconds(200));
            button.Background.BeginAnimation(SolidColorBrush.OpacityProperty, fade);



        }

        //This function shows the modal, login or register modal with generic types.
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

        private void settings_Click(object sender, RoutedEventArgs e)
        {
            Connection c = new Connection();

        }

        private void AllExercisesBtn_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Application.Current.MainWindow.Content = new ExercisesPage();
        }

        private void LoginBtn_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ShowModal(new Modals.ModalLogin());
            loginUIchange();
        }

        private void RegisterBtn_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ShowModal(new Modals.ModalRegister());
            loginUIchange();
        }

        private void SettingsBtn_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //Here comes the reference to the settings page
            Application.Current.MainWindow.Content = new SettingsPage();
        }
        private void seeProgressBtn_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Application.Current.MainWindow.Content = new Results();
        }

        //When the user clicks the multiplayer button
        private void MultiPlayerBtn_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["username"]))

                Application.Current.MainWindow.Content = new Multiplayer.tokenScreen();
            else
            {
                ShowModal(new Modals.ModalLogin());
                loginUIchange();
            }
        }
    }

}
