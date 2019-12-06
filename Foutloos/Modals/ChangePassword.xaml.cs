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
    /// Interaction logic for ChangePassword.xaml
    /// </summary>
    public partial class ChangePassword : Window
    {
        Connection c = new Connection();

        private int userID;


        public ChangePassword(int id)
        {
            InitializeComponent();
            userID = id;
        }

        private void SaveBtn_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

            var r = c.PullData($"SELECT password FROM userTable WHERE userID = '{userID}'");
            oldpassword.Password = r.Rows.ToString();
            
        }

        private void CancelBtn_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
