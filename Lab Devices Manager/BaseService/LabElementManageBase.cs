using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LabMCESystem.LabElement;
using System.Collections.Specialized;
using System.Runtime.Serialization;
namespace LabMCESystem.BaseService
{
    /// <summary>
    /// Lab element manage service base class.
    /// You can get some contract interface form this management.
    /// </summary>
    [Serializable]
    public class LabElementManageBase : ILabElementManageable, IDeviceConnect
    {
        #region Fields

        // registed devices collection.
        private List<LabDevice> _registedDevices;

        // registed experiment areaes collection.
        private List<ExperimentalArea> _expAreaes;


        protected List<LabDevice> RegistedDevices
        {
            get { return _registedDevices; }
            set { _registedDevices = value; }
        }

        protected List<ExperimentalArea> ExpAreaes
        {
            get { return _expAreaes; }
            set { _expAreaes = value; }
        }

        // the key is channel's key code, the dictionary remenber all of the manage channels .
        [NonSerialized]
        private Dictionary<int, AnalogueMeasureChannel> _channelKeyDic;


        #endregion

        #region Build

        /// <summary>
        /// Creat a Lab element management.
        /// </summary>
        public LabElementManageBase()
        {
            ExpAreaes = new List<ExperimentalArea>();
            RegistedDevices = new List<LabDevice>();

            _channelKeyDic = new Dictionary<int, AnalogueMeasureChannel>();

            DevicesChanged += LabElementManageBase_DevicesChanged;

            // deserialize el data set form xml file.
            //_elDataSet.ReadXml(@".\Element Data Set.xml");

        }

        #endregion


        #region Properties

        //--------------------------------------------

        /// <summary>
        /// Management device collection changed event.
        /// </summary>
        public event NotifyCollectionChangedEventHandler DevicesChanged;

        /// <summary>
        /// Management experiment collection  changed event.
        /// </summary>
        public event NotifyCollectionChangedEventHandler ExperimentAreaesChanged;

        /// <summary>
        /// Invoke this event when channels key code have been changed.
        /// </summary>
        public event NotifyCollectionChangedEventHandler ChannlesKeyCodeChanged;

        public event NotifyCollectionChangedEventHandler ExperimentPointsChanged;

        public event NotifyCollectionChangedEventHandler SensorsChanged;

        /// <summary>
        /// Get management devices readonly list.
        /// </summary>
        public IReadOnlyList<LabDevice> Devices
        {
            get
            {
                return _registedDevices.AsReadOnly();
            }
        }

        /// <summary>
        /// Get management experiment areaes readonly list.
        /// </summary>
        public IReadOnlyList<ExperimentalArea> ExperimnetAreas
        {
            get
            {
                return _expAreaes.AsReadOnly();
            }
        }

        /// <summary>
        /// Get all channels.
        /// </summary>
        public List<Channel> AllChannels
        {
            get
            {
                List<Channel> temp = new List<Channel>();

                foreach (var dev in _registedDevices)
                {
                    temp.AddRange(dev.Children);
                }
                return temp;
            }
        }

        /// <summary>
        /// Get all experiment points.
        /// </summary>
        public List<ExperimentalPoint> AllExperimentPoints
        {
            get
            {
                List<ExperimentalPoint> temp = new List<ExperimentalPoint>();

                foreach (var area in _expAreaes)
                {
                    temp.AddRange(area.Children);
                }

                return temp;
            }
        }

        public List<LinerSensor> AllSensors
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IDeviceOperator DeviceOperator
        {
            get
            {
                return this;
            }
        }

        #endregion

        #region Operators
        #region ILabElementManageabel

        /// <summary>
        /// A device client program regist a LabDevice object into this manager.
        /// </summary>
        /// <param name="regDev">Device where devcie client assign.</param>
        /// <returns></returns>
        public int RegistDevice(LabDevice regDev)
        {
            // regID must be only in RegistedDevices.

            // Client have not assign device ID.
            RegistNewDevice(regDev);
            
            return regDev.RegistID;
        }

        /// <summary>
        /// Log in device.
        /// </summary>
        /// <param name="regID"></param>
        /// <returns>If the device with regID is registed, look up.</returns>
        public LabDevice LoginDevice(int regID)
        {
            LabDevice regitedDev = RegistedDevices.Find(o => o.RegistID == regID);
            if (regitedDev != null)
            {
                regitedDev.State = DeviceState.Connected;
            }

            return regitedDev;
        }

