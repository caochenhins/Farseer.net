using Demo.PO;
using Demo.VO.Members;
using FS.Core.Data.Table;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Farseer.Net.Core.Tests.Table
{
    [TestClass]
    public class TableContext1Test
    {
        [TestMethod]
        public void NewTableContextTestMethod()
        {
            using (new UserPO()) { }
        }

        [TestMethod]
        public void StaticTableContextTestMethod()
        {
            Assert.AreEqual(TableContext<UserVO>.Data != null, true);
        }
    }
}