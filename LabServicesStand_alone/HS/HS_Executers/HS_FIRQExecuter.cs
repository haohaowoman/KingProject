using mcLogic;
using mcLogic.Execute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LabMCESystem.LabElement;
namespace LabMCESystem.Servers.HS
{
    /// <summary>
    /// 实验入口流量PID控制调节。
    /// 根据阀门调节与组合调节的滞后性设定PID参数。
    /// Ts = 4s Kp = 0.78 Ti = 6s Td = 0
    /// 安全范围在0 - 5000kg / h
    /// 公差范围在+-50
    /// 根据电炉需求的最低流量做控制限制。
    /// 不自动停步。
    /// 通过递推算法计算流量对应的组合阀开度。
    /// </summary>
    class HS_FIRQExecuter : PredicatePositionPID
    {
        public HS_FIRQExecuter(string designMark, IAnalogueMeasure targetChannel, double minQ) :
            base(minQ, new SafeRange(0, 6200), new PIDParam()
            {
                Ts = 12000,
                Kp = 0.54,
                Ti = 60000,
                Td = 4000
            })
        {
            AllowTolerance = new Tolerance(50);
            AutoFinish = false;

            DesignMark = designMark;

            MinRequirQ = minQ;

            TargetChannel = targetChannel;

            UpdateFedback += HS_FIRQExecuter_UpdateFedback;

            ExecuteChanged += HS_FIRQExecuter_ExecuteChanged;

            FedbackInTolerance += HS_FIRQExecuter_FedbackInTolerance;

            this.ExecutePredicate = FIRQExecutePredicate;

            ExecuteOvered += HS_FIRQExecuter_ExecuteOvered;
        }

        public double FIRQEovStep = 15;

        /// <summary>
        /// 获取/设置控制目标通道。
        /// </summary>
        public IAnalogueMeasure TargetChannel { get; set; }
        /// <summary>
        /// 获取设置本路流量控制的产品入口阀门通道。
        /// </summary>
        public IFeedback ProductInEovChannel { get; set; }
        /// <summary>
        /// 获取设置本路流量控制的产品出口阀门通道。
        /// </summary>
        public IFeedback ProductOutEovChannel { get; set; }
        /// <summary>
        /// 获取/设置最低要求流量。
        /// </summary>
        public double MinRequirQ { get; set; }

        private HS_MultipelEOVExecuter _mEOVExe;
        /// <summary>
        /// 获取/设置组合控制阀组合。
        /// </summary>
        public HS_MultipelEOVExecuter MulEOVExecuter
        {
            get { return _mEOVExe; }
            set { _mEOVExe = value; }
        }

        /// <summary>
        /// 获取当前流量对应开度的比例系数。        
        /// </summary>
        public double QKParam { get; private set; } = 1;

        private Queue<double> _sampleFirq = new Queue<double>();
        /// <summary>
        /// 获取流量采样内的方差。
        /// </summary>
        public double FirqVariance { get; private set; }
        /// <summary>
        /// 获取流量采样内的标准差。
        /// </summary>
        public double FirqDeviation { get; private set; }

        private Queue<double> _sampleEov = new Queue<double>();
        /// <summary>
        /// 获取调节阀采样内的方差。
        /// </summary>
        public double EovVariance { get; set; }
        /// <summary>
        /// 获取调节阀采样内的方差。
        /// </summary>
        public double EovDeviation { get; set; }
        /// <summary>
        /// 采样深度。
        /// </summary>
        private const int DovSampleWidth = 15;
        /// <summary>
        /// 稳态有效方差。
        /// </summary>
        private const double EfficDeviation = 15;
        /// <summary>
        /// 阀门流量关系采样深度。
        /// </summary>
        public const int RelFirq_EovSampleWidth = 10;
        /// <summary>
        /// 获取流量与阀门开度的协方差，获取阀门开度与流量的关系比值。
        /// </summary>
        public double FirqEovCovariance { get; private set; }
        /// <summary>
        /// 流量关系样点集合。
        /// </summary>
        private Queue<double> _relSampFirq = new Queue<double>();
        /// <summary>
        /// 阀门关系样点集合。
        /// </summary>
        private Queue<double> _relSampEov = new Queue<double>();
        /// <summary>
        /// 获取流量阀门协方差关系P系数。
        /// </summary>
        public double FirqEovCovPParam { get; private set; }
        /// <summary>
        /// 获取上一次控制值。
        /// </summary>
        public double LastExeValue { get; private set; }
        #region Operators

