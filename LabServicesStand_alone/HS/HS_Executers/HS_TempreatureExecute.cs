using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mcLogic;
using mcLogic.Execute;
using LabMCESystem.LabElement;
using System.Diagnostics;

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
                Ts = 60 * 1000,
                Kp = 0.72,
                Td = 10 * 1000,
                Ti = 1000 * 60 * 30
            }
            )
        {
            Heaters = new List<HS_HeaterContrller>();
            HeaterChannels = new List<FeedbackChannel>();

            UpdateFedback += HS_TempreatureExecute_UpdateFedback;
            ExecuteChanged += HS_TempreatureExecute_ExecuteChanged;
            ExecutePredicate = TempExecutePridicate;
            ExecuteOvered += HS_TempreatureExecute_ExecuteOvered;
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
                        minFlow += heater.HeaterTrueRequirMinFlow;
                    }
                }
                return minFlow;
            }
        }
        /// <summary>
        /// 记录辅助测量温度值样本集合。
        /// </summary>
        private Queue<double> _secTempSamples = new Queue<double>();
        /// <summary>
        /// 获取辅助测量温度的测量标准差。
        /// </summary>
        public double SecTempDeviation { get; private set; } = 0;
        /// <summary>
        /// 温度采样深度。
        /// </summary>
        private const double TempSamplesWidth = 60;
        /// <summary>
        /// 温度的有效稳态标准差。
        /// </summary>
        private const double EfficTempDev = 1.2;
        #endregion

        #region Operators
        /// <summary>
        /// 计算辅助测量温度的测量标准差过程。
        /// </summary>
        private void ProSecTempDeviation()
        {
            if (SecTempChannel != null)
            {
                _secTempSamples.Enqueue(TargetTempChannel.MeasureValue);
                int sfCount = _secTempSamples.Count;
                double fE = 0;
                if (sfCount > 0)
                {
                    if (sfCount > TempSamplesWidth)
                    {
                        _secTempSamples.Dequeue();

                        sfCount--;
                    }
                    fE = _secTempSamples.Average();

                    double tempSum = 0;
                    foreach (var item in _secTempSamples)
                    {
                        tempSum += (item - fE) * (item - fE);
                    }
                    double t_var = tempSum / ((double)sfCount - 1.0);

                    double t_dev = Math.Sqrt(t_var);

                    SecTempDeviation = t_dev;
                }
            }
        }
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
            // 计算温度标准差。
            ProSecTempDeviation();
            // 反馈温度小于目标温度 升温 等温度稳定，否则降温更快。
            if (TargetVal >= TargetTempChannel.MeasureValue)
            {                              
                if (SecTempDeviation > EfficTempDev)
                {
                    var np = Param;
                    np.Ts = 60 * 1000;
                    Param = np;
                    return false;
                }
            }
            else
            {
                var np = Param;
                np.Ts = 30 * 1000;
                Param = np;
            }

            return be;
        }

        private void HS_TempreatureExecute_ExecuteChanged(object sender, double executedVal)
        {
            var tCh = TargetTempChannel as IAnalogueMeasure;
            var sCh = SecTempChannel as IAnalogueMeasure;
            if (tCh != null && sCh != null)
            {
                // 计算热损失温度。
                //double tempMiss = 1 - tCh.MeasureValue / sCh.MeasureValue;
                //executedVal += tempMiss * executedVal;
                foreach (var hCh in HeaterChannels)
                {
                    var exe = hCh.Controller as HS_ElectricHeaterExecuter;

                    Debug.Assert(exe != null && exe.Heater != null);

                    if (exe.Heater.HeaterIsAuto)
                    {
                        hCh.AOValue = executedVal;
                        hCh.ControllerExecute();
                    }
                }
            }
        }

        private void HS_TempreatureExecute_UpdateFedback(IDataFeedback sender)
        {
            sender.FedbackData = (TargetTempChannel/*SecTempChannel*/ as IAnalogueMeasure)?.MeasureValue ?? 0;
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
        
        private void HS_TempreatureExecute_ExecuteOvered(object obj)
        {
            _secTempSamples.Clear();
            SecTempDeviation = 0;
        }

        #endregion

        #region Overrides

        protected override bool OnExecute(ref double eVal)
        {
            bool rb = base.OnExecute(ref eVal);
            if (ExecuteTCount < 1)
            {
                // 在第一次计算控制值时直接输出目标值。                
                eVal = Math.Max(TargetVal, 100);
            }
            return rb;
        }

        #endregion
    }
}
