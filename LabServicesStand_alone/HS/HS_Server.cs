using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LabMCESystem.LabElement;
using LabMCESystem.BaseService;
using System.Xml.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using LabMCESystem.EException;

namespace LabMCESystem.Servers.HS
{
    [Serializable]
    public class HS_Server
    {
        #region Properties
        private LabElementManageBase _elementManager;
        /// <summary>
        /// 获取/设置实验室元素管理器
        /// </summary>
        public LabElementManageBase ElementManager
        {
            get { return _elementManager; }
            set { _elementManager = value; }
        }

        private MesCtrlDataCenterBase _expDataExchange;
        /// <summary>
        /// 获取/设置数据实验测控数据交互对象
        /// </summary>
        public MesCtrlDataCenterBase ExpDataExchange
        {
            get { return _expDataExchange; }
            set { _expDataExchange = value; }
        }

        private TaskDistributeCenter _expTasker;
        /// <summary>
        /// 获取/设置实验任务发布器
        /// </summary>
        public TaskDistributeCenter ExperimentTasker
        {
            get { return _expTasker; }
            set { _expTasker = value; }
        }


        private HS_MeasCtrlDevice _hs_Device;
        /// <summary>
        /// 获取环散系统测控设备对象
        /// </summary>
        public HS_MeasCtrlDevice HS_Device
        {
            get { return _hs_Device; }
            private set { _hs_Device = value; }
        }

        public EExceptionManager ExcepManager { get; private set; }

        #endregion

        #region Build
        public HS_Server()
        {
            ElementManager = new LabElementManageBase();

            ExpDataExchange = new MesCtrlDataCenterBase();

            ExperimentTasker = new TaskDistributeCenter();

            ExcepManager = new EExceptionManager();

            ExpDataExchange.WriteXml();
            ExpDataExchange.ReadXml();

            _hs_Device = new HS_MeasCtrlDevice();
            _hs_Device.ConnectElementManager(ElementManager);
            _hs_Device.ConnectMultipleDevDataExchange(ExpDataExchange);
            _hs_Device.ConnectTaskDistributeCenter(ExperimentTasker);
            _hs_Device.ConnectEExceptionManager(ExcepManager);

            if (_hs_Device.RegistMain())
            {
                _hs_Device.LoginMain();
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("系统创建连接设备失败！");
            }

            InitialExpArea();

            // Deivce 开始工作。
            _hs_Device.Run();
        }

        #endregion

        #region Operators

        /// <summary>
        /// 关闭服务。
        /// </summary>
        public void Close()
        {
            _hs_Device.Stop();
            System.Threading.Thread.Sleep(100);
            _hs_Device.Close();
        }

