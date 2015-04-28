using System;
using Demo.PO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Farseer.Net.Core.Tests.TableTest
{
    [TestClass]
    public class TableContext1Test
    {
        [TestMethod]
        public void NewTableContextTestMethod()
        {
            using (new Table()) { }
        }

        [TestMethod]
        public void NewAndSaveChangeTableContextTestMethod()
        {
            using (var context = new Table())
            {
                var info = context.User.Where(o => o.ID > 0 && o.CreateAt < DateTime.Now).Desc(o => new { o.ID, o.LoginCount }).Asc(o => o.GenderType).ToInfo();
                info.PassWord = "77777";
                context.User.Where(o => o.ID == 1).Update(info);


                info.LoginIP = "bbbbb";
                context.User.Where(o => o.ID == 1).Update(info);

                info.ID = null;
                info.PassWord = "00000New";
                context.User.Insert(info);


                context.User.Where(o => o.ID == 1).Append(o => o.LoginCount, 1).AddUp();

                var lst = context.User.Where(o => o.ID > 0).Desc(o => new { o.ID, o.LoginCount }).Asc(o => o.GenderType).ToList();

                context.SaveChanges();
            }
        }
    }
}