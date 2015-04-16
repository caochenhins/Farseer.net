using Demo.PO;
using Demo.VO.Members;
using FS.Core.Data.Proc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Farseer.Net.Core.Tests.Proc
{
    [TestClass]
    public class ProcSetTest
    {
        [TestMethod]
        public void ExecuteTestMethod()
        {
            var info = new InsertUserVO { UserName = "now111", PassWord = "old222" };
            ProcContext<InsertUserVO>.Data.Execute(info);
            Assert.IsTrue(info.ID > 1);
        }

        [TestMethod]
        public void ValueTestMethod()
        {
            var info = new ValueUserVO { ID = 1 };
            var value = ProcContext<ValueUserVO>.Data.Value(info, "");
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
            var info = new InfoUserVO { ID = 3 };
            info = ProcContext<InfoUserVO>.Data.ToInfo(info);
            Assert.IsNotNull(info);
            Assert.IsTrue(info.ID == 3);
        }
    }
}