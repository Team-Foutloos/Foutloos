using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foutloos.CustomTools
{
    public class LoginFunctions
    {
        static public bool isTrue()
        {
            return true;
        }

        static public bool login(string username, string password)
        {
            string hashedPassword = SecurePasswordHasher.Hash(password);
            //query that is being executed and being shows in a Table in the application.
            string connectionstring = "Data Source=127.0.0.1,1433; User Id=sa;Password=Foutloos!; Initial Catalog=foutloos_db;";
            string CmdString = $"SELECT * FROM Usertable WHERE username = @username";
            using (SqlConnection con = new SqlConnection(connectionstring))
            {
                try
                {
                    con.Open();
                    SqlCommand insCmd = new SqlCommand(CmdString, con);

                    //Set the max timeout of the sql command to 5.
                    insCmd.CommandTimeout = 5;
                    // use sqlParameters to prevent sql injection!
                    insCmd.Parameters.AddWithValue("@username", username);
                    insCmd.Parameters.AddWithValue("@password", hashedPassword);
                    using (SqlDataReader reader = insCmd.ExecuteReader())
                    {
                        if (reader.Read() && SecurePasswordHasher.Verify(password, (string)reader["password"]))
                        {
                            ConfigurationManager.AppSettings["username"] = (string)reader["username"];
                            ConfigurationManager.AppSettings["userID"] = (string)reader["userID"].ToString();

                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                catch (Exception f)
                {
                    return false;
                }
                con.Close();
                return true;
            }
        }
    }
}
