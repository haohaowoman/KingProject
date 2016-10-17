using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LabMCESystem.LabElement;
namespace LabMCESystem.Task
{
    /// <summary>
    /// 控制设定项
    /// 通过通道关键码确定所需要通道，并执行通道任务目标值
    /// </summary>
    [Serializable]
    public class Setter
    {
        /// <summary>
        /// 获取/设置任务相关的通道关键码
        /// </summary>
        public int ChannelKeyCode { get; protected set; }

        /// <summary>
        /// 任务项通道设定目标
        /// </summary>
        public float TargetValue { get; set; }

        public Setter(int chKeyCode)
        {
            ChannelKeyCode = chKeyCode;
        }
    }

    /// <summary>
    /// 通过通道实现的控制设定项
    /// </summary>
    [Serializable]
    public class ChannelSetter : Setter
    {
        private LabChannel _channel;

        public LabChannel Channel
        {
            get
            {
                return _channel;
            }
            set
            {
                _channel = value;
                if (_channel != null)
                {
                    ChannelKeyCode = _channel.KeyCode;
                }
            }
        }

        public ChannelSetter(LabChannel ch) : base(ch.KeyCode)
        {

        }

        public ChannelSetter() : base(0)
        {

        }

    }

    /// <summary>
    /// 通道试验点实现的控制设定项
    /// </summary>
    [Serializable]
    public class ExpPointSetter : Setter
    {
        private ExperimentPoint _expPoint;

        public ExperimentPoint ExpPoint
        {
            get { return _expPoint; }
            set
            {
                _expPoint = value;
                if ((_expPoint != null) && (_expPoint.PairedChannel != null))
                {
                    ChannelKeyCode = _expPoint.PairedChannel.KeyCode;
                }
            }
        }

        public ExpPointSetter() : base(0)
        {

        }

        public ExpPointSetter(ExperimentPoint exp) : base(0)
        {
            ExpPoint = exp;
        }

        public ChannelSetter ToChnnelSetter()
        {
            if ((_expPoint != null) && (_expPoint.PairedChannel != null))
            {
                return new ChannelSetter(_expPoint.PairedChannel) { TargetValue = TargetValue };
            }
            else
            {
                return null;
            }
        }
    }

    /// <summary>
    /// 任务结果集
    /// </summary>
    [Serializable]
    public class Result
    {
        /// <summary>
        /// 获取/设置任务相关的通道关键码
        /// </summary>
        public int ChannelKeyCode { get; set; }

        /// <summary>
        /// 任务项通道设定目标
        /// </summary>
        public float Value { get; set; }
    }


}
