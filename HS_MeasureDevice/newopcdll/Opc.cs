using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpcRcw.Da;
using OpcRcw.Comn;
using OPCSiemensDAAutomation;
using System.Collections;
using System.Runtime.InteropServices;

using System.Windows.Forms;
using HS_DeviceInteract;
namespace newopcdll
{
    //用于opc的连接，关闭，读取，写入接口
    public class Opc : IOPCDInteract
    {
        OpcRcw.Da.IOPCServer ServerObj;     //定义OPCServer 对象
        OpcRcw.Da.IOPCSyncIO IOPCSyncIO2Obj = null;       //同步读对象
        OpcRcw.Da.IOPCGroupStateMgt IOPCGroupStateMgtObj = null;        //管理OPCGroup组对象
        internal const int LOCALE_ID = 0x407;         //OPCServer语言码-英语
        Object MyobjGroup = null;            //OPCGroup对象
        //object myobjGroup = null;
        int[] ItemServerHandle;              //Item句柄数组
        int pSvrGroupHandle = 0;            //OPCGroup 句柄

        string serverName;
        string ip;

        //定义一个字典容器，将要 组变量 存入其中
        Dictionary<string, object> _groupObj = new Dictionary<string, object>();
        //1.18 定义一个字典 将sever 句柄存入其中
        Dictionary<string, int> _serverobj = new Dictionary<string, int>();

        Dictionary<string, int[]> _goroupsItems = new Dictionary<string, int[]>();
        public Opc()                 //默认构造函数
        {

            serverName = "OPC.SimaticNet";
            ip = "192.168.0.7";

        }
        public Opc(string servername, string ip)
        {
            this.serverName = servername;
            this.ip = ip;
        }


        public bool Open(out string error)
        {

            error = "";
            bool success = true;
            Type svrComponenttyp; //获取 opc server Com 接口
            try
            {
                svrComponenttyp = Type.GetTypeFromProgID(serverName, ip); //opcSever
                ServerObj = (OpcRcw.Da.IOPCServer)Activator.CreateInstance(svrComponenttyp); //注册s
            }
            catch (System.Exception err)
            {
                error = "错误信息：" + err.Message;
                success = false;
            }
            return success;
        }

        public void Close()
        {
            try
            {
                if (IOPCSyncIO2Obj != null)
                {
                    Marshal.ReleaseComObject(IOPCSyncIO2Obj);
                    IOPCSyncIO2Obj = null;
                }               
                //1.18 end

                if (IOPCGroupStateMgtObj != null)
                {
                    Marshal.ReleaseComObject(IOPCGroupStateMgtObj);
                    IOPCGroupStateMgtObj = null;
                }
                if (MyobjGroup != null)
                {
                    Marshal.ReleaseComObject(MyobjGroup);
                    MyobjGroup = null;
                }
                if (ServerObj != null)
                {
                    Marshal.ReleaseComObject(ServerObj);
                    ServerObj = null;
                }
            }
            catch (System.Exception error)
            {
                MessageBox.Show(error.Message, "Result - Stop Server", MessageBoxButtons.OK,
                MessageBoxIcon.Error);
            }
        }

        public bool AddGroup(string GroupName, out string error)
        {
            error = "";
            int LOCALE_ID = 0x407; //本地语言为英语

            float deadband = 0;
            Int32 dwRequestedUpdateRate = 1000;//订阅读取速度
            Int32 hClientGroup = 1;
            int TimeBias = 0;
            int pSvrGroupHandle = 0;            //OPCGroup 句柄
            Int32 pRevUpdateRate;
            //处理非托管COM内存

            GCHandle hTimeBias, hDeadband;
            hTimeBias = GCHandle.Alloc(TimeBias, GCHandleType.Pinned);
            hDeadband = GCHandle.Alloc(deadband, GCHandleType.Pinned);
            Guid iidRequiredInterface = typeof(IOPCItemMgt).GUID;

            try
            {
                //ServerObj.AddGroup("MyOPCGroup1", //组名
                ServerObj.AddGroup(GroupName, //组名
                    0, //创建时，组是否被激活
                    dwRequestedUpdateRate, //组的刷新频率，以ms为单位
                    hClientGroup,
                    hTimeBias.AddrOfPinnedObject(),// 客户号
                    hDeadband.AddrOfPinnedObject(), //没用到
                    LOCALE_ID, //本地语言
                    out pSvrGroupHandle, //移去组时，用到的组ID号
                    out pRevUpdateRate, //返回组中的变量改变时的最短通知时间间隔
                    ref iidRequiredInterface,
                    out MyobjGroup); //指向要求的接口

                _groupObj.Add(GroupName, MyobjGroup);
                _serverobj.Add(GroupName, pSvrGroupHandle);
            }
            catch (System.Exception err)
            {
                error = "错误信息：" + err.Message;
            }
            finally
            {
                if (hDeadband.IsAllocated) hDeadband.Free();
            }
            if (error == "")
            {
                return true;
            }
            else
                return false;
        }

