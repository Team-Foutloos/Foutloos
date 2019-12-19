using System;
using System.Configuration;
using System.Data;
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
                packages = c.PullData($"SELECT TOP 4 description FROM Package P LEFT JOIN License L ON P.packageID = L.packageID WHERE L.userID = {ConfigurationManager.AppSettings.Get("userID")} AND L.used = 1");
            }
            else
            {
                packages = c.PullData("SELECT TOP 4 description FROM Package ORDER BY NEWID()");
            }


            //Going thru all the TextBlocs in the grid to add the hover events.
            for (int i = 0; i < packages.Rows.Count; i++)
            {
                //Setting a standard text to each TextBlock
                //Here will the random exercises from the database come.
                BorderButton button = new BorderButton();
                Border borderButton = button.getButton();
                Grid borderGrid = (Grid)borderButton.Child;
                Image completedIcon = (Image)borderGrid.Children[2];
                TextBlock l1 = (TextBlock)borderGrid.Children[0];
                borderButton.Name = $"Button{i}";
                borderButton.Margin = new Thickness(5);
                l1.FontWeight = FontWeights.Bold;
                l1.Foreground = Brushes.White;
                l1.Text = packages.Rows[i]["description"].ToString();

                /*borderButton.MouseEnter += OnBoxEnter;
                borderButton.MouseLeave += OnBoxLeave;
                borderButton.MouseDown += Exercise;*/
                //Adding the events
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("username")))
                {
                    completedIcon.Visibility = Visibility.Hidden;
                }

                Grid.SetColumn(button.getButton(), i);
                BoxGrid.Children.Add(button.getButton());
            }
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
        }

        public void setButtonIcon(string name)
        {
            //BitmapImage source = new BitmapImage();
            settingsBtn.DynamicIcon = BitmapFrame.Create(new Uri($"pack://application:,,,/assets/{name}"));
        }

        //Boolean that becomes true in case an animation is still going on.
        //This prevents bugging because of overlapping elements.
        bool disableResize = false;


        //When a user clicks on the box, the exercise starts.
        private void Exercise(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement clickedElement = e.Source as FrameworkElement;
            

        }

        //When the mouse enters an Exercise box this happens
        private void OnBoxEnter(object sender, EventArgs e)
        {
            //Only if no other animation is going on this will be true
            if (!disableResize)
            {
                //Set the disableResize so that no other animations can start while this one is going on
                disableResize = true;

                //Declaring the different animation objects
                DoubleAnimation animation = new DoubleAnimation();
                DoubleAnimation fadeAnimation = new DoubleAnimation();
                ThicknessAnimation marginAnimation = new ThicknessAnimation();

                //Get the TextBlock that was hovered over.
                Border hoveredBox = ((Border)sender);

                //Every other TextBlock in the grid will be hidden
                foreach (Border x in BoxGrid.Children)
                {
                    if (x != hoveredBox)
                    {
                        //Changing the opacity of the non-selected boxes to zero with an fading animation
                        fadeAnimation.From = x.Opacity;
                        fadeAnimation.To = 0;
                        fadeAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(200));
                        x.BeginAnimation(OpacityProperty, fadeAnimation);
                    }
                }

                //Adding extra information to the exercise box (Here will the level and the discription be shown)
                TextBlock textBlock = (TextBlock)((Grid)hoveredBox.Child).Children[0];
                textBlock.Text += " Whoa";

                //The margin of the current TextBlock will be set to 0 with an animation
                marginAnimation.From = hoveredBox.Margin;
                marginAnimation.To = new Thickness(0, 0, 0, 0);
                marginAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(200));
                hoveredBox.BeginAnimation(MarginProperty, marginAnimation);

                //The width of the TextBlock will be set to the same width of the GridBox with an animation
                animation.From = BoxGrid.ColumnDefinitions[0].Width.Value;
                animation.To = BoxGrid.Width;
                animation.Duration = new Duration(TimeSpan.FromMilliseconds(200));
                hoveredBox.BeginAnimation(WidthProperty, animation);
            }



        }

        //End of mouse hover (Reset to begin values)
        private void OnBoxLeave(object sender, EventArgs e)
        {
            //Declare all the animation types
            DoubleAnimation animation = new DoubleAnimation();
            DoubleAnimation fadeAnimation = new DoubleAnimation();
            ThicknessAnimation marginAnimation = new ThicknessAnimation();

            //Add Animation_Completed to animation to run it when the animation is completed
            animation.Completed += Animation_Completed;

            //Get the TextBlock that was hovered over.
            Border hoveredBox = ((Border)sender);


            //Setting the hovered TextBlock back to its origional value with an animation
            animation.From = hoveredBox.Width;
            animation.To = BoxGrid.ColumnDefinitions[0].Width.Value;
            animation.Duration = new Duration(TimeSpan.FromMilliseconds(200));
            hoveredBox.BeginAnimation(WidthProperty, animation);


            //Setting the margin of the hovered TextBlock back to the origional value with an animation
            marginAnimation.From = hoveredBox.Margin;
            marginAnimation.To = new Thickness((BoxGrid.Children.IndexOf(hoveredBox)) * 26 + (BoxGrid.Children.IndexOf(hoveredBox) * 122), 0, 0, 0); ;
            marginAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(200));
            hoveredBox.BeginAnimation(MarginProperty, marginAnimation);

            //Set the text of the ExerciseBox back to its origional value
            TextBlock textBlock = (TextBlock)((Grid)hoveredBox.Child).Children[0];
            textBlock.Text = $" Werkt nog nie";

            //Make all other TextBlock visible again.
            foreach (Border x in BoxGrid.Children)
            {
                if (x != hoveredBox)
                {
                    //Animate the visibility to be visible again
                    fadeAnimation.From = x.Opacity;
                    fadeAnimation.To = 1;
                    fadeAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(200));
                    x.BeginAnimation(OpacityProperty, fadeAnimation);

                }
            }


        }

        private void Animation_Completed(object sender, EventArgs e)
        {
            //Set the disableResize to false so other animations can start again
            disableResize = false;
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
