﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LabMCESystem.LabElement;
using LabMCESystem.BaseService;
using System.Xml.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

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
        /// 获取/设置环散系统测控设备对象
        /// </summary>
        public HS_MeasCtrlDevice HS_Device
        {
            get { return _hs_Device; }
            set { _hs_Device = value; }
        }


        #endregion

        #region Build
        public HS_Server()
        {
            ElementManager = new LabElementManageBase();

            ExpDataExchange = new MesCtrlDataCenterBase();

            ExperimentTasker = new TaskDistributeCenter();

            ExpDataExchange.WriteXml();
            ExpDataExchange.ReadXml();

            _hs_Device = new HS_MeasCtrlDevice();
            _hs_Device.ConnectElementManagerment(ElementManager);
            _hs_Device.ConnectMultipleDevDataExchange(ExpDataExchange);
            _hs_Device.ConnectTaskDistributeCenter(ExperimentTasker);

            if (_hs_Device.RegistMain())
            {
                _hs_Device.LoginMain();
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("系统创建连接设备失败！");
            }

            InitialExpArea();

        }

        #endregion

        #region Operators

        /// <summary>
        /// 初始化实验段与测试点
        /// </summary>
        private void InitialExpArea()
        {
            // 一冷
            ExperimentArea area = new ExperimentArea("一冷");

            area.AddElement(new ExperimentPoint("一冷空气流量") { Unit = "Kg/h" });


            area.AddElement(new ExperimentPoint("一冷空气进口温度") { Unit = "℃" });
            area.AddElement(new ExperimentPoint("一冷空气出口温度") { Unit = "℃" });
            area.AddElement(new ExperimentPoint("一冷空气进口压力") { Unit = "KPa" });
            area.AddElement(new ExperimentPoint("一冷空气出口压力") { Unit = "KPa" });

            ElementManager?.RegistNewExpArea(area);

            // 为测试点配对通道
            area.GetElementAsLabel("一冷空气流量").PairedChannel = ElementManager.Devices.First().GetElementAsLabel("FT0103");
            area.GetElementAsLabel("一冷空气进口温度").PairedChannel = ElementManager.Devices.First().GetElementAsLabel("TT01");
            area.GetElementAsLabel("一冷空气出口温度").PairedChannel = ElementManager.Devices.First().GetElementAsLabel("TT03");
            area.GetElementAsLabel("一冷空气进口压力").PairedChannel = ElementManager.Devices.First().GetElementAsLabel("PT01");
            area.GetElementAsLabel("一冷空气出口压力").PairedChannel = ElementManager.Devices.First().GetElementAsLabel("PT03");

            // 二冷
            area = new ExperimentArea("二冷");

            area.AddElement(new ExperimentPoint("二冷空气流量") { Unit = "Kg/h" });

            area.AddElement(new ExperimentPoint("二冷空气进口温度") { Unit = "℃" });
            area.AddElement(new ExperimentPoint("二冷空气出口温度") { Unit = "℃" });
            area.AddElement(new ExperimentPoint("二冷空气进口压力") { Unit = "KPa" });
            area.AddElement(new ExperimentPoint("二冷空气出口压力") { Unit = "KPa" });

            ElementManager?.RegistNewExpArea(area);

            // 为测试点配对通道
            area.GetElementAsLabel("二冷空气流量").PairedChannel = ElementManager.Devices.First().GetElementAsLabel("FT0101");
            area.GetElementAsLabel("二冷空气进口温度").PairedChannel = ElementManager.Devices.First().GetElementAsLabel("TT0105");
            area.GetElementAsLabel("二冷空气出口温度").PairedChannel = ElementManager.Devices.First().GetElementAsLabel("TT0108");
            area.GetElementAsLabel("二冷空气进口压力").PairedChannel = ElementManager.Devices.First().GetElementAsLabel("PT0107");
            area.GetElementAsLabel("二冷空气出口压力").PairedChannel = ElementManager.Devices.First().GetElementAsLabel("PT0110");

            //热边
            area = new ExperimentArea("热边");

            area.AddElement(new ExperimentPoint("热边空气流量") { Unit = "Kg/h" });

            area.AddElement(new ExperimentPoint("热边空气进口温度") { Unit = "℃" });
            area.AddElement(new ExperimentPoint("热边空气出口温度") { Unit = "℃" });
            area.AddElement(new ExperimentPoint("热边空气进口压力") { Unit = "KPa" });
            area.AddElement(new ExperimentPoint("热边空气出口压力") { Unit = "KPa" });

            ElementManager?.RegistNewExpArea(area);

            // 为测试点配对通道
            area.GetElementAsLabel("热边空气流量").PairedChannel = ElementManager.Devices.First().GetElementAsLabel("FT0102");
            area.GetElementAsLabel("热边空气进口温度").PairedChannel = ElementManager.Devices.First().GetElementAsLabel("TT0106");
            area.GetElementAsLabel("热边空气出口温度").PairedChannel = ElementManager.Devices.First().GetElementAsLabel("TT0107");
            area.GetElementAsLabel("热边空气进口压力").PairedChannel = ElementManager.Devices.First().GetElementAsLabel("PT0108");
            area.GetElementAsLabel("热边空气出口压力").PairedChannel = ElementManager.Devices.First().GetElementAsLabel("PT0109");
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
        //    using (FileStream fStream = new FileStream(xmlFlie, FileMode.Open, FileAccess.Read))
        //    {
        //        CurrentLabServer = xmlser.Deserialize(fStream) as SingleServer;
        //    }
        //}

        //#endregion
    }
}