        /// <summary>
        /// 初始化实验段与测试点
        /// </summary>
        private void InitialExpArea()
        {
            // 一冷
            ExperimentalArea area = new ExperimentalArea("一冷");
            
            area.CreatePointIn("一冷空气流量");
            area.CreatePointIn("一冷空气进口温度");
            area.CreatePointIn("一冷空气出口温度");
            area.CreatePointIn("一冷空气进口压力");
            area.CreatePointIn("一冷空气出口压力");
            area.CreatePointIn("一冷压差");
            area.CreatePointIn("散热效率");
            area.CreatePointIn("一冷吸热量");

            ElementManager?.RegistNewExpArea(area);

            // 为测试点配对通道
            area.GetElementAsLabel("一冷空气流量").PairedChannel = ElementManager.Devices.First().GetElementAsLabel("FT0103");
            area.GetElementAsLabel("一冷空气进口温度").PairedChannel = ElementManager.Devices.First().GetElementAsLabel("TT01");
            area.GetElementAsLabel("一冷空气出口温度").PairedChannel = ElementManager.Devices.First().GetElementAsLabel("TT05");
            area.GetElementAsLabel("一冷空气进口压力").PairedChannel = ElementManager.Devices.First().GetElementAsLabel("PT01");
            area.GetElementAsLabel("一冷空气出口压力").PairedChannel = ElementManager.Devices.First().GetElementAsLabel("PT05");
            area.GetElementAsLabel("一冷压差").PairedChannel = ElementManager.Devices.First().GetElementAsLabel("YL_PRESSUREDIFF");
            area.GetElementAsLabel("散热效率").PairedChannel = ElementManager.Devices.First().GetElementAsLabel("HEAT_EMISS_EFFIC");
            area.GetElementAsLabel("一冷吸热量").PairedChannel = ElementManager.Devices.First().GetElementAsLabel("YL_HEAT");
            // 二冷
            area = new ExperimentalArea("二冷");
            
            area.CreatePointIn("二冷空气流量");
            area.CreatePointIn("二冷空气进口温度");
            area.CreatePointIn("二冷空气出口温度");
            area.CreatePointIn("二冷空气进口压力");
            area.CreatePointIn("二冷空气出口压力");

            area.CreatePointIn("二冷压差");
            area.CreatePointIn("二冷吸热量");
            ElementManager?.RegistNewExpArea(area);

            // 为测试点配对通道
            area.GetElementAsLabel("二冷空气流量").PairedChannel = ElementManager.Devices.First().GetElementAsLabel("FT0101");
            area.GetElementAsLabel("二冷空气进口温度").PairedChannel = ElementManager.Devices.First().GetElementAsLabel("TT0105");
            area.GetElementAsLabel("二冷空气出口温度").PairedChannel = ElementManager.Devices.First().GetElementAsLabel("TT0108");
            area.GetElementAsLabel("二冷空气进口压力").PairedChannel = ElementManager.Devices.First().GetElementAsLabel("PT0107");
            area.GetElementAsLabel("二冷空气出口压力").PairedChannel = ElementManager.Devices.First().GetElementAsLabel("PT0110");
            area.GetElementAsLabel("二冷压差").PairedChannel = ElementManager.Devices.First().GetElementAsLabel("EL_PRESSUREDIFF");
            area.GetElementAsLabel("二冷吸热量").PairedChannel = ElementManager.Devices.First().GetElementAsLabel("EL_HEAT");

            //热边
            area = new ExperimentalArea("热边");
            
            area.CreatePointIn("热边空气流量");
            area.CreatePointIn("热边空气进口温度");
            area.CreatePointIn("热边空气出口温度");
            area.CreatePointIn("热边空气进口压力");
            area.CreatePointIn("热边空气出口压力");
            area.CreatePointIn("热边散热量");
            area.CreatePointIn("热边压差");

            ElementManager?.RegistNewExpArea(area);

            // 为测试点配对通道
            area.GetElementAsLabel("热边空气流量").PairedChannel = ElementManager.Devices.First().GetElementAsLabel("FT0102");
            area.GetElementAsLabel("热边空气进口温度").PairedChannel = ElementManager.Devices.First().GetElementAsLabel("TT0106");
            area.GetElementAsLabel("热边空气出口温度").PairedChannel = ElementManager.Devices.First().GetElementAsLabel("TT0107");
            area.GetElementAsLabel("热边空气进口压力").PairedChannel = ElementManager.Devices.First().GetElementAsLabel("PT0108");
            area.GetElementAsLabel("热边空气出口压力").PairedChannel = ElementManager.Devices.First().GetElementAsLabel("PT0109");
            area.GetElementAsLabel("热边压差").PairedChannel = ElementManager.Devices.First().GetElementAsLabel("RL_PRESSUREDIFF");
            area.GetElementAsLabel("热边散热量").PairedChannel = ElementManager.Devices.First().GetElementAsLabel("RL_HEAT");
        }

        #endregion

        //#region Static
        //private static SingleServer s_service = new SingleServer();

        //public static SingleServer CurrentLabServer
        //{
        //    get
        //    {
        //        return s_service;
        //    }
        //    set
        //    {
        //        s_service = value;
        //    }
        //}

        //public static LabElementManageBase CurManager
        //{
        //    get { return s_service.ElementManager; }
        //}

        ///// <summary>
        ///// Serialize this CurrentLabServer to xml file.
        ///// </summary>
        ///// <param name="xmlFile">Dest file path.</param>
        ///// <returns>Successed</returns>
        //public static void CurSrvSerializeXml(string xmlFile = @".\SingleService.xml")
        //{
        //    //XmlSerializer xmlser = new XmlSerializer(typeof(ExperimentPoint));
        //    BinaryFormatter xmlser = new BinaryFormatter();
        //    using (FileStream fStream = new FileStream(xmlFile, FileMode.OpenOrCreate, FileAccess.ReadWrite))
        //    {
        //        xmlser.Serialize(fStream, CurrentLabServer);
        //    }
        //}

        ///// <summary>
        ///// Deserialize current single service from a xml file.
        ///// </summary>
        ///// <param name="xmlFlie">Xml file path.</param>
        //public static void CurSrvDeSerilazeXml(string xmlFlie = @".\SingleService.xml")
        //{
        //    //XmlSerializer xmlser = new XmlSerializer(typeof(SingleServer));
        //    BinaryFormatter xmlser = new BinaryFormatter();
        //    using (FileStream fStream = new FileStream(xmlFlie, FileMode.ToHigh, FileAccess.Read))
        //    {
        //        CurrentLabServer = xmlser.Deserialize(fStream) as SingleServer;
        //    }
        //}

        //#endregion
    }
}
