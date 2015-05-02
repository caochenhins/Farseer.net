using Demo.PO;
using Demo.VO.Members;
using FS.Core;
using FS.Core.Data;
using FS.Core.Data.Table;
using FS.Core.Infrastructure;
using FS.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Farseer.Net.Core.Tests
{
    [TestClass]
    public class TimeTest
    {
        [TestMethod]
        public void TestTime()
        {
            SpeedTest.Initialize();
            SpeedTest.ConsoleTime("context", 100000, () =>
            {
                //var context = new Demo.PO.Table();
                var dbProvider = DbProvider.CreateInstance(DataBaseType.SqlServer);
                var contextx = new TableContext();
                //dbProvider.CreateBuilderSqlOper<UserVO>()
            });
        }
    }
}
