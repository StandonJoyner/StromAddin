using stromaddin.Config;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Excel;
using ExcelDna.Integration;

namespace stromaddin.Core.Excel
{
    class RtdOutput
    {
        private IEnumerable<RtdIndicator> _indis;
        private string _coin;
        public RtdOutput(string coin, IEnumerable<RtdIndicator> indis)
        {
            _indis = indis;
            _coin = coin;
        }

        public void Output()
        {
            TableOutput table = new TableOutput(1, _indis.Count() + 2, 1);
            // header
            table.SetHeader(0, 0, "Name");
            for (int i = 0; i < _indis.Count(); i++)
            {
                table.SetHeader(0, i + 1, _indis.ElementAt(i).Name);
            }
            table.SetHeader(0, _indis.Count() + 1, "Source");

            // data
            _Application app = (_Application)ExcelDnaUtil.Application;
            if (app.Workbooks.Count == 0)
                app.Workbooks.Add();

            Range cell = app.ActiveCell;
            var nameAddr = RefCell(cell, 1, 0);
            var exAddr = RefCell(cell, 1, _indis.Count() + 1);
            table.SetData(0, 0, _coin);
            for (int i = 0; i < _indis.Count(); i++)
            {
                var elem = _indis.ElementAt(i);
                var fmu = MakeFormula(nameAddr, exAddr, elem);
                table.SetData(0, i + 1, fmu);
                table.SetFormat(i + 1, NumberFormat.GetFormat(elem.Type));
            }
            table.SetData(0, _indis.Count() + 1, "Binance");
            table.Output(cell);
        }

        private string MakeFormula(string nameAddr, string exAddr, RtdIndicator ind)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("=CSRTD(");
            sb.Append(nameAddr);
            sb.Append(",\"");
            sb.Append(ind.Key);
            sb.Append("\",\"");

            StringBuilder psb = new StringBuilder();
            foreach (var param in ind.Params)
            {
                psb.Append(param.Key);
                psb.Append("=");
                psb.Append(param.Value);
                psb.Append(",");
            }
            var pstr = psb.ToString();
            if (pstr.Length > 0)
                pstr = pstr.Substring(0, pstr.Length - 1);
            sb.Append(pstr);
            sb.Append("\",");
            sb.Append(exAddr);
            sb.Append(")");
            return sb.ToString();
        }
        private string RefCell(Range rng, int row, int col)
        {
            return rng.Offset[row, col].Address[false, false];
        }
    }
}
