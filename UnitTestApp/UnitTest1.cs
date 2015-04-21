using System;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using MyerMomentUniversal.Helper;
using MyerMomentUniversal.Model;

namespace UnitTestApp
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            MomentStyle style = new MomentStyle("Alone");
            Assert.IsNotNull(style.PreviewImge);
            
        }

    }
}
