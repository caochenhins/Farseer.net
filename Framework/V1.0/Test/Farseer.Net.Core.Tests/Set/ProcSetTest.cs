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
        public void ValueTestMethod()
        {
            var info = new ValueUserPO { ID = 1 };
            var value = ProcContext<ValueUserPO>.Data.Value(info, "");
            Assert.IsTrue(!string.IsNullOrWhiteSpace(value));
        }

        [TestMethod]
        public void ToListTestMethod()
        {
            var lst = ProcContext<ListUserPO>.Data.ToList();
            Assert.IsNotNull(lst);
            Assert.IsTrue(lst.Count > 0);
        }

        [TestMethod]
        public void ToInfoTestMethod()
        {
            var info = new InfoUserPO { ID = 3 };
            info = ProcContext<InfoUserPO>.Data.ToInfo(info);
            Assert.IsNotNull(info);
            Assert.IsTrue(info.ID == 3);
        }
    }
}