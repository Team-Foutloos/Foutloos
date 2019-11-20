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
    /// Interaction logic for HomeScreen.xaml
    /// </summary>
    public partial class HomeScreen : Page
    {
        MainWindow owner;
        public HomeScreen(MainWindow owner)
        {
            this.owner = owner;
            InitializeComponent();

        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            owner.Content = new ExercisesPage();
        }

        //This function handles the clicking of the 'login' button.
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            UIElement rootVisual = this.Content as UIElement;
            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(rootVisual);
            if (rootVisual != null && adornerLayer != null)
            {
                DarkenAdorner darkenAdorner = new DarkenAdorner(rootVisual);
                adornerLayer.Add(darkenAdorner);
                ModalLogin modal = new ModalLogin { Owner = this.owner };
                modal.ShowDialog();
                adornerLayer.Remove(darkenAdorner);
            }
        }
    }

    //Create this class to give the 'Darken effect' used while your inside of the modal.
    public class DarkenAdorner : Adorner
    {
        public Brush DarkenBrush { get; set; }

        public DarkenAdorner(UIElement adornedElement)
          : base(adornedElement)
        {
            Brush darkenBrush = new SolidColorBrush(new Color() { R = 0, G = 0, B = 0, A = 140 });
            darkenBrush.Freeze();
            DarkenBrush = darkenBrush;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            drawingContext.DrawRectangle(DarkenBrush, null, new Rect(0, 0, AdornedElement.RenderSize.Width, AdornedElement.RenderSize.Height));
        }
    }
}
