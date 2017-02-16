using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FP23
{
    public class FP23Cmd
    {
        //保存命令中可以修改的部分
        private string addr;              //设置基地址
        private string addrSec;           //设置下位机的子基地址
        private string cmdType;           //命令类型
        private string cmdCode;           //命令代码
        private string cmdContinuous;     //命令连续码
        private int cmdData;           //通讯数据

        /// <summary>
        /// 设置基地址
        /// <param name="addr">基地址</param>
        /// </summary>
        public void setAddr(int addr)
        {
            this.addr = Convert.ToString(addr, 16);
            if (this.addr.Length <= 1)
            {
                this.addr = "0" + this.addr;
            }
        }

        /// <summary>
        /// 设置基地址
        /// <param name="addrSec">基地址子地址</param>
        /// </summary>
        public void setAddrSec(int addrSec)
        {
            this.addrSec = Convert.ToString(addrSec, 16);
        }

        /// <summary>
        /// 设置命令类型
        /// <param name="cmdType">命令类型</param>
        /// </summary>
        public void setCmdType(string cmdType)
        {
            if (cmdType == "r" || cmdType == "R")
            {
                this.cmdType = "R";
            }
            else if (cmdType == "w" || cmdType == "W")
            {
                this.cmdType = "W";
            }
            else if (cmdType == "b" || cmdType == "B")
            {
                this.cmdType = "B";
            }
            else
            {
                this.cmdType = "";
            }
        }


        /// <summary>
        /// 设置通讯命令代码
        /// <param name="cmdCode">命令代码</param>
        /// </summary>
        public void setCmdCode(string cmdCode)
        {
            this.cmdCode = cmdCode;
        }

        /// <summary>
        /// 设置命令连续码
        /// <param name="cmdContinuous">命令连续码</param>
        /// </summary>
        public void SetcmdContinuous(int cmdContinuous)
        {
            this.cmdContinuous = Convert.ToString(cmdContinuous, 16);
        }

        /// <summary>
        /// 命令数据
        /// <param name="cmdData">命令数据</param>
        /// </summary>
        /// eg：",125,100,45"
        public void setCmdData(int cmdData)
        {
            this.cmdData = cmdData;
        }

        /// <summary>
        /// bcc校验
        /// <param name="bccStr">要校验的数据</param>
        /// <return>校验码</return>
        /// </summary>
        private string bccCheck(string bccStr)
        {
            string tmp;
            byte[] bccTmp;
            int count = 0;
            bccTmp = System.Text.Encoding.ASCII.GetBytes(bccStr);
            for (int i = 0; i < bccTmp.Length; i++)
            {
                count += bccTmp[i];
            }
            tmp = Convert.ToString(count, 16);
            tmp = tmp.ToUpper();
            tmp = tmp.Substring(tmp.Length - 2, 2);
            return tmp;
        } 

        /// <summary>
        /// 获取命令
        /// <param name="addr">基地址</param>
        /// </summary>
        public byte[] getCmd()
        {
            byte[] cmd;
            string tmp = "\u0002";
            tmp += this.addr;
            tmp += this.addrSec;
            if (this.cmdType == "R")
            {
                tmp += this.cmdType;
                tmp += this.cmdCode.ToUpper();
                tmp += this.cmdContinuous;
            }
            else if (this.cmdType == "W" || this.cmdType == "B")
            {
                tmp += this.cmdType;
                tmp += this.cmdCode.ToUpper();
                tmp += this.cmdContinuous;
                tmp += ",";
                string str = string.Format("{0:x4}", this.cmdData);
                tmp += str.ToUpper()/*string.Format("{0:x4}",this.cmdData)*/;
            }
            tmp += "\u0003";
            tmp += bccCheck(tmp);
            //tmp += "\u0015\u0012";
            tmp += "\u000d";
            cmd = System.Text.Encoding.ASCII.GetBytes(tmp);
            return cmd;
        }

        /// <summary>
        /// 写应答
        /// <param name="responseWrite">写应答回应</param>
        /// <return>返回写状态</return>
        /// </summary>
        public int resWrite(byte[] responseWrite)
        {
            string tmp = "";
            tmp = Encoding.ASCII.GetString(responseWrite);
            if (tmp[0] != '\u0002')
            {
                return -1;
            }

            if (tmp[5] != 'W')
            {
                return -1;
            }
            return tmp[7];
        }

        /// <summary>
        /// 读应答
        /// <param name="resRead">读应答回应</param>
        /// <return>返回读数据</return>
        /// </summary>
        public int resRead(byte[] responeRead, string data)
        {
            string tmp = Encoding.ASCII.GetString(responeRead);
            int cmdContinuous = Convert.ToInt32(this.cmdContinuous, 10);

            if (tmp[0] != '\u0002')
            {
                return -1;
            }

            if (tmp[5] != 'R')
            {
                return -1;
            }
            if (tmp[7] == '0')
            {
                data = tmp.Substring(8, cmdContinuous);
                return 0;
            }
            else
            {
                data = "";
                return tmp[7];
            }
        }

    }
}
