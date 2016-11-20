using LabMCESystem.Logic.Execute;
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
            double targetVal = 30):base(designMark, targetVal)
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

            AutoFinish = false;
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

            double tk = _mEOVDefualt / (_mEOV.SafeRange.Hight - _mEOV.SafeRange.Low);
            double tempdia = _mEOV.PipeDiameter * (1.0 - tk);
            double tempAdia = _fEOV.PipeDiameter + tempdia;

            double tempTarget = _mEOVDefualt + executedVal * tempdia / tempAdia;

            _mEOV.TargetVal = tempTarget;

            tempTarget = executedVal * _fEOV.PipeDiameter / tempAdia;

            _mEOV.ExecuteBegin();
            _fEOV.ExecuteBegin();
        }

        // 更新反馈值，通过主、微阀进行反馈运算。
        private void HS_MultipelEOVExecuter_UpdateFedback(IDataFedback sender)
        {
            if (PipeDiameter == 0)
            {
                return;
            }

            double tk = _mEOVDefualt / (_mEOV.SafeRange.Hight - _mEOV.SafeRange.Low);
            double tempdia = _mEOV.PipeDiameter * (1.0 - tk);
            double tempAdia = _fEOV.PipeDiameter + tempdia;

            double km = (_mEOV.FedbackData - _mEOVDefualt) * tempAdia / tempdia;

            double kf = _fEOV.FedbackData * tempAdia / tempAdia;

            sender.FedbackData = km + kf / 2.0;
        }

    }
}
