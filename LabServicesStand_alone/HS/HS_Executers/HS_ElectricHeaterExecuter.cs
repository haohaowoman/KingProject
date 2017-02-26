using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mcLogic.Execute;
using mcLogic;
using LabMCESystem.LabElement;
using LabMCESystem.Servers.HS.HS_Executers;
namespace LabMCESystem.Servers.HS
{
    /// <summary>
    /// 环散系统 电加热器PID控制执行器
    /// 需要加入启动保护条件 拥有基础流量要求
    /// 温度控制滞后性较高 Ts = 20s
    /// 为保护加热器的使用寿命 Kp = 0.6 Ti = 10s Td = 1 
    /// 电加热器为一个加热组 热边加热器包含5个电炉 二冷边加热组包含2个电炉同时与空气混合
    /// </summary>
    class HS_ElectricHeaterExecuter : PredicatePositionPID
    {
        public HS_ElectricHeaterExecuter(string designMark, HS_HeaterContrller heater) : base(24.0, new SafeRange(0, 1000), new PIDParam() { Ts = 180000, Kp = 0.6, Ti = 10000, Td = 1000})
        {
            if (heater == null)
            {
                throw new ArgumentNullException(nameof(heater));
            }

            Heater = heater;

            UpdateFedback += HS_ElectricHeaterExecuter_UpdateFedback;

            ExecuteChanged += HS_ElectricHeaterExecuter_ExecuteChanged;

            // 公差为数据类型的精度。
            AllowTolerance = new Tolerance(0.11);

            AutoFinish = true;
        }

        /// <summary>
        /// 为防止电炉功率过大设置电炉的升温步进为100。
        /// </summary>
        public static double HeaterTempUpStepInterval = 100;

        public HS_HeaterContrller Heater { get; set; }

        /// <summary>
        /// 获取/设置电加热器的最低进入流量。
        /// </summary>
        public double RequireMinInFlow { get; set; }
        
        private void HS_ElectricHeaterExecuter_ExecuteChanged(object sender, double executedVal)
        {
            Heater?.SetTemperature(executedVal);
        }

        private void HS_ElectricHeaterExecuter_UpdateFedback(IDataFeedback sender)
        {
            if (Heater != null)
            {
                double temp = 0;
                if (Heater.GetCtrlTemperature(out temp))
                {
                    sender.FedbackData = temp;
                }
            }
        }

        protected override bool OnExecute(ref double eVal)
        {
            HS_ElectricHeaterExecuter_UpdateFedback(this);

            if (Heater?.HeaterIsRun == true)
            {
                double tc = TargetVal - FedbackData;
                if (tc <= 0)
                {
                    eVal = TargetVal;
                }
                else
                {
                    eVal = FedbackData + Math.Min(HeaterTempUpStepInterval, tc);
                }
            }
            else
            {
                eVal = Math.Min(TargetVal,HeaterTempUpStepInterval);
            }
            //eVal = TargetVal;
            return true;
        }
    }
}
