using Microsoft.VisualStudio.TestTools.UnitTesting;
using Foutloos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace Foutloos.Tests
{
    [TestClass()]
    public class ConnectionTests
    {
        Connection c = new Connection();

        [TestMethod()]
        public void PullDataTest()
        {
            DataTable dt = new DataTable();
            dt = c.PullData("SELECT * from Result join exercise on result.exerciseID = exercise.exerciseID");

            Assert.IsNotNull(dt.Rows.Count);
        }

        [TestMethod()]
        public void getPackagesTest()
        {
            List<int> package = new List<int>();
            package = c.getPackages("select packageID from Usertable join License on Usertable.userID = license.userID where Usertable.username = 'allpackages'");
            Assert.AreEqual(7, package.Count());
        }

        [TestMethod()]
        public void getPasswordTest()
        {
            string password = c.getPassword(31);
            Assert.AreEqual("cn/PFDmYfai/tRJQEw==", password);

        }        

        [TestMethod()]
        public void IDTest()
        {
            int id = c.ID("SELECT userID FROM userTable WHERE username = 'allpackages'");
            Assert.AreEqual(31, id);
        }

        [TestMethod()]
        public void getPackageCountTest()
        {
            int id = c.getPackageCount(1, 1);
            Assert.AreEqual(6, id);
        }

        
        [TestMethod()]
        public void checkConnectionTest()
        {
            Assert.IsTrue(c.checkConnection());
        }
    }
}