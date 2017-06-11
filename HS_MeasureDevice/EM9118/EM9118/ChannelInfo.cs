using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EM9118
{
     class ChannelInfo
    {
        #region 成员变量
        int boardNum;
        #endregion

        #region 构造函数
        /// <summary>
        /// 初始化板卡号 和 通道号
        /// </summary>
        /// <param name="boardNum">板卡号</param>
        public ChannelInfo(int boardNum)
        {
            this.boardNum = boardNum;
        }
        #endregion

        #region 校准采集的数据
        static public double getCorrectData(int boardNum, int channelNum, double data)
        {
            double correctData;  //实际的数据

            if (boardNum > 8 || boardNum < 1)
            {
                return -1;
            }
            switch (boardNum)
            {
                case 1:
                    correctData = board_1(channelNum, data);
                    break;
                case 2:
                    correctData = board_2(channelNum, data);
                    break;
                case 3:
                    correctData = board_3(channelNum, data);
                    break;
                case 4:
                    correctData = board_4(channelNum, data);
                    break;
                case 5:
                    correctData = board_5(channelNum, data);
                    break;
                case 6:
                    correctData = board_6(channelNum, data);
                    break;
                case 7:
                    correctData = board_7(channelNum, data);
                    break;
                case 8:
                    correctData = board_8(channelNum, data);
                    break;
                default:
                    correctData = -1;
                    break;
            }
            return correctData;
        }
        #endregion

        #region 各个通道采集校准
        /// <summary>
        /// 板卡1 通道校准
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        static private double board_1(int channel, double v)
        {
            double realValue;
            switch (channel)
            {
                case 1:
                    realValue = 4.0921 * v - 0.0054;
                    break;
                case 2:
                    realValue = 4.0859 * v - 0.0043;
                    break;
                case 3:
                    realValue = 4.0873 * v - 0.0034;
                    break;
                case 4:
                    realValue = 4.0834 * v - 0.0059;
                    break;
                case 5:
                    realValue = 4.0921 * v - 0.006;
                    break;
                case 6:
                    realValue = 4.0885 * v - 0.0049;
                    break;
                default:
                    realValue = 0.0;
                    break;
            }
            return realValue;
        }

        /// <summary>
        /// 板卡2 通道校准
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        static private double board_2(int channel, double v)
        {
            double realValue;
            switch (channel)
            {
                case 1:
                    realValue = 4.0914 * v - 0.0076;
                    break;
                case 2:
                    realValue = 4.0936 * v - 0.0065;
                    break;
                case 3:
                    realValue = 4.0922 * v - 0.0036;
                    break;
                case 4:
                    realValue = 4.0957 * v - 0.0015;
                    break;
                case 5:
                    realValue = 4.0885 * v - 0.004;
                    break;
                case 6:
                    realValue = 4.0921 * v - 0.0055;
                    break;
                default:
                    realValue = 0.0;
                    break;
            }
            return realValue;
        }

        /// <summary>
        /// 板卡3 通道校准
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        static private double board_3(int channel, double v)
        {
            double realValue;
            switch (channel)
            {
                case 1:
                    realValue = 4.0853 * v - 0.0046;
                    break;
                case 2:
                    realValue = 4.0823 * v - 0.00478;
                    break;
                case 3:
                    realValue = 4.0913 * v - 0.0022;
                    break;
                case 4:
                    realValue = 4.0886 * v - 0.0034;
                    break;
                case 5:
                    realValue = 4.0935 * v - 0.0028;
                    break;
                case 6:
                    realValue = 4.0904 * v - 0.0005;
                    break;
                default:
                    realValue = 0.0;
                    break;
            }
            return realValue;
        }

        /// <summary>
        /// 板卡4 通道校准
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        static private double board_4(int channel, double v)
        {
            double realValue;
            switch (channel)
            {
                case 1:
                    realValue = 4.0927 * v - 0.0071;
                    break;
                case 2:
                    realValue = 4.092 * v - 0.0054;
                    break;
                case 3:
                    realValue = 4.0851 * v - 0.0012;
                    break;
                case 4:
                    realValue = 4.0865 * v - 0.0017;
                    break;
                case 5:
                    realValue = 4.0875 * v - 0.0019;
                    break;
                case 6:
                    realValue = 4.0929 * v - 0.0055;
                    break;
                default:
                    realValue = 0.0;
                    break;
            }
            return realValue;
        }

        /// <summary>
        /// 板卡5 通道校准
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        static private double board_5(int channel, double v)
        {
            double realValue;
            switch (channel)
            {
                case 1:
                    realValue = 4.0767 * v - 0.0027;
                    break;
                case 2:
                    realValue = 4.0946 * v - 0.0073;
                    break;
                case 3:
                    realValue = 4.0952 * v - 0.0013;
                    break;
                case 4:
                    realValue = 4.0931 * v - 0.0036;
                    break;
                case 5:
                    realValue = 4.095 * v - 0.002;
                    break;
                case 6:
                    realValue = 4.0313 * v - 0.003;
                    break;
                default:
                    realValue = 0.0;
                    break;
            }
            return realValue;
        }

        /// <summary>
        /// 板卡6 通道校准
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        static private double board_6(int channel, double v)
        {
            double realValue;
            switch (channel)
            {
                case 1:
                    realValue = 4.0836 * v - 0.0012;
                    break;
                case 2:
                    realValue = 4.0906 * v - 0.0061;
                    break;
                case 3:
                    realValue = 4.0822 * v - 0.0026;
                    break;
                case 4:
                    realValue = 4.0852 * v - 0.0009;
                    break;
                case 5:
                    realValue = 4.0854 * v - 0.0018;
                    break;
                case 6:
                    realValue = 4.09 * v - 0.0031;
                    break;
                default:
                    realValue = 0.0;
                    break;
            }
            return realValue;
        }

        /// <summary>
        /// 板卡7 通道校准
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        static private double board_7(int channel, double v)
        {
            double realValue;
            switch (channel)
            {
                case 1:
                    realValue = 4.096 * v - 0.0083;
                    break;
                case 2:
                    realValue = 4.0926 * v - 0.0131;
                    break;
                case 3:
                    realValue = 4.0949 * v - 0.0007;
                    break;
                case 4:
                    realValue = 4.0933 * v - 0.0023;
                    break;
                case 5:
                    realValue = 4.0914 * v - 0.0032;
                    break;
                case 6:
                    realValue = 4.0945 * v - 0.0001;
                    break;
                default:
                    realValue = 0.0;
                    break;
            }
            return realValue;
        }

        /// <summary>
        /// 板卡8 通道校准
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        static private double board_8(int channel, double v)
        {
            double realValue;
            switch (channel)
            {
                case 1:
                    realValue = 4.1008 * v - 0.0073;
                    break;
                case 2:
                    realValue = 4.0982 * v - 0.0073;
                    break;
                case 3:
                    realValue = 4.097 * v - 0.0016;
                    break;
                case 4:
                    realValue = 4.094 * v - 0.0016;
                    break;
                case 5:
                    realValue = 4.0933 * v - 0.002;
                    break;
                case 6:
                    realValue = 4.0966 * v - 0.0026;
                    break;
                default:
                    realValue = 0.0;
                    break;
            }
            return realValue;
        }
        #endregion
    }
}
