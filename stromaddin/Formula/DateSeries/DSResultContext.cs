using ExcelDna.Integration;
using stromaddin.Core.Excel;
using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Office.Interop.Excel;

namespace stromaddin.Formula.DateSeries
{
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

    internal class DSResultContext
    {
        Status _status = Status.Running;
        DateTime _complateTm;
        Dictionary<ExcelReference, ResultTarget> _targets = new Dictionary<ExcelReference, ResultTarget>();
        TableOutput _data;

        public DSResultContext()
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
        public void SetResult(TableOutput data)
        {
            if (ExcelDnaUtil.MainManagedThreadId != Thread.CurrentThread.ManagedThreadId)
                throw new InvalidOperationException("SetResult must be called from a main thread.");
            _status = Status.Completed;
            _complateTm = DateTime.Now;
            _data = data;
            FillAll();
        }
        public void SetError(string err)
        {
            if (ExcelDnaUtil.MainManagedThreadId != Thread.CurrentThread.ManagedThreadId)
                throw new InvalidOperationException("SetError must be called from a main thread.");
            _status = Status.Failed;
            _complateTm = DateTime.Now;
            _data = null;
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
                        if (_status == Status.Failed)
                        {
                            target.Observer.OnNext("Failed");
                        }
                        else if (_status == Status.Timeout)
                        {
                            target.Observer.OnNext("Timeout");
                        }
                        else
                        {
                            target.Observer.OnNext("Complete");
                            Application excelApp = (Application)ExcelDnaUtil.Application;
                            var a1Caller = target.Caller.ConvertToA1Style();
                            var range = excelApp.Range[a1Caller];
                            _data.Output(range.Offset[1]);
                        }
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
                    target.Observer.OnNext("Complete");
                    Application excelApp = (Application)ExcelDnaUtil.Application;
                    var range = excelApp.Range[target.Caller.ConvertToA1Style()];
                    _data.Output(range.Offset[1]);
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

}
