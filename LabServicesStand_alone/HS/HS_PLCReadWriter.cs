using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LabMCESystem.LabElement;
using HS_DeviceInteract;
using newopcdll;
namespace LabMCESystem.Servers.HS
{
    /// <summary>
    /// 提供了与环散系统PLC OPC服务器读写连接的方法集。
    /// </summary>
    static class HS_PLCReadWriter
    {
        static public double testValue = 0;

        public static double TestValue { get { return testValue += 0.1; } }

        public static bool OpcIsOpened { get; private set; }

        private static IOPCDInteract _opcInteract;

        public static IOPCDInteract OpcInteract
        {
            get { return _opcInteract; }
        }

        public static IOPCDInteract CreateOpcInteract()
        {
            _opcInteract = null;
            _opcInteract = new newopcdll.Opc();
            return _opcInteract;
        }

        public static bool OpenOpcInteract()
        {
            if (_opcInteract != null)
            {
                string error;
                OpcIsOpened = _opcInteract.Open(out error);

                Console.WriteLine($"Opc open feild, error is {error}.");
            }
            else
            {
                OpcIsOpened = false;
            }
            return OpcIsOpened;
        }

        public static bool CloseOpcInteract()
        {
            if (_opcInteract != null && OpcIsOpened)
            {
                try
                {
                    _opcInteract.Close();
                    _opcInteract = null;
                }
                catch (Exception error)
                {
                    Console.WriteLine($"Opc open feild, error is {error}.");
                }
            }
            else
            {
                return false;
            }
            return true;
        }
    }

    public class OPCGroup
    {
        public OPCGroup(string groupName)
        {
            GroupName = groupName;

            _group = new Dictionary<string, ChannelIndex>();
        }

        public OPCGroup(string groupName, IEnumerable<Channel> initialChannels) : this(groupName)
        {
            int index = 0;
            foreach (var item in initialChannels)
            {
                _group.Add(item.Prompt, new ChannelIndex() { Index = index, SubChannel = item });
            }
        }

        private bool _isInitialed = false;

        public bool IsInitialed
        {
            get { return _isInitialed; }
        }

        public struct ChannelIndex
        {
            public ChannelIndex(int index, Channel subChannel)
            {
                Index = index;
                SubChannel = subChannel;
            }

            public Channel SubChannel { get; set; }

            public int Index { get; set; }
        }

        public string GroupName { get; private set; }

        private Dictionary<string, ChannelIndex> _group;

        public IReadOnlyDictionary<string, ChannelIndex> Group
        {
            get { return _group; }
        }

        #region Operators

        /// <summary>
        /// 在为通道添加完成后向OPC服务器添加组。之后不能再添加组元。
        /// </summary>
        /// <returns>成功返回true。</returns>
        public bool InitialGroup()
        {
            if (HS_PLCReadWriter.OpcInteract != null)
            {
                string error;
                _isInitialed = HS_PLCReadWriter.OpcInteract.AddGroup(GroupName, out error);
                 if (_isInitialed && _group.Count > 0)
                {
                    HS_PLCReadWriter.OpcInteract.AddItem(GroupName, _group.Keys.ToArray());
                }
                else
                {
                    Console.WriteLine($"Opc add group feild, error is {error}.");
                }
            }
            else
            {
                _isInitialed = false;
            }
            return _isInitialed;
        }

        /// <summary>
        /// 向OPC服务器删除组。
        /// </summary>
        public void CloseGroup()
        {
            if (_isInitialed)
            {
                HS_PLCReadWriter.OpcInteract?.RemoveGroup(GroupName);
            }
            _isInitialed = false;
        }

        public bool AddSubChannel(Channel nChannel)
        {
            if (!_isInitialed)
            {
                try
                {
                    _group.Add(nChannel.Prompt, new ChannelIndex(_group.Count, nChannel));
                }
                catch (Exception)
                {

                    return false;
                }
            }
            else
            {
                return false;
            }

            return true;
        }

