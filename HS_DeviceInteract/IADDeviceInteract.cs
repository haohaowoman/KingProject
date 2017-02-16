using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HS_DeviceInteract
{
    /// <summary>
    /// 包含与环散数采箱进行交互的方法。
    /// </summary>
    public interface IADDeviceInteract : IDisposable
    {
        /// <summary>
        /// 获取数采箱48个通道的当前有效数据电流值mA。
        /// 每调用一次此方法才进行数据的更新。
        /// </summary>
        double[] AllChannelsValue { get; }

        /// <summary>
        /// 打开采集设备，在些方法中进行命令的初始化。
        /// </summary>
        /// <returns>如果有设备打开失败返回False。</returns>
        bool OpenDevice();

        /// <summary>
        /// 关闭设备，断开连接，释放所有占用，调用Dispose方法。
        /// </summary>
        void Close();

        /// <summary>
        /// 开始进行数据采集。
        /// </summary>
        void StartAD();

        /// <summary>
        /// 停止数据采集。
        /// </summary>
        void StopAD();

        /// <summary>
        /// 获取8张数据采集卡的连接状态。
        /// </summary>
        bool[] CardsConnection { get; }

        /// <summary>
        /// 当数据采集卡的连接状态改变时发生。
        /// </summary>
        event EventHandler CardConnectionChanged;
    }
}
