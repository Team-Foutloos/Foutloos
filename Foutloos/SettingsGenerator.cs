using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Foutloos
{
    class WordGenerator
    {
        public WordGenerator()
        {

        }

        public string startupRandomText()
        {

            Connection c = new Connection();

            string exerciseText = "";

            DataTable mostMistakes = new DataTable();
            DataTable dt0 = new DataTable();

            mostMistakes = c.PullData("SELECT TOP 1 letter FROM Result R RIGHT JOIN Usertable U On R.userID = U.userID " +
                $"JOIN Error E ON R.resultID = E.resultID WHERE username = '{ConfigurationManager.AppSettings["username"]}' AND letter NOT LIKE '% %' " +
                $"GROUP BY letter ORDER BY SUM(count) DESC");

            dt0 = c.PullData($"SELECT * FROM dictionary WHERE list LIKE '%{mostMistakes.Rows[0]["letter"]}%'");
            Random rand = new Random();

            for (int i = 0; i < 20; i++)
            {
                exerciseText += dt0.Rows[rand.Next(0, dt0.Rows.Count)]["list"].ToString();
                if (i != 19)
                {
                    exerciseText += " ";
                }
            }
            return exerciseText;
        }

        public string startupRandomText(int value, bool timerMode)
        {
            Connection c = new Connection();

            if (timerMode)
            {
                //The text for the exercise
                string exerciseText = "";

                //creates new data tabels
                DataTable mostMistakes = new DataTable();
                DataTable dt0 = new DataTable();

                //Pulls a list of words based on the letters you did wrong the most
                mostMistakes = c.PullData("SELECT TOP 1 letter FROM Result R RIGHT JOIN Usertable U On R.userID = U.userID " +
                    $"JOIN Error E ON R.resultID = E.resultID WHERE username = '{ConfigurationManager.AppSettings["username"]}' AND letter NOT LIKE '% %' " +
                    $"GROUP BY letter ORDER BY SUM(count) DESC");
                if (mostMistakes.Rows[0]["letter"].ToString().Equals(" "))
                {
                    dt0 = c.PullData($"SELECT * FROM dictionary WHERE list LIKE '%{mostMistakes.Rows[1]["letter"]}%'");
                }
                else
                {
                    dt0 = c.PullData($"SELECT * FROM dictionary WHERE list LIKE '%{mostMistakes.Rows[0]["letter"]}%'");
                }
                Random rand = new Random();

                //fills the exerciseText with a set amount of text
                for (int i = 0; i < 20; i++)
                {
                    exerciseText += dt0.Rows[rand.Next(0, dt0.Rows.Count)]["list"].ToString();
                    if (i != 19)
                    {
                        exerciseText += " ";
                    }
                }
                return exerciseText;
            }
            else
            {
                //The text for the exercise
                string exerciseText = "";

                //creates new data tabels
                DataTable mostMistakes = new DataTable();
                DataTable dt0 = new DataTable();

                //Pulls a list of words based on the letters you did wrong the most
                mostMistakes = c.PullData("SELECT TOP 1 letter FROM Result R RIGHT JOIN Usertable U On R.userID = U.userID " +
                    $"JOIN Error E ON R.resultID = E.resultID WHERE username = '{ConfigurationManager.AppSettings["username"]}' AND letter NOT LIKE '% %' " +
                    $"GROUP BY letter ORDER BY SUM(count) DESC");
                dt0 = c.PullData($"SELECT * FROM dictionary WHERE list LIKE '%{mostMistakes.Rows[0]["letter"]}%'");
                Random rand = new Random();

                //fills the exerciseText with a set amount of text
                for (int i = 0; i < value; i++)
                {
                    exerciseText += dt0.Rows[rand.Next(0, dt0.Rows.Count)]["list"].ToString();
                    if (i != value - 1)
                    {
                        exerciseText += " ";
                    }
                }
                return exerciseText;
            }
        }


    }
}
