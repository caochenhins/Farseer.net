﻿using System;
using System.Linq;
using System.Linq.Expressions;
using Demo.PO;
using Demo.VO.Members;
using FS.Core.Data.View;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Farseer.Net.Core.Tests.View
{
    [TestClass]
    public class ViewSetTest
    {
        [TestMethod]
        public void ToInfoTestMethod()
        {
            var lst = ViewContext<AccountVO>.Data.Select(o => o.ID).Where(o => o.ID > 0).Asc(o => o.ID).ToList();

            var info = ViewContext<AccountVO>.Data.Select(o => o.ID).Select(o => o.Name).Where(o => o.ID > 1).ToInfo();
            Assert.IsNotNull(info);
            Assert.IsTrue(info.ID > 1);
            Assert.IsTrue(info.Pwd == null && info.Name != null && info.ID != null);
            Assert.IsTrue(info.ID == lst.Find(o => o.ID > 1).ID);


            info = ViewContext<AccountVO>.Data.Select(o => new { o.ID, o.Pwd }).ToInfo();
            Assert.IsNotNull(info);
            Assert.IsTrue(info.Pwd != null &&  info.Name == null && info.ID != null);
            Assert.IsTrue(info.ID == lst[0].ID);



            Expression<Func<AccountVO, object>> select = o => new { o.ID, o.Pwd };
            info = ViewContext<AccountVO>.Data.Select(select).ToInfo();
            Assert.IsNotNull(info);
            Assert.IsTrue(info.Pwd != null &&  info.Name == null && info.ID != null);
            Assert.IsTrue(info.ID == lst[0].ID);



            info = ViewContext<AccountVO>.Data.Select(select).ToInfo();
            Assert.IsNotNull(info);
            Assert.IsTrue(info.Pwd != null &&  info.Name == null && info.ID != null);
            Assert.IsTrue(info.ID == lst[0].ID);
        }

        [TestMethod]
        public void ToListTestMethod()
        {
            var lst = ViewContext<AccountVO>.Data.Desc(o => o.ID).ToList(10, true, true);
            lst = ViewContext<AccountVO>.Data.ToList(0, true, true);
            lst = ViewContext<AccountVO>.Data.ToList();
            Assert.IsTrue(lst != null && lst.Count > 0);
            var ID = lst[0].ID.GetValueOrDefault();

            lst = ViewContext<AccountVO>.Data.Select(o => new { o.ID, o.Pwd, o.GetDate }).Where(o => o.ID == ID).Desc(o => new { o.Name }).Asc(o => o.ID).Desc(o => o.GetDate).ToList();
            var info = lst[0];
            Assert.IsNotNull(lst);
            Assert.IsTrue(lst.Count == 1);
            Assert.IsTrue(info.Pwd != null &&  info.Name == null && info.ID != null);
            Assert.IsTrue(info.ID == ID);


            lst = ViewContext<AccountVO>.Data.ToList(3);
            Assert.IsNotNull(lst);
            Assert.IsTrue(lst.Count <= 3);

            lst = ViewContext<AccountVO>.Data.ToList(3, 2);
            Assert.IsNotNull(lst);
            Assert.IsTrue(lst.Count <= 3);

            var count = ViewContext<AccountVO>.Data.Where(o => o.ID > 10).Count();
            var recordCount = 0;
            lst = ViewContext<AccountVO>.Data.Where(o => o.ID > 10).ToList(99999, 1, out  recordCount).ToList();
            Assert.IsNotNull(lst);
            Assert.IsTrue(count == recordCount);
        }
    }
}