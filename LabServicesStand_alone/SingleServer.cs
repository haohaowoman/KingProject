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

namespace LabMCESystem.Servers
{
    [Serializable]
    public class SingleServer
    {
        #region Properties
        private LabElementManageBase _elementManager;

        public LabElementManageBase ElementManager
        {
            get { return _elementManager; }
            set { _elementManager = value; }
        }

        private MesCtrlDataCenterBase _expDataExchange;

        public MesCtrlDataCenterBase ExpDataExchange
        {
            get { return _expDataExchange; }
            set { _expDataExchange = value; }
        }

        #endregion

        #region Build
        public SingleServer()
        {
            ElementManager = new LabElementManageBase();

            ExpDataExchange = new MesCtrlDataCenterBase();

            ExpDataExchange.WriteXml();
            ExpDataExchange.ReadXml();
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
