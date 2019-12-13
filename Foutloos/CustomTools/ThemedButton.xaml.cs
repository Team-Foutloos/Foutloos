using System.Windows;
using System.Windows.Controls;

namespace Foutloos
{
    /// <summary>
    /// Interaction logic for ThemedButton.xaml
    /// </summary>
    public partial class ThemedButton : UserControl
    {
        //Creating a dependency property that can be used as regular property for adding text
        public static readonly DependencyProperty DynamicTextProperty =
       DependencyProperty.Register(
          "DynamicTextRegular",
          typeof(string),
          typeof(TextBlock),
          new FrameworkPropertyMetadata(null));

        //Setting a variable with getter and setter for storing the dynamic text
        public string DynamicTextRegular
        {
            get { return (string)GetValue(DynamicTextProperty); }
            set { SetValue(DynamicTextProperty, value); ThemedButtonTextBlock.Text = DynamicTextRegular; }
        }

        public ThemedButton()
        {
            InitializeComponent();

        }
    }
}
