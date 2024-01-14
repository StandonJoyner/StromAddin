using ExcelDna.Integration;
using Microsoft.Office.Interop.Excel;
using stromaddin.Config;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stromaddin.Core.Excel
{
    internal class DataHistoryOutput
    {
        IEnumerable<TickerSymbol> _symbols;
        IEnumerable<XMLIndicator> _indis;
        string _startDate;
        string _endDate;
        public DataHistoryOutput(IEnumerable<TickerSymbol> symbols, 
            IEnumerable<XMLIndicator> indis, 
            string startDate, string endDate)
        {
            _symbols = symbols;
            _indis = indis;
            _startDate = startDate;
            _endDate = endDate;
        }

        public void Output()
        {
            _Application app = (_Application)ExcelDnaUtil.Application;
            if (app.Workbooks.Count == 0)
                app.Workbooks.Add();

            int cols = System.Math.Max(_symbols.Count(), _indis.Count()) + 1;
            TableOutput table = new TableOutput(2, cols, 0);
            // formula
            table.SetData(1, 0, MakeFormula(app.ActiveCell));
            // data
            for (int c = 1; c < cols; ++c)
            {
                int refcol = c - 1;
                if (refcol < _symbols.Count())
                    table.SetData(0, c, MakeSymbol(_symbols.ElementAt(refcol)));
                if (refcol < _indis.Count())
                    table.SetData(1, c, MakeIndicator(_indis.ElementAt(refcol)));
            }
            table.Output(null);
        }

        public string MakeIndicator(XMLIndicator indi)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(indi.Key);
            if (indi.Params.Count > 0)
            {
                sb.Append("(");
                for (int i = 0; i < indi.Params.Count; i++)
                {
                    var param = indi.Params[i];
                    if (i > 0)
                        sb.Append(",");
                    sb.Append($"{param.Name}={param.Value}");
                }
                sb.Append(")");
            }
            return sb.ToString();
        }

        public string MakeSymbol(TickerSymbol symbol)
        {
            return symbol.Symbol;
        }

        public string MakeFormula(Range activeCell)
        {
            Range targetCell = activeCell.Offset[1, 0];
            StringBuilder sb = new StringBuilder();
            sb.Append("=CSDH(");
            sb.Append(RefRange(targetCell, -1, 1, -1, _symbols.Count()));
            sb.Append(",\"");
            sb.Append(_startDate);
            sb.Append("\",\"");
            sb.Append(_endDate);
            sb.Append("\",");
            sb.Append(RefRange(targetCell, 0,  1, 0, _indis.Count()));
            sb.Append(")");
            return sb.ToString();
        }

        private string RefRange(Range rng, int row1, int col1, int row2, int col2)
        {
            var startCell = rng.Offset[row1, col1];
            var endCell = rng.Offset[row2, col2];
            return startCell.Address[false, false] + ":" + endCell.Address[false, false];
        }
    }
}
