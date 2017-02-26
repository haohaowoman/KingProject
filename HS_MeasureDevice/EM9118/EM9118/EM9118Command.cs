using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace EM9118
{
    public class EM9118Command : ModbusCommand
    {
        #region 成员变量
        String commondIP;
        int commondPorn;
        Socket commondSocket;
        int commondConnectWay;

        String dataIP;
        int dataPorn;
        Socket dataSocket;
        int dataConnectWay;

        public int[,] saveData; //保存数据
        bool isWork;  //退出线程
        int index;  //二维数组下标
        Thread mthread; //采集数据线程
        int _moveWndWidth; //保存二维数组列数
        byte[] recv = new byte[4800]; //收到数据存放
        private int[] _moveWndVsSum = new int[6];
        short[,] tmp = new short[6, 20];
        int[] tempSum = new int[6];   //收到数据的总和
        short[] tempMax = new short[6]; //求和数据中的最大值
        short[] tempMin = new short[6]; //求和数据中的最小值
        int boardNum;
        private object _dlocker = new object();
        private double[] _chsValue = new double[6];
        #endregion

        /// <summary>
        /// 获取板卡是否连接。
        /// </summary>
        public bool IsConnected
        {
            get { return !isWork; }
            private set
            {
                isWork = !value;
                EMConnectoinChanged?.Invoke(this, value);
            }
        }

        public event EventHandler<bool> EMConnectoinChanged;

        #region 构造函数，初始化
        public EM9118Command(int boardNum)
        {
            _moveWndWidth = 100;
            saveData = new int[6, _moveWndWidth];
            isWork = true;
            index = 0;
            mthread = new Thread(tGetData);
            this.boardNum = boardNum;
        }
        #endregion

        #region 收到数据的线程
        private void tGetData(object param)
        {
            while (!isWork)
            {
                int rl = revMessByData(recv);
                if (rl % 240 == 0)     //收到的数据如果不是20组，则抛弃整个数据
                    extData(rl);
                Thread.Sleep(10);
            }
        }
        #endregion

        #region 启动收数据线程
        public void startThread()
        {
            isWork = false;
            index = 0;
            mthread.Start();
        }
        #endregion

        #region 停止收数据线程
        public void stopThread()
        {
            isWork = true;
            IsConnected = false;
            Thread.Sleep(10);
        }
        #endregion

        #region 链接数据
        /// <summary>
        /// 链接命令端口
        /// <param name="ip">ip地址</param>
        /// <param name="porn">端口</param>
        /// <param name="connectWay">链接方式，1为tcp</param>
        /// <returns>链接是否成功，成功返回1</returns>
        /// </summary>
        public int connectServiceConnect(String ip, int porn, int connectWay)
        {
            this.commondIP = ip;
            this.commondPorn = porn;
            this.commondConnectWay = connectWay;

            IPAddress mIPAddress = IPAddress.Parse(ip);
            switch (connectWay)
            {
                case 1:
                    commondSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    break;
                default:
                    return -1;
            }

            try
            {
                commondSocket.Connect(mIPAddress, porn);
                IsConnected = true;
                return 1;
            }
            catch (Exception)
            {
                //Console.WriteLine("连接服务器失败:" + e);
                return -1;
            }
        }


        /// <summary>
        /// 链接数据端口
        /// <param name="ip">ip地址</param>
        /// <param name="porn">端口</param>
        /// <param name="connectWay">链接方式，1为tcp</param>
        /// <returns>链接是否成功，成功返回1</returns>
        /// </summary>
        public int connectServiceData(String ip, int porn, int connectWay)
        {
            this.dataIP = ip;
            this.dataPorn = porn;
            this.dataConnectWay = connectWay;

            IPAddress mIPAddress = IPAddress.Parse(ip);
            switch (connectWay)
            {
                case 1:
                    dataSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    break;
                default:
                    return -1;
            }

            try
            {
                dataSocket.Connect(mIPAddress, porn);
                return 1;
            }
            catch (Exception)
            {
                //Console.WriteLine("连接服务器失败:" + e);
                return -1;
            }
        }
        #endregion

        #region 发数据
        /// <summary>
        /// 向服务器发送数据,通过命令端口
        /// <param name="messgae">要发送的消息</param>
        /// <returns>发送的字节数</returns>
        /// </summary>
        public int sendMessageByCommond(String mess)
        {
            int sendcount;
            byte[] tmp = hex2byte(mess);
            try
            {
                sendcount = this.commondSocket.Send(tmp);
                return sendcount;
            }
            catch
            {
                return -1;
            }
        }


        /// <summary>
        /// 向服务器发送数据，通过数据端口
        /// <param name="messgae">要发送的消息</param>
        /// <returns>发送的字节数</returns>
        /// </summary>
        public int sendMessageByData(String mess)
        {
            int sendcount;
            byte[] tmp = hex2byte(mess);
            try
            {
                sendcount = this.dataSocket.Send(tmp);
                return sendcount;
            }
            catch
            {
                return -1;
            }
        }
        #endregion

        #region 收数据
        /// <summary>
        /// 从服务器发送数据，通命令据端口
        /// <returns>接收到的数据</returns>
        /// </summary>
        public int revMessByCommond(byte[] recvCommond)
        {
            int recvLenth;
            Array.Clear(recvCommond, 0, recvCommond.Length);
            try
            {
                recvLenth = this.commondSocket.Receive(recvCommond);
                // str = byte2str(recvData, recvLenth);
                return recvLenth;
            }
            catch (Exception)
            {
                return -1;
            }
        }


        /// <summary>
        /// 从服务器发送数据，通数据据端口
        /// <returns>接收到的数据</returns>
        /// </summary>
        private int revMessByData(byte[] recvData)
        {
            int recvLenth;
            try
            {

                recvLenth = this.dataSocket.Receive(recvData, recv.Length, SocketFlags.None);
                return recvLenth;
            }
            catch (Exception)
            {
                isWork = true;
                IsConnected = false;
                return -1;
            }
        }
        #endregion

        #region 数据提取
        public void extData(int bytesCount)
        {
            int pagcount = bytesCount / 240;
            for (int p = 0; p < pagcount; p++)
            {
                Array.Clear(tempSum, 0, 6);
                Array.Clear(tempMax, 0, 6);
                Array.Clear(tempMin, 0, 6);

                for (int c = 0; c < 20; c++)
                {
                    for (int i = 0; i < 6; i++)
                    {
                        tmp[i, c] = (short)((recv[p * 240 + c * 12 + i * 2 + 1] << 8 | recv[p * 240 + c * 12 + i * 2]) & 0xffff);
                        if ((tmp[i, c] & 0xFF00) == 0xFF00)
                        {
                            // 出现错误代码。                            
                            if (c == 0)
                            {
                                tmp[i, c] = 0;
                            }
                            else
                            {
                                tmp[i, c] = tmp[i, c - 1];
                            }
                            //Console.WriteLine($"Card {commondIP} channel {i} data is error 0x{tmp[i, c]:x4}-{tmp[i, c]:d}.");
                        }
                        tempSum[i] += tmp[i, c];
                        tempMax[i] = Math.Max(tempMax[i], tmp[i, c]);
                        tempMin[i] = Math.Max(tempMin[i], tmp[i, c]);

                    }
                }

                lock (_dlocker)
                {
                    for (int i = 0; i < 6; i++)
                    {
                        int lbs = (tempSum[i] - tempMax[i] - tempMin[i]) / (20 - 2);

                        if (index >= _moveWndWidth)
                        {
                            _moveWndVsSum[i] -= saveData[i, index % _moveWndWidth];
                        }
                        
                        if (index > 0)
                        {
                            // 排除掉较大跳变值。
                            int ct = lbs - saveData[i, (index - 1) % _moveWndWidth];
                            if (ct >= 9600/*saveData[i, (index - 1) % _moveWndWidth] / 2*/ || ct <= -9600/*-saveData[i, (index - 1) % _moveWndWidth] / 2*/)
                            {
#if DEBUG
                                Console.WriteLine($"Card {commondIP} channel {i} data {lbs} is error re={saveData[i, (index - 1) % _moveWndWidth]} at {DateTime.Now.ToShortTimeString()}.");
#endif
                                lbs = saveData[i, (index - 1) % _moveWndWidth];
                                
                            }
                        }

                        saveData[i, index % _moveWndWidth] = lbs;
                        _moveWndVsSum[i] += lbs;

                        if (index == 0)
                        {
                            _chsValue[i] = finalVer(i, (double)_moveWndVsSum[i]);
                        }
                        else if (index < _moveWndWidth)
                        {
                            _chsValue[i] = finalVer(i, _moveWndVsSum[i] / (double)index);
                        }
                        else
                        {
                            _chsValue[i] = finalVer(i, _moveWndVsSum[i] / (double)_moveWndWidth);
                        }
                    }

                    index++;
                }
            }
        }
        #endregion

        #region 返回数据
        public double[] ChannelsValue
        {
            get
            {
                lock (_dlocker)
                {

                }
                return _chsValue;
            }
        }
        #endregion

        #region 计算并校准电流值
        public double finalVer(int channelNum, double recvData)
        {
            double tmp = (((double)recvData / 29490.0) * 4.500);
            tmp = ChannelInfo.getCorrectData(this.boardNum, channelNum + 1, tmp);
            return tmp;
        }
        #endregion

        #region 关闭socket
        public void closeCommandSocket()
        {
            this.commondSocket.Close();
        }

        public void closeDaraSocket()
        {
            if (!isWork)
            {
                stopThread();
                Thread.Sleep(10);
                if (mthread.IsAlive)
                {
                    mthread.Abort();
                }
            }
            this.dataSocket.Close();

        }
        #endregion

        #region 一些工具
        public String getCommandIP()
        {
            return this.commondIP;
        }

        public String getDataIP()
        {
            return this.dataIP;
        }

        public int getcommandPorn()
        {
            return this.commondPorn;
        }

        public int getDataPorn()
        {
            return this.dataPorn;
        }

        public String getCommandConnectWay()
        {
            String str;
            switch (this.commondConnectWay)
            {
                case 1:
                    str = "TCP";
                    break;
                default:
                    str = "";
                    break;
            }
            return str;
        }

        public String getDataConnectWay()
        {
            String str;
            switch (this.dataConnectWay)
            {
                case 1:
                    str = "TCP";
                    break;
                default:
                    str = "";
                    break;
            }
            return str;
        }

        public Socket getSocketCommand()
        {
            return this.commondSocket;
        }

        public Socket getsocketDara()
        {
            return this.dataSocket;
        }

        #endregion
    }
}
