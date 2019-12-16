using System;
using System.Data;
using System.Windows.Controls;

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
