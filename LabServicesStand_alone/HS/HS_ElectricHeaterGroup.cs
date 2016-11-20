using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabMCESystem.Servers.HS
{
    class HS_ElectricHeater
    {
        /// <summary>
        /// 获取/设置电炉485通讯命令地址。
        /// </summary>
        public byte CmdAddr { get; set; }

        /// <summary>
        /// 获取/设置电炉是否通讯在线。
        /// </summary>
        public bool IsOnline { get; set; }

        /// <summary>
        /// 获取/设置电炉当前温度。
        /// </summary>
        public double Temperature { get; set; }
    }

    /// <summary>
    /// 环散系统电炉对象。
    /// 包括电炉的开关机状态，子炉数量，子炉通讯地址集合，
    /// 当前运行状态，实现与电炉的测控，实现电炉485通讯，实现电炉故障状态监控。
    /// </summary>
    class HS_ElectricHeaterGroup
    {
        private List<HS_ElectricHeater> _heaters;

        public List<HS_ElectricHeater> Heaters
        {
            get { return _heaters; }
        }

        /// <summary>
        /// 判断组内所有电炉是否已开机在线。
        /// </summary>
        public bool IsAllOpened
        {
            get
            {
                bool b = true;
                foreach (var heater in _heaters)
                {
                    b &= heater.IsOnline;
                }
                return b;
            }
        }

        /// <summary>
        /// 指定电炉个数创建组。
        /// </summary>
        /// <param name="heaterCount">初始化组的电炉个数。</param>
        public HS_ElectricHeaterGroup(int heaterCount = 1)
        {
            _heaters = new List<HS_ElectricHeater>(heaterCount);
        }


    }
}
