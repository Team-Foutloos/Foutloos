using Foutloos.Multiplayer;
using System.Windows;

namespace Foutloos
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {

            InitializeComponent();
            this.Content = new HomeScreen();

            //Prohibit window from being rescaled
            MouseDoubleClick += (sender, args) =>
            {
                args.Handled = true;
            };
        }
    }
}
