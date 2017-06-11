using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HS_DeviceInteract
{
    /// <summary>
    /// OPC 交互接口，封装与OPC交互的常用方法。
    /// </summary>
    public interface IOPCDInteract
    {
        bool AddGroup(string GroupName, out string error);
        void RemoveGroup(string GroupName);
        void AddItem(string GroupName, string[] itemsName);
        void Close();
        bool Open(out string error);
        void Read(string groupName, int itemNum, object[] result);
        void Write(string groupName, int itemNum, object[] values);
    }
}
