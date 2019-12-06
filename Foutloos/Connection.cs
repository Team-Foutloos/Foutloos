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
                System.Windows.Forms.MessageBox.Show("No connection");
            }

            return dataTable;
        }


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
                System.Windows.Forms.MessageBox.Show("No connection or wrong query");
                return false;
            }

        }

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
                }


            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show("No connection");
            }


            return id;
        }


    }
}
 