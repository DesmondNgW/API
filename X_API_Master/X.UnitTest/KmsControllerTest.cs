using System;
using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using X.Interface.Other;
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
            Assert.AreEqual(ret.Data, new KmsManagerService().Now());
        }

        [TestMethod]
        public void MobileEncrypt()
        {
            var api = new KmsController();
            var ret = api.MobileEncrypt("1234567890");
            Assert.AreEqual(ret, new KmsManagerService().MobileEncrypt("1234567890"));
        }
    }
}
