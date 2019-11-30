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

namespace Foutloos.Modals
{
    /// <summary>
    /// Interaction logic for YesCancelModal.xaml
    /// </summary>
    public partial class YesCancelModal : Window
    {
        public YesCancelModal()
        {
            InitializeComponent();
        }

        private void ThemedButton_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
            Application.Current.MainWindow.Content = new Foutloos.ExercisesPage();
        }

        private void ThemedButton_PreviewMouseDown_1(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }
    }
}
