using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                x.Text = x.Name;

                //Adding the events
                x.MouseEnter += OnBoxEnter;
                x.MouseLeave += OnBoxLeave;
            }
            

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            owner.Content = new ExercisesPage();
        }


        //Start of mouse hover
        private void OnBoxEnter(object sender, EventArgs e)
        {
            //Get the TextBlock that was hovered over.
            TextBlock hoveredBox = ((TextBlock)sender);
            //Every other TextBlock in the grid will be hidden
            foreach (TextBlock x in BoxGrid.Children)
            {
                if (x != hoveredBox)
                {
                    x.Visibility = Visibility.Hidden;
                }
            }
            //The margin of the current TextBlock will be set to 0
            hoveredBox.Margin = new Thickness(0, 0, 0, 0);
            //The width of the TextBlock will be set to the same width of the GridBox
            hoveredBox.Width = BoxGrid.Width;

            
        }

        //End of mouse hover (Reset to begin values)
        private void OnBoxLeave(object sender, EventArgs e)
        {
            //Get the TextBlock that was hovered over.
            TextBlock hoveredBox = ((TextBlock)sender);
            //Setting the hovered TextBlock back to its origional value
            hoveredBox.Width = 122;
            //Setting the margin of the hovered TextBlock back to the origional value
            hoveredBox.Margin = new Thickness((BoxGrid.Children.IndexOf(hoveredBox)) *26 + (BoxGrid.Children.IndexOf(hoveredBox)*122), 0 ,0 ,0);

            //Make all other TextBlock visible again.
            foreach (TextBlock x in BoxGrid.Children)
            {
                if (x != hoveredBox)
                {
                    x.Visibility = Visibility.Visible;
                }
            }
        }
    }
}
