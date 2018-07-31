using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VegetPriceBLL.comm
{
    public class DateTimeHelper
    {
        public static string ConvertToDateStr(object obj)
        {
            var dat = (DateTime)obj;
            if (obj == null)
            {
                return string.Empty;
            }
            return dat.ToString("yyyy-MM-dd");
        }
    }
}
