using LabMCESystem.LabElement;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabMCESystem.BaseService
{
    /// <summary>
    /// It could look up all channles of lab element management.
    /// </summary>
    public interface IDeviceElementListen
    {
        /// <summary>
        /// Get all channels.
        /// </summary>
        List<Channel> AllChannels { get; }

        /// <summary>
        /// Get all sensors.
        /// </summary>
        List<LinerSensor> AllSensors { get; }

        /// <summary>
        /// Get all devices.
        /// </summary>
        IReadOnlyList<LabDevice> Devices { get; }

        /// <summary>
        /// Look up a device with label.
        /// </summary>
        /// <param name="label">Assign a device label.</param>
        /// <returns>Return null if there is no this device as label.</returns>
        LabDevice LookUpDevice(string label);

        /// <summary>
        /// Look up a channel with key code.
        /// </summary>
        /// <param name="chKeyCode">Assign a channle key code.</param>
        /// <returns>Return null if there is no this lab channel as key code.</returns>
        AnalogueMeasureChannel LookUpChannel(int chKeyCode);

        /// <summary>
        /// Look up a sensor with sensor number.
        /// </summary>
        /// <param name="sensorNumber">Assgin a sensor number.</param>
        /// <returns>Return null if there is no this sensor as the number.</returns>
        LinerSensor LookUpSensor(string sensorNumber);

        /// <summary>
        /// Invoke this event when channels key code have been changed.
        /// </summary>
        event NotifyCollectionChangedEventHandler ChannlesKeyCodeChanged;

        /// <summary>
        /// Invoke this event when sensors collection have been changed.
        /// </summary>
        event NotifyCollectionChangedEventHandler SensorsChanged;
    }
}
