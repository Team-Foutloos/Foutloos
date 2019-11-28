using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Foutloos
{
    /// <summary>
    /// Interaction logic for HomeScreen.xaml
    /// </summary>
    public partial class HomeScreen : Page
    {
        public static MainWindow owner;
        public HomeScreen(MainWindow Owner)
        {
            owner = Owner;
            InitializeComponent();

            //Update the UI.
            this.loginUIchange();

            //Going thru all the TextBlocs in the grid to add the hover events.
            foreach (Border x in BoxGrid.Children)
            {
                //Setting a standard text to each TextBlock
                //Here will the random exercises from the database come.
                TextBlock textBlock = ((TextBlock)x.Child);
                textBlock.Text += "\nTest tekst";
                textBlock.Text = $"{BoxGrid.Children.IndexOf(x)+1}";

                //Adding the events
                x.MouseEnter += OnBoxEnter;
                x.MouseLeave += OnBoxLeave;
                x.MouseDown += Exercise;
            }
            //Add a listener to all the 'Buttons' (All exercises, login and register)

        }

        //Change things when a user logs in.
        public void loginUIchange()
        {
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["username"]))
            {
                //settingsBtn.DynamicText = "Account";
                Title.Content = $"Welcome {ConfigurationManager.AppSettings["username"]}";
                ButtonRowAccount.Visibility = Visibility.Collapsed;
                seeProgress.Visibility = Visibility.Visible;
            }
            else
            {
                //settingsBtn.DynamicText = "Settings";
                seeProgress.Visibility = Visibility.Collapsed;
            }
        }



        //Boolean that becomes true in case an animation is still going on.
        //This prevents bugging because of overlapping elements.
        bool disableResize = false;


        //When a user clicks on the box, the exercise starts.
        private void Exercise(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement clickedElement = e.Source as FrameworkElement;
            if (clickedElement == BoxBorder1 || clickedElement == Box1)
            {
                owner.Content = new VoiceExercise();
            }
            else
            {
                owner.Content = new Exercise(owner);
            }
            
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
                TextBlock textBlock = ((TextBlock)hoveredBox.Child);  
                textBlock.Text += " - This exercise is amazing!!!!!!1!";

                //The margin of the current TextBlock will be set to 0 with an animation
                marginAnimation.From = hoveredBox.Margin;
                marginAnimation.To = new Thickness(0, 0, 0, 0);
                marginAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(200));
                hoveredBox.BeginAnimation(MarginProperty, marginAnimation);

                //The width of the TextBlock will be set to the same width of the GridBox with an animation
                animation.From = hoveredBox.Width;
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
            animation.To = 122;
            animation.Duration = new Duration(TimeSpan.FromMilliseconds(200));
            hoveredBox.BeginAnimation(WidthProperty, animation);


            //Setting the margin of the hovered TextBlock back to the origional value with an animation
            marginAnimation.From = hoveredBox.Margin;
            marginAnimation.To = new Thickness((BoxGrid.Children.IndexOf(hoveredBox)) * 26 + (BoxGrid.Children.IndexOf(hoveredBox) * 122), 0, 0, 0); ;
            marginAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(200));
            hoveredBox.BeginAnimation(MarginProperty, marginAnimation);

            //Set the text of the ExerciseBox back to its origional value
            TextBlock textBlock = ((TextBlock)hoveredBox.Child);
            textBlock.Text = $"{BoxGrid.Children.IndexOf(hoveredBox)+1}";

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
                DarkenAdorner darkenAdorner = new DarkenAdorner(rootVisual);
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
            owner.Content = new ExercisesPage();
        }

        private void LoginBtn_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ShowModal(new ModalLogin(this) { Owner = HomeScreen.owner });
        }

        private void RegisterBtn_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ShowModal(new ModalRegister { Owner = HomeScreen.owner });
        }
    }

    //Create this class to give the 'Darken effect' used while the modal is opened.
    public class DarkenAdorner : Adorner
    {
        public Brush DarkenBrush { get; set; }

        public DarkenAdorner(UIElement adornedElement)
          : base(adornedElement)
        {
            Brush darkenBrush = new SolidColorBrush(new Color() { R = 0, G = 0, B = 0, A = 140 });
            darkenBrush.Freeze();
            DarkenBrush = darkenBrush;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            drawingContext.DrawRectangle(DarkenBrush, null, new Rect(0, 0, AdornedElement.RenderSize.Width, AdornedElement.RenderSize.Height));
        }
    }
}
