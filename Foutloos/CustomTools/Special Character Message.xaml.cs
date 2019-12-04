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
    /// Interaction logic for Special_Character_Message.xaml
    /// </summary>
    public partial class Special_Character_Message : UserControl
    {
        public Special_Character_Message(char c)
        {
            InitializeComponent();

            if(c == 129)
            {
                Text.Text = "shift & \" + u";
            }
            else if(c == 130)
            {
                Text.Text = "\' + e";
            }
            else if(c == 132)
            {
                Text.Text = "\" + a";
            }
            else if(c == 133)
            {
                Text.Text = "` + a";
            }
            else if(c == 137)
            {
                Text.Text = "shift & \" + e";
            }
            else if(c == 138)
            {
                Text.Text = "` + e";
            }
            else if(c == 139)
            {
                Text.Text = "shift & \" + i";
            }
            else if(c == 141)
            {
                Text.Text = "` + i";
            }
        }
    }
}
