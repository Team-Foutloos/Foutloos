using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foutloos
{
    public class Connection
    {
        string connectionstring = "Data Source=127.0.0.1,1433; User Id=sa;Password=Foutloos!; Initial Catalog=foutloos_db;";
        string CmdString = string.Empty;
        


        public List<List<string>> QueryDataExercisesTable(string query)
        {
        List<List<string>> value = new List<List<string>>();
        

            try
            {
                using (SqlConnection con = new SqlConnection(connectionstring))
                {
                    SqlCommand cmd = new SqlCommand(query, con);

                    con.Open();

                    SqlDataReader dr = cmd.ExecuteReader();

                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            List<string> oneRow = new List<string>();
                                                        
                            oneRow.Add(dr.GetString(1));
                            oneRow.Add(dr.GetString(2));
                            

                            value.Add(oneRow);
                        }
                    }
                }                
            }

            
            catch (Exception e)
            {
                Console.WriteLine("NO CONNECTION");
            }

            return value;

        }

    }
}
