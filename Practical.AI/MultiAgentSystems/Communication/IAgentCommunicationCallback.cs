using System.Collections.Generic;
using System.ServiceModel;

namespace Practical.AI.MultiAgentSystems.Communication
{
    public interface IAgentCommunicationCallback
    {
        string Id { get; set; }
        
        [OperationContract(IsOneWay = true)]
        void SendUpdatedList(List<string> messages);
    }
}