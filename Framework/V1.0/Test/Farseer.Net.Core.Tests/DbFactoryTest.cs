using System;
using Demo.PO;
using Demo.VO.Members;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Farseer.Net.Core.Tests
{
    [TestClass]
    public class DbFactoryTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            object o = new UserPO();
            for (int i = 0; i < 1000000; i++)
            {
                var p = (UserPO)o;
                o = p;
                p = (UserPO)o;

                o = p;
                p = (UserPO)o;

                o = p;
                p = (UserPO)o;
            }
        }
    }
}
