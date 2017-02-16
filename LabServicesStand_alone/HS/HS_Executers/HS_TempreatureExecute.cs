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
    class HS_TempreatureExecute : ClosedLoopExecuter, IPredicateExecute
    {
        public HS_TempreatureExecute() : base(0, new SafeRange(0, 800))
        {
            UpdateFedback += HS_TempreatureExecute_UpdateFedback;
            ExecuteChanged += HS_TempreatureExecute_ExecuteChanged;
            Heaters = new List<HS_HeaterContrller>();
        }

        #region Properties
        /// <summary>
        /// 获取温度控制器的控制电炉集合。
        /// </summary>
        public List<HS_HeaterContrller> Heaters { get; private set; }
        /// <summary>
        /// 获取/设置需要控制的温度通道。
        /// </summary>
        public Channel TemperatureChannel { get; set; }

        /// <summary>
        /// 获取/设置控制器的执行条件。
        /// </summary>
        public ExecutePredicateEventHandler ExecutePredicate
        {
            get;
            set;
        }

        #endregion

        #region Operators

        private void HS_TempreatureExecute_ExecuteChanged(object sender, double executedVal)
        {
            foreach (var heater in Heaters)
            {
                bool? rb = heater?.SetTemperature(executedVal);
#if DEBUG
                if (rb != true)
                {
                    Console.WriteLine($"Channel {TemperatureChannel?.Label} Heater control fault, heater si {heater.Caption}.");
                }
#endif
            }
        }

        private void HS_TempreatureExecute_UpdateFedback(IDataFeedback sender)
        {
            sender.FedbackData = (TemperatureChannel as IAnalogueMeasure)?.MeasureValue ?? 0;
        }

        #endregion

        #region Overrides

        protected override bool OnExecute(ref double eVal)
        {            
            if (base.OnExecute(ref eVal) && ExecutePredicate?.Invoke(this,ref eVal) != false)
            {
                return true;
            }
            return false;
        }

        #endregion
    }
}
