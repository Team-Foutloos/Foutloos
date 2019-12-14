using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Foutloos.Modals
{
    /// <summary>
    /// Interaction logic for ChangePassword.xaml
    /// </summary>
    public partial class ChangePassword : Window
    {
        Connection c = new Connection();

        private int userID;
        private string errorMessage;


        public ChangePassword(int id)
        {
            InitializeComponent();
            userID = id;
        }

        private void SaveBtn_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            string passwordFD = c.getPassword(userID);
            string passwordOld = SecurePasswordHasher.Hash(oldpassword.Password);
            if (SecurePasswordHasher.Verify(oldpassword.Password, passwordFD))
            {
                string passwordNew = newpassword.Password;
                string passwordRepeat = Repeatpassword.Password;
                bool fHasSpace = passwordNew.Contains(" ");
                if (newpassword.Password != null && newpassword.Password != "" && !fHasSpace)
                {
                    if (passwordNew.Count() < 8)
                    {
                        errorMessage += "Password is too short or to long";
                    }
                    else if (passwordNew != passwordRepeat)
                    {
                        errorMessage += "A password is incorrect";
                        oldpassword.Clear();
                        newpassword.Clear();
                        Repeatpassword.Clear();
                    }
                    else
                    {
                        string CmdString = $"UPDATE Usertable SET password = '{SecurePasswordHasher.Hash(passwordNew)}' WHERE userID = '{userID}'";
                        if (c.insertInto(CmdString))
                        {
                            this.Close();
                        }
                    }
                }
                else
                {
                    errorMessage += "Please add a new password at 'new Password'";
                }
            }
            else
            {
                errorMessage += "A password is incorrect";
                oldpassword.Clear();
                newpassword.Clear();
                Repeatpassword.Clear();
            }
            ErrorMessage.Content = errorMessage;
            errorMessage = "";
        }

        private void CancelBtn_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }
    }
}
