using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foutloos
{
    class License
    {

        Connection c = new Connection();

        public void insertLicense(string license)
        {

            int id = c.ID($"select licenseID from license WHERE license = '{license}'");
            
            int idused = c.ID($"select used from license WHERE licenseID = '{id}'");
            
            int userID = c.ID($"select userID from Usertable WHERE username = '{ConfigurationManager.AppSettings["username"]}'");
           

            if (id > 0 && idused == 0)
            {
                c.insertInto($"UPDATE License SET userID = {userID}, used = 1 WHERE licenseID = {id}; ");
                System.Windows.Forms.MessageBox.Show("License Key has been added to your account");
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("License Key is already used or wrong");
            }
        }

    }
}
