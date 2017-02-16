using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EM9118
{
    public class tools
    {
        /// <summary>
        /// 设置IP地址  SET_IPADDR
        /// <param name="IPstr">IP地址</param>
        /// <returns>16进制字串</returns>
        /// </summary>
        public String IP2HexStr(String IPstr)
        {
            String decStr = String.Empty;
            String temp;
            int dec;
            int count;

            //分割字符串
            String[] sArray = IPstr.Split(new char[] {'.'});
            foreach (string i in sArray)
            {
                //遍历所有字符串，转化成16进制的字符串
                dec = Convert.ToInt32(i, 10);
                temp = Convert.ToString(dec, 16);
                count = temp.Length;
                if (count < 2)
                {
                    temp = "0" + temp; 
                }
                decStr += temp;
              
            }
            decStr = decStr.Insert(6, " ");
            decStr = decStr.Insert(4, " ");
            decStr = decStr.Insert(2, " ");
            return decStr;
        }


        /// <summary>
        /// 字串转成16进制字串 2字节
        /// <param name="str">10进制字串</param>
        /// <returns>2字节 16进制字串</returns>
        /// </summary>
        public String str2Hex2(String str)
        {
            String decstr = string.Empty;
            String temp;
            int dec;
            int count;

            dec = Convert.ToInt32(str, 10);
            temp = Convert.ToString(dec, 16);

            count = temp.Length;
            switch (count)
            {
                case 1:
                    temp = "000" + temp;
                    break;
                case 2:
                    temp = "00" + temp;
                    break;
                case 3:
                    temp = "0" + temp;
                    break;
                default:
                    break;
            }
            decstr = temp.Insert(2, " ");
            return decstr;
        }


        /// <summary>
        /// 字串转成16进制字串 4字节
        /// <param name="str">10进制字串</param>
        /// <returns>4字节 16进制字串</returns>
        /// </summary>
        public String str2Hex4(String str)
        {
            String decstr = string.Empty;
            String temp;
            int dec;
            int count;

            dec = Convert.ToInt32(str, 10);
            temp = Convert.ToString(dec, 16);

            count = temp.Length;
            switch (count)
            {
                case 1:
                    temp = "0000000" + temp;
                    break;
                case 2:
                    temp = "000000" + temp;
                    break;
                case 3:
                    temp = "00000" + temp;
                    break;
                case 4:
                    temp = "0000" + temp;
                    break;
                case 5:
                    temp = "000" + temp;
                    break;
                case 6:
                    temp = "00" + temp;
                    break;
                case 7:
                    temp = "0" + temp;
                    break;
                default:
                    break;
            }
            decstr = temp.Insert(6, " ");
            decstr = decstr.Insert(4, " ");
            decstr = decstr.Insert(2, " ");
            return decstr;
        }

        /// <summary>
        /// 字串转成 byte[]
        /// <param name="str">字符串</param>
        /// <returns>byte数组</returns>
        /// </summary>
        public byte[] hex2byte(String str)
        {
            str += " ";
            byte[] arr = new byte[(str.Length / 3)];
            String tmp;
            int i = 0;
            while (str != "")
            {
                tmp = str.Substring(0, 3);
                tmp = tmp.Substring(0, 2);
                arr[i] = (byte)Convert.ToInt32(tmp, 16);
                str = str.Substring(3, str.Length - 3);
                i++;
            }
            return arr;
        }


        /// <summary>
        /// byte[] 转16进制字符串
        /// <param name="arr">数组</param>
        /// <param name="len">长度</param>
        /// <returns>byte数组</returns>
        /// </summary>
        public String byte2str(byte[] arr, int len)
        {
            String str = String.Empty;
            String tmp;
            for (int i = 0; i < len; i++)
            {
                tmp = Convert.ToString(arr[i], 16);
                if (tmp.Length == 1)
                {
                    tmp = "0" + tmp;
                }
                str += tmp;
                str += " ";
            }
            str = str.Substring(0, str.Length - 1);
            return str;
        }
    }
}
