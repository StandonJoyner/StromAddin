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
using stromaddin.Core.Enums;
using stromaddin.Exchanges.Binance;

namespace stromaddin.Core
{
    public class SymbolsSet
    {
        static private bool inited = false;
        //static private object lockObj = new object();
        static private SortedDictionary<string, TickerSymbol> _symbols = new SortedDictionary<string, TickerSymbol>();
        static public string FindSymbol(string symbol)
        {
            lock (_symbols)
            {
                if (!inited)
                {
                    InitByDB();
                }
            }
            if (_symbols.ContainsKey(symbol))
                return symbol;

            return "";
        }
        static public List<TickerSymbol> Symbols
        {
            get
            {
                lock (_symbols)
                {
                    if (!inited)
                    {
                        InitByDB();
                    }
                    return _symbols.Values.ToList();
                }
            }
        }

        static void InitByDB()
        {
            var syms = ExchangeInfo.GetAllSymbols(MarketType.SPOT, true);
            foreach (var s in syms)
            {
                _symbols.Add(s.Name, new TickerSymbol { Symbol = s.Name, Alias = s.Name });
            }
            inited = true;
        }
    }

    public class TickerSymbol
    {
        public string Symbol { get; set; }
        public string Alias { get; set; }
    }
}