        /// <summary>
        /// Look up a experiment area with label.
        /// </summary>
        /// <param name="label">Area label.</param>
        /// <returns>If there is no area named as label return null.</returns>
        public ExperimentalArea LookUpExpArea(string label)
        {
            return _expAreaes.Find(o => label == o.Label);
        }

        /// <summary>
        /// Look up a device with label.
        /// </summary>
        /// <param name="label">Device label.</param>
        /// <returns>If there is no device named as label return null.</returns>
        public LabDevice LookUpDevice(string label)
        {
            return _registedDevices.Find(o => o.Label == label);
        }

        /// <summary>
        /// Look up a channel with key code.
        /// </summary>
        /// <param name="chKeyCode">Assign a channle key code.</param>
        /// <returns>Return null if there is no this lab channel as key code.</returns>
        public AnalogueMeasureChannel LookUpChannel(int chKeyCode)
        {
            AnalogueMeasureChannel temp = null;
            try
            {
                temp = _channelKeyDic[chKeyCode];
            }
            catch (KeyNotFoundException kex)
            {
                Console.WriteLine($"{kex.Message}, Key Code is {chKeyCode}");
            }

            return temp;
        }

        /// <summary>
        /// IDeviceOperator, regist a new device into this management.
        /// </summary>
        /// <param name="nDev">A new registed device.</param>
        /// <returns>Result arguments.</returns>
        public bool RegistNewDevice(LabDevice nDev)
        {

            //First no rebuild nDv's registID, is it contain device that same with nDev in this.

            LabDevice temp = _registedDevices.Find(o => o.Label == nDev.Label || o.RegistID == nDev.RegistID);

            if (temp != null)
            {
                LabDevice.ReBuildDeviceID(nDev);
                // Find again.
                temp = _registedDevices.Find(o => o.Label == nDev.Label || o.RegistID == nDev.RegistID);

            }
            if (temp == null)
            {
                _registedDevices.Add(nDev);

                nDev.State = DeviceState.Registed;
                // add to data set .
                //_elDataSet.LabDevices.AddLabDevicesRow(nDev.RegistID, nDev.Label, nDev.Children.Count);

                DevicesChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, nDev));

