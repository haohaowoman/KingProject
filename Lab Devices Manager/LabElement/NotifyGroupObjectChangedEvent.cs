using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace LabMCESystem.LabElement
{

    /// <summary>
    /// Class of group changed event argument in LabGroupSubElement object.
    /// </summary>
    [Serializable]
    public class NotifyGroupObjectChangedEventArgs : EventArgs
    {
        public LabElement OldGroup { get; private set; }

        public LabElement NewGroup { get; private set; }

        public NotifyGroupObjectChangedEventArgs(LabElement newGroup, LabElement oldGroup)
        {
            NewGroup = newGroup;
            OldGroup = oldGroup;
        }

        public NotifyGroupObjectChangedEventArgs(LabElement newGroup) : this(newGroup, null)
        {

        }
    }

    /// <summary>
    /// Delegate of group object changed.
    /// </summary>
    /// <param name="send">Event sourece.</param>
    /// <param name="e">Event argument.</param>
    public delegate void NotifyGroupObjectChangedEventHandler(LabElement sender, NotifyGroupObjectChangedEventArgs e);
    
}
