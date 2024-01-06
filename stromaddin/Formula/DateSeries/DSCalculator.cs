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

namespace stromddin.Formula.DateSeries
{
    internal class DSCalculator
    {
        private readonly string _symbols;
        private readonly string _begDate;
        private readonly string _endDate;
        private readonly string _indis;
        private readonly string _ext;
        private readonly string _source;

        TableOutput _data;
        string _err;
        public DSCalculator(string symbols, string begDate, string endDate,
            string indis, string ext, string source)
        {
            _symbols = symbols;
            _begDate = begDate;
            _endDate = endDate;
            _indis = indis;
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
            string tbeg;
            string tend;
            try
            {
                tend = _endDate;
                if (_endDate.Length == 0)
                {
                    tend = DateTime.Now.ToString("yyyy-MM-dd");
                }
                var dtBeg = DateTime.Parse(_begDate);
                var dtEnd = DateTime.Parse(_endDate);
                if (dtEnd < dtBeg)
                {
                    _err = "End date must be greater than begin date";
                    return Status.Failed;
                }
                tbeg = dtBeg.ToString("yyyy-MM-dd");
                tend = dtEnd.ToString("yyyy-MM-dd");
            }
            catch (Exception ex)
            {
                _err = ex.Message;
                return Status.Failed;
            }
            string ext = "";

            string url = $"https://localhost:7083/v1/BN/Klines/dateseries?" +
                $"symbols={_symbols}&indis={_indis}&tbeg={tbeg}&tend={tend}&ext={ext}";

            var client = new HttpClient();
            client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
            HttpResponseMessage response = await client.GetAsync(url);

            try
            {
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();

                var result = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseBody);
                if (result.ContainsKey("data"))
                {
                    var data = JsonConvert.DeserializeObject<List<KeyValuePair<string, ListTable>>>(result["data"].ToString());
                    _data = Converter.ConvertListTable(data[0].Value, true);
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
    }

}
