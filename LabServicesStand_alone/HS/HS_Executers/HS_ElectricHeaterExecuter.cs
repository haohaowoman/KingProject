using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mcLogic.Execute;
using mcLogic;

namespace LabMCESystem.Servers.HS
{
    /// <summary>
    /// 环散系统 电加热器PID控制执行器
    /// 需要加入启动保护条件 拥有基础流量要求
    /// 温度控制滞后性较高 Ts = 20s
    /// 为保护加热器的使用寿命 Kp = 0.6 Ti = 10s Td = 1 
    /// 电加热器为一个加热组 热边加热器包含5个电炉 二冷边加热组包含2个电炉同时与空气混合
    /// </summary>
    abstract class HS_ElectricHeaterExecuter : PredicatePositionPID
    {
        public HS_ElectricHeaterExecuter(string designMark, HS_MeasCtrlDevice dev) : base(24.0, new SafeRange(0, 1000), new PIDParam() { Ts = 20000, Kp = 0.6, Ti = 10000, Td = 1000})
        {
            HS_Device = dev;
        }

        /// <summary>
        /// 获取/设置所属的设备。
        /// </summary>
        public HS_MeasCtrlDevice HS_Device { get; set; }
        
        /// <summary>
        /// 获取/设置电加热器的最低进入流量
        /// </summary>
        public double RequireMinInFlow { get; set; }
        /// <summary>
        /// 获取/设置电炉组。
        /// </summary>
        public HS_ElectricHeaterGroup Heaters { get; set; }

        /// <summary>
        /// 打开加热器组。
        /// </summary>
        public void OpenHeaters()
        {
            OpenHeatersFlow();
        }
        /// <summary>
        /// 重写此函数以实现加热器打开流程逻辑。
        /// </summary>
        protected abstract void OpenHeatersFlow();

        /// <summary>
        /// 关闭加热器组。
        /// </summary>
        public void CloseHeaters()
        {
            CloseHeatersFlow();
        }
        /// <summary>
        /// 重写此函数以实现加热器的关闭流程。
        /// </summary>
        protected abstract void CloseHeatersFlow();


    }
}
