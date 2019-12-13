using System;
using System.Collections.Generic;
using System.Data;
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
    /// Interaction logic for RandomExcersice.xaml
    /// </summary>
    public partial class RandomExcersice : Page
    {
        Connection c;
        DataTable dt0;
        string exerciseText;
        public RandomExcersice()
        {
            c = new Connection();
            InitializeComponent();

            
            dt0 = new DataTable();

            dt0 = c.PullData($"SELECT * FROM Dictionary");
            Random rand = new Random();

            for (int i = 0; i < 20; i++)
            {
                exerciseText += dt0.Rows[rand.Next(0, dt0.Rows.Count)]["list"].ToString();
                exerciseText += " ";
            }
            text.Text = exerciseText;
        }

    }
}
