using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabMCESystem.Logic.Execute
{
    /// <summary>
    /// 根据设定目标值而产生执行动作的控制器
    /// 抽象类，其它控制执行器的基类
    /// </summary>
    public class Executer
    {
        /// <summary>
        /// 指定目标值与安全输出范围创建
        /// </summary>
        /// <param name="targetVal">目标值</param>
        /// <param name="srange">安全范围</param>
        public Executer(float targetVal, SafeRange srange)
        {
            _safeRange = srange;

            if (!_safeRange.IsSafeIn(targetVal))
            {
                throw new ArgumentOutOfRangeException("设置的TargetVal值超出了所规定的SafeRange范围。");
            }
        }

        #region Properties, Fields

        private float _targetVal;
        /// <summary>
        /// 获取/设置执行目标值
        /// </summary>
        public float TargetVal
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
        /// 获取/设置执行器的安全输出范围
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
        public float ExecuteVal { get; protected set; }

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
        /// 调用Execute后已经执行时发生
        /// </summary>
        public event ExecuteChangedEventHandler ExecuteChanged;

        /// <summary>
        /// 执行完成 Status为Over时发生
        /// </summary>
        public event Action<object> ExecuteOvered;

        #endregion

        #region Mehods

        /// <summary>
        /// 执行控制输出
        /// </summary>
        public void Execute()
        {
            float teVal = 0f;
            if (OnExecute(ref teVal))
            {
                Status = ExecuteStatus.Excuting;

                ExecuteVal = teVal;

                ExecuteChanged?.Invoke(this, ExecuteVal);

                if (ExecuteVal == TargetVal)
                {
                    ExecuteOver();
                }
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
        /// <returns>反加True进行执行状态，并产生Executed事件</returns>
        protected virtual bool OnExecute(ref float eVal)
        {
            eVal = _targetVal;
            return true;
        }

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
        Excuting,
        // 完成
        Over
    }

    /// <summary>
    /// 控制执行器的安全输出范围
    /// </summary>
    public struct SafeRange
    {
        /// <summary>
        /// 获取/设置范围低值
        /// </summary>
        public float Low { set; get; }
        /// <summary>
        /// 获取/设置范围高值
        /// </summary>
        public float Hight { get; set; }

        public SafeRange(float l = float.NaN, float h = float.NaN)
        {
            if (h < l)
            {
                throw new ArgumentOutOfRangeException("安全范围的高值不能小于低值。");
            }

            Low = l;
            Hight = h;
        }

        /// <summary>
        /// 判断值是否在范围内
        /// </summary>
        /// <param name="val">检测值</param>
        /// <returns>在范围内为True</returns>
        public bool IsSafeIn(float val)
        {            
            return val >= Low && val <= Hight;
        }
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
        public float OldVal { get; private set; }

        /// <summary>
        /// 获取要新设定的执行目标值
        /// </summary>
        public float NewVal { get; private set; }

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
        public TargetValChangeEventArgs(float newVal, float oldVal, SafeRange range)
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
        public void ResetTrueVal(float reVal)
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
    /// 执行器执行发生委托
    /// </summary>
    /// <param name="sender">Executer事件源</param>
    /// <param name="executedVal">执行值</param>
    public delegate void ExecuteChangedEventHandler(object sender, float executedVal);
}
