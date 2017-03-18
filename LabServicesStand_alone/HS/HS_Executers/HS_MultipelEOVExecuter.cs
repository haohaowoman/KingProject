using mcLogic.Execute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabMCESystem.Servers.HS
{
    /// <summary>
    /// 在进行流量PID控制时，是通过电炉前两个电动调节阀进行组合控制，
    /// 根据其口径与主要作用不同，将开度的输出按照一定算法比例输出。
    /// 主控阀的基础开度为50，微调阀从主控阀50开度时开始起调。
    /// </summary>
    class HS_MultipelEOVExecuter : HS_EOVPIDExecuter
    {
        /// <summary>
        /// 指定主调阀与微调阀创建多阀组合控制。
        /// </summary>
        /// <param name="mEOV">主阀。</param>
        /// <param name="fEOV">次阀。</param>
        /// <param name="designMark"></param>
        /// <param name="targetVal"></param>
        /// <exception cref="ArgumentNullException">需要为主控阀和微调阀设置PipeDiameter参数，0为不合法参数。</exception>
        public HS_MultipelEOVExecuter(
            HS_EOVPIDExecuter mEOV,
            HS_EOVPIDExecuter fEOV,
            string designMark,
            double targetVal = 30) : base(designMark, targetVal)
        {
            _fEOV = fEOV;
            _mEOV = mEOV;

            PipeDiameter = _fEOV.PipeDiameter + _mEOV.PipeDiameter;

            if (PipeDiameter == 0)
            {
                throw new ArgumentNullException(nameof(PipeDiameter), "需要为主控阀和微调阀设置PipeDiameter参数，0为不合法参数。");
            }
            if (_mEOV.PipeDiameter >= _fEOV.PipeDiameter)
            {
                //主控阀的默认开度为次控阀门管径一半的对应开度。
                MainEOVDefualt = _fEOV.PipeDiameter * 100.0 / 3.0 / _mEOV.PipeDiameter;
            }
            ExecuteChanged += HS_MultipelEOVExecuter_ExecuteChanged;

            UpdateFedback += HS_MultipelEOVExecuter_UpdateFedback;

            //AutoFinish = false;
        }

        // 主调阀
        private HS_EOVPIDExecuter _mEOV;

        private double _mEOVDefualt = 50;
        /// <summary>
        /// 获取/设置主控调节阀的默认起始开度。
        /// </summary>
        public double MainEOVDefualt
        {
            get { return _mEOVDefualt; }
            set { _mEOVDefualt = value; }
        }


        // 微调阀
        private HS_EOVPIDExecuter _fEOV;

        // 输出将由主 微 调节阀进行实际输出。
        private void HS_MultipelEOVExecuter_ExecuteChanged(object sender, double executedVal)
        {
            if (PipeDiameter == 0)
            {
                return;
            }

            double tMEovOut = 0;
            double tFEovOut = 0;

            double curEov = GetCurrentMultiEovData();
            // 比例计算 管径开度
            double tempPipeDiameter = executedVal * PipeDiameter / SafeRange.Length;

            // 主阀默认开度对应管径。
            double tDefMEovPipeDia = _mEOVDefualt * _mEOV.PipeDiameter / _mEOV.SafeRange.Length;

            //double lPieDia = tempPipeDiameter - tDefMEovPipeDia;
            double lPieDia = tempPipeDiameter - curEov * PipeDiameter / SafeRange.Length;

            double pMParam = _mEOV.PipeDiameter / PipeDiameter;
            double pFParam = _fEOV.PipeDiameter / PipeDiameter;

            // 调节开度小于主阀默认开度是只动作主阀。
            if (tempPipeDiameter <= tDefMEovPipeDia)
            {
                pMParam = 1;
                pFParam = -Math.Sqrt(1 - pMParam * pMParam);
            }
            else
            {
                // 如果打开阀门小于 次条阀门的 1/4 则主要动作次阀门。
                if (lPieDia <= _fEOV.PipeDiameter * pFParam)
                {
                    double tempFk = lPieDia * _fEOV.SafeRange.Length / _fEOV.PipeDiameter;
                    double tnFk = _fEOV.EovFeedbackChannel.MeasureValue + tempFk;

                    if (_fEOV.SafeRange.IsSafeIn(tnFk))
                    {
                        pMParam = pMParam * pFParam;
                        pFParam = 1;
                    }
                    else
                    {
                        pMParam = 1 - pMParam * pMParam;
                        pFParam = -pFParam * pMParam;
                    }
                }
                else
                {
                    pFParam = -Math.Sqrt(1 - pMParam * pMParam);
                    pMParam = 1;
                }

                //if (tDefMEovPipeDia + lPieDia * pMParam > _mEOV.PipeDiameter)
                //{
                //    tMEovOut = _mEOV.SafeRange.Height;
                //    tFEovOut = (tempPipeDiameter - _mEOV.PipeDiameter) * _fEOV.SafeRange.Length / _fEOV.PipeDiameter;
                //}
                //else
                //{
                //    tMEovOut = (tDefMEovPipeDia + lPieDia * pMParam) * _mEOV.SafeRange.Length / _mEOV.PipeDiameter;

                //    tFEovOut = lPieDia * pFParam * _fEOV.SafeRange.Length / _fEOV.PipeDiameter;
                //}
            }
            double tRMEov = (lPieDia * pMParam) * _mEOV.SafeRange.Length / _mEOV.PipeDiameter;
            double tRFEov = (lPieDia * pFParam) * _fEOV.SafeRange.Length / _fEOV.PipeDiameter;

            tMEovOut = _mEOV.EovFeedbackChannel.MeasureValue + tRMEov;
            tFEovOut = _fEOV.EovFeedbackChannel.MeasureValue + tRFEov;
            if (tMEovOut > _mEOV.SafeRange.Height)
            {
                tFEovOut += tMEovOut - _mEOV.SafeRange.Height;
            }            

            tMEovOut = Math.Min(_mEOV.SafeRange.Height, tMEovOut);
            tMEovOut = Math.Max(_mEOV.SafeRange.Low, tMEovOut);

            tFEovOut = Math.Min(_fEOV.SafeRange.Height, tFEovOut);
            tFEovOut = Math.Max(_fEOV.SafeRange.Low, tFEovOut);

            _mEOV.TargetVal = tMEovOut;
            _fEOV.TargetVal = tFEovOut;

            _mEOV.ExecuteBegin();
            _fEOV.ExecuteBegin();

        }

        // 更新反馈值，通过主、微阀进行反馈运算。
        private void HS_MultipelEOVExecuter_UpdateFedback(IDataFeedback sender)
        {
            if (PipeDiameter == 0)
            {
                return;
            }

            // 主阀默认开度对应管径。
            //double tDefMEovPipeDia = _mEOVDefualt * _mEOV.PipeDiameter / _mEOV.SafeRange.Length;

            double tMEovOpenedPipeDia = _mEOV.EovFeedbackChannel.MeasureValue * _mEOV.PipeDiameter / _mEOV.SafeRange.Length;
            double tFEovOpenedPipeDia = _fEOV.EovFeedbackChannel.MeasureValue * _fEOV.PipeDiameter / _fEOV.SafeRange.Length;

            sender.FedbackData = (tMEovOpenedPipeDia + tFEovOpenedPipeDia) * SafeRange.Length / PipeDiameter;

        }

        /// <summary>
        /// 计算获取当前组合阀门换算开度。
        /// </summary>
        /// <returns>计算表示的开度。</returns>
        public double GetCurrentMultiEovData()
        {
            if (PipeDiameter == 0)
            {
                return 0;
            }
            
            double tMEovOpenedPipeDia = _mEOV.EovFeedbackChannel.MeasureValue * _mEOV.PipeDiameter / _mEOV.SafeRange.Length;
            double tFEovOpenedPipeDia = _fEOV.EovFeedbackChannel.MeasureValue * _fEOV.PipeDiameter / _fEOV.SafeRange.Length;

            return (tMEovOpenedPipeDia + tFEovOpenedPipeDia) * SafeRange.Length / PipeDiameter;
        }
    }
}
