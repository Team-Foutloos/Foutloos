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
    public partial class ToggleButton : UserControl
    {
        private Thickness leftSide = new Thickness(-39, 0, 0, 0);
        private Thickness rightSide = new Thickness(0, 0, -39, 0);
        private SolidColorBrush off = new SolidColorBrush(Color.FromRgb(160, 160, 160));
        private SolidColorBrush on = new SolidColorBrush(Color.FromRgb(130, 190, 125));
        public bool Toggled { get; set; } = true;

        public ToggleButton()
        {
            InitializeComponent();
            Back.Fill = on;
            Dot.Margin = rightSide;
        }

        private void Dot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(Toggled)
            {
                Back.Fill = off;
                Toggled = false;
                Dot.Margin = leftSide;
            }
            else
            {
                Back.Fill = on;
                Toggled = true;
                Dot.Margin = rightSide;
            }
        }

        private void Back_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Toggled)
            {
                Back.Fill = off;
                Toggled = false;
                Dot.Margin = leftSide;
            }
            else
            {
                Back.Fill = on;
                Toggled = true;
                Dot.Margin = rightSide;
            }
        }
    }
}
