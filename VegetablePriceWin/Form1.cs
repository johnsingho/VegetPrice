using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using VegetPriceBLL;
using VegetPriceDAL;

namespace VegetablePriceWin
{
    public partial class MainFrm : Form
    {
        private static readonly string MARKET_URL = @"http://www.jnmarket.net/import/list-1.html";
        private VegetPriceCollector mCollector = null;
        private DataTable mData = null;
        

        public MainFrm()
        {
            InitializeComponent();
            mCollector = new VegetPriceCollector(MARKET_URL);
        }

        private void btnGet_Click(object sender, EventArgs e)
        {
            var dtBegin = dtpBegin.Value;
            var dtEnd = dtpEnd.Value;
            if(dtBegin > dtEnd)
            {
                var dtTemp = dtBegin;
                dtBegin = dtEnd;
                dtEnd = dtTemp;
            }
            this.Cursor = Cursors.WaitCursor;
            var dt = mCollector.GetByDateRange(dtBegin, dtEnd);
            this.Cursor = Cursors.Default;

            RefreshView(dt);
        }

        private void RefreshView(DataTable dt)
        {
            var bHas = (dt != null);
            if (bHas)
            {
                var qry = dt.AsEnumerable();
                var vegets = qry
                            .Select(r => new { name = r[VegetPriceCollector.COL_NAME] })
                            .Distinct();
                var dates = qry
                            .Select(r => new { date = r[VegetPriceCollector.COL_DATE].ToString() })
                            .Distinct()
                            .OrderBy(x => x.date)
                            .Select(x => new DataColumn(x.date));

                var dtTrans = new DataTable();
                dtTrans.Columns.Add("名称");
                dtTrans.Columns.AddRange(dates.ToArray());
                foreach(var veg in vegets)
                {
                    var r = dtTrans.NewRow();
                    r["名称"] = veg.name;
                    var filterVeg = qry.Where(x => veg.name.Equals(x[VegetPriceCollector.COL_NAME]))
                                       .ToDictionary(
                                            x => x[VegetPriceCollector.COL_DATE].ToString(),
                                            x => x[VegetPriceCollector.COL_PRICE].ToString()
                                       );
                    foreach(var col in dates)
                    {
                        var colName = col.ColumnName;
                        if (filterVeg.ContainsKey(colName))
                        {
                            r[colName] = filterVeg[colName];
                        }
                        else
                        {
                            r[colName] = "";
                        }
                    }
                    dtTrans.Rows.Add(r);
                }
                mData = dtTrans;

                //mData.Columns[VegetPriceCollector.COL_NAME].ColumnName = "名称";
                //mData.Columns[VegetPriceCollector.COL_PRICE].ColumnName = "均价（元/公斤）";
                //mData.Columns[VegetPriceCollector.COL_DATE].ColumnName = "发布日期";                
            }
            else
            {
                mData = null;
            }
            dgView.DataSource = mData;
            btnExport.Enabled = bHas;
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDlg = new SaveFileDialog();

            saveFileDlg.Filter = "Excel files (*.xlsx)|*.xlsx";
            saveFileDlg.FilterIndex = 1;
            saveFileDlg.RestoreDirectory = true;

            if (saveFileDlg.ShowDialog() == DialogResult.OK)
            {
                var bys = VegetPriceDB.GetExcelBytes(mData);
                if (null != bys && bys.Length > 0)
                {
                    Stream myStream;
                    if ((myStream = saveFileDlg.OpenFile()) != null)
                    {
                        myStream.Write(bys, 0, bys.Length);
                        myStream.Close();
                        var str = string.Format("成功导出到: {0}", saveFileDlg.FileName);
                        MessageBox.Show(str, "导出", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    MessageBox.Show("导出失败", "导出", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


    }
}
