using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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
          "DynamicTextIcon",
          typeof(string),
          typeof(TextBlock),
          new FrameworkPropertyMetadata(null));

        //Setting a variable with getter and setter for storing the dynamic text
        public string DynamicTextIcon
        {
            get { return (string)GetValue(DynamicTextProperty); }
            set { SetValue(DynamicTextProperty, value); ThemedIconButtonTextBlock.Text = DynamicTextIcon; }
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
            set { SetValue(DynamicIconProperty, value);  }
        }

        public ThemedIconButton()
        {
            InitializeComponent();
        }

    }
}






