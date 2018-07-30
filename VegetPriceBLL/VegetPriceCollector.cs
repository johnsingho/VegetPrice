using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace VegetPriceBLL
{
    public class VegetPriceCollector
    {
        private string mMarketUrl = string.Empty;
        public VegetPriceCollector(string sMarketUrl)
        {
            mMarketUrl = sMarketUrl;
        }
        
        public string GetNthPage(int i)
        {
            var pos = mMarketUrl.LastIndexOf(".htm", StringComparison.InvariantCultureIgnoreCase);
            if (pos < 0) { return string.Empty; }
            var sLast = mMarketUrl.Substring(pos);
            var sBegin = mMarketUrl.Substring(0, pos);
            var sRet = string.Format("{0}_{1}{2}", sBegin, i, sLast);
            return sRet;
        }

        public static string COL_NAME = "Name";
        public static string COL_PRICE = "YuanPerKG"; //"均价（元/公斤）";
        public static string COL_DATE = "PubDate";

        private DataTable MakeDatatable()
        {
            var dt = new DataTable();
            dt.Columns.Add(COL_NAME, typeof(string));
            dt.Columns.Add(COL_PRICE, typeof(double));
            dt.Columns.Add(COL_DATE, typeof(DateTime));
            return dt;
        }

        public DataTable GetByDateRange(DateTime dtBegin, DateTime dtEnd)
        {
            var web = new HtmlWeb();
            web.OverrideEncoding = Encoding.GetEncoding("GBK");
            web.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win32; x86) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/60.0 Safari/537.36";

            var sUrl = mMarketUrl;
            var dt = MakeDatatable();
            var i = 1;
            do
            {
                sUrl = GetNthPage(i++);
                var doc = web.Load(sUrl);
                var tRes = Decode(doc, dtBegin, dtEnd, ref dt);
                if( !tRes.Item1 &&
                    tRes.Item2 < dtBegin)
                {
                    break;
                }
            } while(true);
            return dt;
        }

        private Tuple<bool, DateTime> Decode(HtmlDocument doc, DateTime dtBegin, DateTime dtEnd, ref DataTable dt)
        {
            var ret = new Tuple<bool, DateTime>(false, DateTime.Now);

            var tabNode = doc.DocumentNode.SelectSingleNode("//table[@class='price-table']");
            if (null == tabNode) { return ret; }
            var trNodes = tabNode.SelectNodes("tbody/tr");
            var bInRange = true;
            DateTime dtLast = default(DateTime);

            foreach (var tr in trNodes)
            {
                var tdNodes = tr.SelectNodes("td");
                var sName = tdNodes[0].InnerText;
                var sPrice = tdNodes[2].InnerText;
                var sDate = tdNodes[4].InnerText;

                var dDay = DateTime.Now;
                if ( !DateTime.TryParseExact(sDate, "yy-MM-dd", System.Globalization.CultureInfo.CurrentCulture,
                        System.Globalization.DateTimeStyles.None, out dDay)
                   )
                {
                    continue;
                }
                double dbPrice = 0.0F;
                if (!double.TryParse(sPrice, out dbPrice))
                {
                    continue;
                }

                if (dDay<dtBegin || dDay > dtEnd)
                {
                    bInRange = false;
                    break; //!
                }

                var dr = dt.NewRow();
                dr[COL_NAME] = sName;
                dr[COL_PRICE] = dbPrice;
                dr[COL_DATE] = dDay;
                dt.Rows.Add(dr);
            }
            return new Tuple<bool, DateTime>(bInRange, dtLast);
        }
    }
}
