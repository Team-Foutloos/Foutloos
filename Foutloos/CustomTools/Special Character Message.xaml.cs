using System.Windows.Controls;

namespace Foutloos
{
    public partial class Special_Character_Message : UserControl
    {
        public Special_Character_Message()
        {
            InitializeComponent();
        }

        public void ChangeText(char c)
        {
            if (c == 252)
            {
                Text.Text = "shift & \" + u";
            }
            else if (c == 233)
            {
                Text.Text = "\' + e";
            }
            else if (c == 228)
            {
                Text.Text = "shift & \" + a";
            }
            else if (c == 224)
            {
                Text.Text = "` + a";
            }
            else if (c == 235)
            {
                Text.Text = "shift & \" + e";
            }
            else if (c == 232)
            {
                Text.Text = "` + e";
            }
            else if (c == 239)
            {
                Text.Text = "shift & \" + i";
            }
            else if (c == 236)
            {
                Text.Text = "` + i";
            }
            else if (c == 246)
            {
                Text.Text = "shift & \" + o";
            }
            else if (c == 242)
            {
                Text.Text = "` + o";
            }
            else if (c == 249)
            {
                Text.Text = "` + u";
            }
            else if (c == 225)
            {
                Text.Text = "' + a";
            }
            else if (c == 237)
            {
                Text.Text = "' + i";
            }
            else if (c == 243)
            {
                Text.Text = "' + o";
            }
            else if (c == 250)
            {
                Text.Text = "' + u";
            }
        }
    }
}