        public bool RemoveSubChannel(Channel oChannel)
        {
            if (!_isInitialed && oChannel.Prompt != null)
            {
                return _group.Remove(oChannel.Prompt);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 从OPC读取数据，并直接更新到通道值。
        /// </summary>
        public void Read()
        {
            //...
            int count = _group.Count;
            if (count == 0 || !_isInitialed)
            {
                return;
            }
            var ichs = _group.Values;
            object[] rary = new object[count];

            HS_PLCReadWriter.OpcInteract?.Read(GroupName, count, rary);

            //...
            foreach (var item in ichs)
            {
                if (item.SubChannel == null)
                {
                    continue;
                }

                switch (item.SubChannel.Style)
                {
                    case ExperimentStyle.Measure:
                        {
                            var amc = item.SubChannel as IAnalogueMeasure;
                            if (amc != null)
                            {
                                if (rary[item.Index] != null)
                                {
                                    amc.MeasureValue = (double)rary[item.Index];
                                }
                                else amc.MeasureValue = HS_PLCReadWriter.TestValue;
                            }
                        }
                        break;

                    case ExperimentStyle.Feedback:
                        {
                            var fbc = item.SubChannel as IFeedback;
                            if (fbc != null)
                            {
                                if (rary[item.Index] != null)
                                {
                                    fbc.MeasureValue = (double)rary[item.Index];
                                }
                                else fbc.MeasureValue = HS_PLCReadWriter.TestValue;
                            }
                        }
                        break;
                    case ExperimentStyle.Status:
                        {
                            var fbc = item.SubChannel as IStatusExpress;
                            if (fbc != null)
                            {
                                if (rary[item.Index] != null)
                                {
                                    fbc.Status = ((double)rary[item.Index] == 0 ? false : true);
                                }
                                else
                                {
                                    fbc.Status = new Random().Next(0, 100) % 2 == 0;
                                }

                            }
                        }
                        break;
                    case ExperimentStyle.StatusControl:
                        {
                            var fbc = item.SubChannel as IStatusExpress;
                            if (rary[item.Index] != null)
                            {
                                fbc.Status = ((double)rary[item.Index] == 0 ? false : true);
                            }
                            else
                            {
                                fbc.Status = new Random().Next(0, 100) % 2 == 0;
                            }
                        }
                        break;
                    default:
                        item.SubChannel.Value = rary[item.Index];
                        break;
                }
            }
        }

        /// <summary>
        /// 如果组里包含控制通道，则将控制值直接写入。
        /// </summary>
        public void Write()
        {
            int count = _group.Count;

            if (count > 0 && _isInitialed)
            {
                object[] wary = new object[count];

                //...
                var ichs = _group.Values;
                foreach (var item in ichs)
                {
                    wary[item.Index] = null;

                    if (item.SubChannel == null)
                    {
                        continue;
                    }

                    switch (item.SubChannel.Style)
                    {
                        case ExperimentStyle.Control:
                            {
                                var cch = item.SubChannel as IAnalogueOutput;
                                if (cch != null)
                                {
                                    wary[item.Index] = cch.AOValue;
                                }
                            }
                            break;
                        case ExperimentStyle.Feedback:
                            {
                                var cch = item.SubChannel as IFeedback;
                                if (cch != null)
                                {
                                    wary[item.Index] = cch.AOValue;
                                }
                            }
                            break;
                        case ExperimentStyle.StatusControl:
                            {
                                var cch = item.SubChannel as IStatusController;
                                if (cch != null)
                                {
                                    wary[item.Index] = cch.NextStatus;
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }

                HS_PLCReadWriter.OpcInteract?.Write(GroupName, count, wary);
            }

        }

        /// <summary>
        /// 针对状态控制通道控制输出。
        /// </summary>
        /// <param name="prompt">通道的设计快速标记。</param>
        /// <param name="doValue">需要输出的状态。</param>
        public bool Write(string prompt, bool doValue)
        {
            int count = _group.Count;
            if (count > 0 && _isInitialed)
            {
                ChannelIndex chi;
                if (_group.TryGetValue(prompt, out chi))
                {
                    var scc = chi.SubChannel as IStatusController;
                    if (scc != null)
                    {
                        object[] wary = new object[count];

                        wary[chi.Index] = doValue;

                        //...
#if DEBUG
                        Console.WriteLine($"Channel : {chi.SubChannel} write opc value {wary[chi.Index]}.");
#endif
                        HS_PLCReadWriter.OpcInteract?.Write(GroupName, count, wary);

                        return true;
                    }



                }
            }
            return false;
        }

        /// <summary>
        /// 针对模拟量控制通道控制输出。
        /// </summary>
        /// <param name="prompt">通道的设计快速标记。</param>
        /// <param name="aoValue">需要输出的模拟量。</param>
        /// <returns>成功为true。</returns>
        public bool Write(string prompt, double aoValue)
        {
            int count = _group.Count;
            if (count > 0 && _isInitialed)
            {
                ChannelIndex chi;
                if (_group.TryGetValue(prompt, out chi))
                {
                    var scc = chi.SubChannel as IAnalogueOutput;
                    if (scc != null)
                    {
                        object[] wary = new object[count];

                        wary[chi.Index] = (float)aoValue;

                        //...

#if DEBUG
                        Console.WriteLine($"Channel : {chi.SubChannel} write opc value {wary[chi.Index]}.");
#endif
                        HS_PLCReadWriter.OpcInteract?.Write(GroupName, count, wary);
                        return true;
                    }



                }
            }
            return false;
        }
        #endregion
    }
}
