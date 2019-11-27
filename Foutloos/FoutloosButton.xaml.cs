using System;
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

        public bool HomeButton { get; set; }

        //public BitmapImage Icon { get; set; }

        public FoutloosButton()
        {
            InitializeComponent();

        }

        /*public void SetIcon()
        {
            Image iconImage = new Image();
            iconImage.Source = Icon;
            iconImage.Margin = new Thickness(0, 0, 0, 0);
            iconImage.Width = 25;

            stackPanel.Children.Add(iconImage);
        }*/


    }
}
