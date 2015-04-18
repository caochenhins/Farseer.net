using System;
using System.Data.Common;
using Demo.PO;
using Demo.VO.Members;
using FS.Configs;
using FS.Core;
using FS.Core.Client.SqlServer;
using FS.Core.Data;
using FS.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Farseer.Net.Core.Tests.Table
{
    [TestClass]
    public class TableContextTest
    {

        [TestMethod]
        public void NewAndSaveChangeTableContextTestMethod()
        {
            using (var context = new Demo.PO.Table())
            {
                context.User.Where(o => o.ID > 0).ToList();
            }
            
            new Demo.PO.Table().User.Where(o => o.ID > 0).ToList();

            Demo.PO.Table.Instance.User.Where(o => o.ID > 0).ToList();
        }
    }
}
