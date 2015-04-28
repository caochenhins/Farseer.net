using Demo.PO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Farseer.Net.Core.Tests.TableTest
{
    [TestClass]
    public class TableContextTest
    {

        [TestMethod]
        public void NewAndSaveChangeTableContextTestMethod()
        {
            using (var context = new Table())
            {
                context.User.Update(new Demo.VO.Members.UserVO {PassWord = "xxx"});
                context.User.AddUp(o => o.LoginCount, 1);
                context.SaveChanges();


                context.User.Where(o => o.ID > 0).ToList();
            }
            
            new Table().User.Where(o => o.ID > 0).ToList();

            Table.Data.User.AddUp(o => o.LoginCount, 1);
            Table.Data.User.Where(o => o.ID > 0).ToList();

            Table.Data.User.Where(o => o.ID > 0).ToList();
        }
    }
}
