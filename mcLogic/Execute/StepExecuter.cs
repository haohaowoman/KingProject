using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabMCESystem.Logic.Execute
{
    /// <summary>
    /// 表式步进的执行方向
    /// </summary>
    public enum StepDirection
    {
        // 上升
        Ascending,
        // 下降
        Descending
    }

    /// <summary>
    /// 步进执行器
    /// 可以按照指定的步进进行执行输出控制
    /// </summary>
    public class StepExecuter : Executer
    {
        /// <summary>
        /// 指定目标值与安全输出范围创建
        /// </summary>
        /// <param name="targetVal">目标值</param>
        /// <param name="srange">安全范围</param>
        public StepExecuter(double targetVal, SafeRange srange) : base(targetVal, srange)
        {
            BeforeExecuted += StepExecuter_BeforeExecuted;

            TargetValChanged += StepExecuter_TargetValChanged;
        }

        public StepExecuter(double targetVal, SafeRange srange, double step) : this(targetVal, srange)
        {
            Step = step;
        }

        #region Propertis

        private double _step;
        /// <summary>
        /// 获取/设置步进值
        /// 确定为正数
        /// </summary>
        public double Step
        {
            get { return _step; }
            set
            {
                if (value < 0f)
                {
                    throw new ArgumentException("不能将StepExecuter的Step属性设置为负值。");
                }
                else
                {
                    _step = value;
                }

            }
        }

        private double _iniVal;
        /// <summary>
        /// 获取/设置初始执行值
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">设定初始值超过安全范围时引发异常。</exception>
        public double IniVal
        {
            get { return _iniVal; }
            set
            {
                if (SafeRange.IsSafeIn(value))
                {
                    _iniVal = value;

                    Dircetion = _iniVal < TargetVal ? StepDirection.Ascending : StepDirection.Descending;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("设置的IniVal初始值超出了所规定的SafeRange范围。");
                }
            }
        }

        /// <summary>
        /// 获取当前的步进方向
        /// </summary>
        public StepDirection Dircetion { get; private set; }

        #endregion

        #region Override

        protected override bool OnExecute(ref double eVal)
        {
            return true;
        }

        private void StepExecuter_BeforeExecuted(object sender, ref double eVal)
        {
            eVal = RunStep();
        }

        private void StepExecuter_TargetValChanged(object sender, TargetValChangeEventArgs e)
        {
            // 重新设定目标值后需要更新步进控制方向
            Dircetion = _iniVal < TargetVal ? StepDirection.Ascending : StepDirection.Descending;
        }

        #endregion

        //实现Step逻辑
        private double RunStep()
        {
            double teVal = 0;
            switch (Status)
            {
                case ExecuteStatus.Wait:

                    teVal = ExecuteVal = _iniVal;

                    break;
                case ExecuteStatus.Executing:

                    double tempStep = Dircetion == StepDirection.Ascending ? Step : -Step;

                    double tempVal = ExecuteVal + tempStep;

                    // 判断执行本次步进是否能达到目标值
                    SafeRange tempRange = new SafeRange(Math.Min(ExecuteVal, tempVal), Math.Max(ExecuteVal, tempVal));

                    if (tempRange.IsSafeIn(TargetVal))
                    {
                        tempVal = TargetVal;
                    }

                    teVal = tempVal;

                    if (!SafeRange.IsSafeIn(teVal))
                    {
                        teVal = Math.Min(teVal, SafeRange.Hight);
                        teVal = Math.Max(teVal, SafeRange.Low);
                    }

                    break;
                case ExecuteStatus.Over:

                    break;
                default:
                    break;
            }

            return teVal;
        }
    }

    /// <summary>
    /// 带反馈的步进执行器
    /// </summary>
    public class FedbackStepExecuter : StepExecuter, IDataFedback
    {
        public FedbackStepExecuter(double targetVal, SafeRange srange) : base(targetVal, srange)
        {
        }

        public FedbackStepExecuter(double targetVal, SafeRange srange, double step) : base(targetVal, srange, step)
        {

        }

        #region Properties

        //反馈数据
        private double _fedbackData;
        /// <summary>
        /// 获取/设置执行器反馈
        /// </summary>
        public double FedbackData
        {
            get
            {
                return _fedbackData;
            }
            set
            {
                _fedbackData = value;

                if (!SafeRange.IsSafeIn(_fedbackData))
                {
                    FedbackDataOutOfSafeRange?.Invoke(this, _fedbackData);
                }
            }
        }

        public event FedbackDataOutOfSafeRangeEventHandler FedbackDataOutOfSafeRange;
        public event UpdateFedbackValEventHandler UpdateFedback;

        #endregion

        #region Override

        protected override bool OnExecute(ref double eVal)
        {
            UpdateFedback?.Invoke(this);

            // 以反馈数据确定当前输出量
            ExecuteVal = FedbackData;

            return true;
        }

        #endregion
    }
}
