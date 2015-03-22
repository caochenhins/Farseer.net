using Demo.PO.Proc;
using FS.Core.Context;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Farseer.Net.Core.Tests.Set
{
    [TestClass]
    public class ProcSetTest
    {
        [TestMethod]
        public void ExecuteTestMethod()
        {
            var info = new InsertUserPO { UserName = "now111", PassWord = "old222" };
            ProcContext<InsertUserPO>.Data.Execute(info);
            Assert.IsTrue(info.ID > 1);
        }

        [TestMethod]
        public void ToListTestMethod()
        {

        }
    }
}