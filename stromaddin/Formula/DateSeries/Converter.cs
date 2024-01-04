using stromaddin.Core.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DictTable = System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<object>>;

namespace stromddin.Formula.DateSeries
{
    internal class Converter
    {
        TableOutput ConvertDictTable(DictTable dt, bool header)
        {
            int cols = dt.First().Value.Count;
            TableOutput tbl = new TableOutput(dt.Count, cols, header ? 1 : 0);
            if (header)
            {
                int i = 0;
                foreach (var kvp in dt)
                {
                    tbl.SetHeader(0, i, kvp.Key);
                    i++;
                }
            }
            {
                int i = 0;
                foreach (var kvp in dt)
                {
                    for (int j = 0; j < kvp.Value.Count; j++)
                    {
                        tbl.SetData(i, j, kvp.Value[j]);
                    }
                    i++;
                }
            }
            return tbl;
        }
        TableOutput ConvertByFirstDim(Dictionary<string, DictTable> dt)
        {
            TableOutput tbl = new TableOutput(dt.Count, dt.First().Value.Count, 0);
            int i = 0;
            foreach (var kvp in dt)
            {
                for (int j = 0; j < kvp.Value.Count; j++)
                {
                    //tbl.SetData(i, j, kvp.Value[j]);
                }
                i++;
            }
            return tbl;
        }
    }
}
