using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LabMCESystem.LabElement;
using LabMCESystem.BaseService;

namespace LabMCESystem.ETask
{
    /// <summary>
    /// 控制设定项
    /// 通过通道关键码确定所需要通道，并执行通道任务目标值。
    /// </summary>
    [Serializable]
    public struct TaskSetter
    {
        /// <summary>
        /// 获取/设置任务相关的通道关键码
        /// </summary>
        public int ChannelKeyCode { get; set; }

        /// <summary>
        /// 任务项通道设定目标
        /// </summary>
        public double TargetValue { get; set; }

        public TaskSetter(int chKeyCode, double targetVal)
        {
            ChannelKeyCode = chKeyCode;
            TargetValue = targetVal;
        }
    }

    /// <summary>
    /// 通过通道实现的控制设定项。
    /// </summary>
    [Serializable]
    public class ChannelSetter : IOwnTaskSetter
    {
        private Channel _channel;
        /// <summary>
        /// 获取/设置指定项通道。
        /// </summary>
        public Channel Channel
        {
            get
            {
                return _channel;
            }
            private set
            {
                _channel = value;
            }
        }

        private TaskSetter _setter;
        /// <summary>
        /// 获取/设置指定项的设备器。
        /// </summary>
        public TaskSetter Setter
        {
            get
            {
                return _setter;
            }
            set
            {
                _setter = value;
            }
        }

        /// <summary>
        /// 为通道指定目标值创建。
        /// </summary>
        /// <param name="ch">指定通道</param>
        /// <param name="targetVal">设置目标值</param>
        public ChannelSetter(Channel ch, double targetVal) : this(ch, new TaskSetter(/*ch.KeyCode*/1, targetVal))
        {

        }
        /// <summary>
        /// 通道指定TaskSetter指定项创建。
        /// </summary>
        /// <param name="ch">指定通道</param>
        /// <param name="setter">任务指定项</param>
        protected ChannelSetter(Channel ch, TaskSetter setter)
        {
            Channel = ch;
            Setter = setter;
        }

        /// <summary>
        /// 通过指定TaskSetter从IDeviceElementListen服务中得到一个ChannelSetter。
        /// </summary>
        /// <param name="setter">任务指定项</param>
        /// <param name="del">管理服务</param>
        /// <returns>如果setter所指定的通道无效则返回null</returns>
        public static ChannelSetter FromTaskSetter(TaskSetter setter, IDeviceElementListen del)
        {
            AnalogueMeasureChannel ch = del.LookUpChannel(setter.ChannelKeyCode);

            return new ChannelSetter(ch, setter.TargetValue);
        }
        /// <summary>
        /// 通过指定TaskSetter从LabDevice所拥有的通道中中得到一个ChannelSetter。
        /// </summary>
        /// <param name="setter">任务指定项</param>
        /// <param name="dev">设备对象</param>
        /// <returns>如果setter所指定的通道无效则返回null</returns>
        public static ChannelSetter FromTaskSetter(TaskSetter setter, LabDevice dev)
        {
            AnalogueMeasureChannel ch = null;//dev[setter.ChannelKeyCode];

            return new ChannelSetter(ch, setter.TargetValue);
        }
    }

    /// <summary>
    /// 通道试验点实现设定项
    /// </summary>
    [Serializable]
    public class ExpPointSetter : IOwnTaskSetter
    {
        // 通道任务设定项
        private ChannelSetter _chSetter;
        /// <summary>
        /// 获取所配对的通道设定项。
        /// </summary>
        public ChannelSetter PChannelSetter
        {
            get
            {
                return _chSetter;
            }
        }

        private ExperimentalPoint _expPoint;
        /// <summary>
        /// 获取指定项的测试点。
        /// </summary>
        public ExperimentalPoint ExpPoint
        {
            get { return _expPoint; }
            private set
            {
                _expPoint = value;
            }
        }

        public TaskSetter Setter
        {
            get
            {
                return ((IOwnTaskSetter)_chSetter).Setter;
            }

            set
            {
                ((IOwnTaskSetter)_chSetter).Setter = value;
            }
        }

        /// <summary>
        /// 指定测试点对象，与设定目标值创建。
        /// </summary>
        /// <param name="exp">测试点</param>
        /// <param name="targetVal">目标值</param>
        public ExpPointSetter(ExperimentalPoint exp, double targetVal)
        {
            ExpPoint = exp;
            if (exp != null)
            {
                //_chSetter = new ChannelSetter(exp.PairedChannel, targetVal);
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
