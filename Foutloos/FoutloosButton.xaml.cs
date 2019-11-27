using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Foutloos
{
    /// <summary>
    /// Interaction logic for FoutloosButton.xaml
    /// </summary>
    public partial class FoutloosButton : UserControl
    {
        private bool hasIcon;
        private BitmapFrame icon;

        //Make setting the text of the button possible
        public string Text
        {
            //Get the text in the button
            get { return FoutloosButtonName.Text; }
            //Set the text of the button
            set
            {
                FoutloosButtonName.Text = value;
                //SetIcon();
            }
        }

        
        public bool HasIcon
        {
            get { return hasIcon; }
            set { this.hasIcon = value; }
        }

        public BitmapFrame Icon
        {
            get { return this.icon; }
            set { this.icon = value; }
        }

        public FoutloosButton()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            SetIcon();
        }

        public void SetIcon()
        {
            iconImage.Source = this.icon;
            iconImage.Margin = new Thickness(10, 0, 0, 0);
            iconImage.Visibility = Visibility.Visible;

        }



    }
}
