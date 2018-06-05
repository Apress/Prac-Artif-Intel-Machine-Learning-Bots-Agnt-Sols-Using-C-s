using System.ServiceModel;

namespace AgentClient
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IAgentCommunication" in both code and config file together.
    [ServiceContract(CallbackContract = typeof(IAgentCommunicationCallback))]
    public interface IAgentCommunicationService
    {
        [OperationContract(IsOneWay = true)]
        void Subscribe();

        [OperationContract(IsOneWay = true)]
        void Send(string from, string to, string message);
    }
}
