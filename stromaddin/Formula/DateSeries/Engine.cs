using CryptoExchange.Net.CommonObjects;
using ExcelDna.Integration;
using Microsoft.Office.Interop.Excel;
using Microsoft.Xaml.Behaviors.Media;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

using DictTable = System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<object>>;

namespace stromddin.Formula.DateSeries
{
    internal enum Status
    {
        None,
        Running,
        Completed,
        Timeout,
        Failed,
        Faulted
    }

    internal class Calculator
    {
        string _symbols, _begDate, _endDate, _indis, _ext, _source;
        Dictionary<string, DictTable> _data;
        public Calculator(string symbols, string begDate, string endDate,
            string indis, string ext, string source) 
        { 
            _symbols = symbols;
            _begDate = begDate;
            _endDate = endDate;
            _indis = indis;
            _ext = ext;
            _source = source;
        }
        public string Key { get=> $"=CSDS([{_symbols}],{_begDate},{_endDate},[{_indis}],{_ext},{_source})"; }

        public async Task<Status> Calculate()
        {
            string symbols = "BTCUSDT";
            string indis = "open,close";
            string tbeg = DateTime.Now.AddDays(-10).ToString("yyyy-MM-dd");
            string tend = DateTime.Now.ToString("yyyy-MM-dd");
            string ext = "ext";

            string url = $"https://localhost:7083/v1/BN/Klines/dateseries?symbols={symbols}&indis={indis}&tbeg={tbeg}&tend={tend}&ext={ext}";

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
                    _data = JsonConvert.DeserializeObject<Dictionary<string, DictTable>>(result["data"].ToString());
                }
            }
            catch (Exception ex)
            {
                return Status.Failed;
            }
            return Status.Completed;
        }
        public Dictionary<string, DictTable> ResultData { get => _data; }
    }

    internal class DateSeriesObservable : IExcelObservable
    {
        Calculator _calc;
        string _fmu;
        ExcelReference _caller;
        public DateSeriesObservable(ExcelReference caller, string fmu, Calculator calc)
        {
            _caller = caller;
            _fmu = fmu;
            _calc = calc;
        }
        public IDisposable Subscribe(IExcelObserver observer)
        {
            var target = new ResultTarget(_caller, _fmu, observer);
            Engine.Instance.Calculate(_calc, target);
            return DummyDisposable.Instance;
        }
    }

    class DummyDisposable : IDisposable
    {
        public static readonly DummyDisposable Instance = new DummyDisposable();
        private DummyDisposable()
        {
        }
        public void Dispose()
        {
        }
    }

    internal struct ResultTarget
    {
        public IExcelObserver Observer;
        public ExcelReference Caller;
        public string Fmu;
        public ResultTarget(ExcelReference caller, string fmu, IExcelObserver observer)
        {
            Caller = caller;
            Fmu = fmu;
            Observer = observer;
        }
    }

    internal class ResultContext
    {
        Status   _status = Status.Running;
        DateTime _complateTm;
        Dictionary<ExcelReference, ResultTarget> _targets = new Dictionary<ExcelReference, ResultTarget>();
        Dictionary<string, DictTable> _data;

        public ResultContext()
        {
        }
        public Status GetStatus()
        {
            if (ExcelDnaUtil.MainManagedThreadId != Thread.CurrentThread.ManagedThreadId)
                throw new InvalidOperationException("GetStatus must be called from a main thread.");
            if (_status == Status.Completed)
            {
                var diff = DateTime.Now - _complateTm;
                if (diff > TimeSpan.FromMinutes(5))
                    _status = Status.Timeout;
            }
            return _status;
        }
        public void SetResult(Dictionary<string, DictTable> data)
        {
            if (ExcelDnaUtil.MainManagedThreadId != Thread.CurrentThread.ManagedThreadId)
                throw new InvalidOperationException("SetResult must be called from a main thread.");
            _status = Status.Completed;
            _complateTm = DateTime.Now;
            _data = data;
            FillAll();
        }
        public void FillAll()
        {
            ExcelAsyncUtil.QueueAsMacro(() =>
            {
                lock (_targets)
                {
                    foreach (var target in _targets.Values)
                    {
                        target.Observer.OnNext(_data.Count);
                    }
                }
            });
        }

        private void FillOne(ExcelReference caller)
        {
            ExcelAsyncUtil.QueueAsMacro(() =>
            {
                lock (_targets)
                {
                    _targets.TryGetValue(caller, out ResultTarget target);
                    target.Observer.OnNext(_data.Count);
                }
            });
        }

        public void AddTarget(ExcelReference caller, ResultTarget target)
        {
            lock (_targets)
            {
                _targets.Add(caller, target);
                if (GetStatus() == Status.Completed)
                    FillOne(caller);
            }
        }
    }

    internal class Engine
    {
        Dictionary<string, ResultContext> _results = new Dictionary<string, ResultContext>();
        public static Engine Instance = new Engine();
        private Engine()
        {
        }

        public void Calculate(Calculator calc, ResultTarget target)
        {
            var status = GetStatus(calc.Key);
            if (status == Status.None)
                NewCalculate(calc, target);
            else if (status == Status.Running)
                WaitingCalculate(calc, target);
            else if (status == Status.Completed)
                FillResultTo(calc, target);
            else if (status == Status.Timeout)
                Recalculate(calc, target);
            else if (status == Status.Failed)
                RetryCalculate(calc, target);
            else
            {
                // Faulted
            }
        }

        public void NewCalculate(Calculator calc, ResultTarget target)
        {
            var rc = new ResultContext();
            rc.AddTarget(target.Caller, target);

            var fmuKey = calc.Key;
            lock (_results)
            {
                _results.Add(fmuKey, rc);
                target.Observer.OnNext("Waiting...");
            }

            Task.Run(async () =>
            {
                var status = await calc.Calculate();
                if (status == Status.Completed)
                {
                    ExcelAsyncUtil.QueueAsMacro(()=>rc.SetResult(calc.ResultData));
                }
            });
        }

        public void FillResultTo(Calculator calc, ResultTarget target)
        {
            var fmuKey = calc.Key;
            lock (_results)
            {
                _results.TryGetValue(fmuKey, out ResultContext value);
                target.Observer.OnNext("Waiting...");
                value.AddTarget(target.Caller, target);
            }
        }

        public void WaitingCalculate(Calculator calc, ResultTarget target)
        {
            var fmuKey = calc.Key;
            lock (_results)
            {
                _results.TryGetValue(fmuKey, out ResultContext value);
                target.Observer.OnNext("Waiting...");
                value.AddTarget(target.Caller, target);
            }
        }

        public void Recalculate(Calculator calc, ResultTarget target)
        {
            var fmuKey = calc.Key;
            lock (_results)
            {
                _results.TryGetValue(fmuKey, out ResultContext value);
                target.Observer.OnNext("Waiting...");
                value.AddTarget(target.Caller, target);
            }

            Task.Run(async () =>
            {
                var status = await calc.Calculate();
                if (status == Status.Completed)
                {
                    ExcelAsyncUtil.QueueAsMacro(()=>{
                        lock (_results)
                        {
                            _results.TryGetValue(fmuKey, out ResultContext value);
                            value.SetResult(calc.ResultData);
                        }
                    });
                }
            });
        }

        public string RetryCalculate(Calculator calc, ResultTarget target)
        {

            return "";
        }

        private Status GetStatus(string context)
        {
            lock (_results)
            {
                _results.TryGetValue(context, out ResultContext result);
                if (result == null)
                    return Status.None;
                else
                    return result.GetStatus();
            }
        }
    }
}
