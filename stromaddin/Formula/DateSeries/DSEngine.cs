using ExcelDna.Integration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace stromddin.Formula.DateSeries
{
    internal enum Status
    {
        NotStarted,
        Running,
        Completed,
        Timeout,
        Failed,
        Faulted
    }

    internal class DSObservable : IExcelObservable
    {
        DSCalculator _calc;
        string _fmu;
        ExcelReference _caller;
        public DSObservable(ExcelReference caller, string fmu, DSCalculator calc)
        {
            _caller = caller;
            _fmu = fmu;
            _calc = calc;
        }
        public IDisposable Subscribe(IExcelObserver observer)
        {
            var target = new ResultTarget(_caller, _fmu, observer);
            DSEngine.Instance.Calculate(_calc, target);
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

    internal class DSEngine
    {
        Dictionary<string, DSResultContext> _results = new Dictionary<string, DSResultContext>();
        public static DSEngine Instance = new DSEngine();
        private DSEngine()
        {
        }

        public void Calculate(DSCalculator calc, ResultTarget target)
        {
            var status = GetStatus(calc.Key);
            if (status == Status.NotStarted)
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

        public void NewCalculate(DSCalculator calc, ResultTarget target)
        {
            var rc = new DSResultContext();
            rc.AddTarget(target.Caller, target);

            var fmuKey = calc.Key;
            lock (_results)
            {
                _results.Add(fmuKey, rc);
                target.Observer.OnNext("Waiting...");
            }

            Task.Run(async () => await ImplCalculate(calc));
        }

        public void FillResultTo(DSCalculator calc, ResultTarget target)
        {
            var fmuKey = calc.Key;
            lock (_results)
            {
                _results.TryGetValue(fmuKey, out DSResultContext value);
                target.Observer.OnNext("Waiting...");
                value.AddTarget(target.Caller, target);
            }
        }

        public void WaitingCalculate(DSCalculator calc, ResultTarget target)
        {
            var fmuKey = calc.Key;
            lock (_results)
            {
                _results.TryGetValue(fmuKey, out DSResultContext value);
                target.Observer.OnNext("Waiting...");
                value.AddTarget(target.Caller, target);
            }
        }

        public void Recalculate(DSCalculator calc, ResultTarget target)
        {
            var fmuKey = calc.Key;
            lock (_results)
            {
                _results.TryGetValue(fmuKey, out DSResultContext value);
                target.Observer.OnNext("Waiting...");
                value.AddTarget(target.Caller, target);
            }
            Task.Run(async () => await ImplCalculate(calc));
        }

        public string RetryCalculate(DSCalculator calc, ResultTarget target)
        {

            return "";
        }

        private async Task ImplCalculate(DSCalculator calc)
        {
            var fmuKey = calc.Key;
            var status = await calc.Calculate();

            ExcelAsyncUtil.QueueAsMacro(() =>
            {
                lock (_results)
                {
                    _results.TryGetValue(fmuKey, out DSResultContext value);
                    if (status == Status.Completed)
                    {
                        var dt = calc.ResultData;
                        value.SetResult(dt);
                    }
                    else
                    {
                        value.SetResult(null);
                    }
                }
            });
        }
        
        private Status GetStatus(string context)
        {
            lock (_results)
            {
                _results.TryGetValue(context, out DSResultContext result);
                if (result == null)
                    return Status.NotStarted;
                else
                    return result.GetStatus();
            }
        }
    }
}
