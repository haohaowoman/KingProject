using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using ExpRuntimeApp.Modules;
using ExpRuntimeApp.ViewModules;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace ExpRuntimeApp.ExpTask
{
    /// <summary>
    /// 表示公差范围
    /// </summary>
    public struct Tolerance
    {
        /// <summary>
        /// 获取/设置正公差
        /// </summary>
        public double Positive { get; set; }

        /// <summary>
        /// 获取/设置负公差
        /// </summary>
        public double Negative { get; set; }

        /// <summary>
        /// 通过指定公差创建
        /// </summary>
        /// <param name="tolerance">公差，一般可表示为+—t</param>
        public Tolerance(double tolerance)
        {
            Positive = tolerance;
            Negative = -tolerance;
        }

        /// <summary>
        /// 判断一个反馈数据是否在目标值的本公差范围内
        /// </summary>
        /// <param name="targetVal">目标值</param>
        /// <param name="fedbackVal">反馈值</param>
        /// <returns></returns>
        public bool IsInTolerance(double targetVal, double fedbackVal)
        {
            return (targetVal + Negative) <= fedbackVal && fedbackVal <= (targetVal + Positive);
        }
    }

    class TaskElement : INotifyPropertyChanged
    {
        public TaskElement()
        {

        }
        public TaskElement(MdChannel tch)
        {
            TargetChannel = tch;
        }
        private MdChannel _targChannel;
        public MdChannel TargetChannel
        {
            get { return _targChannel; }
            set
            {
                _targChannel = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TargetChannel"));
            }
        }

        private double? _targetValue;

        public double? TargetValue
        {
            get { return _targetValue; }
            set
            {
                _targetValue = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TargetValue"));
            }
        }

        public MCommand ExecuteCommand { get; set; }

        public MCommand StopExecuteCommand { get; set; }

        public Tolerance AllowTolerance { get; set; } = new Tolerance(10);

        public double ToleranceValue
        {
            get { return AllowTolerance.Positive; }
            set
            {
                if (value < 0)
                {
                    value = -value;
                }
                AllowTolerance = new Tolerance(value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ToleranceValue)));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsEffictive()
        {
            if (TargetValue == null)
            {
                return true;
            }
            if (TargetChannel?.AsAIChannel == null)
            {
                return false;
            }
            return AllowTolerance.IsInTolerance((double)TargetValue, TargetChannel.MeasureValue);
        }

        public void Execute()
        {
            if (TargetValue != null && TargetChannel != null)
            {
                TargetChannel.AOValue = (double)TargetValue;
                ExecuteCommand?.Execute(TargetChannel);
            }
        }

        public void StopExecute()
        {
            if (TargetChannel != null)
            {
                StopExecuteCommand?.Execute(TargetChannel);
            }
        }

        public TaskElement Clone()
        {
            var el = new TaskElement();
            el.AllowTolerance = AllowTolerance;
            el.ExecuteCommand = ExecuteCommand;
            el.TargetChannel = TargetChannel;
            el.TargetValue = TargetValue;
            return el;
        }
    }

    enum TaskerState
    {
        Wait,
        Running,
        Overred
    }

    class Tasker : IDisposable, INotifyPropertyChanged
    {
        public Tasker()
        {
            _runTimer.Elapsed += _runTimer_Elapsed;
            Interval = 10;
        }

        private Timer _runTimer = new Timer();

        public List<TaskElement> Elements { get; set; } = new List<TaskElement>();

        private TaskerState _state = TaskerState.Wait;

        public TaskerState State
        {
            get { return _state; }
            private set
            {
                if (value != _state)
                {
                    _state = value;
                    TaskerStateChanged?.Invoke(this, value);
                }
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(State)));
            }
        }

        public int ThroughCount { get; set; } = 3;

        private int _throughTick = 0;
        public int ThroughTick
        {
            get { return _throughTick; }
            private set
            {
                _throughTick = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ThroughTick)));
            }
        }

        public int Interval
        {
            get { return (int)_runTimer.Interval / 1000; }
            set
            {
                _runTimer.Interval = value * 1000.0;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Interval)));
            }
        }

        public event EventHandler<TaskerState> TaskerStateChanged;

        public event EventHandler<int> TaskerThroughTickChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        public void Run()
        {
            if (State == TaskerState.Wait)
            {
                State = TaskerState.Running;
                foreach (var el in Elements)
                {
                    el.Execute();
                }
                _runTimer.Start();
            }
        }

        public void Reset()
        {
            _runTimer.Stop();
            _state = TaskerState.Wait;
            ThroughTick = 0;
        }

        public void Stop()
        {
            _runTimer.Stop();
            if (State == TaskerState.Running)
            {
                foreach (var el in Elements)
                {
                    el.StopExecute();
                }
            }
            State = TaskerState.Wait;
        }

        public Tasker Clone()
        {
            var tasker = new Tasker();
            foreach (var el in Elements)
            {
                tasker.Elements.Add(el.Clone());
            }
            tasker.Interval = Interval;
            tasker.ThroughCount = ThroughCount;
            return tasker;
        }

        private void _runTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            bool bt = true;
            foreach (var el in Elements)
            {
                bt &= el.IsEffictive();
            }
            if (bt)
            {
                ThroughTick++;
                TaskerThroughTickChanged?.Invoke(this, ThroughTick);
                if (ThroughTick >= ThroughCount)
                {
                    _runTimer.Stop();
                    State = TaskerState.Overred;
                }
            }
        }

        public void Dispose()
        {
            _runTimer.Close();
        }
    }

    class TaskerRunEventArgs : EventArgs
    {
        public TaskerRunEventArgs(Tasker tasker)
        {
            SourceTasker = tasker;
            if (SourceTasker != null)
            {
                ThroughTick = SourceTasker.ThroughTick;
                State = SourceTasker.State;
            }
        }

        public Tasker SourceTasker { get; private set; }

        public int ThroughTick { get; private set; }

        public TaskerState State { get; set; }

    }

    class TaskerCollection : ObservableCollection<Tasker>, IDisposable
    {
        public TaskerCollection()
        {
            this.CollectionChanged += TaskerCollection_CollectionChanged;
        }

        public event EventHandler<TaskerRunEventArgs> TaskerRun;

        /// <summary>
        /// 判断是否已有任务正在运行。
        /// </summary>
        public bool IsRunning
        {
            get
            {
                bool br = false;
                foreach (var item in this)
                {
                    if (item.State == TaskerState.Running)
                    {
                        br = true;
                        break;
                    }
                }
                return br;
            }
        }

        public Tasker CurRunTasker
        {
            get;
            private set;
        }

        public bool RunOneTasker(Tasker tasker)
        {
            bool br = false;
            if (Contains(tasker) && !IsRunning)
            {
                tasker.Run();
                br = true;
            }
            return br;
        }

        public bool RunFirstTasker()
        {
            bool br = false;
            if (!IsRunning && Count > 0)
            {
                this[0].Run();
                br = true;
            }
            return br;
        }

        public bool RunNextTasker()
        {
            if (CurRunTasker == null)
            {
                return RunFirstTasker();
            }
            else
            {
                int cIndex = IndexOf(CurRunTasker);
                if (cIndex >= 0 && cIndex < Count - 1)
                {
                    return RunOneTasker(this[cIndex + 1]);
                }
                else
                {
                    return false;
                }
            }
        }

        public void Stop()
        {
            foreach (var item in this)
            {
                item.Stop();
            }
        }

        private void Tasker_TaskerThroughTickChanged(object sender, int e)
        {
            TaskerRun?.Invoke(this, new TaskerRunEventArgs(sender as Tasker));
        }

        private void Tasker_TaskerStateChanged(object sender, TaskerState e)
        {
            switch (e)
            {
                case TaskerState.Wait:
                    break;
                case TaskerState.Running:
                    CurRunTasker = sender as Tasker;
                    break;
                case TaskerState.Overred:
                    break;
                default:
                    break;
            }

            TaskerRun?.Invoke(this, new TaskerRunEventArgs(sender as Tasker));
        }


        private void TaskerCollection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    {
                        if (e.NewItems != null)
                        {
                            foreach (var item in e.NewItems)
                            {
                                var tasker = item as Tasker;
                                if (tasker != null)
                                {
                                    tasker.TaskerStateChanged += Tasker_TaskerStateChanged;
                                    tasker.TaskerThroughTickChanged += Tasker_TaskerThroughTickChanged;
                                }
                            }
                        }
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    {
                        if (e.OldItems != null)
                        {
                            foreach (var item in e.OldItems)
                            {
                                var tasker = item as Tasker;
                                if (tasker != null)
                                {
                                    tasker.TaskerStateChanged -= Tasker_TaskerStateChanged;
                                    tasker.TaskerThroughTickChanged -= Tasker_TaskerThroughTickChanged;
                                    tasker.Dispose();
                                }
                            }
                        }
                        if (e.NewItems != null)
                        {
                            foreach (var item in e.NewItems)
                            {
                                var tasker = item as Tasker;
                                if (tasker != null)
                                {
                                    tasker.TaskerStateChanged += Tasker_TaskerStateChanged;
                                    tasker.TaskerThroughTickChanged += Tasker_TaskerThroughTickChanged;
                                }
                            }
                        }
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                    {
                        if (e.OldItems != null)
                        {
                            foreach (var item in e.OldItems)
                            {
                                var tasker = item as Tasker;
                                if (tasker != null)
                                {
                                    tasker.TaskerStateChanged -= Tasker_TaskerStateChanged;
                                    tasker.TaskerThroughTickChanged -= Tasker_TaskerThroughTickChanged;
                                    tasker.Dispose();
                                }
                            }
                        }
                        if (e.NewItems != null)
                        {
                            foreach (var item in e.NewItems)
                            {
                                var tasker = item as Tasker;
                                if (tasker != null)
                                {
                                    tasker.TaskerStateChanged += Tasker_TaskerStateChanged;
                                    tasker.TaskerThroughTickChanged += Tasker_TaskerThroughTickChanged;
                                }
                            }
                        }
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    {
                        if (e.OldItems != null)
                        {
                            foreach (var item in e.OldItems)
                            {
                                var tasker = item as Tasker;
                                if (tasker != null)
                                {
                                    tasker.TaskerStateChanged -= Tasker_TaskerStateChanged;
                                    tasker.TaskerThroughTickChanged -= Tasker_TaskerThroughTickChanged;
                                    tasker.Dispose();
                                }
                            }
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        public void Dispose()
        {
            foreach (var item in this)
            {
                item?.Dispose();
            }
        }
    }
}