        /// <summary>
        /// 计算流量与阀门反馈方差过程。
        /// </summary>
        private void ProFeedbackDev()
        {
            _sampleFirq.Enqueue(TargetChannel.MeasureValue);
            int sfCount = _sampleFirq.Count;
            double fE = 0;
            if (sfCount > 0)
            {
                if (sfCount > DovSampleWidth)
                {
                    _sampleFirq.Dequeue();

                    sfCount--;
                }
                fE = _sampleFirq.Average();

                double tempSum = 0;
                foreach (var item in _sampleFirq)
                {
                    tempSum += (item - fE) * (item - fE);
                }
                double f_var = tempSum / ((double)sfCount - 1.0);

                double f_dev = Math.Sqrt(f_var);

                FirqVariance = f_var;
                FirqDeviation = f_dev;
            }

            _sampleEov.Enqueue(_mEOVExe.GetCurrentMultiEovData());
            int seCount = _sampleEov.Count;
            double eE = 0;
            if (seCount > 0)
            {
                if (seCount > DovSampleWidth)
                {
                    _sampleEov.Dequeue();

                    seCount--;
                }
                eE = _sampleEov.Average();

                double tempSum = 0;
                foreach (var item in _sampleEov)
                {
                    tempSum += (item - eE) * (item - eE);
                }

                double e_var = tempSum / ((double)seCount - 1.0);

                double e_dev = Math.Sqrt(e_var);

                EovVariance = e_var;
                EovDeviation = e_dev;

            }
        }
        
        /// <summary>
        /// 计算调节流量稳定后的流量阀门开度协方差关系。
        /// </summary>
        private void RelationFirq_EovCovariance()
        {
            // 已经进行足够采样。
            if (_sampleFirq.Count >= RelFirq_EovSampleWidth)
            {
                if (FirqDeviation <= EfficDeviation && EovDeviation <= EfficDeviation)
                {
                    _relSampFirq.Enqueue(TargetChannel.MeasureValue);
                    _relSampEov.Enqueue(_mEOVExe.GetCurrentMultiEovData());

                    int rCount = _relSampFirq.Count;
                    if (rCount > 0)
                    {
                        if (rCount > RelFirq_EovSampleWidth)
                        {
                            _relSampEov.Dequeue();
                            _relSampFirq.Dequeue();
                            rCount--;
                        }

                        var rfsAry = _relSampFirq.ToArray();
                        var resAry = _relSampEov.ToArray();
                        var fe_Ary = new double[rCount];

                        double tSum_f = 0;
                        double tSum_e = 0;
                        double tSum_fe = 0;
                        for (int i = 0; i < rCount; i++)
                        {
                            tSum_f += rfsAry[i];
                            tSum_e += resAry[i];
                            tSum_fe += rfsAry[i] * resAry[i];
                        }

                        double tEf = tSum_f / (double)rCount;
                        double tEe = tSum_e / (double)rCount;
                        double tEfe = tSum_fe / (double)rCount;

                        FirqEovCovariance = tEfe - tEf * tEe;

                        tSum_f = 0;
                        tSum_e = 0;
                        for (int i = 0; i < rCount; i++)
                        {
                            tSum_f += (rfsAry[i] - tEf) * (rfsAry[i] - tEf);
                            tSum_e += (resAry[i] - tEe) * (resAry[i] - tEe);
                        }

                        double f_var = tSum_f / ((double)rCount - 1);
                        double e_var = tSum_e / ((double)rCount - 1);

                        if (f_var == 0 || e_var == 0)
                        {
                            return;
                        }
                        FirqEovCovPParam = FirqEovCovariance / (Math.Sqrt(f_var) * Math.Sqrt(e_var));
                    }
                }
            }
        }

        private void ClearSamples()
        {
            _sampleFirq.Clear();
            _sampleEov.Clear();
            _relSampFirq.Clear();
            _relSampEov.Clear();
            FirqDeviation = FirqVariance = EovDeviation = EovVariance = FirqEovCovariance = 0;
        }

