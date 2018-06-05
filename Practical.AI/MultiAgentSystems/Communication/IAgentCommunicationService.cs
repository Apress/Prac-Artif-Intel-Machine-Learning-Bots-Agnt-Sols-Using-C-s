using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Windows.Forms;

namespace Practical.AI.MultiAgentSystems.Communication
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IAgentCommunication" in both code and config file together.
    [ServiceContract(CallbackContract = typeof(IAgentCommunicationCallback))]
    public interface IAgentCommunicationService
    {
        [OperationContract(IsOneWay = true)]
        void Connect(string id);

        [OperationContract(IsOneWay = true)]
        void Send(string from, string to, string message);
    }
}
