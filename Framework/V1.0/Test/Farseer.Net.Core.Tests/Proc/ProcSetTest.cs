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
            InsertUserPO.Data.Execute(info);
            Demo.PO.Proc.Instance.InsertUser.Execute(info);
            Assert.IsTrue(info.ID > 1);
        }

        [TestMethod]
        public void ValueTestMethod()
        {
            var info = new ValueUserVO { ID = 1 };
            var value = ValueUserPO.Data.Value(info, "");
            Assert.IsTrue(!string.IsNullOrWhiteSpace(value));
        }

        [TestMethod]
        public void ToListTestMethod()
        {
            var lst = ListUserPO.Data.ToList();
            Assert.IsNotNull(lst);
            Assert.IsTrue(lst.Count > 0);
        }

        [TestMethod]
        public void ToInfoTestMethod()
        {
            var info = new InfoUserVO { ID = 3 };
            info = InfoUserPO.Data.ToInfo(info);
            Assert.IsNotNull(info);
            Assert.IsTrue(info.ID == 3);
        }
    }
}