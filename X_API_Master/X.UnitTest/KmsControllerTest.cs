using System;
using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using X.UI.API.Controllers;

namespace X.UnitTest
{
    [TestClass]
    public class KmsControllerTest
    {
        [TestMethod]
        public void Now()
        {
            var api = new KmsController();
            var ret = api.Now();
            Assert.AreEqual(ret.Data, DateTime.Now);
        }
    }
}
