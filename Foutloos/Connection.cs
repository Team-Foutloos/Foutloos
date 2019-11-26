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
        


        public List<List<object>> QueryDataExercisesTable(string query)
        {
        List<List<object>> value = new List<List<object>>();
        

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
                            List<object> oneRow = new List<object>();

                            oneRow.Add(dr.GetInt32(0));
                            oneRow.Add(dr.GetString(1));
                            oneRow.Add(dr.GetString(2));
                            oneRow.Add(dr.GetInt32(3));

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
