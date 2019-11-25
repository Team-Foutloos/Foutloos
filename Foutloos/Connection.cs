using System;
using System.Data.SqlClient;
using System.Windows.Controls;
using System.Windows.Forms;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace Foutloos
{     

    public partial class Connection : Form
    {

      

        public Connection()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            runQuery();
        }

        private void runQuery()
        {


            string server = "127.0.0.1,1433";
            string username = "sa";
            string password = "Foutloos!";
            string database = "foutloos_db"; 
                       
            string myConnectionString = "Data Source=" + server + ";" +
                                        "Initial Catalog=" + database + ";" +
                                        "User ID=" + username + ";" +
                                        "Password=" + password + ";";

            string query = textBox1.Text;

            if (query == "")
            {
                MessageBox.Show("Please insert a sql query");
                return;
            }




            SqlConnection databaseConnection = new SqlConnection(myConnectionString);
            SqlCommand commandDatabase = new SqlCommand(query, databaseConnection);

            commandDatabase.CommandTimeout = 60;

            try
            {
                databaseConnection.Open();
                MessageBox.Show("Connection Open ! ");
                SqlDataReader myReader = commandDatabase.ExecuteReader();

                if (myReader.HasRows)
                {

                    MessageBox.Show("Your query generated results");

                    while (myReader.Read())
                    {
                        Console.WriteLine(myReader.GetString(0));
                    }
                }
                else
                {
                    MessageBox.Show("Query succecsfully executed");
                }

            }
            catch (Exception e)
            {
                MessageBox.Show("Failed: " + e.Message);
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}

