using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using X.UI.Consoles;

namespace X.UnitTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            Assert.AreEqual(ConsoleHelper.ConvertIpv4("172.168.5.1"), 2896692481);
        }

        [TestMethod]
        public void TestMethod2()
        {
            Assert.AreEqual(ConsoleHelper.ConvertIpv4(" 172.168.5.1 "), 2896692481);
        }

        [TestMethod]
        public void TestMethod3()
        {
            Assert.AreEqual(ConsoleHelper.ConvertIpv4("172 . 168 . 5 . 1"), 2896692481);
        }

        [TestMethod]
        public void TestMethod4()
        {
            Assert.AreEqual(ConsoleHelper.ConvertIpv4("172 .168 .5 .1"), 2896692481);
        }

        [TestMethod]
        public void TestMethod5()
        {
            Assert.AreEqual(ConsoleHelper.ConvertIpv4("172. 168. 5. 1"), 2896692481);
        }

        [TestMethod]
        public void TestMethod6()
        {
            try
            {
                ConsoleHelper.ConvertIpv4("1 72. 1 68. 5. 1");
                Assert.Fail();
            }
            catch(Exception e)
            {
                Assert.IsTrue(e is ArgumentException);
            }
        }
    }
}
