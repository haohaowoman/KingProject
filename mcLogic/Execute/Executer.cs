using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mcLogic;

namespace mcLogic.Execute
{
    /// <summary>
    /// 根据设定目标值而产生执行动作的控制器
    /// 抽象类，其它控制执行器的基类
    /// </summary>
    public abstract class Executer
    {
        /// <summary>
        /// 指定目标值与安全输出范围创建。
        /// </summary>
        /// <param name="targetVal">目标值</param>
        /// <param name="srange">安全范围</param>
        /// <exception cref="ArgumentOutOfRangeException">目标值超出安全范围时引发异常。</exception>
        public Executer(double targetVal, SafeRange srange)
        {
            _safeRange = srange;

            if (!_safeRange.IsSafeIn(targetVal))
            {
                throw new ArgumentOutOfRangeException("设置的TargetVal值超出了所规定的SafeRange范围。");
            }
            else
            {
                _targetVal = targetVal;
            }
        }

        #region Properties, Fields

        private double _targetVal;
        /// <summary>
        /// 获取/设置执行目标值。
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">目标值超出安全范围时引发异常。</exception>
        public double TargetVal
        {
            get { return _targetVal; }
            set
            {
                TargetValChangeEventArgs args = new TargetValChangeEventArgs(value, _targetVal, _safeRange);

                TargetValChanging?.Invoke(this, args);

                if (args.Allowed)
                {
                    _targetVal = args.NewVal;

                    TargetValChanged?.Invoke(this, args);
                }
                else
                {
                    throw new ArgumentOutOfRangeException("设置的TargetVal值超出了所规定的SafeRange范围。");
                }
            }
        }

        private SafeRange _safeRange;
        /// <summary>
        /// 获取/设置执行器的安全输出范围。
        /// </summary>
        public SafeRange SafeRange
        {
            get { return _safeRange; }
            set
            {
                // 在更新之前需要保定当前TargetVal是否处在新范围之内
                if (value.IsSafeIn(_targetVal))
                {
                    _safeRange = value;
                }

            }
        }

        private ExecuteStatus _status;
        /// <summary>
        /// 获取执行的状态
        /// </summary>
        public ExecuteStatus Status
        {
            get { return _status; }
            protected set { _status = value; }
        }

        /// <summary>
        /// 获取已执行输出值
        /// </summary>
        public double ExecuteVal { get; protected set; }

        /// <summary>
        /// 获取/设置执行器的设计标记
        /// </summary>
        public string DesignMark { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// 执行目标值正在发生改变时发生
        /// 可以检查目标值的有效性，并可以对其验证改变
        /// </summary>
        public event ExecuterTargetValChangingEventHandler TargetValChanging;
        /// <summary>
        /// 执行目标值已经改变了发生
        /// </summary>
        public event ExecuterTargetValChangedEventHandler TargetValChanged;

        /// <summary>
        /// 执行器改变输之前发生
        /// 客户端有机会在此做出执行输出数据的更新
        /// </summary>
        public event BeforeExecutedEventHandler BeforeExecuted;

        /// <summary>
        /// 调用Execute后已经执行时发生
        /// </summary>
        public event ExecuteChangedEventHandler ExecuteChanged;

        /// <summary>
        /// 执行完成 Status为Over时发生
        /// </summary>
        public event Action<object> ExecuteOvered;

        #endregion

        #region Methods

        /// <summary>
        /// 执行开始，默认执行Execute()。
        /// </summary>
        public virtual void ExecuteBegin()
        {
            Execute();
        }

        /// <summary>
        /// 执行控制输出，单次执行。
        /// </summary>
        public void Execute()
        {
            double teVal = ExecuteVal;
            if (OnExecute(ref teVal))
            {
                Status = ExecuteStatus.Executing;

                BeforeExecuted?.Invoke(this, ref teVal);

                // 在输出时进行安全范围验证
                if (!SafeRange.IsSafeIn(teVal))
                {
                    teVal = Math.Max(teVal, SafeRange.Low);
                    teVal = Math.Min(teVal, SafeRange.Height);
                }

                ExecuteVal = teVal;

                ExecuteChanged?.Invoke(this, ExecuteVal);
                                
            }
        }

        /// <summary>
        /// 执行完成操作
        /// </summary>
        public virtual void ExecuteOver()
        {
            Status = ExecuteStatus.Over;

            ExecuteOvered?.Invoke(this);            
        }

        /// <summary>
        /// 得置执行器
        /// </summary>
        public virtual void Reset()
        {
            Status = ExecuteStatus.Wait;
        }

        /// <summary>
        /// 派生类实现，调用Execute时产生，做为执行前调用逻辑
        /// </summary>
        /// <returns>反回True进行执行状态，并产生Executed事件</returns>
        protected abstract bool OnExecute(ref double eVal);

        #endregion
    }

