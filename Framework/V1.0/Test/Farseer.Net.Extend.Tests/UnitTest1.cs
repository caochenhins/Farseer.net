using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Farseer.Net.Extend.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {

            Action a = () => { var ss = 1; ss++; };
            for (var i = 0; i < 100000000; i++)
            {
                a();
            }
        }
        [TestMethod]
        public void TestMethod2()
        {
            var ss = 1;

            for (var i = 0; i < 100000000; i++)
            {
                ss++;
            }
        }
    }
}