        private bool FIRQExecutePredicate(object sender, ref double eVal)
        {
            if (ProductInEovChannel != null && ProductInEovChannel.MeasureValue < 95)
            {
                ProductInEovChannel.AOValue = 100;
                ProductInEovChannel.ControllerExecute();
                return false;
            }
            if (ProductOutEovChannel != null && ProductOutEovChannel.MeasureValue < 95)
            {
                ProductOutEovChannel.AOValue = 100;
                ProductOutEovChannel.ControllerExecute();
                return false;
            }

            if (TargetChannel != null && _mEOVExe != null)
            {
                // 当阀门有10以上的开度是 流量取未反应 则不满足执行条件。
                if (_mEOVExe.GetCurrentMultiEovData() >= (FIRQEovStep - 2) && TargetChannel.MeasureValue < 100)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            ProFeedbackDev();
#if DEBUG
            Console.WriteLine($"/*********Firq channel {DesignMark}.**********/");
            Console.WriteLine($"Firq variance is {FirqVariance:F3}, firq deviation is {FirqDeviation:F3}, eov variance is {EovVariance:F3}, eov deviation is {EovDeviation:F3}.");
            Console.WriteLine($"Covariance of Firq with Eov is {FirqEovCovariance:F3}, p param is {FirqEovCovPParam:F3}.");
#endif
            // 流量方差>10 反应流量震荡较大。
            if (FirqDeviation > EfficDeviation)
            {
#if DEBUG
                Console.WriteLine($"/*************Firq execute fault.****************/");
#endif
                return false;
            }

            return true;
        }

        private void HS_FIRQExecuter_FedbackInTolerance(object sender, double e)
        {

        }

        // 更新目标通道的反馈。
        private void HS_FIRQExecuter_UpdateFedback(IDataFeedback sender)
        {
            sender.FedbackData = TargetChannel?.MeasureValue ?? 0;
        }

        // 执行输出。
        private void HS_FIRQExecuter_ExecuteChanged(object sender, double executedVal)
        {
            if (TargetChannel != null && _mEOVExe != null)
            {

                RelationFirq_EovCovariance();
                double eovFv = _mEOVExe.GetCurrentMultiEovData();

                double exeDrt = executedVal - LastExeValue;
                if (ExecuteTCount <= 1)
                {
                    exeDrt = executedVal - FedbackData;
                }

                if (FedbackData != 0)
                {
                    if (executedVal != 0 && eovFv <= 0)
                    {
                        // 排除刚运行状态下的输出和反馈同时为零。
                        QKParam = FIRQEovStep;
                    }
                    else
                    {
                        // y = a * x + b; b = 0;
                        //double kparam = FedbackData / eovFv;

                        //if (ExecuteTCount > 1 && 0.5 <= FirqEovCovPParam && FirqEovCovPParam < 1)
                        //{
                        //    //
                        //    QKParam = eovFv + exeDrt * Math.Sqrt(1 - FirqEovCovPParam * FirqEovCovPParam) / kparam;
                        //}
                        //else
                        //{
                        if (eovFv >= 5 && FedbackData >= 100)
                        {
                            // 阀门特性关系cv(%) = 3.127* e^(0.036x)；
                            double tempCv = 3.127 * Math.Pow(Math.E, 0.036 * eovFv);
                            double tempMaxFirq = FedbackData / (tempCv / 100.0);
                            double tempFirqOut = FedbackData + exeDrt;
                            double tempEovOut = Math.Log(tempFirqOut * 100.0 / tempMaxFirq / 3.127) / 0.036;
                            QKParam = tempEovOut;
                            // 
                            if (ExecuteTCount <= 1 && AllowTolerance.IsInTolerance(TargetVal, FedbackData))
                            {
                                QKParam = eovFv;
                            }
                        }
                        else
                        {
                            double kparam = FedbackData / eovFv;
                            QKParam = executedVal / kparam;
                        }
                        //}  
                    }
                }
                else
                {
                    QKParam = FIRQEovStep;
                }

                LastExeValue = executedVal;

                double dec = QKParam - eovFv;

                if (FirqEovCovPParam > 0 && FirqEovCovPParam < 1)
                {
                    // 协方差>0，流量 阀门呈正相关性。
                    dec += (1 - FirqEovCovPParam * FirqEovCovPParam) * dec;
                }
                else if (FirqEovCovPParam < 0 && FirqEovCovPParam > -1)
                {
                    //如果协方差<0，流量 阀门呈负相关性，则气源不足，或者气源补气过快。
                    dec +=  Math.Sqrt(1 - FirqEovCovPParam * FirqEovCovPParam) * dec;
                }

                // 限制阀门第一次最大调节步进为15%，往后为20%。
                //if (ExecuteTCount >1)
                //{
                //    FIRQEovStep = 20;
                //}
                dec = dec >= 0 ? Math.Min(dec, FIRQEovStep) : Math.Max(dec, -FIRQEovStep);

                QKParam = eovFv + dec;

                double Keov = QKParam;

                if (_mEOVExe.SafeRange.IsSafeIn(Keov))
                {
                    _mEOVExe.TargetVal = Keov;
                }
                else
                {
                    _mEOVExe.TargetVal = _mEOVExe.SafeRange.Height;
                }

                _mEOVExe.ExecuteBegin();
            }
        }

        private void HS_FIRQExecuter_ExecuteOvered(object obj)
        {
            ClearSamples();
        }

        #endregion

        #region Override

        public override void Reset()
        {
            ClearSamples();
            base.Reset();
        }

        #endregion
    }
}
