using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace VegetPriceDAL
{
    public class SqlServerHelper
    {
        public static ulong BulkToDB(string constring, DataTable dt, string tarTble, out string sErr)
        {
            sErr = string.Empty;
            try
            {
                ulong nItem = 0;
                //声明SqlBulkCopy ,using释放非托管资源
                using (var sqlBC = new SqlBulkCopy(constring))
                {
                    //一次批量的插入的数据量
                    sqlBC.BatchSize = 3000;
                    //超时则事务回滚
                    sqlBC.BulkCopyTimeout = 180;
                    //设置要批量写入的表
                    sqlBC.DestinationTableName = tarTble;
                    sqlBC.SqlRowsCopied += new SqlRowsCopiedEventHandler((obj, args) =>
                    {
                        nItem = (ulong)args.RowsCopied;
                    });
                    sqlBC.NotifyAfter = dt.Rows.Count;

                    //自定义的OleDbDataReader和数据库的字段进行对应
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        sqlBC.ColumnMappings.Add(dt.Columns[i].ColumnName, dt.Columns[i].ColumnName);
                    }

                    //批量写入
                    sqlBC.WriteToServer(dt);
                    return nItem;
                }
            }
            catch (System.Exception ex)
            {
                sErr = ex.Message;
                return 0;
            }
        }
    }
}