        public void AddItem(string GroupName, string[] itemsName)
        {

            OpcRcw.Da.OPCITEMDEF[] ItemArray;


            object io;

            if (true == _groupObj.TryGetValue(GroupName, out io))
            {

                IOPCSyncIO2Obj = (IOPCSyncIO)io;
                //Query interface for sync calls on group object;
                IOPCGroupStateMgtObj = (IOPCGroupStateMgt)io;
                //定义读写的item，共计多少个变量
            }

            ItemArray = new OPCITEMDEF[itemsName.Length];
            for (int i = 0; i < itemsName.Length; i++)
            {
                ItemArray[i].szAccessPath = "";

                ItemArray[i].szItemID = "S71500ET200MP station_1.PLC_1." + itemsName[i];
                //地址，不同数据类型表示方法不同。
                ItemArray[i].bActive = 1;  //是否激活
                ItemArray[i].hClient = 1; //表示id
                ItemArray[i].dwBlobSize = 0;
                ItemArray[i].pBlob = IntPtr.Zero;
                ItemArray[i].vtRequestedDataType = 5;
                //11 代表 bool  byte 17 decimal 14 double 5 integer 2 long 3 single 4 string 8
            }


            IntPtr pResults = IntPtr.Zero;
            IntPtr pErrors = IntPtr.Zero;


            try
            {
                ///注意这里有个数字 是代表那个分组的数字
                ((OpcRcw.Da.IOPCItemMgt)io).AddItems(itemsName.Length, ItemArray, out pResults, out pErrors);
                int[] errors = new int[itemsName.Length];
                IntPtr pos = pResults;

                var items = new int[itemsName.Length];
                
                ItemServerHandle = new int[itemsName.Length];
                Marshal.Copy(pErrors, errors, 0, itemsName.Length);

                for (int i = 0; i < itemsName.Length; i++)
                {
                    if (i == 0)
                    {
                        OPCITEMRESULT result = (OPCITEMRESULT)Marshal.PtrToStructure(pos, typeof(OPCITEMRESULT));
                        ItemServerHandle[0] = result.hServer;

                        items[0] = result.hServer;
                    }
                    else
                    {
                        pos = new IntPtr(pos.ToInt32() + Marshal.SizeOf(typeof(OPCITEMRESULT)));
                        OPCITEMRESULT result = (OPCITEMRESULT)Marshal.PtrToStructure(pos, typeof(OPCITEMRESULT));
                        ItemServerHandle[i] = result.hServer;

                        items[i] = result.hServer;
                        ////1.17 16:53
                    }
                }
                _goroupsItems.Add(GroupName, items);
            }
            catch (System.Exception err) // catch for add items
            {
                //MessageBox.Show(err.Message, "Result - Adding Items",
                //MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            finally
            {
                // Free the memory
                if (pResults != IntPtr.Zero)
                {
                    Marshal.FreeCoTaskMem(pResults);
                    pResults = IntPtr.Zero;
                }
                if (pErrors != IntPtr.Zero)
                {
                    Marshal.FreeCoTaskMem(pErrors);
                    pErrors = IntPtr.Zero;
                }
            }


        }


        private DateTime ToDateTime(OpcRcw.Da.FILETIME ft)
        {
            long highbuf = (long)ft.dwHighDateTime;
            long buffer = (highbuf << 32) + ft.dwLowDateTime;
            return DateTime.FromFileTimeUtc(buffer);
        }

        private String GetQuality(long wQuality)
        {

            String strQuality = "";
            switch (wQuality)
            {

                case Qualities.OPC_QUALITY_GOOD:
                    strQuality = "Good";
                    break;
                case Qualities.OPC_QUALITY_BAD:
                    strQuality = "Bad";
                    break;
                case Qualities.OPC_QUALITY_CONFIG_ERROR:
                    strQuality = "BadConfigurationError";
                    break;
                case Qualities.OPC_QUALITY_NOT_CONNECTED:
                    strQuality = "BadNotConnected";
                    break;
                case Qualities.OPC_QUALITY_DEVICE_FAILURE:
                    strQuality = "BadDeviceFailure";
                    break;
                case Qualities.OPC_QUALITY_SENSOR_FAILURE:
                    strQuality = "BadSensorFailure";
                    break;
                case Qualities.OPC_QUALITY_COMM_FAILURE:
                    strQuality = "BadCommFailure";
                    break;
                case Qualities.OPC_QUALITY_OUT_OF_SERVICE:
                    strQuality = "BadOutOfService";
                    break;
                case Qualities.OPC_QUALITY_WAITING_FOR_INITIAL_DATA:
                    strQuality = "BadWaitingForInitialData";
                    break;
                case Qualities.OPC_QUALITY_EGU_EXCEEDED:
                    strQuality = "UncertainEGUExceeded";
                    break;
                case Qualities.OPC_QUALITY_SUB_NORMAL:
                    strQuality = "UncertainSubNormal";
                    break;
                default:
                    strQuality = "Not handled";
                    break;
            }
            return strQuality;
        }

        public void Read(string groupName, int itemNum, object[] result)
        {

            IntPtr pItemValues = IntPtr.Zero;
            IntPtr pErrors = IntPtr.Zero;
            try
            {
                int[] items = null;
                if (!_goroupsItems.TryGetValue(groupName,out items))
                {
                    return;
                }
                object io;
                if (!_groupObj.TryGetValue(groupName, out io))
                {
                    return;                    
                }

                //IOPCSyncIO2Obj.Read(OPCDATASOURCE.OPC_DS_DEVICE, itemNum, ItemServerHandle, out
                //  pItemValues, out pErrors);

                ((IOPCSyncIO)io).Read(OPCDATASOURCE.OPC_DS_DEVICE, itemNum, items, out
                  pItemValues, out pErrors);

                int[] errors = new int[itemNum];
                Marshal.Copy(pErrors, errors, 0, itemNum);
                OPCITEMSTATE[] pItemState = new OPCITEMSTATE[itemNum];
                for (int i = 0; i < itemNum; i++)
                {
                    if (errors[i] == 0)
                    {
                        //从非托管区封送数据到托管区
                        pItemState[i] = (OPCITEMSTATE)Marshal.PtrToStructure(pItemValues, typeof(OPCITEMSTATE));
                        pItemValues = new IntPtr(pItemValues.ToInt32() + Marshal.SizeOf(typeof(OPCITEMSTATE)));
                        result[i] = pItemState[i].vDataValue;
                    }
                }


            }
            catch (System.Exception error)
            {
                //MessageBox.Show(error.Message, "Result - Read Items", MessageBoxButtons.OK,
                //MessageBoxIcon.Error);
            }
            finally
            {
                // Free the unmanaged memory
                if (pItemValues != IntPtr.Zero)
                {
                    //Marshal.FreeCoTaskMem(pItemValues);
                    pItemValues = IntPtr.Zero;
                }
                if (pErrors != IntPtr.Zero)
                {
                    //Marshal.FreeCoTaskMem(pErrors);
                    pErrors = IntPtr.Zero;
                }
            }

        }



        public void Write(string groupName, int itemNum, object[] values)
        {

            IntPtr pErrors = IntPtr.Zero;

            try
            {
                int[] items = null;
                if (!_goroupsItems.TryGetValue(groupName, out items))
                {
                    return;
                }
                object io;
                if (!_groupObj.TryGetValue(groupName, out io))
                {
                    return;
                    
                }

                //IOPCSyncIO2Obj.Write(itemNum, ItemServerHandle, values, out pErrors);
                ((IOPCSyncIO)io).Write(itemNum, items, values, out pErrors);
                int[] errors = new int[itemNum];
                Marshal.Copy(pErrors, errors, 0, itemNum);

                for (int i = 0; i < itemNum; i++)
                {
                    if (errors[i] != 0)
                    {
                        pErrors = IntPtr.Zero;

                    }
                }
                //MessageBox.Show("woicao");
            }
            catch (System.Exception error)
            {
                //MessageBox.Show(error.Message, "Result - WriteItem", MessageBoxButtons.OK,
                //MessageBoxIcon.Error);
            }
            finally
            {
                if (pErrors != IntPtr.Zero)
                {
                    Marshal.FreeCoTaskMem(pErrors);
                    pErrors = IntPtr.Zero;
                }
            }

        }

        public void RemoveGroup(string GroupName)
        {
            try
            {                
                //1.18
                int pSvrGroupHandle;

                if (true == _serverobj.TryGetValue(GroupName, out pSvrGroupHandle))
                {
                    ServerObj.RemoveGroup(pSvrGroupHandle, 0);
                    _serverobj.Remove(GroupName);
                    _goroupsItems.Remove(GroupName);
                    _groupObj.Remove(GroupName);
                }                
            }
            catch (System.Exception error)
            {
                //MessageBox.Show(error.Message, "Result - Stop Server", MessageBoxButtons.OK,
                //MessageBoxIcon.Error);
            }
        }

    }


}
