using System;
using System.Linq.Expressions;
using Demo.PO.Table.Members;
using FS.Core.Context;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Farseer.Net.Core.Tests.Context
{
    [TestClass]
    public class TableSetTest
    {
        [TestMethod]
        public void ToInfoTestMethod()
        {
            var lst = TableContext<UserPO>.Data.ToList();

            var info = TableContext<UserPO>.Data.Select(o => o.ID).Select(o => o.LoginCount).Where(o => o.ID > 1).ToInfo();
            Assert.IsNotNull(info);
            Assert.IsTrue(info.ID > 1);
            Assert.IsTrue(info.PassWord == null && info.GenderType == null && info.LoginIP == null && info.UserName == null && info.ID != null && info.LoginCount != null);
            Assert.IsTrue(info.ID == lst.Find(o => o.ID > 1).ID);


            info = TableContext<UserPO>.Data.Select(o => new { o.ID, o.PassWord }).ToInfo();
            Assert.IsNotNull(info);
            Assert.IsTrue(info.PassWord != null && info.GenderType == null && info.LoginIP == null && info.UserName == null && info.ID != null && info.LoginCount == null);
            Assert.IsTrue(info.ID == lst[0].ID);



            Expression<Func<UserPO, object>> select = o => new { o.ID, o.PassWord };
            info = TableContext<UserPO>.Data.Select(select).ToInfo();
            Assert.IsNotNull(info);
            Assert.IsTrue(info.PassWord != null && info.GenderType == null && info.LoginIP == null && info.UserName == null && info.ID != null && info.LoginCount == null);
            Assert.IsTrue(info.ID == lst[0].ID);



            info = UserPO.Data.Select(select).ToInfo();
            Assert.IsNotNull(info);
            Assert.IsTrue(info.PassWord != null && info.GenderType == null && info.LoginIP == null && info.UserName == null && info.ID != null && info.LoginCount == null);
            Assert.IsTrue(info.ID == lst[0].ID);
        }

        [TestMethod]
        public void ToListTestMethod()
        {
            var lst = TableContext<UserPO>.Data.ToList();
            Assert.IsTrue(lst != null && lst.Count > 0);
            var ID = lst[0].ID.GetValueOrDefault();


            lst = TableContext<UserPO>.Data.Select(o => new { o.ID, o.PassWord }).Where(o => o.ID == ID).Desc(o => new { o.LoginCount, o.GenderType }).Asc(o => o.ID).ToList();
            var info = lst[0];
            Assert.IsNotNull(lst);
            Assert.IsTrue(lst.Count == 1);
            Assert.IsTrue(info.PassWord != null && info.GenderType == null && info.LoginIP == null && info.UserName == null && info.ID != null && info.LoginCount == null);
            Assert.IsTrue(info.ID == ID);
        }

        [TestMethod]
        public void InsertTestMethod()
        {
            var count = UserPO.Data.Count();
            using (var table = new TableContext<UserPO>())
            {
                table.TableSet.Insert(new UserPO() { UserName = "xx" });
                table.SaveChanges();
                Assert.IsTrue(table.TableSet.Desc(o => o.ID).ToInfo().UserName == "xx");
            }

            TableContext<UserPO>.Data.Insert(new UserPO() { UserName = "yy" });
            Assert.IsTrue(UserPO.Data.Desc(o => o.ID).ToInfo().UserName == "yy");

            Assert.IsTrue(UserPO.Data.Count() == count + 2);


        }

        [TestMethod]
        public void UpdateTestMethod()
        {
            var ID = 0;
            using (var table = new TableContext<UserPO>())
            {
                ID = table.TableSet.Desc(o => o.ID).ToInfo().ID.GetValueOrDefault();

                table.TableSet.Where(o => o.ID == ID).Update(new UserPO() { UserName = "zz" });
                table.SaveChanges();
                Assert.IsTrue(table.TableSet.Desc(o => o.ID).ToInfo().UserName == "zz");
            }

            TableContext<UserPO>.Data.Where(o => o.ID == ID).Update(new UserPO() { UserName = "bb" });
            Assert.IsTrue(UserPO.Data.Desc(o => o.ID).ToInfo().UserName == "bb");

        }
    }
}
