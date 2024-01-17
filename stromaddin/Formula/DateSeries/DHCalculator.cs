using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ListTable = System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<string, System.Collections.Generic.List<object>>>;
using stromaddin.Core.Excel;
using ExcelDna.Integration;
using CryptoExchange.Net.CommonObjects;
using Newtonsoft.Json.Linq;
using System.Globalization;

namespace stromaddin.Formula.DateSeries
{
    internal class DHCalculator
    {
        private readonly string _symbols;
        private readonly DateTime _begDate;
        private readonly DateTime _endDate;
        private readonly string _indis;
        private readonly string _ext;
        private readonly string _source;

        TableOutput _data;
        string _err;
        public DHCalculator(object symbols, object begDate, object endDate,
            object indis, string ext, string source)
        {
            _symbols = ConvertObjectToString(symbols);
            _indis = ConvertObjectToString(indis);
            _begDate = ConvertObjectToDateTime(begDate);
            _endDate = ConvertObjectToDateTime(endDate);
            if (_endDate == DateTime.MinValue)
                _endDate = DateTime.Today;
            _ext = ext;
            _source = source;
        }
        public string Key { get => $"=CSDS([{_symbols}],{_begDate},{_endDate},[{_indis}],{_ext},{_source})"; }

        public async Task<Status> Calculate()
        {
            if (_symbols.Length == 0)
            {
                _err = "Symbols is empty";
                return Status.Failed;
            }
            if (_indis.Length == 0)
            {
                _err = "Indicators is empty";
                return Status.Failed;
            }
            if (_begDate == DateTime.MinValue)
            {
                _err = "Begin date is empty";
                return Status.Failed;
            }
            if (_begDate > _endDate)
            {
                _err = "Begin date is greater than end date";
                return Status.Failed;
            }
            string begDate = _begDate.ToString("yyyy-MM-dd");
            string endDate = _endDate.ToString("yyyy-MM-dd");
            string ext = "";
            string url = $"https://localhost:7083/v1/BN/Klines/dateseries?" +
                $"symbols={_symbols}&indis={_indis}&tbeg={begDate}&tend={endDate}&ext={ext}";
            
            try
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();

                var result = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseBody);
                if (result.ContainsKey("data"))
                {
                    var data = JsonConvert.DeserializeObject<List<KeyValuePair<string, ListTable>>>(result["data"].ToString());
                    _data = Converter.ConvertByFirstDim(data);
                }
            }
            catch (Exception ex)
            {
                _err = ex.Message;
                return Status.Failed;
            }
            return Status.Completed;
        }
        public TableOutput ResultData {
            get => _data;
        }
        public string ErrMessage {
            get => _err;
        }

        private string ConvertObjectToString(object obj)
        {
            if (obj is ExcelEmpty)
            {
                return "";
            }
            else if (obj is ExcelMissing)
            {
                return "";
            }
            else if (obj is ExcelReference)
            {
                var objRef = obj as ExcelReference;
                var objArray = objRef.GetValue() as object[,];
                return ArrayToString(objArray);
            }
            else if (obj is object[,])
            {
                var objArray = obj as object[,];
                return ArrayToString(objArray);
            }
            else
            {
                return obj.ToString();
            }
        }
        private string ArrayToString(object[,] array)
        {
            string strobj = "";
            if (array != null)
            {
                int rows = array.GetLength(0);
                int cols = array.GetLength(1);
                if (rows * cols > 1000)
                {
                    return "";
                }
                for (int rw = 0; rw < rows; ++ rw)
                {
                    for (int i = 0; i < cols; i++)
                    {
                        var symbol = array[rw, i];
                        strobj += $"{symbol},";
                    }
                    strobj = strobj.TrimEnd(',');
                }
            }
            return strobj;
        }
        private DateTime ConvertObjectToDateTime(object obj)
        {
            if (obj is DateTime)
            {
                return (DateTime)obj;
            }
            else if (obj is string)
            {
                string[] formats = { "dd/MM/yyyy", "yyyy/MM/dd", "dd-MM-yyyy", "yyyy-MM-dd" };
                string dateString = obj as string;

                DateTime dateValue;
                if (DateTime.TryParseExact(dateString, formats,
                                           CultureInfo.InvariantCulture,
                                           DateTimeStyles.None,
                                           out dateValue))
                {
                    return dateValue;
                }
                else
                {
                    return DateTime.MinValue;
                }
            }
            else
            {
                return DateTime.MinValue;
            }
        }
    }

}
