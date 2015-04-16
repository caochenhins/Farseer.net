using System;
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

        [TestMethod]
        public void NewAndSaveChangeTableContextTestMethod()
        {
            using (var context = new UserPO())
            {
                var info = context.Set.Where(o => o.ID > 0 && o.CreateAt < DateTime.Now).Desc(o => new { o.ID, o.LoginCount }).Asc(o => o.GenderType).ToInfo();
                info.PassWord = "77777";
                context.Set.Where(o => o.ID == 1).Update(info);


                info.LoginIP = "bbbbb";
                context.Set.Where(o => o.ID == 1).Update(info);

                info.ID = null;
                info.PassWord = "00000New";
                context.Set.Insert(info);


                context.Set.Where(o => o.ID == 1).Append(o => o.LoginCount, 1).AddUp();

                var lst = context.Set.Where(o => o.ID > 0).Desc(o => new { o.ID, o.LoginCount }).Asc(o => o.GenderType).ToList();

                context.SaveChanges();
            }
        }
    }
}