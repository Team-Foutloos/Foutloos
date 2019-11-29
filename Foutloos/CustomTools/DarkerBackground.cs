using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace Foutloos.CustomTools
{
    //Create this class to give the 'Darken effect' used while the modal is opened.
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
