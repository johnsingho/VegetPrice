using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VegetPriceBLL;
using VegetPriceDAL;

namespace TestBLL
{
    [TestClass]
    public class UnitTest1
    {
        private static readonly string MARKET_URL = @"http://www.jnmarket.net/import/list-1.html";

        [TestMethod]
        public void TestMethod1()
        {
            var collector = new VegetPriceCollector(MARKET_URL);
            var s10 = collector.GetNthPage(10);
            var exp10 = @"http://www.jnmarket.net/import/list-1_10.html";

            Assert.AreEqual(exp10, s10);
        }

        [TestMethod]
        public void TestMethod2()
        {
            var collector = new VegetPriceCollector(MARKET_URL);
            var s2 = collector.GetNthPage(2);
            var exp2= @"http://www.jnmarket.net/import/list-1_2.html";

            Assert.AreEqual(exp2, s2);
        }

        [TestMethod]
        public void TestGetByDateRange1()
        {
            var collector = new VegetPriceCollector(MARKET_URL);
            var dtBegin = new DateTime(2018, 7, 23);
            var dtEnd = new DateTime(2018, 7, 28);
            var dt = collector.GetByDateRange(dtBegin, dtEnd);

            Assert.IsNotNull(dt);
            var nGet = dt.Rows.Count;
            Assert.IsTrue(nGet > 0);
            //Assert.AreEqual(exp2, s2);

            var nWrite = VegetPriceDB.SaveToDB(dt);
            Assert.IsTrue(nGet == (int)nWrite);
        }


    }
}
