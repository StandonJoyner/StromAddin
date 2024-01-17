using ExcelDna.Integration;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace stromaddin.Formula.DateSeries
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
        DHCalculator _calc;
        string _fmu;
        ExcelReference _caller;
        public DSObservable(ExcelReference caller, string fmu, DHCalculator calc)
        {
            _caller = caller;
            _fmu = fmu;
            _calc = calc;
        }
        public IDisposable Subscribe(IExcelObserver observer)
        {
            var target = new ResultTarget(_caller, _fmu, observer);
            DHEngine.Instance.Calculate(_calc, target);
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

    internal class DHEngine
    {
        // Add a BlockingCollection to queue tasks
        private BlockingCollection<Task> _taskQueue = new BlockingCollection<Task>();

        Dictionary<string, DHResultContext> _results = new Dictionary<string, DHResultContext>();
        public static DHEngine Instance = new DHEngine();
        private DHEngine()
        {
            // Start a dedicated thread to process tasks in the queue
            new Thread(() =>
            {
                foreach (var task in _taskQueue.GetConsumingEnumerable())
                {
                    task.Start();
                }
            })
            { IsBackground = true }.Start();
        }

        public void Calculate(DHCalculator calc, ResultTarget target)
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

        public void NewCalculate(DHCalculator calc, ResultTarget target)
        {
            var rc = new DHResultContext();
            rc.AddTarget(target.Caller, target);

            var fmuKey = calc.Key;
            lock (_results)
            {
                _results.Add(fmuKey, rc);
                target.Observer.OnNext("Waiting...");
            }

            // Add the task to the queue instead of running it immediately
            _taskQueue.Add(new Task(async () => await ImplCalculate(calc)));
        }

        public void FillResultTo(DHCalculator calc, ResultTarget target)
        {
            var fmuKey = calc.Key;
            lock (_results)
            {
                _results.TryGetValue(fmuKey, out DHResultContext value);
                target.Observer.OnNext("Waiting...");
                value.AddTarget(target.Caller, target);
            }
        }

        public void WaitingCalculate(DHCalculator calc, ResultTarget target)
        {
            var fmuKey = calc.Key;
            lock (_results)
            {
                _results.TryGetValue(fmuKey, out DHResultContext value);
                target.Observer.OnNext("Waiting...");
                value.AddTarget(target.Caller, target);
            }
        }

        public void Recalculate(DHCalculator calc, ResultTarget target)
        {
            var fmuKey = calc.Key;
            lock (_results)
            {
                _results.TryGetValue(fmuKey, out DHResultContext value);
                target.Observer.OnNext("Waiting...");
                value.AddTarget(target.Caller, target);
            }
            // Add the task to the queue instead of running it immediately
            _taskQueue.Add(new Task(async () => await ImplCalculate(calc)));
        }

        public string RetryCalculate(DHCalculator calc, ResultTarget target)
        {

            return "";
        }

        private async Task ImplCalculate(DHCalculator calc)
        {
            var fmuKey = calc.Key;
            var status = await calc.Calculate();

            ExcelAsyncUtil.QueueAsMacro(() =>
            {
                lock (_results)
                {
                    _results.TryGetValue(fmuKey, out DHResultContext value);
                    if (status == Status.Completed)
                    {
                        var dt = calc.ResultData;
                        value.SetResult(dt);
                    }
                    else
                    {
                        value.SetError(calc.ErrMessage);
                    }
                }
            });
        }
        
        private Status GetStatus(string context)
        {
            lock (_results)
            {
                _results.TryGetValue(context, out DHResultContext result);
                if (result == null)
                    return Status.NotStarted;
                else
                    return result.GetStatus();
            }
        }
    }
}
