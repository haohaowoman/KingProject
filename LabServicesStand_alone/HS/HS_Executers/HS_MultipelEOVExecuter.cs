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
        /// <param name="mEOV">主阀</param>
        /// <param name="fEOV">微调阀</param>
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

            //修改PID参数设置
            PIDParam p = PIDParam.PExecuterParam;
            p.Ts = 2000;

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

            // 比例计算 管径开度
            double tempPipeDiameter = executedVal * PipeDiameter / SafeRange.Length;

            // 主阀默认开度对应管径。
            double tDefMEovPipeDia = _mEOVDefualt * _mEOV.PipeDiameter / _mEOV.SafeRange.Length;

            // 调节开度小于主阀默认开度是只动作主阀。
            if (tempPipeDiameter <= tDefMEovPipeDia)
            {
                tMEovOut = tempPipeDiameter * _mEOV.SafeRange.Length / _mEOV.PipeDiameter;
                tFEovOut = 0;
            }
            else
            {
                double lPieDia = tempPipeDiameter - tDefMEovPipeDia;
                double pMParam = _mEOV.PipeDiameter / PipeDiameter;
                double pFParam = _fEOV.PipeDiameter / PipeDiameter;

                if (tDefMEovPipeDia + lPieDia * pMParam > _mEOV.PipeDiameter)
                {
                    tMEovOut = _mEOV.SafeRange.Height;
                    tFEovOut = (tempPipeDiameter - _mEOV.PipeDiameter) * _fEOV.SafeRange.Length / _fEOV.PipeDiameter;
                }
                else
                {
                    tMEovOut = (tDefMEovPipeDia + lPieDia * pMParam) * _mEOV.SafeRange.Length / _mEOV.PipeDiameter;

                    tFEovOut = lPieDia * pFParam * _fEOV.SafeRange.Length / _fEOV.PipeDiameter;
                }

            }

            if (!_mEOV.SafeRange.IsSafeIn(tMEovOut))
            {
                tMEovOut = _mEOV.SafeRange.Height;
            }
            if (!_fEOV.SafeRange.IsSafeIn(tFEovOut))
            {
                tFEovOut = _mEOV.SafeRange.Height;
            }

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
            double tDefMEovPipeDia = _mEOVDefualt * _mEOV.PipeDiameter / _mEOV.SafeRange.Length;

            double tMEovOpenedPipeDia = _mEOV.FedbackData * _mEOV.PipeDiameter / _mEOV.SafeRange.Length;
            double tFEovOpenedPipeDia = _fEOV.FedbackData * _fEOV.PipeDiameter / _fEOV.SafeRange.Length;

            sender.FedbackData = (tMEovOpenedPipeDia + tFEovOpenedPipeDia) * SafeRange.Length / PipeDiameter;

        }

        /// <summary>
        /// 计算获取当前组合阀门换算开度。
        /// </summary>
        /// <returns></returns>
        public double GetCurrentMultiEovData()
        {
            if (PipeDiameter == 0)
            {
                return 0;
            }

            // 主阀默认开度对应管径。
            double tDefMEovPipeDia = _mEOVDefualt * _mEOV.PipeDiameter / _mEOV.SafeRange.Length;

            double tMEovOpenedPipeDia = _mEOV.FedbackData * _mEOV.PipeDiameter / _mEOV.SafeRange.Length;
            double tFEovOpenedPipeDia = _fEOV.FedbackData * _fEOV.PipeDiameter / _fEOV.SafeRange.Length;

            return (tMEovOpenedPipeDia + tFEovOpenedPipeDia) * SafeRange.Length / PipeDiameter;
        }
    }
}
