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
    /// Interaction logic for ThemedButton.xaml
    /// </summary>
    public partial class ThemedIconButton : UserControl
    {

        //Creating a dependency property that can be used as regular property for adding text
        public static readonly DependencyProperty DynamicTextProperty =
       DependencyProperty.Register(
          "DynamicText",
          typeof(string),
          typeof(TextBlock),
          new FrameworkPropertyMetadata(null));

        //Setting a variable with getter and setter for storing the dynamic text
        public string DynamicText
        {
            get { return (string)GetValue(DynamicTextProperty); }
            set { SetValue(DynamicTextProperty, value);  }
        }

        //Creating a dependency property that can be used as regular property for adding an icon
        public static readonly DependencyProperty DynamicIconProperty =
      DependencyProperty.Register(
         "DynamicIcon",
         typeof(BitmapFrame),
         typeof(ImageSource),
         new FrameworkPropertyMetadata(null));

        //Setting a variable with getter and setter for storing the dynamic icon
        public BitmapFrame DynamicIcon
        {
            get { return (BitmapFrame)GetValue(DynamicIconProperty); }
            set { SetValue(DynamicIconProperty, value); }
        }

        public ThemedIconButton()
        {
            InitializeComponent();
        }
    }
}






