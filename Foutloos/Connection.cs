﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

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
                //System.Windows.Forms.MessageBox.Show($"No connection{e}");
            }

            return dataTable;
        }

        //returns the amount of packages connected to an account.
        public List<int> getPackages(string query)
        {
            List<int> packages = new List<int>();
            SqlConnection conn = new SqlConnection(connectionstring);
            conn.Open();

            try
            {

                
                SqlCommand cmd = new SqlCommand(query, conn);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        packages.Add(reader.GetInt32(0));
                    }
                }


            }
            catch (Exception e)
            {

            }
            conn.Close();

            return packages;

        }


        public string getPassword(int ID)
        {
            try
            {
                string hashedPassword = null;
                string query = $"SELECT password FROM userTable WHERE userID = '{ID}'";
                SqlConnection conn = new SqlConnection(connectionstring);
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                using (SqlDataReader oReader = cmd.ExecuteReader())
                {
                    try
                    {
                        while (oReader.Read())
                        {
                            hashedPassword = oReader["password"].ToString();
                        }
                        conn.Close();
                        return hashedPassword;
                    }
                    catch (Exception e)
                    {
                        System.Windows.Forms.MessageBox.Show($"Could not read and return any data{e}");
                        return hashedPassword;
                    }
                }
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show($"No connection or wrong query{e}");
                return null;
            }
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
                //System.Windows.Forms.MessageBox.Show($"No connection or wrong query{e}");
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

        public int getPackageCount(int packageID, int difficulty)
        {
            int id = 0;
            string query = $"SELECT count(exerciseID) FROM exercise WHERE packageID = {packageID} AND difficulty = {difficulty}";

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

        public int getPackageCount(int packageID)
        {
            int id = 0;
            string query = $"SELECT count(exerciseID) FROM exercise WHERE packageID = {packageID}";

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

        public int getPackageCountFinished(int userID)
        {
            int id = 0;
            string query = $"select count(distinct exerciseID) from Result where userID = {userID};";

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

        public string getPackageName(string query)
        {
            string packageName;

            try
            {
                SqlConnection conn = new SqlConnection(connectionstring);
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                
                conn.Close();
                
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show($"No connection or wrong query{e}");
                
            }

            return null;
        }

        public void leaveRoom()
        {
            this.insertInto($"DELETE FROM roomplayer WHERE userID = {ConfigurationManager.AppSettings["userID"]}");
            this.insertInto($"DELETE FROM roomExercise WHERE roomID NOT IN (SELECT roomID FROM roomplayer)");
            //Check if the room is empy, if it is, delete the room.
            this.insertInto($"DELETE FROM room WHERE roomID NOT IN (SELECT roomID FROM roomplayer)");
        }

        public bool checkConnection()
        {
            SqlConnection conn = new SqlConnection(connectionstring);
            try
            {
                conn.Open();
            }catch(Exception e)
            {
                return false;
            }
            return true;
        }

    }
}