    /// <summary>
    /// 枚举执行器的状态
    /// </summary>
    public enum ExecuteStatus
    {
        // 等待，未执行任务动作
        Wait,
        // 正在执行输出
        Executing,
        // 完成
        Over
    }

    /// <summary>
    /// 执行器目标值正在改变事件参数
    /// 可以停止其改变状态，并验其数据的执行安全性
    /// 首先会验证输出范围的安全性
    /// </summary>
    public class TargetValChangeEventArgs : EventArgs
    {

        /// <summary>
        /// 获取旧执行目标值
        /// </summary>
        public double OldVal { get; private set; }

        /// <summary>
        /// 获取要新设定的执行目标值
        /// </summary>
        public double NewVal { get; private set; }

        /// <summary>
        /// 获取新新设定的执行目标值是否超出其安全输出范围
        /// </summary>
        public bool IsOutSafeRangeForNew { get; private set; } = false;

        /// <summary>
        /// 获取是否同意改变为新的执行目标值
        /// 可以在事件响应中验证数据的有效性，更改其值控制目标值的更新逻辑
        /// </summary>
        public bool Allowed { get; private set; } = true;

        /// <summary>
        /// 获取数据的安全有效范围
        /// </summary>
        public SafeRange Range { get; private set; }

        /// <summary>
        /// Executer在设置TargetVal时创建
        /// </summary>
        /// <param name="newVal">新TargetVal值</param>
        /// <param name="oldVal">拥有的旧TargetVal</param>
        /// <param name="range">Executer的安全范围</param>
        public TargetValChangeEventArgs(double newVal, double oldVal, SafeRange range)
        {
            NewVal = newVal;
            OldVal = oldVal;

            Range = range;

            if (!range.IsSafeIn(NewVal))
            {
                IsOutSafeRangeForNew = true;

                Allowed = false;
            }
        }

        /// <summary>
        /// 如果出现了数据有效性证错误，可以在此重新设置正确的值
        /// </summary>
        /// <param name="reVal"></param>
        public void ResetTrueVal(double reVal)
        {
            if (Range.IsSafeIn(reVal))
            {
                NewVal = reVal;
                Allowed = true;
                IsOutSafeRangeForNew = false;
            }
        }
    }

    /// <summary>
    /// 执行器目标值正在改变委托
    /// </summary>
    /// <param name="sender">Executer事件源</param>
    /// <param name="args">包含TargetVal值改变事件参数</param>
    public delegate void ExecuterTargetValChangingEventHandler(object sender, TargetValChangeEventArgs e);
    /// <summary>
    /// 执行器目标值已经改变委托
    /// </summary>
    /// <param name="sender">Executer事件源</param>
    /// <param name="args">包含TargetVal值改变事件参数</param>
    public delegate void ExecuterTargetValChangedEventHandler(object sender, TargetValChangeEventArgs e);

    /// <summary>
    /// 执行器执行改变之前委托
    /// </summary>
    /// <param name="sender">Executer事件源</param>
    /// <param name="eVal">将执行的数据</param>
    public delegate void BeforeExecutedEventHandler(object sender, ref double eVal);

    /// <summary>
    /// 执行器执行改变委托
    /// </summary>
    /// <param name="sender">Executer事件源</param>
    /// <param name="executedVal">执行值</param>
    public delegate void ExecuteChangedEventHandler(object sender, double executedVal);
}
