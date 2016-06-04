using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Core;

namespace UnitTests
{
    [TestClass]
    public class Core
    {
        [TestMethod]
        public void Mocking()
        {
            Logic core = new Logic();
            Assert.AreEqual("0 = 0", core.Go("0=0"));
            Assert.AreEqual("0 = 0", core.Go("0=   0"));
            Assert.AreEqual("0 = 0", core.Go("-1-1=-(1+1)"));
        }
    }
}
