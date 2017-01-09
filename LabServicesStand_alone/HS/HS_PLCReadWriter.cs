using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabMCESystem.Servers.HS
{
    /// <summary>
    /// 提供了与环散系统PLC OPC服务器读写连接的方法集。
    /// </summary>
    static class HS_PLCReadWriter
    {
        private static object _opcConnecter;

        public static object OpcConnecter
        {
            get { return _opcConnecter; }
            private set { _opcConnecter = value; }
        }

        public static bool WriteStatus(string design, bool status)
        {
            return true;
        }

        public static bool ReadStatus(string design)
        {
            return (new Random().Next(0, 100) % 2) == 0;
        }

        public static bool WriteAnaloge(string design, double value)
        {
            return true;
        }

        public static double ReadAnaloge(string design)
        {
            return new Random().Next(0, 100);
        }

        public static bool State { get { return false; } }

        public static bool ConnectOPCSever()
        {
            return true;
        }

        public static bool QuitConnecter()
        {
            return true;
        }
    }
}
