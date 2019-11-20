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
    /// Interaction logic for ExercisesPage.xaml
    /// </summary>
    public partial class ExercisesPage : Page
    {

        private bool text = false;
        private bool spoken = false;
      
        public ExercisesPage()
        {
            InitializeComponent();
            Description.Content = "jkdsklfjdsfjdsjkfjsl";
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            HomeScreen.owner.Content = new HomeScreen(HomeScreen.owner);
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void check_radio()
        {
            if (Text.IsChecked == true)
            {
                this.text = true;
            }
            if (Spoken.IsChecked == true)
            {
                this.spoken = true;
            }            
        } 



        
    }
}
