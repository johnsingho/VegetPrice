using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VegetPriceBLL;

namespace VegetablPriceCollect
{
    class Program
    {
        private static readonly string MARKET_URL = @"http://www.jnmarket.net/import/list-1.html";
        static void Main(string[] args)
        {
            var collector = new VegetPriceCollector(MARKET_URL);
            var dtBegin = DateTime.Now.AddDays(-2);
            var dtEnd = DateTime.Now;
            collector.GetByDateRange(dtBegin, dtEnd);

        }
    }
}
