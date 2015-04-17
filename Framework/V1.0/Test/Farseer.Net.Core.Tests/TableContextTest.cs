using FS.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Farseer.Net.Core.Tests
{
    [TestClass]
    public class TimeTest
    {
        [TestMethod]
        public void TestTime()
        {
            SpeedTest.Initialize();
            SpeedTest.ConsoleTime("context", 100000, () =>
            {
                var context = new Demo.PO.Table();
            });
        }
    }
}
