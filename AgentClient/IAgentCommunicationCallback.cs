using System.Collections.Generic;
using System.ServiceModel;

namespace AgentClient
{
    public interface IAgentCommunicationCallback
    {
        [OperationContract(IsOneWay = true)]
        void SendUpdatedList(List<string> messages);
    }
}