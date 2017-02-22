using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mcLogic;
using mcLogic.Execute;
using LabMCESystem.LabElement;
namespace LabMCESystem.Servers.HS.HS_Executers
{
    /// <summary>
    /// 实现温度控制的执行器，其控制多个加热器进行产品入品温度的调节。
    /// </summary>
    class HS_TempreatureExecute : PredicatePositionPID, IPredicateExecute
    {
        public HS_TempreatureExecute() : base(0, new SafeRange(0, 800),
            new PIDParam()
            {
                Ts = 60 * 1000 * 10,
                Kp = 0.66,
                Td = 0,
                Ti = 0
            }
            )
        {
            Heaters = new List<HS_HeaterContrller>();
            HeaterChannels = new List<FeedbackChannel>();

            UpdateFedback += HS_TempreatureExecute_UpdateFedback;
            ExecuteChanged += HS_TempreatureExecute_ExecuteChanged;
            ExecutePredicate = TempExecutePridicate;

        }

        #region Properties
        /// <summary>
        /// 获取温度控制器的控制电炉集合。
        /// </summary>
        public List<HS_HeaterContrller> Heaters { get; private set; }

        /// <summary>
        /// 获取电炉通道集合。
        /// </summary>
        public List<FeedbackChannel> HeaterChannels { get; private set; }

        /// <summary>
        /// 获取/设置需要控制的目标温度通道。
        /// </summary>
        public FeedbackChannel TargetTempChannel { get; set; }
        /// <summary>
        /// 获取/设置次要辅助测量的温度通道。
        /// </summary>
        public AnalogueMeasureChannel SecTempChannel { get; set; }
        /// <summary>
        /// 获取/设置加热器所在的流量通道。
        /// </summary>
        public FeedbackChannel FlowChannel { get; set; }

        /// <summary>
        /// 获取加热器中当前的真实最低要求流量。
        /// </summary>
        public double CurrentRequirMinFlow
        {
            get
            {
                double minFlow = 0;
                foreach (var heater in Heaters)
                {
                    if (heater.HeaterIsAuto)
                    {
                        minFlow += HS_HeaterContrller.HeaterTrueRequirMinFlow;
                    }
                }
                return minFlow;
            }
        }

        #endregion

        #region Operators
        // 加热条件。
        private bool TempExecutePridicate(object excuter, ref double val)
        {
            // 如果当前流量少于最低要求流量则停止电炉的运行，并不执行本次。
            bool be = true;
            if (FlowChannel.MeasureValue < CurrentRequirMinFlow)
            {
                StopHeatersOutput();
                be = false;
            }

            return be;
        }

        private void HS_TempreatureExecute_ExecuteChanged(object sender, double executedVal)
        {
            foreach (var hCh in HeaterChannels)
            {
                hCh.AOValue = executedVal;
                hCh.ControllerExecute();
            }
        }

        private void HS_TempreatureExecute_UpdateFedback(IDataFeedback sender)
        {
            sender.FedbackData = (TargetTempChannel as IAnalogueMeasure)?.MeasureValue ?? 0;
        }
        /// <summary>
        /// 从已添加的加热器通道添加电炉集合。
        /// </summary>
        public void HeatersFromChannels()
        {
            if (Heaters.Count == 0)
            {
                foreach (var ch in HeaterChannels)
                {
                    var exe = ch.Controller as HS_ElectricHeaterExecuter;
                    System.Diagnostics.Debug.Assert(exe != null);
                    Heaters.Add(exe.Heater);
                }
            }            
        }
        /// <summary>
        /// 停止所有电炉的输出运行。
        /// </summary>
        public void StopHeatersOutput()
        {
            foreach (var hCh in HeaterChannels)
            {
                hCh.StopControllerExecute();
            }
        }
        #endregion

        #region Overrides

        protected override bool OnExecute(ref double eVal)
        {
            if (ExecutePredicate?.Invoke(this, ref eVal) != false && base.OnExecute(ref eVal))
            {
                return true;
            }
            return false;
        }

        #endregion
    }
}
