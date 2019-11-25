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
using System.Windows.Shapes;

namespace Foutloos
{
    /// <summary>
    /// Interaction logic for ModalLogin.xaml
    /// </summary>
    public partial class ModalRegister : Window
    {
        public ModalRegister()
        {
            InitializeComponent();

            //Add the events to the mousedown, both being to cancel the modal at this time.
            register.MouseDown += Button_Click;
            cancelRegister.MouseDown += Button_Click;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
