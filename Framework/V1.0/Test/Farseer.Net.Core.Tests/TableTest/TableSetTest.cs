using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Demo.Common;
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
                // 取前十条 随机 非重复的数据
                Assert.IsTrue(context.User.Desc(o => o.ID).ToList(10, true, true).Count <= 10);
                // 取 随机 非重复的数据
                context.User.ToList(0, true, true);
                // 取 随机 的数据
                context.User.ToList(0, true);
                // 取 非重复 的数据
                context.User.ToList(0, false, true);
                // 只取ID
                var ID = context.User.Select(o => new { o.ID }).ToList(1)[0].ID.GetValueOrDefault();
                // 筛选字段、条件、正序、倒序
                var lst = context.User.Select(o => new { o.ID, o.PassWord, o.GetDate }).Where(o => o.ID == ID).Desc(o => new { o.LoginCount, o.GenderType }).Asc(o => o.ID).Desc(o => o.GetDate).ToList();
                Assert.IsTrue(lst.Count == 1);
                Assert.IsTrue(lst[0].PassWord != null && lst[0].GenderType == null && lst[0].LoginIP == null && lst[0].UserName == null && lst[0].ID != null && lst[0].LoginCount == null && lst[0].ID == ID);
                // 取第2页的数据（每页显示3条数据）
                Assert.IsTrue(context.User.ToList(3, 2).Count <= 3);

                var recordCount = 0;
                // 取前99999条数据，并返回总数据
                Assert.IsTrue(context.User.Where(o => o.ID > 10).ToList(99999, 1, out recordCount).ToList().Count == recordCount);
                // 取ID为：1、2、3 的数据
                Assert.IsTrue(context.User.ToList(new List<int> { 1, 2, 3 }).Count <= 3);
                var lstIDs = new List<int> { 1, 2, 3 };
                Assert.IsTrue(context.User.ToList(lstIDs).Count <= 3);

                // 来一个复杂条件的数据
                context.User.Select(o => new { o.ID, o.PassWord, o.GetDate })
                    .Where(
                        o =>
                            o.ID == ID ||
                            o.LoginCount < 1 ||
                            o.CreateAt < DateTime.Now ||
                            o.CreateAt > DateTime.Now.AddDays(-365) ||
                            o.UserName.Contains("x") ||
                            o.UserName.StartsWith("x") ||
                            o.UserName.EndsWith("x") ||
                            o.UserName.Length > 0 ||
                            o.GenderType == eumGenderType.Man || 
                            !o.PassWord.Contains("x"))
                    .Desc(o => new { o.LoginCount, o.GenderType })
                    .Asc(o => o.ID)
                    .Desc(o => o.GetDate)
                    .ToList();
            }
        }

        [TestMethod]
        public void ToSelectList()
        {
            using (var context = new Table())
            {

            }
        }

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