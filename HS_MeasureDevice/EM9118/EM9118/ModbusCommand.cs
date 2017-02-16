using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EM9118
{
    public class ModbusCommand : tools
    {
        /// <summary>
        /// Modbus  设置IP地址
        /// <param name="paraStr">IP地址</param>
        /// <returns>封装好的请求协议</returns>
        /// </summary>
        /// 0

        public String setIPAddr(String paramStr)
        {
            String decStr = String.Empty;
            decStr = "00 00 00 00 00 0b 01 10 00 00 00 02 04 ";
            decStr = decStr + IP2HexStr(paramStr);
            return decStr;
        }


        /// <summary>
        /// Modbus  设置子网掩码
        /// <param name="paramStr">子网掩码</param>
        /// <returns>封装好的请求协议</returns>
        /// </summary>
        /// 1
        public String setSubNetMark(String paramStr)
        {
            String decStr = String.Empty;
            decStr = "00 00 00 00 00 0b 01 10 00 02 00 02 04 ";
            decStr = decStr + IP2HexStr(paramStr);
            return decStr;
        }


        /// <summary>
        /// Modbus  设置数据端口号
        /// <param name="paramStr">端口号</param>
        /// <returns>封装好的请求协议</returns>
        /// </summary>
        /// 2
        public String setDataPorn(String porn)
        {
            String decStr = String.Empty;
            decStr = "00 00 00 00 00 09 01 10 00 04 00 01 02 ";
            decStr = decStr + str2Hex2(porn);
            return decStr;
        }


        /// <summary>
        /// Modbus  设置网关地址
        /// <param name="paramStr">网关地址</param>
        /// <returns>封装好的请求协议</returns>
        /// </summary>
        /// 3
        public String setNetAddr(String paramStr)
        {
            String decStr = String.Empty;
            decStr = "00 00 00 00 00 0b 01 10 00 07 00 02 04 ";
            decStr = decStr + IP2HexStr(paramStr);
            return decStr;
        }


        /// <summary>
        /// Modbus  设置端口号
        /// <param name="paramStr">端口号</param>
        /// <returns>封装好的请求协议</returns>
        /// </summary>
        /// 4
        public String setComPorn(String paramStr)
        {
            String decStr = String.Empty;
            decStr = "00 00 00 00 00 09 01 10 00 09 00 01 02 ";
            decStr = decStr + str2Hex2(paramStr);
            return decStr;
        }


        /// <summary>
        /// Modbus  设置AD采集范围
        /// <param name="paramStr">端口号</param>
        /// <param name="markID">功能码</param>
        /// <returns>封装好的请求协议</returns>
        /// </summary>
        /// 6
        public String setADRange(String paramStr, int markID)
        {
            String decStr = String.Empty;
            decStr = "00 00 00 00 00 09 01 ";
            decStr += Convert.ToString(markID, 16);
            decStr += " 00 0a 00 01 02 ";
            decStr += str2Hex2(paramStr);
            return decStr;
        }


        /// <summary>
        /// Modbus  设置AD采集通道 传4字节16进制数
        /// <param name="paramStr">通道</param>
        /// <returns>封装好的请求协议</returns>
        /// </summary>
        /// 7
        public String setADChannel(String paramStr)
        {
            String decStr = String.Empty;
            decStr = "00 00 00 00 00 0b 01 10 00 0E 00 02 04 ";
            paramStr = paramStr.Insert(6, " ");
            paramStr = paramStr.Insert(4, " ");
            paramStr = paramStr.Insert(2, " ");

            decStr += paramStr;
            return decStr;
        }


        /// <summary>
        /// Modbus  设置组频率
        /// <param name="paramStr">频率</param>
        /// <returns>封装好的请求协议</returns>
        /// </summary>
        /// 8
        public String setGroupHZ(String paramStr)
        {
            int tmp;
            String decStr = String.Empty;
            String subStr1;
            String subStr2;
            decStr = "00 00 00 00 00 0b 01 10 00 10 00 02 04 ";
            tmp = Convert.ToInt32(paramStr, 10);
            tmp = 36000000 / (tmp * 1000);
            paramStr = Convert.ToString(tmp, 10);
            subStr1 = str2Hex4(paramStr).Substring(0, 5);
            subStr2 = str2Hex4(paramStr).Substring(6, 5);
            decStr += subStr2 + " " + subStr1;
            return decStr;
        }


        /// <summary>
        /// Modbus  设置每帧组数
        /// <param name="paramStr">组数</param>
        /// <returns>封装好的请求协议</returns>
        /// </summary>
        /// 9
        public String setFroupFTP(String paramStr)
        {
            String decStr = String.Empty;
            decStr = "00 00 00 00 00 09 01 10 00 14 00 01 02 ";
            decStr += str2Hex2(paramStr);
            return decStr;
        }


        /// <summary>
        /// Modbus  启动或停止硬件控制采集
        /// <param name="paramStr">组数</param>
        /// <returns>封装好的请求协议</returns>
        /// </summary>
        /// 10
        public String startOrStopHardWare(String paramStr)
        {
            
            String decStr = String.Empty;
            //String temp;
            int tmp;
            int count;
            decStr = "00 00 00 00 00 09 01 10 00 16 00 01 02 00 ";
            tmp = Convert.ToInt32(paramStr, 10);
            paramStr = Convert.ToString(tmp, 16);
            count = paramStr.Length;
            if (count == 1)
            {
                paramStr = "0" + paramStr;
            }
            decStr += paramStr;
            return decStr;
        }


        /// <summary>
        /// Modbus  启动或停止硬件控制采集
        /// <param name="isWork">是否主动传输</param>
        /// <returns>封装好的请求协议</returns>
        /// </summary>
        /// 11
        public String setActiveTransport(bool isWork)
        {
            String decStr = String.Empty;
            decStr = "00 00 00 00 00 09 01 10 00 17 00 01 02 00 ";
            if (isWork)
            {
                decStr +="01";
            }
            else
            {
                decStr += "00";
            }
            return decStr;
        }


        /// <summary>
        /// Modbus  设置外触发采集组数
        /// <param name="paramGroup">采集组数</param>
        /// <returns>封装好的请求协议</returns>
        /// </summary>
        /// 12
        public String setOutSide(String paramGroup)
        {
            String decStr = String.Empty;
            decStr = "00 00 00 00 00 0b 01 10 00 1A 00 02 04 ";
            decStr += str2Hex4(paramGroup);
            return decStr;
        }


        /// <summary>
        /// Modbus  设置DO管脚功能
        /// <param name="DO1">DO1以及功能</param>
        /// <param name="DO2">DO2以及功能</param>
        /// <param name="markID">功能码</param>
        /// <returns>封装好的请求协议</returns>
        /// </summary>
        /// 13
        public String setDOFun(int DO1, int DO2, int markID)
        {
            String decStr = String.Empty;
            String str;
            int tmp;
            decStr = "00 00 00 00 00 09 01 ";
            decStr += Convert.ToString(markID, 16);
            decStr += " 00 22 00 01 02 ";

            str = Convert.ToString(DO1, 16);
            tmp = str.Length;
            if (tmp == 1)
            {
                str = "0" + str;
            }
            decStr += str;

            str = Convert.ToString(DO2, 16);
            tmp = str.Length;
            if (tmp == 1)
            {
                str = "0" + str;
            }
            decStr += " " + str;
            return decStr;
        }


        /// <summary>
        /// Modbus  设置IO输出值
        /// <param name="DO">DO以及功能</param>
        /// <param name="markID">功能码</param>
        /// <returns>封装好的请求协议</returns>
        /// </summary>
        /// 14
        public String setIOOutputValue(int DO, int markID)
        {
            String decStr = String.Empty;
            String tmp;
            int count;
            decStr = "00 00 00 00 00 09 01 ";
            decStr += Convert.ToString(markID, 16);
            decStr += " 00 24 00 01 02 00 ";
            
            tmp = Convert.ToString(DO, 16);
            count = tmp.Length;
            if (count == 1)
            {
                tmp = "0" + tmp;
            }
            decStr += tmp;
            return decStr;
        }


        /// <summary>
        /// Modbus  设置计数器测频基准分频系数
        /// <param name="dividing">分频系数</param>
        /// <param name="markID">功能码</param>
        /// <returns>封装好的请求协议</returns>
        /// </summary>
        /// 15
        public String setFreDiv(String dividing, int markID)
        {
            String decStr = String.Empty;
            decStr = "00 00 00 00 00 0b 01 ";
            decStr += Convert.ToString(markID, 16);
            decStr += " 00 28 00 02 04 ";

            decStr += str2Hex4(dividing);
            return decStr;
        }


        /// <summary>
        /// Modbus  设置计数器工作方式
        /// <param name="workWay">计数器工作方式</param>
        /// <param name="markID">功能码</param>
        /// <returns>封装好的请求协议</returns>
        /// </summary>
        /// 16
        public String setWorkWay(int workWay, int markID)
        {
            String decStr = String.Empty;
            //String temp;
            String tmp;
            int count;
            decStr = "00 00 00 00 00 09 01 ";
            decStr += Convert.ToString(markID, 16);

            tmp = Convert.ToString(workWay, 10);
            count = tmp.Length;
            if (count == 1)
            {
                tmp = "0" + tmp;
            }
            decStr += " 00 34 00 01 02 00 ";
            decStr += tmp;
            return decStr;
        }

        /// <summary>
        /// Modbus  设置PWM输出频率分频系数
        /// <param name="pwm">选择pwm</param>
        /// <param name="div">分频系数</param>
        /// <param name="markID">功能码</param>
        /// <returns>封装好的请求协议</returns>
        /// </summary>
        /// 17
        public String setPwmDiv(int pwm, int div, int markID)
        {
            String decStr = String.Empty;
            String tmp;
            decStr = "00 00 00 00 00 09 01 ";
            decStr += Convert.ToString(markID, 16);
            if (pwm == 1)
            {
                decStr += " 00 50 00 01 02 ";
            }
            else
            {
                decStr += " 00 51 00 01 02 ";
            }
            //divtmp = 4000000 / (div * 1000);
            tmp = Convert.ToString(div, 10);
            decStr += str2Hex2(tmp);
            return decStr;
        }


        /// <summary>
        /// Modbus  设置PWM输出高电平时间系数
        /// <param name="pwm">选择pwm</param>
        /// <param name="time">分频系数</param>
        /// <param name="markID">功能码</param>
        /// <returns>封装好的请求协议</returns>
        /// </summary>
        /// 18
        public String setPwmOutTime(int pwm, int time, int markID)
        {
            String decStr = String.Empty;
            //String temp;
            String tmp;
            decStr = "00 00 00 00 00 09 01 ";
            decStr += Convert.ToString(markID, 16);

            if (pwm == 1)
            {
                decStr += " 00 58 00 01 02 ";
            }
            else
            {
                decStr += " 00 58 00 01 02 ";
            }
            time = time * 4;
            tmp = Convert.ToString(time, 10);
            decStr += str2Hex2(tmp);
            return decStr;
        }


        /// <summary>
        /// Modbus  设置PWM脉冲数
        /// <param name="pwm">选择pwm</param>
        /// <param name="pwmPulse">脉冲数</param>
        /// <param name="markID">功能码</param>
        /// <returns>封装好的请求协议</returns>
        /// </summary>
        /// 19
        public String setPwmPulse(int pwm, int pwmPluse, int markID)
        {
            String decStr = String.Empty;
            //String temp;
            String tmp;
            decStr = "00 00 00 00 00 09 01 ";
            decStr += Convert.ToString(markID, 16);

            if (pwm == 1)
            {
                decStr += " 00 60 00 01 02 ";
            }
            else
            {
                decStr += " 00 61 00 01 02 ";
            }
            tmp = Convert.ToString(pwmPluse, 10);
            decStr += str2Hex2(tmp);
            return decStr;
        }


        /// <summary>
        /// Modbus  设置PWM输出相位  SET_PWMOUT_ASPECTS
        /// <param name="display">是否延时</param>
        /// <param name="markID">功能码</param>
        /// <returns>封装好的请求协议</returns>
        /// </summary>
        /// 20
        public String setPwmOutAspects(String display, int markID)
        {
            String decStr = String.Empty;
            //String temp;
            int tmp;
            decStr = "00 00 00 00 00 09 01 ";
            decStr += Convert.ToString(markID, 16);
            decStr += "00 68 00 01 02 00 ";
            tmp = Convert.ToInt32(display, 10);
            display = Convert.ToString(tmp, 16);
            if(display.Length == 1)
            {
                display = "0" + display;
            }
            decStr += display;
            return decStr;
        }


        /// <summary>
        /// Modbus  使能PWM输出
        /// <param name="pwmOut">是否延时</param>
        /// <param name="markID">功能码</param>
        /// <returns>封装好的请求协议</returns>
        /// </summary>
        /// 21
        public String pwmOutPut(String pwmOut, int markID)
        {
            String decStr = String.Empty;
            //String temp;
            int tmp;

            decStr = "00 00 00 00 00 09 01 ";
            decStr += Convert.ToString(markID, 16);
            decStr += " 00 69 00 01 02 00 ";
            
            tmp = Convert.ToInt32(pwmOut, 10);
            pwmOut = Convert.ToString(tmp, 16);
            if (pwmOut.Length == 1)
            {
                pwmOut = "0" + pwmOut;
            }
            decStr += pwmOut;
            return decStr;
        }


        /// <summary>
        /// Modbus  读取PWM输出是否完成
        /// <returns>封装好的请求协议</returns>
        /// </summary>
        /// 22
        public String readPwm()
        {
            String decStr;
            decStr = "00 00 00 00 00 06 01 03 00 69 00 01";
            return decStr;
        }



        /// <summary>
        /// Modbus  通讯超时设置
        ///param name="time">超时时间</param>
        /// <param name="markID">功能码</param> 
        /// <returns>封装好的请求协议</returns>
        /// </summary>
        /// 23
        public String setConnectTime(String time, int markID)
        {
            String decStr = String.Empty;
            //String temp;
            decStr = "00 00 00 00 00 09 01 ";
            decStr += Convert.ToString(markID, 16);
            decStr += " 00 b4 00 01 02 ";
            decStr += str2Hex2(time);
            return decStr;
        }


        /// <summary>
        /// Modbus  设置下位机时间
        ///param name="year">年</param>
        ///param name="mounth">月</param>
        ///param name="day">日</param>
        ///param name="hour">时</param>
        ///param name="min">分</param>
        ///param name="sec">秒</param>
        /// <returns>封装好的请求协议</returns>
        /// </summary>
        /// 24
        public String setLowerComputerTime(int year, int mounth, int day, int hour, int min, int sec)
        {
            String decStr = String.Empty;
            String tmp;
            decStr = "00 00 00 00 00 0F 01 10 00 bd 00 04 08 ";
            tmp = Convert.ToString(year, 16);
            if (tmp.Length == 1)
            {
                tmp = "0" + tmp;
            }
            decStr += tmp + " ";

            tmp = Convert.ToString(mounth, 16);
            if (tmp.Length == 1)
            {
                tmp = "0" + tmp;
            }
            decStr += tmp + " ";

            tmp = Convert.ToString(day, 16);
            if (tmp.Length == 1)
            {
                tmp = "0" + tmp;
            }
            decStr += tmp + " ";

            tmp = Convert.ToString(hour, 16);
            if (tmp.Length == 1)
            {
                tmp = "0" + tmp;
            }
            decStr += tmp + " ";

            tmp = Convert.ToString(min, 16);
            if (tmp.Length == 1)
            {
                tmp = "0" + tmp;
            }
            decStr += tmp + " ";

            tmp = Convert.ToString(sec, 16);
            if (tmp.Length == 1)
            {
                tmp = "0" + tmp;
            }
            decStr += tmp + " 00 00";
            return decStr;
        }



        /// <summary>
        /// Modbus  读取下位机时间
        /// <returns>封装好的请求协议</returns>
        /// </summary>
        /// 25
        public String getLowerComputerTime()
        {
            String decStr = String.Empty;
            decStr = "00 00 00 00 00 06 01 03 00 bd 00 04";
            return decStr;
        }


        /// <summary>
        /// Modbus 编码器清零 
        ///<param name="param">编码器清零信息</param>
        /// <param name="markID">功能码</param> 
        /// <returns>封装好的请求协议</returns>
        /// </summary>
        /// 26
        public String clearEncoder(String param, int markID)
        {
            String decStr = String.Empty;
            //String temp;
            decStr = "00 00 00 00 00 09 01 ";
            decStr += Convert.ToString(markID, 16);
            decStr += " 00 c1 00 01 02 ";
            decStr += str2Hex2(param);
            return decStr;
        }

        /// <summary>
        /// Modbus 写入AD零点信息 
        ///<param name="channel">AD通道号</param>
        /// <returns>封装好的请求协议</returns>
        /// </summary>
        /// 27
        public String setADZeroInfo(int channel)
        {
            String decStr = String.Empty;
            decStr = "00 00 00 00 00 0b 01 10 00 C8 00 02 04 ";
            decStr += str2Hex2(Convert.ToString(channel, 10));
            decStr += " 00 00";
           
            return decStr;
        }


        /// <summary>
        /// Modbus 写入AD满度信息 
        ///<param name="param">AD满度信息</param>
        /// <returns>封装好的请求协议</returns>
        /// </summary>
        /// 28


        /// <summary>
        /// Modbus 读取零点满度信息 
        /// <returns>封装好的请求协议</returns>
        /// </summary>
        /// 29
        public String readADZeroInfo()
        {
            String decStr = String.Empty;
            decStr = "00 00 00 00 00 06 01 03 00 C8 00 24";
            return decStr;
        }

        /// <summary>
        /// Modbus 读取单次AD数据
        ///  <param name="param">AD通道</param>
        /// <returns>封装好的请求协议</returns>
        /// </summary>
        /// 30
        public String readADOnce(String param)
        {
            String decStr = String.Empty;
            //String temp;
            decStr = "00 00 00 00 00 06 01 03 01 00 ";
            param = str2Hex2(param);
            decStr += param;
            return decStr;
        }


        /// <summary>
        /// Modbus 读取计数器的值
        ///  <param name="param">AD通道</param>
        /// <returns>封装好的请求协议</returns>
        /// </summary>
        /// 31


        /// <summary>
        /// Modbus  读取IO输入值
        /// <returns>封装好的请求协议</returns>
        /// </summary>
        /// 32
        public String readIO()
        {
            String decStr = String.Empty;
            decStr = "00 00 00 00 00 06 01 03 01 1A 00 01";
            return decStr;
        }


        /// <summary>
        /// Modbus  读取编码器数据 
        /// <returns>封装好的请求协议</returns>
        /// </summary>
        /// 33
    }
}
