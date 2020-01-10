using Microsoft.VisualStudio.TestTools.UnitTesting;
using Foutloos.CustomTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foutloos.CustomTools.Tests
{
    [TestClass()]
    public class LoginFunctionsTests
    {
        //Test het inloggen
        [TestMethod()]
        public void testLogin()
        {
            Assert.IsTrue(LoginFunctions.login("allpackages", "Foutloos!"));
        }

        [TestMethod()]
        public void loginTest()
        {
            Assert.Fail();
        }
    }
}
