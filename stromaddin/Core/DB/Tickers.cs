//using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace stromaddin.Core.DB
{
    public class Tickers
    {
        static private bool inited = false;
        //static private object lockObj = new object();
        static private readonly Dictionary<string, string> aliasMap = new Dictionary<string, string>();
        static private readonly HashSet<string> symbolSet = new HashSet<string>();
        static private List<TickerSymbol> _symbolList = new List<TickerSymbol>(); 
        static public string FindSymbol(string symbol)
        {
            lock (aliasMap)
            {
                if (!inited)
                {
                    InitByDB();
                }
            }
            if (aliasMap.TryGetValue(symbol, out var realsymbol))
                return realsymbol;

            if (symbolSet.Contains(symbol))
                return symbol;

            return "";
        }
        static public List<TickerSymbol> Symbols {
            get {
                lock (aliasMap)
                {
                    if (!inited)
                    {
                        InitByDB();
                    }
                }
                return _symbolList;
            }
        }

        static void InitByDB()
        {
            string xllPath = ExcelDna.Integration.ExcelDnaUtil.XllPath;
            string cfgPath = xllPath.Substring(0, xllPath.LastIndexOf('\\')) + "\\Resources\\cfg.db";

            using (var _connection = new SQLiteConnection("Data Source="+ cfgPath))
            {
                _connection.Open();
                _symbolList = _connection.Query<TickerSymbol>("SELECT Symbol, Alias FROM symbols;").AsList();
                foreach (var item in _symbolList)
                {
                    if (item.Alias.Length > 0)
                    {
                        aliasMap[item.Alias] = item.Symbol;
                    }
                    symbolSet.Add(item.Symbol);
                }
            }
        }
    }

    public class TickerSymbol
    {
        public string Symbol { get; set; }
        public string Alias { get; set; }
    }
}
