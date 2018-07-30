using Common;
using System.Data;

namespace VegetPriceDAL
{
    public class VegetPriceDB
    {
        private static readonly string CONN_STR = "Data Source=dmnnte801;Initial Catalog=FavLink_Test;User ID=admin;Password=dmn@1a2b3c4d;";

        public static ulong SaveToDB(DataTable dt)
        {
            var sErr = string.Empty;
            var sTab = "tbl_VegetPrice";

            return SqlServerHelper.BulkToDB(CONN_STR, dt, sTab, out sErr);
        }

        public static byte[] GetExcelBytes(DataTable dt)
        {
            var bys = EPPExcelHelper.BuilderExcel(dt);
            return bys;
        }
    }
}
