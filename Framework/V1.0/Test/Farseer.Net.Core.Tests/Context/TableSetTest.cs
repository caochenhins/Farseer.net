using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Demo.PO.Table.Members;
using FS.Core.Client.SqlServer;
using FS.Core.Client.SqlServer.Visit;
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
            Expression<Func<UserPO, object>> select = o => new { o.ID, o.PassWord };
            Expression<Func<UserPO, object>> select2 = o => new { o.LoginCount, o.LoginIP };


            //var lstIDs = new List<int> { 1, 2, 3, 4, 5, 6 };
            //TableContext<UserPO>.Data.Select(o => o.ID).Select(o => o.PassWord).Where(o => o.ID > 0 && (o.UserName.Contains("aa") || lstIDs.Contains(o.ID.GetValueOrDefault())) && o.ID != 1 && o.PassWord.Length >= 2 && !lstIDs.Contains(o.ID.GetValueOrDefault())).Asc(o => new { o.ID, o.PassWord }).Asc(o => o.LoginCount).Desc(o => o.LoginIP).ToList();
            //return;
            var lst = TableContext<UserPO>.Data.Select(o => o.ID).Where(o => o.ID > 0).Asc(o => o.ID).ToList();

            var info = TableContext<UserPO>.Data.Select(o => o.ID).Select(o => o.LoginCount).Where(o => o.ID > 1).ToInfo();
            Assert.IsNotNull(info);
            Assert.IsTrue(info.ID > 1);
            Assert.IsTrue(info.PassWord == null && info.GenderType == null && info.LoginIP == null && info.UserName == null && info.ID != null && info.LoginCount != null);
            Assert.IsTrue(info.ID == lst.Find(o => o.ID > 1).ID);


            info = TableContext<UserPO>.Data.Select(o => new { o.ID, o.PassWord }).ToInfo();
            Assert.IsNotNull(info);
            Assert.IsTrue(info.PassWord != null && info.GenderType == null && info.LoginIP == null && info.UserName == null && info.ID != null && info.LoginCount == null);
            Assert.IsTrue(info.ID == lst[0].ID);



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


            lst = TableContext<UserPO>.Data.Select(o => new { o.ID, o.PassWord, o.GetDate }).Where(o => o.ID == ID).Desc(o => new { o.LoginCount, o.GenderType }).Asc(o => o.ID).Desc(o => o.GetDate).ToList();
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
            var currentCount = 0;
            UserPO info;
            using (var table = new TableContext<UserPO>())
            {
                table.TableSet.Insert(new UserPO() { UserName = "xx" });
                table.SaveChanges();

                info = table.TableSet.Desc(o => o.ID).ToInfo();
                Assert.IsTrue(info.UserName == "xx");

                currentCount = table.TableSet.Count();
                Assert.IsTrue(currentCount == count + 1);
            }

            TableContext<UserPO>.Data.Insert(new UserPO() { UserName = "yy" });


            info = UserPO.Data.Desc(o => o.ID).ToInfo();
            Assert.IsTrue(info.UserName == "yy");

            currentCount = UserPO.Data.Count();
            Assert.IsTrue(currentCount == count + 2);


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