                // add a event callback for device subelements changed.
                nDev.GroupChanged += NDev_ElementGroupChanged;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Discharge device from this management.
        /// </summary>
        /// <param name="devLabel">Device named label.</param>
        /// <returns>Succeeful status.</returns>
        public bool DischargeDevice(string devLabel)
        {
            LabDevice temp = _registedDevices.Find(o => o.Label == devLabel);
            if (temp != null)
            {
                bool br = _registedDevices.Remove(temp);
                if (br)
                {
                    DevicesChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, temp));

                    return br;
                }
            }
            return false;
        }

        /// <summary>
        /// Discharge device from this management.
        /// </summary>
        /// <param name="devLabel">Device object.</param>
        /// <returns>Succeeful status.</returns>
        public bool DischargeDevice(LabDevice dev)
        {
            bool br = _registedDevices.Remove(dev);
            if (br)
            {
                DevicesChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, dev));
            }
            return br;
        }


        public bool ExitLogDevice(int devRegID)
        {
            throw new NotImplementedException();
        }

        public bool ExitLogDevice(LabDevice dev)
        {
            return true;
        }

        public bool RegistNewExpArea(ExperimentalArea nArea)
        {
            bool bc = _expAreaes.Contains(nArea);
            if (!bc)
            {
                _expAreaes.Add(nArea);
            }
            return !bc;
        }

        public bool DischargeExpArea(string eaLabel)
        {
            throw new NotImplementedException();
        }

        public bool DischargeExpArea(ExperimentalArea area)
        {
            throw new NotImplementedException();
        }


        public bool AddNewSensor(LinerSensor newSensor)
        {
            throw new NotImplementedException();
        }

        public bool RemoveSensor(LinerSensor newSensor)
        {
            throw new NotImplementedException();
        }

        public bool RemoveSensorAsNumber(string sensorNumber)
        {
            throw new NotImplementedException();
        }

        public LinerSensor LookUpSensor(string sensorNumber)
        {
            throw new NotImplementedException();
        }


        #endregion
        #endregion

        #region Events

        // There should refresh the channel key dictionary when device's group element changed.
        private void NDev_ElementGroupChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:

                    //foreach (var item in e.NewItems)
                    //{
                    //    AnalogueMeasureChannel ch = item as AnalogueMeasureChannel;

                    //    if (_channelKeyDic.ContainsKey(ch.KeyCode))
                    //    {
                    //        throw new InvalidChannelKeyCodeException("Invalid channel has been add in lab element management repetitive key code.", sender as LabDevice);

                    //    }
                    //    else
                    //    {
                    //        _channelKeyDic.Add(ch.KeyCode, ch);

                    //        // add channel's key code changed event callback.k
                    //        ch.ChannelKeyCodeChanged += Ch_ChannelKeyCodeChanged;
                    //    }
                    //}

                    break;
                case NotifyCollectionChangedAction.Remove:
                    break;
                case NotifyCollectionChangedAction.Replace:
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    break;
                default:
                    break;
            }
        }


        // There should refresh the channel key dictionary when device collection changed.
        private void LabElementManageBase_DevicesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        //foreach (var dev in e.NewItems)
                        //{
                        //    LabDevice ndev = dev as LabDevice;

                        //    if (e.NewItems != null && ndev != null)
                        //    {
                        //        // add dictionary item
                        //        foreach (var ch in ndev.Children)
                        //        {
                        //            if (_channelKeyDic.ContainsKey(ch.KeyCode))
                        //            {
                        //                throw new InvalidChannelKeyCodeException("Invalid channel has been add in lab element management repetitive key code.", ndev);

                        //            }
                        //            else
                        //            {
                        //                _channelKeyDic.Add(ch.KeyCode, ch);

                        //                // add channel's key code changed event callback.k
                        //                ch.ChannelKeyCodeChanged += Ch_ChannelKeyCodeChanged;
                        //            }
                        //        }
                        //    }
                        //    else
                        //    {
                        //        throw new ArgumentNullException("Lab element management devices changed item is null");
                        //    }
                        //}
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    {
                        //foreach (var dev in e.OldItems)
                        //{
                        //    LabDevice odev = dev as LabDevice;
                        //    if (e.OldItems != null && odev != null)
                        //    {
                        //        // remove dictionary item
                        //        foreach (var ch in odev.Children)
                        //        {
                        //            bool br = _channelKeyDic.Remove(ch.KeyCode);
                        //            if (br)
                        //            {
                        //                // remove key code changed event callback at the same time.
                        //                ch.ChannelKeyCodeChanged -= Ch_ChannelKeyCodeChanged;
                        //            }
                        //        }
                        //    }
                        //    else
                        //    {
                        //        throw new ArgumentNullException("Lab element management devices changed item is null");
                        //    }
                        //}
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    break;
                default:
                    break;
            }

        }

        // There should update the dictionary key value when channel key code changed.
        private void Ch_ChannelKeyCodeChanged(AnalogueMeasureChannel ch, ChannelKeyCodeChangedEventArgs e)
        {
            try
            {
                _channelKeyDic.Remove(e.OldKeyCode);
                _channelKeyDic.Add(e.NewKeyCode, ch);
            }
            catch (KeyNotFoundException kex)
            {
                Console.WriteLine(kex.Message);
            }
            catch (ArgumentException aex)
            {
                Console.WriteLine(aex.Message);
            }
        }

        // Do something on deserialized.
        [OnDeserialized]
        private void OnDeserializedMethod(StreamingContext context)
        {
            //if (_channelKeyDic == null)
            //{
            //    _channelKeyDic = new Dictionary<int, AnalogueMeasureChannel>();
            //}
            //// Refresh this key code dictionary of channel.
            //foreach (var dev in _registedDevices)
            //{
            //    foreach (var ch in dev.Children)
            //    {
            //        _channelKeyDic.Add(ch.KeyCode, ch);
            //        // channel key code changed event.
            //        ch.ChannelKeyCodeChanged += Ch_ChannelKeyCodeChanged;
            //    }
            //    // device element group changed event.
            //    dev.GroupChanged += Dev_ElementGroupChanged;
            //}

            //// Check this experiment element collection all of experiment areaes,
            //// if experiment point's paired channel key code is not zero, then find channel in key code dictionary
            //// and paired it for point.
            //foreach (var area in _expAreaes)
            //{
            //    foreach (var p in area.Children)
            //    {
            //        try
            //        {
            //            p.PairedChannel = _channelKeyDic[p.PairedChannelKeyCode];
            //        }
            //        catch (KeyNotFoundException kex)
            //        {
            //            Console.WriteLine($"{kex.Message}\nExperiment point {p} paired key code {p.PairedChannelKeyCode} can not find is channel key code dictionary.");
            //        }
            //    }
            //}
        }

        private void Dev_ElementGroupChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
