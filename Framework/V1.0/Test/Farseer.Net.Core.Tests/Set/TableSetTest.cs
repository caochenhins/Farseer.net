using System;
using System.Linq;
using System.Linq.Expressions;
using Demo.VO.Members;
using FS.Core.Data.Table;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Farseer.Net.Core.Tests.Set
{
    [TestClass]
    public class TableSetTest
    {
        [TestMethod]
        public void ToInfoTestMethod()
        {
            var lst = TableContext<UserVO>.Data.Select(o => o.ID).Where(o => o.ID > 0).Asc(o => o.ID).ToList();

            var info = TableContext<UserVO>.Data.Select(o => o.ID).Select(o => o.LoginCount).Where(o => o.ID > 1).ToInfo();
            Assert.IsNotNull(info);
            Assert.IsTrue(info.ID > 1);
            Assert.IsTrue(info.PassWord == null && info.GenderType == null && info.LoginIP == null && info.UserName == null && info.ID != null && info.LoginCount != null);
            Assert.IsTrue(info.ID == lst.Find(o => o.ID > 1).ID);


            info = TableContext<UserVO>.Data.Select(o => new { o.ID, o.PassWord }).ToInfo();
            Assert.IsNotNull(info);
            Assert.IsTrue(info.PassWord != null && info.GenderType == null && info.LoginIP == null && info.UserName == null && info.ID != null && info.LoginCount == null);
            Assert.IsTrue(info.ID == lst[0].ID);



            Expression<Func<UserVO, object>> select = o => new { o.ID, o.PassWord };
            info = TableContext<UserVO>.Data.Select(select).ToInfo();
            Assert.IsNotNull(info);
            Assert.IsTrue(info.PassWord != null && info.GenderType == null && info.LoginIP == null && info.UserName == null && info.ID != null && info.LoginCount == null);
            Assert.IsTrue(info.ID == lst[0].ID);



            info = TableContext<UserVO>.Data.Select(select).ToInfo();
            Assert.IsNotNull(info);
            Assert.IsTrue(info.PassWord != null && info.GenderType == null && info.LoginIP == null && info.UserName == null && info.ID != null && info.LoginCount == null);
            Assert.IsTrue(info.ID == lst[0].ID);
        }

        [TestMethod]
        public void ToListTestMethod()
        {
            var lst = TableContext<UserVO>.Data.Desc(o => o.ID).ToList(10, true, true);
            lst = TableContext<UserVO>.Data.ToList(0, true, true);
            lst = TableContext<UserVO>.Data.ToList();
            Assert.IsTrue(lst != null && lst.Count > 0);
            var ID = lst[0].ID.GetValueOrDefault();

            lst = TableContext<UserVO>.Data.Select(o => new { o.ID, o.PassWord, o.GetDate }).Where(o => o.ID == ID).Desc(o => new { o.LoginCount, o.GenderType }).Asc(o => o.ID).Desc(o => o.GetDate).ToList();
            var info = lst[0];
            Assert.IsNotNull(lst);
            Assert.IsTrue(lst.Count == 1);
            Assert.IsTrue(info.PassWord != null && info.GenderType == null && info.LoginIP == null && info.UserName == null && info.ID != null && info.LoginCount == null);
            Assert.IsTrue(info.ID == ID);


            lst = TableContext<UserVO>.Data.ToList(3);
            Assert.IsNotNull(lst);
            Assert.IsTrue(lst.Count <= 3);

            lst = TableContext<UserVO>.Data.ToList(3, 2);
            Assert.IsNotNull(lst);
            Assert.IsTrue(lst.Count <= 3);

            var count = TableContext<UserVO>.Data.Where(o => o.ID > 10).Count();
            var recordCount = 0;
            lst = TableContext<UserVO>.Data.Where(o => o.ID > 10).ToList(99999, 1, out  recordCount).ToList();
            Assert.IsNotNull(lst);
            Assert.IsTrue(count == recordCount);
        }

        [TestMethod]
        public void InsertTestMethod()
        {
            var count = TableContext<UserVO>.Data.Count();
            var currentCount = 0;
            UserVO info;
            using (var table = new TableContext<UserVO>())
            {
                table.TableSet.Insert(new UserVO() { UserName = "xx" });
                table.SaveChanges();

                info = table.TableSet.Desc(o => o.ID).ToInfo();
                Assert.IsTrue(info.UserName == "xx");

                currentCount = table.TableSet.Count();
                Assert.IsTrue(currentCount == count + 1);
            }

            TableContext<UserVO>.Data.Insert(new UserVO() { UserName = "yy" });


            info = TableContext<UserVO>.Data.Desc(o => o.ID).ToInfo();
            Assert.IsTrue(info.UserName == "yy");

            currentCount = TableContext<UserVO>.Data.Count();
            Assert.IsTrue(currentCount == count + 2);


        }

        [TestMethod]
        public void UpdateTestMethod()
        {
            var ID = 0;
            using (var table = new TableContext<UserVO>())
            {
                ID = table.TableSet.Desc(o => o.ID).ToInfo().ID.GetValueOrDefault();

                table.TableSet.Where(o => o.ID == ID).Update(new UserVO() { UserName = "zz" });
                table.SaveChanges();
                Assert.IsTrue(table.TableSet.Desc(o => o.ID).ToInfo().UserName == "zz");
            }

            TableContext<UserVO>.Data.Where(o => o.ID == ID).Update(new UserVO() { UserName = "bb" });
            Assert.IsTrue(TableContext<UserVO>.Data.Desc(o => o.ID).ToInfo().UserName == "bb");

        }

        [TestMethod]
        public void AddUpTestMethod()
        {
            UserVO info;
            using (var table = new TableContext<UserVO>())
            {
                info = table.TableSet.Desc(o => o.ID).ToInfo();

                table.TableSet.Where(o => o.ID == info.ID).Append(o => new { o.LoginCount }, 4).AddUp();
                table.SaveChanges();
                var info2 = table.TableSet.Desc(o => o.ID).ToInfo();
                Assert.IsTrue(info2.LoginCount == info.LoginCount + 4);
            }

            TableContext<UserVO>.Data.Where(o => o.ID == info.ID).Update(new UserVO() { UserName = "bb" });
            Assert.IsTrue(TableContext<UserVO>.Data.Desc(o => o.ID).ToInfo().UserName == "bb");

        }
    }
}