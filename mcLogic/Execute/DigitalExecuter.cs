﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mcLogic.Execute
{
    /// <summary>
    /// 实现离散量摈制执行器
    /// 将只有True 和 False的状态 
    /// 0为False 1为True
    /// </summary>
    public class DigitalExecuter : Executer
    {
        public DigitalExecuter() : base(0, new SafeRange(0, 1.0))
        {

        }

        /// <summary>
        /// 重写，执行打开和关闭
        /// </summary>
        /// <param name="eVal"></param>
        /// <returns></returns>
        protected override bool OnExecute(ref double eVal)
        {
            eVal = TargetVal;
            return true;
        }

        /// <summary>
        /// 将状态置为高位，true。
        /// </summary>
        public void ToHigh()
        {
            TargetVal = 1.0;
            Execute();
        }

        /// <summary>
        /// 将状态转为低位，false。
        /// </summary>
        public void ToLow()
        {
            TargetVal = 0;
            Execute();
        }

        /// <summary>
        /// 获取当前离散的打开或关闭状态。
        /// </summary>
        public bool Enable { get { return ExecuteVal == 1.0; } }

    }
}
