using System;
using System.Collections;
using System.Collections.Generic;
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
        MainWindow owner;
        public HomeScreen(MainWindow owner)
        {
            this.owner = owner;
            InitializeComponent();

            //Going thru all the TextBlocs in the grid to add the hover events.
            foreach(TextBlock x in BoxGrid.Children)
            {
                //Setting a standard text to each TextBlock
                //Here will the random exercises from the database come.
                x.Text = $"Exercise {x.Name}";

                //Adding the events
                x.MouseEnter += OnBoxEnter;
                x.MouseLeave += OnBoxLeave;
            }
            

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            owner.Content = new ExercisesPage();
        }


        bool disableResize = false;


        //Start of mouse hover
        private void OnBoxEnter(object sender, EventArgs e)
        {
            if (!disableResize)
            {
                disableResize = true;
                DoubleAnimation animation = new DoubleAnimation();
                DoubleAnimation fadeAnimation = new DoubleAnimation();
                ThicknessAnimation marginAnimation = new ThicknessAnimation();

                //Get the TextBlock that was hovered over.
                TextBlock hoveredBox = ((TextBlock)sender);
                //Every other TextBlock in the grid will be hidden
                foreach (TextBlock x in BoxGrid.Children)
                {
                    if (x != hoveredBox)
                    {
                        fadeAnimation.From = x.Opacity;
                        fadeAnimation.To = 0;
                        fadeAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(200));
                        x.BeginAnimation(OpacityProperty, fadeAnimation);
                    }
                }

                hoveredBox.Text += "\nTest tekst";

                //The margin of the current TextBlock will be set to 0
                marginAnimation.From = hoveredBox.Margin;
                marginAnimation.To = new Thickness(0, 0, 0, 0);
                marginAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(200));
                hoveredBox.BeginAnimation(MarginProperty, marginAnimation);

                //The width of the TextBlock will be set to the same width of the GridBox
                animation.From = hoveredBox.Width;
                animation.To = BoxGrid.Width;
                animation.Duration = new Duration(TimeSpan.FromMilliseconds(200));
                hoveredBox.BeginAnimation(WidthProperty, animation);
            }
            

            
        }

        //End of mouse hover (Reset to begin values)
        private void OnBoxLeave(object sender, EventArgs e)
        {
            DoubleAnimation animation = new DoubleAnimation();
            DoubleAnimation fadeAnimation = new DoubleAnimation();
            ThicknessAnimation marginAnimation = new ThicknessAnimation();

            animation.Completed += Animation_Completed;

            //Get the TextBlock that was hovered over.
            TextBlock hoveredBox = ((TextBlock)sender);


            //Setting the hovered TextBlock back to its origional value
            animation.From = hoveredBox.Width;
            animation.To = 122;
            animation.Duration = new Duration(TimeSpan.FromMilliseconds(200));
            hoveredBox.BeginAnimation(WidthProperty, animation);


            //Setting the margin of the hovered TextBlock back to the origional value
            marginAnimation.From = hoveredBox.Margin;
            marginAnimation.To = new Thickness((BoxGrid.Children.IndexOf(hoveredBox)) * 26 + (BoxGrid.Children.IndexOf(hoveredBox) * 122), 0, 0, 0); ;
            marginAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(200));
            hoveredBox.BeginAnimation(MarginProperty, marginAnimation);

            hoveredBox.Text = $"Exercise {hoveredBox.Name}";

            //Make all other TextBlock visible again.
            foreach (TextBlock x in BoxGrid.Children)
            {
                if (x != hoveredBox)
                {
                    fadeAnimation.From = x.Opacity;
                    fadeAnimation.To = 1;
                    fadeAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(200));
                    x.BeginAnimation(OpacityProperty, fadeAnimation);
                    
                }
            }

            
        }

        private void Animation_Completed(object sender, EventArgs e)
        {
            disableResize = false;
        }
    }
}
