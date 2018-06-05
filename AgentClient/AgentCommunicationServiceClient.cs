using System.ServiceModel;

namespace AgentClient
{
    public class AgentCommunicationServiceClient : DuplexClientBase<IAgentCommunicationService>, IAgentCommunicationService
    {
        public AgentCommunicationServiceClient(InstanceContext callbackInstance, WSDualHttpBinding binding, EndpointAddress endpointAddress) 
            : base(callbackInstance, binding, endpointAddress) { }

        public void Subscribe()
        {
            Channel.Subscribe();
        }

        public void Send(string from, string to, string message)
        {
            Channel.Send(from, to, message);
        }
    }
}
