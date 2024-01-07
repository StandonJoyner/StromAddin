using ExcelDna.Integration;
using stromddin.Formula.DateSeries;
using System;
using System.Data;
using System.Data.SQLite;
using System.Threading.Tasks;
using System.Windows;

namespace stromaddin.Formula
{
    public static class Functions
    {
        [ExcelFunction(Description = "My first .NET function")]
        public static string SayHello(string name)
        {
            return "Hello " + name;
        }

        [ExcelFunction(Category = "CoinStrom", Description = "Provides real-time market data (powered by CoinStrom)")]
        public static object CSRTD(string symbol, string indi, string parm, string source)
        {
            symbol = symbol.Trim().ToUpper();
            symbol = stromddin.Core.SymbolsSet.FindSymbol(symbol);
            if (symbol.Length == 0)
                return ExcelErrorUtil.ToComError(ExcelError.ExcelErrorValue);
            string[] prams = {
                symbol,
                indi.Trim().ToLower(),
                parm.Trim(),
                source.Trim().ToLower()
            };
            return XlCall.RTD(RTD.RTDServer.ProgId, null, prams);
        }

        [ExcelFunction(Category = "CoinStrom", Description = "Provides data history (powered by CoinStrom)")]
        public static object CSDH(string symbols, string begDate, string endDate,
            string indis, string ext, string source)
        {
            ExcelReference caller = XlCall.Excel(XlCall.xlfCaller) as ExcelReference;
            string fmu = XlCall.Excel(XlCall.xlfFormulatext, caller) as string;
            if (fmu == null)
                return ExcelErrorUtil.ToComError(ExcelError.ExcelErrorValue);
            DSCalculator calc = new DSCalculator(symbols, begDate, endDate, indis, ext, source);
            return ExcelAsyncUtil.Observe("CSDH", new object[] { caller, fmu },
                ()=> new DSObservable(caller, fmu, calc));
        }
        //static SQLiteConnection _connection;
        //static SQLiteCommand _productNameCommand;

        //private static void EnsureConnection()
        //{
        //    if (_connection == null)
        //    {
        //        _connection = new SQLiteConnection(@"Data Source=C:\Temp\Northwind.db");
        //        _connection.Open();

        //        _productNameCommand = new SQLiteCommand("SELECT ProductName FROM Products WHERE ProductID = @ProductID", _connection);
        //        _productNameCommand.Parameters.Add("@ProductID", DbType.Int32);
        //    }
        //}

    }
}
