using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Demo.PO.Table.Members;
using FS.Core.Context;

namespace Farseer.Net.Core.Tests.Context
{
    [TestClass]
    public class TableContextTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            using (var context = new TableContext<UserPO>())
            {
                var info = new UserPO();
                //var info = context.TableSet.Where(o => o.ID > 0).Desc(o => new { o.ID, o.LoginCount }).Asc(o => o.GenderType).ToInfo();
                info.PassWord = "77777";
                context.TableSet.Where(o => o.ID == 1).Update(info);


                info.LoginIP = "bbbbb";
                context.TableSet.Where(o => o.ID == 1).Update(info);

                info.ID = null;
                info.PassWord = "00000New";
                context.TableSet.Insert(info);


                //context.TableSet.Where(o => o.ID == 1).Append(o => o.LoginCount, 1).Append(o => o.LoginCount, 1).AddUp();

                //var lst = context.TableSet.Where(o => o.ID > 0).Desc(o => new { o.ID, o.LoginCount }).Asc(o => o.GenderType).ToList();

                //context.SaveChanges();
            }
        }
    }
}