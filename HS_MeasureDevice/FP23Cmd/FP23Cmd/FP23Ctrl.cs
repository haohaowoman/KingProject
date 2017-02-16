using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;


namespace FP23
{
    public struct PIDInfo 
    {
        public float PB;
        public int IT;
        public int DT;
        public float MR;
        public int DF;
        public float O2_L;
        public float O2_H;
        public float SF;
    };
    public class FP23Ctrl
    {
        private SerialPort sport;
        private string m_com = "";
        private string error = "";
        private bool bInit = false;
        private byte[] recv;
        private FP23.FP23Cmd mFP23Cmd = new FP23.FP23Cmd();
        public FP23Ctrl(string com, Int32 bps,Parity parity, Int32 n, StopBits stopBit)
        {
            m_com = com;
            sport = new SerialPort(com, bps, parity, n, stopBit);
            error = "";
            bInit = false;
            recv = new byte[128];
        }
        public FP23Ctrl(string com) 
        {
            m_com = com;
            sport = new SerialPort(com, 9600, Parity.Even, 7, StopBits.One);
            error = "";
            bInit = false;
            recv = new byte[128];
        }
        private byte[] GetFP23Cmd(int addr, int childAddr, string cmdType, string cmdCode, int nCount, int cmdData)
        {
            byte[] cmd;
            mFP23Cmd.setAddr(addr);
            mFP23Cmd.setAddrSec(childAddr);
            mFP23Cmd.setCmdType(cmdType.ToUpper());
            mFP23Cmd.setCmdCode(cmdCode.ToUpper());
            mFP23Cmd.SetcmdContinuous(nCount);
            mFP23Cmd.setCmdData(cmdData);
            cmd = mFP23Cmd.getCmd();
            return cmd;
        }
        /// <summary>
        /// 初始化FP23控制，设置为COM控制和FIX模式
        /// </summary>
        /// <returns>成功返回true</returns>
        public bool InitCtrl()
        {
            try
            {
                if (!(sport.IsOpen))
                {
                    sport.Open();
                    bInit = true;
                    return true;
                }
                return true;
            }
            catch (System.Exception ex)
            {
                return false;
            }
        }
        /// <summary>
        /// 获取错误信息
        /// </summary>
        /// <returns>返回错误信息</returns>
        public string GetErrorInfo()
        {
            return error;
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        /// <returns>成功返回true</returns>
        public bool ReleaseCtrl() 
        {
            try
            {
                if (sport.IsOpen)
                {
                    sport.Close();
                }
                bInit = false;
                return true;
            }
            catch (System.Exception ex)
            {
                return false;
            }
        }
        /// <summary>
        /// 设置控制模式（只写）
        /// </summary>
        /// <param name="bCom">0为LOC本地控制，1为COM控制</param>
        /// <returns>设置成功返回true</returns>
        public bool SetComStyle(bool bCom)
        {
            if (!bInit)
                return false;
            if (sport.IsOpen)
            {
                byte[] cmd;
                if (bCom)
                    cmd = GetFP23Cmd(Int32.Parse(m_com.Substring(3)), 1, "W", "018C", 0, 1);//启动为COM控制
                else
                    cmd = GetFP23Cmd(Int32.Parse(m_com.Substring(3)), 1, "W", "018C", 0, 0);//关闭COM控制，设为本地LOC

                sport.Write(cmd, 0, cmd.GetLength(0));
                System.Threading.Thread.Sleep(20);
                Int32 n = sport.Read(recv, 0, 128);
                if (n > 0)
                {
                    if (GetError(recv))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// 设置程序模式(R/W)
        /// </summary>
        /// <param name="bFix">0: PROG 1: FIX</param>
        /// <returns>设置成功返回true</returns>
        public bool SetFixStyle(bool bFix)
        {
            if (!bInit)
                return false;
            if (sport.IsOpen)
            {
                byte[] cmd;
                if (bFix)
                    cmd = GetFP23Cmd(Int32.Parse(m_com.Substring(3)), 1, "W", "0800", 0, 1);//0: PROG 1: FIX
                else
                    cmd = GetFP23Cmd(Int32.Parse(m_com.Substring(3)), 1, "W", "0800", 0, 0);
                sport.Write(cmd, 0, cmd.GetLength(0));
                System.Threading.Thread.Sleep(20);
                Int32 n = sport.Read(recv, 0, 128);
                if (n > 0)
                {
                    if (GetError(recv))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// 获取程序模式
        /// </summary>
        /// <param name="style">0: PROG 1: FIX</param>
        /// <returns>获取成功返回true</returns>
        public bool GetFixStyle(out int style)
        {
            style = 0;
            if (!bInit)
                return false;
            if (sport.IsOpen)
            {
                byte[] cmd;
                cmd = GetFP23Cmd(Int32.Parse(m_com.Substring(3)), 1, "R", "0800", 0, 1);//0: PROG 1: FIX
                sport.Write(cmd, 0, cmd.GetLength(0));
                System.Threading.Thread.Sleep(20);
                Int32 n = sport.Read(recv, 0, 128);
                if (n > 0)
                {
                    if (recv[8] == '0' && recv[9] == '0' && recv[10] == '0' && recv[11] == '1')
                    {
                        style = 1;
                    }
                    else if (recv[8] == '0' && recv[9] == '0' && recv[10] == '0' && recv[11] == '0')
                    {
                        style = 0;
                    }
                    else
                        return false;
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 获取PV值
        /// </summary>
        /// <param name="temp">保存获取到的PV值</param>
        /// <returns>成功返回true</returns>
        public bool GetPVData(out float temp)
        {
            temp = 0;
            if (!bInit)
                return false;
            if (sport.IsOpen)
            {
                byte[] cmd;
                cmd = GetFP23Cmd(Int32.Parse(m_com.Substring(3)), 1, "R", "0100", 0, 1);
                sport.Write(cmd, 0, cmd.GetLength(0));
                System.Threading.Thread.Sleep(20);
                Int32 n = sport.Read(recv, 0, 128);
                if (n > 0)
                {
                    if (GetError(recv))
                    {
                        int[] data;
                        GetData(recv,1,out data);
                        temp = (float)(data[0] / 10.0);
                        return true;
                    }
                    /*string str = Encoding.ASCII.GetString(recv);
                    string abc = str.Substring(8, 4);
                    Convert.ToInt32(abc);
                    temp = (float)(Convert.ToInt32(str.Substring(8,4))/10);*/

                }
            }
            return false;
        }
        /// <summary>
        /// 设置OUT1值
        /// </summary>
        /// <param name="temp">要设置的OUT1值，暂时不能使用</param>
        /// <returns>成功返回true</returns>
        public bool SetOUT1Data(float temp)
        {
            if (!bInit)
                return false;
            if (SetFixStyle(false) && sport.IsOpen)
            {
                byte[] cmd;
                cmd = GetFP23Cmd(Int32.Parse(m_com.Substring(3)), 1, "R", "0182", 0, FloatToInt(temp));
                sport.Write(cmd, 0, cmd.GetLength(0));
                System.Threading.Thread.Sleep(20);
                Int32 n = sport.Read(recv, 0, 128);
                if (n > 0)
                {
                    if (GetError(recv))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// 获取OUT1值
        /// </summary>
        /// <param name="temp">保存获取到的OUT1值</param>
        /// <returns>成功返回true</returns>
        public bool GetOUT1Data(out float temp)
        {
            temp = 0; 
            if (!bInit)
                return false;
            if (sport.IsOpen)
            {
                byte[] cmd;
                cmd = GetFP23Cmd(Int32.Parse(m_com.Substring(3)), 1, "R", "0102", 0, 1);
                sport.Write(cmd, 0, cmd.GetLength(0));
                System.Threading.Thread.Sleep(20);
                Int32 n = sport.Read(recv, 0, 128);
                if (n > 0)
                {
                    if (GetError(recv))
                    {
                        int[] data;
                        GetData(recv,1,out data);
                        temp = (float)(data[0] / 10.0);
                        return true;
                    }
                    /*string str = Encoding.ASCII.GetString(recv);
                    string abc = str.Substring(8, 4);
                    Convert.ToInt32(abc);
                    temp = (float)(Convert.ToInt32(str.Substring(8,4))/10);*/

                }
            }
            return false;
        }
        /// <summary>
        /// 获取PID号
        /// </summary>
        /// <param name="temp">保存获取到的PID号</param>
        /// <returns>成功返回true</returns>
        public bool GetPIDNum(out float temp)
        {
            temp = 0;
            if (!bInit)
                return false;
            if (sport.IsOpen)
            {
                byte[] cmd;
                cmd = GetFP23Cmd(Int32.Parse(m_com.Substring(3)), 1, "R", "0107", 0, 1);
                sport.Write(cmd, 0, cmd.GetLength(0));
                System.Threading.Thread.Sleep(20);
                Int32 n = sport.Read(recv, 0, 128);
                if (n > 0)
                {
                    if (GetError(recv))
                    {
                        int[] data;
                        GetData(recv,1,out data);
                        temp = (float)(data[0]);
                        return true;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// 获取PID参数
        /// </summary>
        /// <param name="data">PID参数数组，数据依次是PB 比例系数0.0到999.9％(0.0=OFF)，IT积分时间0到6000sec(0=OFF)，
        /// DT微分时间0到3600sec(0=OFF)，MR手动设置：-50.0 到 50.0%，DF滞后1 到 9999 Unit，O1_L输出下限0.0 到 100.0%，
        /// O1_H输出上限0.0 到 100.0%，SF超调控制系数数0-1.00</param>
        /// <returns></returns>
        public bool GetPID(int index, out PIDInfo pid)
        {
            
            int[] data = new int[8];
            pid.PB = pid.MR = pid.O2_L = pid.O2_H = pid.SF = 0;
            pid.IT = pid.DT = pid.DF = 0;
            if (!bInit)
                return false;
            if (sport.IsOpen)
            {
                byte[] cmd;
                int t = 1024 + (index - 1) * 8;
                cmd = GetFP23Cmd(Int32.Parse(m_com.Substring(3)), 1, "R", string.Format("{0:x4}", t), 7, 1);
                sport.Write(cmd, 0, cmd.GetLength(0));
                System.Threading.Thread.Sleep(20);
                Int32 n = sport.Read(recv, 0, 128);
                if (n > 0)
                {
                    if (GetError(recv))
                    {
                        GetData(recv,8,out data);
                    }
                }
                pid.PB = (float)(data[0] / 10.0);
                pid.IT = data[1];
                pid.DT = data[2];
                pid.MR = (float)(data[3] / 10.0);
                pid.DF = data[4];
                pid.O2_L = (float)(data[5] / 10.0);
                pid.O2_H = (float)(data[6]/10.0);
                pid.SF = (float)(data[7] / 10.0);
                return true;
            }
            return false;
        }
        /// <summary>
        /// 设置PID参数
        /// </summary>
        /// <param name="index">PID的索引，1-10</param>
        /// <param name="dataPB">索引对应的PB值</param>
        /// <param name="dataIT">索引对应的IT值</param>
        /// <param name="dataDT">索引对应的DT值</param>
        /// <returns></returns>
        public bool SetPID(int index, float dataPB, int dataIT, int dataDT)
        {
            if (!bInit)
                return false;
            if (index <= 0 || index > 10 )
            {
                return false;
            }
            if (sport.IsOpen)
            {
                byte[] cmd;
                int[] data = new int[3];
                data[0] = FloatToInt(dataPB);
                data[1] = dataIT;
                data[2] = dataDT;
                int t = 1024 + (index - 1) * 8;
                for (int i = 0; i < 3; i++)
                {
                    cmd = GetFP23Cmd(Int32.Parse(m_com.Substring(3)), 1, "W", string.Format("{0:x4}", t + i), 0, data[i]);
                    sport.Write(cmd, 0, cmd.GetLength(0));
                    System.Threading.Thread.Sleep(20);
                    Int32 n = sport.Read(recv, 0, 128);
                    if (n > 0)
                    {
                        if (!GetError(recv))
                        {
                            return false;
                        }
                    }
                    else
                        return false;
                }
                return true;
            }
            return false;
        }
        /// <summary>
        /// 设置PID参数
        /// </summary>
        /// <param name="index">PID的索引，1-10</param>
        /// <param name="data">PID参数数组，数据依次是PB 比例系数0.0到999.9％(0.0=OFF)，IT积分时间0到6000sec(0=OFF)，
        /// DT微分时间0到3600sec(0=OFF)，MR手动设置：-50.0 到 50.0%，DF滞后1 到 9999 Unit，O1_L输出下限0.0 到 100.0%，
        /// O1_H输出上限0.0 到 100.0%，SF超调控制系数数0-1.00</param>
        /// <returns></returns>
        public bool SetPID(int index, ref PIDInfo pid)
        {
            if (!bInit)
                return false;
            if (index <= 0 || index > 10)
            {
                return false;
            }
            if (sport.IsOpen)
            {
                byte[] cmd;
                int[] data = new int[8];
                data[0] = FloatToInt(pid.PB);
                data[1] = pid.IT;
                data[2] = pid.DT;
                data[3] = FloatToInt(pid.MR);
                data[4] = pid.DF;
                data[5] = FloatToInt(pid.O2_L);
                data[6] = FloatToInt(pid.O2_H);
                data[7] = FloatToInt(pid.SF);
                int t = 1024 + (index - 1) * 8;
                for (int i = 0; i < 8; i++)
                {
                    cmd = GetFP23Cmd(Int32.Parse(m_com.Substring(3)), 1, "W", string.Format("{0:x4}", t + i), 0, data[i]);
                    sport.Write(cmd, 0, cmd.GetLength(0));
                    System.Threading.Thread.Sleep(20);
                    Int32 n = sport.Read(recv, 0, 128);
                    if (n > 0)
                    {
                        if (!GetError(recv))
                        {
                            return false;
                        }
                    }
                    else
                        return false;
                }
                return true;
            }
            return false;
        }
        /// <summary>
        /// 设置温度
        /// </summary>
        /// <param name="temp">temp为温度值0-800,对应传入参数0-8000</param>
        /// <returns>设置成功返回true</returns>
        public bool SetTemp(float temp)
        {
            if (!bInit)
                return false;
            if (temp < 0 || temp > 800.0)
            {
                return false;
            }
            if (sport.IsOpen)
            {
                byte[] cmd;
                cmd = GetFP23Cmd(Int32.Parse(m_com.Substring(3)), 1, "W", "0300", 0, FloatToInt(temp));
                sport.Write(cmd, 0, cmd.GetLength(0));
                System.Threading.Thread.Sleep(20);
                Int32 n = sport.Read(recv, 0, 128);
                if (n > 0)
                {
                    if (GetError(recv))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// 获取温度值（SV值）
        /// </summary>
        /// <param name="temp">保存返回的温度值</param>
        /// <returns>成功返回true</returns>
        public bool GetTemp(out float temp)
        {
            temp = 0;
            if (!bInit)
                return false;
            if (sport.IsOpen)
            {
                byte[] cmd;
                cmd = GetFP23Cmd(Int32.Parse(m_com.Substring(3)), 1, "R", "0101", 0, 1);
                sport.Write(cmd, 0, cmd.GetLength(0));
                System.Threading.Thread.Sleep(20);
                Int32 n = sport.Read(recv, 0, 128);
                if (n > 0)
                {
                    if (GetError(recv))
                    {
                        int[] data;
                        GetData(recv,1,out data);
                        temp = (float)(data[0] / 10.0);
                        return true;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// 设置自动调整执行
        /// </summary>
        /// <param name="temp">0：OFF1：ON</param>
        /// <returns>设置成功返回true</returns>
        public bool SetAT(bool bAt)
        {
            if (!bInit)
                return false;
            if (sport.IsOpen)
            {
                byte[] cmd;
                if (bAt)
                    cmd = GetFP23Cmd(Int32.Parse(m_com.Substring(3)), 1, "W", "0184", 0, 1);
                else
                    cmd = GetFP23Cmd(Int32.Parse(m_com.Substring(3)), 1, "W", "0184", 0, 0);
                sport.Write(cmd, 0, cmd.GetLength(0));
                System.Threading.Thread.Sleep(20);
                Int32 n = sport.Read(recv, 0, 128);
                if (n > 0)
                {
                    if (GetError(recv))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// 设置手动操作
        /// </summary>
        /// <param name="temp">0：OFF1：ON</param>
        /// <returns>设置成功返回true</returns>
        public bool SetMan(bool bMan)
        {
            if (!bInit)
                return false;
            if (sport.IsOpen)
            {
                byte[] cmd;
                if (bMan)
                    cmd = GetFP23Cmd(Int32.Parse(m_com.Substring(3)), 1, "W", "0185", 0, 1);
                else
                    cmd = GetFP23Cmd(Int32.Parse(m_com.Substring(3)), 1, "W", "0185", 0, 0);
                sport.Write(cmd, 0, cmd.GetLength(0));
                System.Threading.Thread.Sleep(20);
                Int32 n = sport.Read(recv, 0, 128);
                if (n > 0)
                {
                    if (GetError(recv))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// 设置RUN
        /// </summary>
        /// <param name="temp">0：RESET 1：RUN</param>
        /// <returns>设置成功返回true</returns>
        public bool SetRun(bool bRun)
        {
            if (!bInit)
                return false;
            if (sport.IsOpen)
            {
                byte[] cmd;
                if (bRun)
                    cmd = GetFP23Cmd(Int32.Parse(m_com.Substring(3)), 1, "W", "0190", 0, 1);
                else
                    cmd = GetFP23Cmd(Int32.Parse(m_com.Substring(3)), 1, "W", "0190", 0, 0);
                sport.Write(cmd, 0, cmd.GetLength(0));
                System.Threading.Thread.Sleep(20);
                Int32 n = sport.Read(recv, 0, 128);
                if (n > 0)
                {
                    if (GetError(recv))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// 设置程序控制
        /// </summary>
        /// <param name="temp">0: OFF 1: ON</param>
        /// <returns>设置成功返回true</returns>
        public bool SetHLD(bool bHld)
        {
            if (!bInit)
                return false;
            if (sport.IsOpen)
            {
                byte[] cmd;
                if (bHld)
                    cmd = GetFP23Cmd(Int32.Parse(m_com.Substring(3)), 1, "W", "0191", 0, 1);
                else
                    cmd = GetFP23Cmd(Int32.Parse(m_com.Substring(3)), 1, "W", "0191", 0, 0);
                sport.Write(cmd, 0, cmd.GetLength(0));
                System.Threading.Thread.Sleep(20);
                Int32 n = sport.Read(recv, 0, 128);
                if (n > 0)
                {
                    if (GetError(recv))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// 设置程序远行
        /// </summary>
        /// <param name="temp">0: OFF 1: ON</param>
        /// <returns>设置成功返回true</returns>
        public bool SetADV(bool bAdv)
        {
            if (!bInit)
                return false;
            if (sport.IsOpen)
            {
                byte[] cmd;
                if (bAdv)
                    cmd = GetFP23Cmd(Int32.Parse(m_com.Substring(3)), 1, "W", "0192", 0, 1);
                else
                    cmd = GetFP23Cmd(Int32.Parse(m_com.Substring(3)), 1, "W", "0192", 0, 0);
                sport.Write(cmd, 0, cmd.GetLength(0));
                System.Threading.Thread.Sleep(20);
                Int32 n = sport.Read(recv, 0, 128);
                if (n > 0)
                {
                    if (GetError(recv))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// 设置自整定CH1/CH2 同时
        /// </summary>
        /// <param name="temp">0: OFF 1: ON</param>
        /// <returns>设置成功返回true</returns>
        public bool SetAtCH(bool bAt)
        {
            if (!bInit)
                return false;
            if (sport.IsOpen)
            {
                byte[] cmd;
                if (bAt)
                    cmd = GetFP23Cmd(Int32.Parse(m_com.Substring(3)), 1, "W", "0244", 0, 1);
                else
                    cmd = GetFP23Cmd(Int32.Parse(m_com.Substring(3)), 1, "W", "0244", 0, 0);
                sport.Write(cmd, 0, cmd.GetLength(0));
                System.Threading.Thread.Sleep(20);
                Int32 n = sport.Read(recv, 0, 128);
                if (n > 0)
                {
                    if (GetError(recv))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// 设置手动操作（CH1/CH2 同时）
        /// </summary>
        /// <param name="temp">0: OFF 1: ON</param>
        /// <returns>设置成功返回true</returns>
        public bool SetMANCH(bool bMan)
        {
            if (!bInit)
                return false;
            if (sport.IsOpen)
            {
                byte[] cmd;
                if (bMan)
                    cmd = GetFP23Cmd(Int32.Parse(m_com.Substring(3)), 1, "W", "0245", 0, 1);
                else
                    cmd = GetFP23Cmd(Int32.Parse(m_com.Substring(3)), 1, "W", "0245", 0, 0);
                sport.Write(cmd, 0, cmd.GetLength(0));
                System.Threading.Thread.Sleep(20);
                Int32 n = sport.Read(recv, 0, 128);
                if (n > 0)
                {
                    if (GetError(recv))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// 设置程序设定（CH1/CH2 同时)
        /// </summary>
        /// <param name="temp">0: OFF 1: ON</param>
        /// <returns>设置成功返回true</returns>
        public bool SetRunCH(bool bRun)
        {
            if (!bInit)
                return false;
            if (sport.IsOpen)
            {
                byte[] cmd;
                if (bRun)
                    cmd = GetFP23Cmd(Int32.Parse(m_com.Substring(3)), 1, "W", "0250", 0, 1);
                else
                    cmd = GetFP23Cmd(Int32.Parse(m_com.Substring(3)), 1, "W", "0250", 0, 0);
                sport.Write(cmd, 0, cmd.GetLength(0));
                System.Threading.Thread.Sleep(20);
                Int32 n = sport.Read(recv, 0, 128);
                if (n > 0)
                {
                    if (GetError(recv))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// 程序控制（CH1/CH2 同时）
        /// </summary>
        /// <param name="temp">0: OFF 1: ON</param>
        /// <returns>设置成功返回true</returns>
        public bool SetHLDCH(bool bHld)
        {
            if (!bInit)
                return false;
            if (sport.IsOpen)
            {
                byte[] cmd;
                if (bHld)
                    cmd = GetFP23Cmd(Int32.Parse(m_com.Substring(3)), 1, "W", "0251", 0, 1);
                else
                    cmd = GetFP23Cmd(Int32.Parse(m_com.Substring(3)), 1, "W", "0251", 0, 0);
                sport.Write(cmd, 0, cmd.GetLength(0));
                System.Threading.Thread.Sleep(20);
                Int32 n = sport.Read(recv, 0, 128);
                if (n > 0)
                {
                    if (GetError(recv))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// 程序运行（CH1/CH2 同时）
        /// </summary>
        /// <param name="temp">0: OFF 1: ON</param>
        /// <returns>设置成功返回true</returns>
        public bool SetADVCH(bool bAdv)
        {
            if (!bInit)
                return false;
            if (sport.IsOpen)
            {
                byte[] cmd;
                if (bAdv)
                    cmd = GetFP23Cmd(Int32.Parse(m_com.Substring(3)), 1, "W", "0252", 0, 1);
                else
                    cmd = GetFP23Cmd(Int32.Parse(m_com.Substring(3)), 1, "W", "0252", 0, 0);
                sport.Write(cmd, 0, cmd.GetLength(0));
                System.Threading.Thread.Sleep(20);
                Int32 n = sport.Read(recv, 0, 128);
                if (n > 0)
                {
                    if (GetError(recv))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// 设置DO_MD参数
        /// </summary>
        /// <param name="index">DO的索引，1-13</param>
        /// <param name="dataPB">索引对应的MD值</param>
        /// <returns></returns>
        public bool SetDO_MD(int index, int data)
        {
            if (!bInit)
                return false;
            if (index < 1 || index > 13)
            {
                return false;
            }
            if (sport.IsOpen)
            {
                byte[] cmd;
                int t = 1304 + (index - 1) * 5;
                for (int i = 0; i < 3; i++)
                {
                    cmd = GetFP23Cmd(Int32.Parse(m_com.Substring(3)), 1, "W", string.Format("{0:x4}", t + i), 0, data);
                    sport.Write(cmd, 0, cmd.GetLength(0));
                    System.Threading.Thread.Sleep(20);
                    Int32 n = sport.Read(recv, 0, 128);
                    if (n > 0)
                    {
                        if (!GetError(recv))
                        {
                            return false;
                        }
                    }
                    else
                        return false;
                }
                return true;
            }
            return false;
        }
        /// <summary>
        /// 设置DO_DF参数
        /// </summary>
        /// <param name="index">DO的索引，1-13</param>
        /// <param name="dataPB">索引对应的DF值</param>
        /// <returns></returns>
        public bool SetDO_DF(int index, int data)
        {
            if (!bInit)
                return false;
            if (index < 1 || index > 13)
            {
                return false;
            }
            if (sport.IsOpen)
            {
                byte[] cmd;
                int t = 1306 + (index - 1) * 5;
                for (int i = 0; i < 3; i++)
                {
                    cmd = GetFP23Cmd(Int32.Parse(m_com.Substring(3)), 1, "W", string.Format("{0:x4}", t + i), 0, data);
                    sport.Write(cmd, 0, cmd.GetLength(0));
                    System.Threading.Thread.Sleep(20);
                    Int32 n = sport.Read(recv, 0, 128);
                    if (n > 0)
                    {
                        if (!GetError(recv))
                        {
                            return false;
                        }
                    }
                    else
                        return false;
                }
                return true;
            }
            return false;
        }
        /// <summary>
        /// 设置DO_STB参数
        /// </summary>
        /// <param name="index">DO的索引，1-13</param>
        /// <param name="dataPB">索引对应的STB值</param>
        /// <returns></returns>
        public bool SetDO_STB(int index, int data)
        {
            if (!bInit)
                return false;
            if (index < 1 || index > 13)
            {
                return false;
            }
            if (sport.IsOpen)
            {
                byte[] cmd;
                int t = 1307 + (index - 1) * 5;
                for (int i = 0; i < 3; i++)
                {
                    cmd = GetFP23Cmd(Int32.Parse(m_com.Substring(3)), 1, "W", string.Format("{0:x4}", t + i), 0, data);
                    sport.Write(cmd, 0, cmd.GetLength(0));
                    System.Threading.Thread.Sleep(20);
                    Int32 n = sport.Read(recv, 0, 128);
                    if (n > 0)
                    {
                        if (!GetError(recv))
                        {
                            return false;
                        }
                    }
                    else
                        return false;
                }
                return true;
            }
            return false;
        }
        /// <summary>
        /// 设置DO_TM参数
        /// </summary>
        /// <param name="index">DO的索引，1-13</param>
        /// <param name="dataPB">索引对应的TM值</param>
        /// <returns></returns>
        public bool SetDO_TM(int index, int data)
        {
            if (!bInit)
                return false;
            if (index < 1 || index > 13)
            {
                return false;
            }
            if (sport.IsOpen)
            {
                byte[] cmd;
                int t = 1308 + (index - 1) * 5;
                for (int i = 0; i < 3; i++)
                {
                    cmd = GetFP23Cmd(Int32.Parse(m_com.Substring(3)), 1, "W", string.Format("{0:x4}", t + i), 0, data);
                    sport.Write(cmd, 0, cmd.GetLength(0));
                    System.Threading.Thread.Sleep(20);
                    Int32 n = sport.Read(recv, 0, 128);
                    if (n > 0)
                    {
                        if (!GetError(recv))
                        {
                            return false;
                        }
                    }
                    else
                        return false;
                }
                return true;
            }
            return false;
        }
        /// <summary>
        /// 设置DO_CHR参数
        /// </summary>
        /// <param name="index">DO的索引，1-13</param>
        /// <param name="dataPB">索引对应的CHR值</param>
        /// <returns></returns>
        public bool SetDO_CHR(int index, int data)
        {
            if (!bInit)
                return false;
            if (index < 1 || index > 13)
            {
                return false;
            }
            if (sport.IsOpen)
            {
                byte[] cmd;
                int t = 1309 + (index - 1) * 5;
                for (int i = 0; i < 3; i++)
                {
                    cmd = GetFP23Cmd(Int32.Parse(m_com.Substring(3)), 1, "W", string.Format("{0:x4}", t + i), 0, data);
                    sport.Write(cmd, 0, cmd.GetLength(0));
                    System.Threading.Thread.Sleep(20);
                    Int32 n = sport.Read(recv, 0, 128);
                    if (n > 0)
                    {
                        if (!GetError(recv))
                        {
                            return false;
                        }
                    }
                    else
                        return false;
                }
                return true;
            }
            return false;
        }
        /// <summary>
        /// 设置DI参数
        /// </summary>
        /// <param name="index">DI的索引，1-10</param>
        /// <param name="dataPB">索引对应的DI信息</param>
        /// <returns></returns>
        public bool SetDIInfo(int index, int data)
        {
            if (!bInit)
                return false;
            if (index < 1 || index > 10)
            {
                return false;
            }
            if (sport.IsOpen)
            {
                byte[] cmd;
                int t = 1408 + index - 1;
                for (int i = 0; i < 3; i++)
                {
                    cmd = GetFP23Cmd(Int32.Parse(m_com.Substring(3)), 1, "W", string.Format("{0:x4}", t + i), 0, data);
                    sport.Write(cmd, 0, cmd.GetLength(0));
                    System.Threading.Thread.Sleep(20);
                    Int32 n = sport.Read(recv, 0, 128);
                    if (n > 0)
                    {
                        if (!GetError(recv))
                        {
                            return false;
                        }
                    }
                    else
                        return false;
                }
                return true;
            }
            return false;
        }
        /// <summary>
        /// 设置加热器过载报警
        /// </summary>
        /// <param name="data">0.0 to 50.0A(0.0=OFF)</param>
        /// <returns></returns>
        public bool SetHBS(float data)
        {
            if (!bInit)
                return false;
            if (data < 0.0 || data > 50.0)
            {
                return false;
            }
            if (sport.IsOpen)
            {
                byte[] cmd;
                cmd = GetFP23Cmd(Int32.Parse(m_com.Substring(3)), 1, "W", "0590", 0,(int)(data*10));
                sport.Write(cmd, 0, cmd.GetLength(0));
                System.Threading.Thread.Sleep(20);
                Int32 n = sport.Read(recv, 0, 128);
                if (n > 0)
                {
                    if (GetError(recv))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// 设置加热器循环报警
        /// </summary>
        /// <param name="data">0.0 to 50.0A(0.0=OFF)</param>
        /// <returns></returns>
        public bool SetHBL(float data)
        {
            if (!bInit)
                return false;
            if (data < 0.0 || data > 50.0)
            {
                return false;
            }
            if (sport.IsOpen)
            {
                byte[] cmd;
                cmd = GetFP23Cmd(Int32.Parse(m_com.Substring(3)), 1, "W", "0591", 0,(int)(data*10));
                sport.Write(cmd, 0, cmd.GetLength(0));
                System.Threading.Thread.Sleep(20);
                Int32 n = sport.Read(recv, 0, 128);
                if (n > 0)
                {
                    if (GetError(recv))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// 设置加热器过载模式
        /// </summary>
        /// <param name="data">0: Lock 1: Real</param>
        /// <returns></returns>
        public bool SetHB_MD(bool data)
        {
            if (!bInit)
                return false;
            if (sport.IsOpen)
            {
                byte[] cmd;
                if (data)
                    cmd = GetFP23Cmd(Int32.Parse(m_com.Substring(3)), 1, "W", "0592", 0, 1);
                else
                    cmd = GetFP23Cmd(Int32.Parse(m_com.Substring(3)), 1, "W", "0592", 0, 0);
                
                sport.Write(cmd, 0, cmd.GetLength(0));
                System.Threading.Thread.Sleep(20);
                Int32 n = sport.Read(recv, 0, 128);
                if (n > 0)
                {
                    if (GetError(recv))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// 设置HB选择模式
        /// </summary>
        /// <param name="data">0: OUT1 1: OUT2</param>
        /// <returns></returns>
        public bool SetHB_SEL(bool data)
        {
            if (!bInit)
                return false;
            if (sport.IsOpen)
            {
                byte[] cmd;
                if (data)
                    cmd = GetFP23Cmd(Int32.Parse(m_com.Substring(3)), 1, "W", "0597", 0, 1);
                else
                    cmd = GetFP23Cmd(Int32.Parse(m_com.Substring(3)), 1, "W", "0597", 0, 0);
                
                sport.Write(cmd, 0, cmd.GetLength(0));
                System.Threading.Thread.Sleep(20);
                Int32 n = sport.Read(recv, 0, 128);
                if (n > 0)
                {
                    if (GetError(recv))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// 设置通信存储模式
        /// </summary>
        /// <param name="data">0: EEP 1: RAM 2: R_E</param>
        /// <returns></returns>
        public bool SetCOMMEM(int data)
        {
            if (!bInit)
                return false;
            if (data < 0 || data > 2)
            {
                return false;
            }
            if (sport.IsOpen)
            {
                byte[] cmd;
                cmd = GetFP23Cmd(Int32.Parse(m_com.Substring(3)), 1, "W", "05B0", 0, data);
                
                sport.Write(cmd, 0, cmd.GetLength(0));
                System.Threading.Thread.Sleep(20);
                Int32 n = sport.Read(recv, 0, 128);
                if (n > 0)
                {
                    if (GetError(recv))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// 设置键锁定
        /// </summary>
        /// <param name="data">0: OFF 1: LOCK1 2: LOCK2 3: LOCK3</param>
        /// <returns></returns>
        public bool SetKLOCK(int data)
        {
            if (!bInit)
                return false;
            if (data < 0 || data > 3)
            {
                return false;
            }
            if (sport.IsOpen)
            {
                byte[] cmd;
                cmd = GetFP23Cmd(Int32.Parse(m_com.Substring(3)), 1, "W", "0611", 0, data);
                
                sport.Write(cmd, 0, cmd.GetLength(0));
                System.Threading.Thread.Sleep(20);
                Int32 n = sport.Read(recv, 0, 128);
                if (n > 0)
                {
                    if (GetError(recv))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// 设置输出模式选择
        /// </summary>
        /// <param name="data">0: Single（单输出） 1: Dual（双输出）</param>
        /// <returns></returns>
        public bool SetOUT_MD(int data)
        {
            if (!bInit)
                return false;
            if (data < 0 || data > 1)
            {
                return false;
            }
            if (sport.IsOpen)
            {
                byte[] cmd;
                cmd = GetFP23Cmd(Int32.Parse(m_com.Substring(3)), 1, "W", "0614", 0, data);
                
                sport.Write(cmd, 0, cmd.GetLength(0));
                System.Threading.Thread.Sleep(20);
                Int32 n = sport.Read(recv, 0, 128);
                if (n > 0)
                {
                    if (GetError(recv))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// 设置启动模式
        /// </summary>
        /// <param name="data">启动模式 No.: 1 to 20</param>
        /// <returns></returns>
        public bool SetST_PTN(int data)
        {
            if (!bInit)
                return false;
            if (data < 1 || data > 20)
            {
                return false;
            }
            if (sport.IsOpen)
            {
                byte[] cmd;
                cmd = GetFP23Cmd(Int32.Parse(m_com.Substring(3)), 1, "W", "0802", 0, data);
                
                sport.Write(cmd, 0, cmd.GetLength(0));
                System.Threading.Thread.Sleep(20);
                Int32 n = sport.Read(recv, 0, 128);
                if (n > 0)
                {
                    if (GetError(recv))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// 设置阶跃SV 值
        /// </summary>
        /// <param name="data">阶跃SV 值: :在测量范围内</param>
        /// <returns></returns>
        public bool SetSTEP_SV(int data)
        {
            if (!bInit)
                return false;
            if (sport.IsOpen)
            {
                byte[] cmd;
                cmd = GetFP23Cmd(Int32.Parse(m_com.Substring(3)), 1, "W", "0950", 0, data);
                
                sport.Write(cmd, 0, cmd.GetLength(0));
                System.Threading.Thread.Sleep(20);
                Int32 n = sport.Read(recv, 0, 128);
                if (n > 0)
                {
                    if (GetError(recv))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// 设置阶跃时间
        /// </summary>
        /// <param name="data">00: 00 to 99: 59(unit: sec or min)</param>
        /// <returns></returns>
        public bool SetSTEP_TM(int data)
        {
            if (!bInit)
                return false;
            if (sport.IsOpen)
            {
                byte[] cmd;
                cmd = GetFP23Cmd(Int32.Parse(m_com.Substring(3)), 1, "W", "0951", 0, data);
                
                sport.Write(cmd, 0, cmd.GetLength(0));
                System.Threading.Thread.Sleep(20);
                Int32 n = sport.Read(recv, 0, 128);
                if (n > 0)
                {
                    if (GetError(recv))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// 设置阶跃PID
        /// </summary>
        /// <param name="data">0 to 10</param>
        /// <returns></returns>
        public bool SetSTEP_PID(int data)
        {
            if (!bInit)
                return false;
            if (data < 0 || data > 10)
            {
                return false;
            }
            if (sport.IsOpen)
            {
                byte[] cmd;
                cmd = GetFP23Cmd(Int32.Parse(m_com.Substring(3)), 1, "W", "0952", 0, data);
                
                sport.Write(cmd, 0, cmd.GetLength(0));
                System.Threading.Thread.Sleep(20);
                Int32 n = sport.Read(recv, 0, 128);
                if (n > 0)
                {
                    if (GetError(recv))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// 获取串口接收到的数据，如温度等
        /// </summary>
        /// <param name="data">接收到的byte数组</param>
        /// <returns>返回数据结果</returns>
        private void GetData(byte[] data,int ncount,out int[] result)
        {
            int[] a = new int[4];
            result = new int[ncount];
            for (int n = 0; n < ncount;n++ )
            {
                for (int i = 0; i < 4; i++)
                {
                    if (data[8 + i + n*4 ] >= '0' && data[8 + i + n*4] <= '9')
                    {
                        a[i] = data[8 + i + n*4] - '0';
                    }
                    else if (data[8 + i + n*4] >= 'A' && data[8 + i + n*4] <= 'F')
                    {
                        a[i] = data[8 + i + n*4] - 'A' + 10;
                    }
                }
                result[n] = a[0] * 16 * 16 * 16 + a[1] * 16 * 16 + a[2] * 16 + a[3];
            }
        }
        /// <summary>
        /// 将float转换为int
        /// </summary>
        /// <param name="m_f">只在实际值是表值的10倍时，如3.0实际传入时需要变成30</param>
        /// <returns>返回转换后的int值</returns>
        private int FloatToInt(float m_f) 
        {
            int a = 0;
            if ((float)m_f*10.0 - (int)(m_f*10) >= 0.5 )
            {
                a = (int)(m_f*10) + 1;
            }
            else
            {
                a = (int)(m_f * 10);
            }
            return a;
        }
        /// <summary>
        /// 获取错误信息
        /// </summary>
        /// <param name="recv">参数recv是接收到的byte数组</param>
        /// <returns>正确应答返回TRUE，错误应答返回FALSE</returns>
        private bool GetError(byte[] recv)
        {
            if (recv[5] == '0' && recv[6] == '0')
            {
                error = "正确的应答";
                return true;
            }
            else
            {
                if (recv[5] == '0' && recv[6] == '1')
                    error = "硬件错误";
                else if (recv[5] == '0' && recv[6] == '7')
                    error = "格式错误";
                else if (recv[5] == '0' && recv[6] == '8')
                    error = "命令或数据的数量错误";
                else if (recv[5] == '0' && recv[6] == '9')
                    error = "数据错误";
                else if (recv[5] == '0' && recv[6] == 'A')
                    error = "执行命令错误";
                else if (recv[5] == '0' && recv[6] == 'B')
                    error = "写模式错误";
                else if (recv[5] == '0' && recv[6] == 'C')
                    error = "其他或操作错误";
                else
                    error = "未知错误";
            }
            return false;
        }
    }
}
