using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mcLogic.Execute
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

    /// <summary>
    /// 闭环执行控制器
    /// 实现IDataFedback接口
    /// </summary>
    public abstract class ClosedLoopExecuter : Executer, IDataFeedback
    {
        public ClosedLoopExecuter(double targetVal, SafeRange srange) : base(targetVal, srange)
        {
            
        }

        // 允许公差范围
        private Tolerance _allowTolerance;
        /// <summary>
        /// 获取/设置执行器的允许公差范围
        /// </summary>
        public Tolerance AllowTolerance
        {
            get { return _allowTolerance; }
            set { _allowTolerance = value; }
        }
        
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

                if (_allowTolerance.IsInTolerance(TargetVal, _fedbackData))
                {
                    FedbackInTolerance?.Invoke(this, _fedbackData);
                }
            }
        }

        /// <summary>
        /// 需要更新反馈数据时发生
        /// </summary>
        public event UpdateFedbackValEventHandler UpdateFedback;

        /// <summary>
        /// 反馈超出安全范围时发生
        /// </summary>
        public event FedbackDataOutOfSafeRangeEventHandler FedbackDataOutOfSafeRange;

        /// <summary>
        /// 反馈值达到了目标值的允许公差范围时发生
        /// </summary>
        public event EventHandler<double> FedbackInTolerance;
        #region Override

        /// <summary>
        /// 重载方法，获得必须的反馈值
        /// 派生类若需要仍需要重载此方法，并首先调用base.OnExecute进行判断
        /// </summary>
        /// <param name="eVal">执行值的引用</param>
        /// <returns>是否进行执行</returns>
        protected override bool OnExecute(ref double eVal)
        {
            // 必须获取反馈
            if (UpdateFedback != null)
            {
                UpdateFedback.Invoke(this);

                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion
    }
}
