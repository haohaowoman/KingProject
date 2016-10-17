using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabMCESystem.Logic.Execute
{
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
        public StepExecuter(float targetVal, SafeRange srange) : base(targetVal, srange)
        {

        }

        public StepExecuter(float targetVal, SafeRange srange, float step) : this(targetVal, srange)
        {
            Step = step;
        }

        #region Propertis

        /// <summary>
        /// 获取/设置步进值
        /// </summary>
        public float Step { get; set; }

        private float _iniVal;
        /// <summary>
        /// 获取/设置初始执行值
        /// </summary>
        public float IniVal
        {
            get { return _iniVal; }
            set
            {
                if (SafeRange.IsSafeIn(value))
                {
                    _iniVal = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("设置的IniVal初始值超出了所规定的SafeRange范围。");
                }
            }
        }

        #endregion

        #region Override

        //实现Step逻辑
        protected override bool OnExecute(ref float eVal)
        {
            return base.OnExecute(ref eVal);
        }

        #endregion

        private float RunStep()
        {
            float teVal = 0;
            switch (Status)
            {
                case ExecuteStatus.Wait:

                    teVal = ExecuteVal = _iniVal;

                    break;
                case ExecuteStatus.Excuting:

                    float tempVal = ExecuteVal + Step;

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
}
