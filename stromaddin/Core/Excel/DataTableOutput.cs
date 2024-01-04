using System.Collections.Generic;
using System.Data;
using System.Linq;

using DictTable = System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<object>>;

namespace stromaddin.Core.Excel
{
    internal class DataTableOutput : TableOutput
    {
        public DataTableOutput(int rows, int cols, int headerRows = 1) : base(rows, cols, headerRows)
        {
        }
    }
}
