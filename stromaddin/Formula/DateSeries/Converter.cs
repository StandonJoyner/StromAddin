using stromaddin.Core.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ListTable = System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<string, System.Collections.Generic.List<object>>>;

namespace stromaddin.Formula.DateSeries
{
    internal class Converter
    {
        public static TableOutput ConvertListTable(ListTable dt, bool header)
        {
            int rows = dt.First().Value.Count;
            TableOutput tbl = new TableOutput(rows, dt.Count, header ? 1 : 0);
            if (header)
            {
                int i = 0;
                foreach (var col in dt)
                {
                    tbl.SetHeader(0, i, col.Key);
                    i++;
                }
            }
            {
                int i = 0;
                foreach (var col in dt)
                {
                    for (int row = 0; row < col.Value.Count; row++)
                    {
                        tbl.SetData(row, i, col.Value[row]);
                    }
                    i++;
                }
            }
            return tbl;
        }

        private static int GetMaxCols(List<KeyValuePair<string, ListTable>> dtt)
        {
            int cols = 0;
            foreach (var kvp in dtt)
            {
                var dt = kvp.Value;
                if (dt.Count > 0)
                {
                    int tableCols = dt.Count;
                    cols = dtt.Count * (tableCols - 1) + 1;
                    break;
                }
            }
            return cols;
        }
        private static List<DateTime> MergeEachFirstColumn(List<KeyValuePair<string, ListTable>> dtt)
        {
            var opentimes = new SortedSet<DateTime>();
            foreach (var kvp in dtt)
            {
                var dt = kvp.Value;
                if (dt.Count > 0)
                {
                    var tmList = dt.First().Value;
                    foreach (var tm in tmList)
                    {
                        if (tm is DateTime dateTime)
                        {
                            opentimes.Add(dateTime);
                        }
                    }
                }
            }
            return opentimes.ToList();
        }
        public static TableOutput ConvertByFirstDim(List<KeyValuePair<string, ListTable>> dtt)
        {
            int cols = GetMaxCols(dtt);
            var openTimes = MergeEachFirstColumn(dtt);

            TableOutput tbl = new TableOutput(openTimes.Count, cols, 2);
            #region set_header
            tbl.SetHeader(1, 0, "date");
            tbl.SetFormat(0, NumberFormat.GetDateFormat());
            int tbCol = 0;
            for (int tbi = 0; tbi < dtt.Count; ++tbi)
            {
                var kvp = dtt[tbi];
                var dt = kvp.Value;
                for (int col = 1; col < dt.Count; ++col)
                {
                    tbl.SetHeader(0, tbCol + col, kvp.Key);
                    tbl.SetHeader(1, tbCol + col, dt[col].Key);
                }
                tbCol += dt.Count - 1;
            }
            #endregion
            for (int row = 0; row < openTimes.Count; ++row)
            {
                tbl.SetData(row, 0, openTimes[row]);
            }
            tbCol = 0;
            for (int tbi = 0; tbi < dtt.Count; ++tbi)
            {
                var kvp = dtt[tbi];
                var dt = kvp.Value;
                int valRow = 0;
                for (int row = 0; row < openTimes.Count; ++row)
                {
                    var tm = openTimes[row];
                    var firstCol = dt.First().Value;
                    if (firstCol.Count > valRow
                        && firstCol[valRow] is DateTime asDateTime
                        && asDateTime == tm)
                    {
                        for (int col = 1; col < dt.Count; ++col)
                        {
                            tbl.SetData(row, tbCol + col, dt[col].Value[valRow]);
                        }
                        ++valRow;
                    }
                    else
                    {
                        for (int col = 1; col < dt.Count; ++col)
                        {
                            tbl.SetData(row, tbCol + col, null);
                        }
                    }
                }
                tbCol += dt.Count - 1;
            }
            return tbl;
        }

        public static TableOutput ConvertBySecondDim(List<KeyValuePair<string, ListTable>> dtt)
        {
            int cols = GetMaxCols(dtt);
            var openTimes = MergeEachFirstColumn(dtt);

            TableOutput tbl = new TableOutput(openTimes.Count, cols, 2);
            #region set_header
            tbl.SetHeader(1, 0, "date");
            tbl.SetFormat(0, NumberFormat.GetDateFormat());
            for (int tbi = 0; tbi < dtt.Count; ++tbi)
            {
                var kvp = dtt[tbi];
                var dt = dtt[tbi].Value;
                for (int col = 1; col < dt.Count; ++col)
                {
                    int calcCol = (col - 1) * dtt.Count + 1 + tbi;
                    tbl.SetHeader(0, calcCol, dt[col].Key);
                    tbl.SetHeader(1, calcCol, kvp.Key);
                }
            }
            #endregion
            // set open time(first column)
            for (int row = 0; row < openTimes.Count; ++row)
            {
                tbl.SetData(row, 0, openTimes[row]);
            }

            for (int tbi = 0; tbi < dtt.Count; ++tbi)
            {
                var kvp = dtt[tbi];
                var dt = kvp.Value;
                int valRow = 0;
                for (int row = 0; row < openTimes.Count; ++row)
                {
                    var tm = openTimes[row];
                    var firstCol = dt.First().Value;
                    if (firstCol.Count > valRow
                        && firstCol[valRow] is DateTime asDateTime
                        && asDateTime == tm)
                    {
                        for (int col = 1; col < dt.Count; ++col)
                        {
                            tbl.SetData(row, (col - 1) * dtt.Count + 1 + tbi, dt[col].Value[valRow]);
                        }
                        ++valRow;
                    }
                    else
                    {
                        for (int col = 1; col < dt.Count; ++col)
                        {
                            tbl.SetData(row, (col - 1) * dtt.Count + 1 + tbi, null);
                        }
                    }
                }
            }
            return tbl;
        }
    }
}
