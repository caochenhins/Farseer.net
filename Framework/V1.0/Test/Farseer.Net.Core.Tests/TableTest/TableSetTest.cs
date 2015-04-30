using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Demo.PO;
using Demo.VO.Members;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Farseer.Net.Core.Tests.TableTest
{
    [TestClass]
    public class TableSetTest
    {
        [TestMethod]
        public void ToList()
        {
            using (var context = new Table())
            {
                Assert.IsTrue(context.User.Desc(o => o.ID).ToList(10, true, true).Count <= 10);
                context.User.ToList(0, true, true);
                context.User.ToList(0, true);
                context.User.ToList(0, false, true);

                var ID = context.User.Select(o => new { o.ID }).ToList(1)[0].ID.GetValueOrDefault();

                var lst = context.User.Select(o => new { o.ID, o.PassWord, o.GetDate }).Where(o => o.ID == ID).Desc(o => new { o.LoginCount, o.GenderType }).Asc(o => o.ID).Desc(o => o.GetDate).ToList();
                Assert.IsTrue(lst.Count == 1);
                Assert.IsTrue(lst[0].PassWord != null && lst[0].GenderType == null && lst[0].LoginIP == null && lst[0].UserName == null && lst[0].ID != null && lst[0].LoginCount == null && lst[0].ID == ID);

                Assert.IsTrue(context.User.ToList(3, 2).Count <= 3);

                var count = context.User.Where(o => o.ID > 10).Count();
                var recordCount = 0;
                Assert.IsTrue(context.User.Where(o => o.ID > 10).ToList(99999, 1, out recordCount).ToList().Count == recordCount);
                Assert.IsTrue(context.User.ToList(new List<int> { 1, 2, 3 }).Count <= 3);
                var lstIDs = new List<int> {1, 2, 3};
                Assert.IsTrue(context.User.ToList(lstIDs).Count <= 3);
            }
        }

        [TestMethod]
        public void ToSelectList() { }

        [TestMethod]
        public void ToEntity()
        {
            var lst = Table.Data.User.Select(o => o.ID).Where(o => o.ID > 0).Asc(o => o.ID).ToList();

            var info = Table.Data.User.Select(o => o.ID).Select(o => o.LoginCount).Where(o => o.ID > 1).ToEntity();
            Assert.IsNotNull(info);
            Assert.IsTrue(info.ID > 1);
            Assert.IsTrue(info.PassWord == null && info.GenderType == null && info.LoginIP == null && info.UserName == null && info.ID != null && info.LoginCount != null);
            Assert.IsTrue(info.ID == lst.Find(o => o.ID > 1).ID);


            info = Table.Data.User.Select(o => new { o.ID, o.PassWord }).ToEntity();
            Assert.IsNotNull(info);
            Assert.IsTrue(info.PassWord != null && info.GenderType == null && info.LoginIP == null && info.UserName == null && info.ID != null && info.LoginCount == null);
            Assert.IsTrue(info.ID == lst[0].ID);



            Expression<Func<UserVO, object>> select = o => new { o.ID, o.PassWord };
            info = Table.Data.User.Select(select).ToEntity();
            Assert.IsNotNull(info);
            Assert.IsTrue(info.PassWord != null && info.GenderType == null && info.LoginIP == null && info.UserName == null && info.ID != null && info.LoginCount == null);
            Assert.IsTrue(info.ID == lst[0].ID);



            info = Table.Data.User.Select(select).ToEntity();
            Assert.IsNotNull(info);
            Assert.IsTrue(info.PassWord != null && info.GenderType == null && info.LoginIP == null && info.UserName == null && info.ID != null && info.LoginCount == null);
            Assert.IsTrue(info.ID == lst[0].ID);
        }

        [TestMethod]
        public void Count() { }

        [TestMethod]
        public void Copy() { }

        [TestMethod]
        public void IsHaving() { }

        [TestMethod]
        public void Update()
        {
            var ID = 0;
            using (var context = new Table())
            {
                ID = context.User.Desc(o => o.ID).ToEntity().ID.GetValueOrDefault();

                context.User.Where(o => o.ID == ID).Update(new UserVO() { UserName = "zz" });
                context.SaveChanges();
                Assert.IsTrue(context.User.Desc(o => o.ID).ToEntity().UserName == "zz");
            }

            Table.Data.User.Where(o => o.ID == ID).Update(new UserVO() { UserName = "bb" });
            Assert.IsTrue(Table.Data.User.Desc(o => o.ID).ToEntity().UserName == "bb");

            Table.Data.User.Update(new UserVO() { UserName = "bb", ID = ID });
        }

        [TestMethod]
        public void Insert()
        {
            var count = Table.Data.User.Count();
            var currentCount = 0;
            UserVO info;
            using (var context = new Table())
            {
                context.User.Insert(new UserVO() { UserName = "xx" });
                context.SaveChanges();

                info = context.User.Desc(o => o.ID).ToEntity();
                Assert.IsTrue(info.UserName == "xx");

                currentCount = context.User.Count();
                Assert.IsTrue(currentCount == count + 1);
            }

            Table.Data.User.Insert(new UserVO() { UserName = "yy" });


            info = Table.Data.User.Desc(o => o.ID).ToEntity();
            Assert.IsTrue(info.UserName == "yy");

            currentCount = Table.Data.User.Count();
            Assert.IsTrue(currentCount == count + 2);


        }

        [TestMethod]
        public void Delete() { }

        [TestMethod]
        public void AddUp()
        {
            UserVO info;
            using (var context = new Table())
            {
                info = context.User.Desc(o => o.ID).ToEntity();

                context.User.Where(o => o.ID == info.ID).Append(o => new { o.LoginCount }, 4).AddUp();
                context.SaveChanges();
                var info2 = context.User.Desc(o => o.ID).ToEntity();
                Assert.IsTrue(info2.LoginCount == info.LoginCount + 4);
            }

            Table.Data.User.Where(o => o.ID == info.ID).Update(new UserVO() { UserName = "bb" });
            Assert.IsTrue(Table.Data.User.Desc(o => o.ID).ToEntity().UserName == "bb");

        }

        [TestMethod]
        public void Statistics() { }
    }
}