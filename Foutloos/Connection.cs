using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foutloos
{
    public class Connection
    {
        string connectionstring = "Data Source=127.0.0.1,1433; User Id=sa;Password=Foutloos!; Initial Catalog=foutloos_db;";

        public DataTable PullData(string query)
        {

            DataTable dataTable = new DataTable();

            try
            {
                SqlConnection conn = new SqlConnection(connectionstring);
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();

                // create data adapter
                SqlDataAdapter da = new SqlDataAdapter(cmd);

                // this will query your database and return the result to your datatable
                da.Fill(dataTable);
                conn.Close();
                da.Dispose();

            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show($"No connection{e}");
            }

            return dataTable;
        }

        //returns the amount of packages connected to an account.
        public List<int> getPackages(string query)
        {
            List<int> packages = new List<int>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionstring))
                {
                    SqlCommand cmd = new SqlCommand(query, conn);
                    conn.Open();
                    packages.Add((Int32)cmd.ExecuteScalar());
                    conn.Close();
                }


            }
            catch (Exception e)
            {
                
            }

            return packages;

        } 

        //insert data int to the database.
        public bool insertInto(string query)
        {

            try
            {
                SqlConnection conn = new SqlConnection(connectionstring);
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
                return true;
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show($"No connection or wrong query{e}");
                return false;
            }

        }

        //returns 1 integer out of the database.
        public int ID(string query)
        {
            int id = 0;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionstring))
                {
                    SqlCommand cmd = new SqlCommand(query, conn);
                    conn.Open();
                    id = (Int32)cmd.ExecuteScalar();
                    conn.Close();
                }


            }
            catch (Exception e)
            {
                //System.Windows.Forms.MessageBox.Show($"No connection");
            }


            return id;
        }


    }
}
 