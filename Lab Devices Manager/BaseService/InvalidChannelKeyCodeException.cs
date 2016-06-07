using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabMCESystem.BaseService
{
    [Serializable]
    public class InvalidChannelKeyCodeException : Exception
    {
        public LabElement.LabDevice OwnerDevice { get; private set; }

        public InvalidChannelKeyCodeException() { }
        public InvalidChannelKeyCodeException(string message) : base(message) { }
        public InvalidChannelKeyCodeException(string message, LabElement.LabDevice owner) : base(message)
        {
            OwnerDevice = owner;
        }

        protected InvalidChannelKeyCodeException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context)
        { }
    }
}
