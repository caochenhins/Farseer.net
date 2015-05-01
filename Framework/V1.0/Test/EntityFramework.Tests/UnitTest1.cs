using System.Data.Entity;
using Demo.VO.Members;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EntityFramework.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            using (var po = new POTest())
            {
                var type = po.User.GetType();
            }
        }
    }

    public class POTest : DbContext
    {
        public DbSet<UserVO> User { get; set; }
    }
}
