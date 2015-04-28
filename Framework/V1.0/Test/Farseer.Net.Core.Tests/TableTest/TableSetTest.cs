using System;
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
        public void ToInfoTestMethod()
        {
            var lst = Table.Instance.User.Select(o => o.ID).Where(o => o.ID > 0).Asc(o => o.ID).ToList();

            var info = Table.Instance.User.Select(o => o.ID).Select(o => o.LoginCount).Where(o => o.ID > 1).ToInfo();
            Assert.IsNotNull(info);
            Assert.IsTrue(info.ID > 1);
            Assert.IsTrue(info.PassWord == null && info.GenderType == null && info.LoginIP == null && info.UserName == null && info.ID != null && info.LoginCount != null);
            Assert.IsTrue(info.ID == lst.Find(o => o.ID > 1).ID);


            info = Table.Instance.User.Select(o => new { o.ID, o.PassWord }).ToInfo();
            Assert.IsNotNull(info);
            Assert.IsTrue(info.PassWord != null && info.GenderType == null && info.LoginIP == null && info.UserName == null && info.ID != null && info.LoginCount == null);
            Assert.IsTrue(info.ID == lst[0].ID);



            Expression<Func<UserVO, object>> select = o => new { o.ID, o.PassWord };
            info = Table.Instance.User.Select(select).ToInfo();
            Assert.IsNotNull(info);
            Assert.IsTrue(info.PassWord != null && info.GenderType == null && info.LoginIP == null && info.UserName == null && info.ID != null && info.LoginCount == null);
            Assert.IsTrue(info.ID == lst[0].ID);



            info = Table.Instance.User.Select(select).ToInfo();
            Assert.IsNotNull(info);
            Assert.IsTrue(info.PassWord != null && info.GenderType == null && info.LoginIP == null && info.UserName == null && info.ID != null && info.LoginCount == null);
            Assert.IsTrue(info.ID == lst[0].ID);
        }

        [TestMethod]
        public void ToListTestMethod()
        {
            var lst = Table.Instance.User.Desc(o => o.ID).ToList(10, true, true);
            lst = Table.Instance.User.ToList(0, true, true);
            lst = Table.Instance.User.ToList();
            Assert.IsTrue(lst != null && lst.Count > 0);
            var ID = lst[0].ID.GetValueOrDefault();

            lst = Table.Instance.User.Select(o => new { o.ID, o.PassWord, o.GetDate }).Where(o => o.ID == ID).Desc(o => new { o.LoginCount, o.GenderType }).Asc(o => o.ID).Desc(o => o.GetDate).ToList();
            var info = lst[0];
            Assert.IsNotNull(lst);
            Assert.IsTrue(lst.Count == 1);
            Assert.IsTrue(info.PassWord != null && info.GenderType == null && info.LoginIP == null && info.UserName == null && info.ID != null && info.LoginCount == null);
            Assert.IsTrue(info.ID == ID);


            lst = Table.Instance.User.ToList(3);
            Assert.IsNotNull(lst);
            Assert.IsTrue(lst.Count <= 3);

            lst = Table.Instance.User.ToList(3, 2);
            Assert.IsNotNull(lst);
            Assert.IsTrue(lst.Count <= 3);

            var count = Table.Instance.User.Where(o => o.ID > 10).Count();
            var recordCount = 0;
            lst = Table.Instance.User.Where(o => o.ID > 10).ToList(99999, 1, out  recordCount).ToList();
            Assert.IsNotNull(lst);
            Assert.IsTrue(count == recordCount);
        }

        [TestMethod]
        public void InsertTestMethod()
        {
            var count = Table.Instance.User.Count();
            var currentCount = 0;
            UserVO info;
            using (var context = new Table())
            {
                context.User.Insert(new UserVO() { UserName = "xx" });
                context.SaveChanges();

                info = context.User.Desc(o => o.ID).ToInfo();
                Assert.IsTrue(info.UserName == "xx");

                currentCount = context.User.Count();
                Assert.IsTrue(currentCount == count + 1);
            }

            Table.Instance.User.Insert(new UserVO() { UserName = "yy" });


            info = Table.Instance.User.Desc(o => o.ID).ToInfo();
            Assert.IsTrue(info.UserName == "yy");

            currentCount = Table.Instance.User.Count();
            Assert.IsTrue(currentCount == count + 2);


        }

        [TestMethod]
        public void UpdateTestMethod()
        {
            var ID = 0;
            using (var context = new Table())
            {
                ID = context.User.Desc(o => o.ID).ToInfo().ID.GetValueOrDefault();

                context.User.Where(o => o.ID == ID).Update(new UserVO() { UserName = "zz" });
                context.SaveChanges();
                Assert.IsTrue(context.User.Desc(o => o.ID).ToInfo().UserName == "zz");
            }

            Table.Instance.User.Where(o => o.ID == ID).Update(new UserVO() { UserName = "bb" });
            Assert.IsTrue(Table.Instance.User.Desc(o => o.ID).ToInfo().UserName == "bb");

        }

        [TestMethod]
        public void AddUpTestMethod()
        {
            UserVO info;
            using (var context = new Table())
            {
                info = context.User.Desc(o => o.ID).ToInfo();

                context.User.Where(o => o.ID == info.ID).Append(o => new { o.LoginCount }, 4).AddUp();
                context.SaveChanges();
                var info2 = context.User.Desc(o => o.ID).ToInfo();
                Assert.IsTrue(info2.LoginCount == info.LoginCount + 4);
            }

            Table.Instance.User.Where(o => o.ID == info.ID).Update(new UserVO() { UserName = "bb" });
            Assert.IsTrue(Table.Instance.User.Desc(o => o.ID).ToInfo().UserName == "bb");

        }
    }
}