using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabElement
{
    /// <summary>
    /// 试验设备服务
    /// 提供试验设备管理方法，数据存储服务
    /// </summary>
    public class LabDevService
    {
        #region static properties

        static private List<LabDevice> samplingDevices = new List<LabDevice>();
        /// <summary>
        /// 获取/设置服务的静态采集设备集合
        /// </summary>
        static public List<LabDevice> SamplingDevices
        {
            get { return samplingDevices; }
            private set { samplingDevices = value; }
        }

        static private List<LabDevice> ctrlProDevices = new List<LabDevice>();
        /// <summary>
        /// 获取/设置服务的静态控制设备集合
        /// </summary>
        static public List<LabDevice> CtrlProDevices
        {
            get { return ctrlProDevices; }
            private set { ctrlProDevices = value; }
        }

        static private List<ExperimentArea> expAreaes;

        /// <summary>
        /// 获取/设置实验段集合
        /// </summary>
        static public List<ExperimentArea> ExpAreaes
        {
            get { return expAreaes; }
            set { expAreaes = value; }
        }

        #endregion

        #region static metods

        /// <summary>
        /// 根据设备编号获取已在采集设备集合中的设备单元
        /// </summary>
        /// <param name="unitNumber">设备编号</param>
        /// <returns></returns>
        static LabDevice GetRegistedSampDev(uint unitNumber)
        {            
            return SamplingDevices.Find(o => o.RegisterCode == unitNumber);            
        }

        /// <summary>
        /// 根据设备编号获取已在控制设备集合中的设备单元
        /// </summary>
        /// <param name="unitNumber">设备编号</param>
        /// <returns></returns>
        static LabDevice GetRegistedCtrlDev(uint unitNumber)
        {
            return ctrlProDevices.Find(o => o.RegisterCode == unitNumber);
        }
        #endregion
    }
}
