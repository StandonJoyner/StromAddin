using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExcelDna.Integration;
using Microsoft.Office.Interop.Excel;

namespace stromaddin.Core.Excel
{
    class TableOutput
    {
        object[,] _tblHeader;
        object[,] _tblData;
        string[]  _tblFmt;
        
        public TableOutput(int rows, int cols, int headerRows = 1)
        {
            _tblFmt = new string[cols];
            if (headerRows > 0)
            {
                _tblHeader = new object[headerRows, cols];
            }
            else
            {
                _tblHeader = null;
            }
            _tblData = new object[rows, cols];
        }
        public object[,] Data
        {
            get
            {
                return _tblData;
            }
        }
        public void SetData(int row, int col, object val)
        {
            _tblData[row, col] = val;
        }
        public object[,] Header
        {
            get
            {
                return _tblHeader;
            }
        }
        public void SetHeader(int row, int col, object val)
        {
            _tblHeader[row, col] = val;
        }
        public void SetFormat(int col, string fmt)
        {
            _tblFmt[col] = fmt;
        }

        public void Output(Range xlref)
        {
            if (xlref == null)
            {
                _Application app = (_Application)ExcelDnaUtil.Application;
                if (app.Workbooks.Count == 0)
                    app.Workbooks.Add();
                xlref = app.ActiveCell;
            }
            if (_tblHeader != null)
            {
                OutputHeader(xlref);
                int hrows = _tblHeader.GetLength(0);
                xlref = xlref.Offset[hrows, 0];
            }
            int rows = _tblData.GetLength(0);
            int cols = _tblData.GetLength(1);
            ExcelAsyncUtil.QueueAsMacro(() =>
            {
                var sheet = (Worksheet)xlref.Worksheet;
                var range = sheet.Range[xlref, xlref.Offset[rows - 1, cols - 1]];
                range.Value2 = _tblData;
                for (int i = 0; i < cols; ++i)
                {
                    if (_tblFmt[i] != null)
                    {
                        try
                        {
                            Range col = (Range)range.Columns[i + 1];
                            col.NumberFormat = _tblFmt[i];
                        }
                        catch (System.Exception)
                        {
                        }
                    }
                }
            });
        }
        public void OutputHeader(Range xlref)
        {
            if (_tblHeader == null)
                return;
            int rows = _tblHeader.GetLength(0);
            int cols = _tblHeader.GetLength(1);
            ExcelAsyncUtil.QueueAsMacro(() =>
            {
                var sheet = (Worksheet)xlref.Worksheet;
                var range = sheet.Range[xlref, xlref.Offset[rows - 1, cols - 1]];
                range.Value2 = _tblHeader;
            });
        }
    }
}
