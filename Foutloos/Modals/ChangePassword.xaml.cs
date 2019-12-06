using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
            string passwordFD = c.getPassword(userID);
            string passwordOld = SecurePasswordHasher.Hash(oldpassword.Password);
            if (passwordFD.Equals(passwordOld))
            {
                string passwordNew = SecurePasswordHasher.Hash(newpassword.Password);
                string passwordRepeat = SecurePasswordHasher.Hash(Repeatpassword.Password);
                if (passwordRepeat.Equals(passwordNew))
                {
                    string CmdString = $"UPDATE Usertable SET password = '{passwordNew}' WHERE userID = '{userID}'";
                    if (c.insertInto(CmdString))
                    {
                        this.Close();
                    }
                }
            }
        }    
        
        private void CancelBtn_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
                this.Close();
        }
    }
}
