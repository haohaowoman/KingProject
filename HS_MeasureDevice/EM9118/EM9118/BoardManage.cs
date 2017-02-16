using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace EM9118
{
    public class BoardManage : HS_DeviceInteract.IADDeviceInteract
    {
        #region 成员变量
        bool[] isWork; //八张板卡的通信状态
        EM9118Command[] mEM9118; //每张板卡的实例
        ModbusCommand mModCmd; //modBus命令
        double[] allValue;

        public event EventHandler CardConnectionChanged;
        #endregion

        #region 构造函数
        public BoardManage()
        {
            mEM9118 = new EM9118Command[8];
            for (int i = 0; i < 8; i++)
            {
                mEM9118[i] = new EM9118Command(i + 1);
                mEM9118[i].EMConnectoinChanged += BoardManage_EMConnectoinChanged;
            }

            isWork = new bool[8];
            for (int j = 0; j < 8; j++)
            {
                isWork[j] = false;
            }

            mModCmd = new ModbusCommand();
            allValue = new double[48];
        }

        // 板卡的连接状态改变事件。
        private void BoardManage_EMConnectoinChanged(object sender, bool e)
        {
            int cc = mEM9118.Length;
            for (int i = 0; i < cc; i++)
            {
                if (mEM9118[i] == sender)
                {
                    isWork[i] = e;
                    CardConnectionChanged?.Invoke(this, null);
                }
            }
        }

        #endregion

        #region  打开八张板卡，链接数据端口和，命令端口OpenDevice()
        public bool OpenDevice()
        {
            //链接命令端口
            if (!cmdCnt())
            {
                return false;
            }

            //设置每张板卡的采集参数
            for (int i = 0; i < 8; i++)
            {
                if (!setCmd(mEM9118[i], mModCmd))
                {
                    return false;
                }
            }

            //链接数据端口
            if (!dataCnt())
            {
                return false;
            }

            for (int i = 0; i < isWork.Length; i++)
            {
                isWork[i] = true;
            }
            return true;
        }

        #region 链接命令端口
        private bool cmdCnt()
        {
            try
            {
                if (mEM9118[0].connectServiceConnect("192.168.0.126", 8000, 1) == 1)
                {
                    isWork[0] = true;
                }
                if (mEM9118[1].connectServiceConnect("192.168.0.127", 8000, 1) == 1)
                {
                    isWork[1] = true;
                }
                if (mEM9118[2].connectServiceConnect("192.168.0.128", 8000, 1) == 1)
                {
                    isWork[2] = true;
                }
                if (mEM9118[3].connectServiceConnect("192.168.0.129", 8000, 1) == 1)
                {
                    isWork[3] = true;
                }
                if (mEM9118[4].connectServiceConnect("192.168.0.130", 8000, 1) == 1)
                {
                    isWork[4] = true;
                }
                if (mEM9118[5].connectServiceConnect("192.168.0.131", 8000, 1) == 1)
                {
                    isWork[5] = true;
                }
                if (mEM9118[6].connectServiceConnect("192.168.0.132", 8000, 1) == 1)
                {
                    isWork[6] = true;
                }
                if (mEM9118[7].connectServiceConnect("192.168.0.133", 8000, 1) == 1)
                {
                    isWork[7] = true;
                }
            }
            catch
            {
                return false;
            }
            return true;

        }
        #endregion

        #region 设置板卡的信息
        private bool setCmd(EM9118Command mEM9118Cmd, ModbusCommand mModbusCommand)
        {
            string cmd;
            byte[] resCmd = new byte[1024];
            Array.Clear(resCmd, 0, 1024);
            int len;
            try
            {
                //采集范围-10v ~ 10v
                cmd = mModbusCommand.setADRange("1", 16);
                mEM9118Cmd.sendMessageByCommond(cmd);
                Thread.Sleep(10);
                len = mEM9118Cmd.revMessByCommond(resCmd);

                //读取零点满度信息
                cmd = mModbusCommand.readADZeroInfo();
                mEM9118Cmd.sendMessageByCommond(cmd);
                Thread.Sleep(10);
                len = mEM9118Cmd.revMessByCommond(resCmd);

                //设置AD采集使能通道
                cmd = mModbusCommand.setADChannel("3F000000");
                mEM9118Cmd.sendMessageByCommond(cmd);
                Thread.Sleep(10);
                mEM9118Cmd.revMessByCommond(resCmd);

                //设置组频率
                cmd = mModbusCommand.setGroupHZ("1");
                mEM9118Cmd.sendMessageByCommond(cmd);
                Thread.Sleep(10);
                //resCmd = mEM9118Cmd.revMessByCommond();
                //resCmd0.Text = byte2str(resCmd, 12);

                //设置每帧组数
                cmd = mModbusCommand.setFroupFTP("20");
                mEM9118Cmd.sendMessageByCommond(cmd);
                Thread.Sleep(10);

                //允许主动传输数据
                cmd = mModbusCommand.setActiveTransport(true);
                mEM9118Cmd.sendMessageByCommond(cmd);
                Thread.Sleep(10);
            }
            catch (Exception )
            {
                return false;
            }
            return true;
        }
        #endregion

        #region 链接数据端口
        private bool dataCnt()
        {
            try
            {
                if (mEM9118[0].connectServiceData("192.168.0.126", 8001, 1) == 1)
                {
                    isWork[0] = true;
                }
                if (mEM9118[1].connectServiceData("192.168.0.127", 8001, 1) == 1)
                {
                    isWork[1] = true;
                }
                if (mEM9118[2].connectServiceData("192.168.0.128", 8001, 1) == 1)
                {
                    isWork[2] = true;
                }
                if (mEM9118[3].connectServiceData("192.168.0.129", 8001, 1) == 1)
                {
                    isWork[3] = true;
                }
                if (mEM9118[4].connectServiceData("192.168.0.130", 8001, 1) == 1)
                {
                    isWork[4] = true;
                }
                if (mEM9118[5].connectServiceData("192.168.0.131", 8001, 1) == 1)
                {
                    isWork[5] = true;
                }
                if (mEM9118[6].connectServiceData("192.168.0.132", 8001, 1) == 1)
                {
                    isWork[6] = true;
                }
                if (mEM9118[7].connectServiceData("192.168.0.133", 8001, 1) == 1)
                {
                    isWork[7] = true;
                }
            }
            catch
            {
                return false;
            }
            return true;
        }
        #endregion

        #endregion

        #region 开始采集数据 StartAD()
        public void StartAD()
        {
            string cmd = "";
            for (int i = 0; i < 8; i++)
            {
                cmd = mModCmd.startOrStopHardWare("01");
                mEM9118[i].sendMessageByCommond(cmd);
                Thread.Sleep(10);

                mEM9118[i].startThread();
            }
        }
        #endregion

        #region 停止采集 StopAD()
        public void StopAD()
        {
            string cmd; //= mModbusCommand[0].startOrStopHardWare("01");
            for (int i = 0; i < 8; i++)
            {
                cmd = mModCmd.startOrStopHardWare("00");
                mEM9118[i].sendMessageByCommond(cmd);
                Thread.Sleep(1);

                mEM9118[i].stopThread();
            }

        }
        #endregion

        #region 关闭采集，释放资源 Close()
        public void Close()
        {
            for (int i = 0; i < 8; i++)
            {
                mEM9118[i].closeDaraSocket();
                mEM9118[i].closeCommandSocket();
            }
        }

        public void Dispose()
        {
            Close();
        }
        #endregion

        #region 获取所有通道的值 AllChannelsValue
        public double[] AllChannelsValue
        {
            get
            {

                double[] tmp;
                for (int i = 0; i < 8; i++)
                {
                    tmp = mEM9118[i].ChannelsValue;
                    for (int j = 0; j < 6; j++)
                    {
                        allValue[i * 6 + j] = tmp[j];
                    }
                }
                return allValue;
            }
        }
        #endregion

        #region 获取板卡链接 
        public bool[] CardsConnection
        {
            get
            {
                return isWork;
            }
        }
        #endregion
    }
}
