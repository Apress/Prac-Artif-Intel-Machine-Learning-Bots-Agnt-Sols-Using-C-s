using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace AgentClient
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "AgentCommunication" in both code and config file together.
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class AgentCommunicationService : IAgentCommunicationService
    {
        private static List<IAgentCommunicationCallback> _callbackChannels = new List<IAgentCommunicationCallback>();
        private static List<string> _messages = new List<string>();
        private static readonly object _sycnRoot = new object();

        public void Subscribe()
        {
            try
            {
                    var callbackChannel =
                        OperationContext.Current.GetCallbackChannel<IAgentCommunicationCallback>();
                    
                    lock (_sycnRoot)
                    {
                        if (!_callbackChannels.Contains(callbackChannel))
                        {
                            _callbackChannels.Add(callbackChannel);
                            Console.WriteLine("Added Callback Channel: {0}", callbackChannel.GetHashCode());
                            callbackChannel.SendUpdatedList(_messages);
                        }
                    }
            }
            catch 
            {
                
            }
        }

        public void Send(string from, string to, string message)
        {
            lock (_sycnRoot)
            {
                _messages.Add(message);

                Console.WriteLine("-- Message List --");
                _messages.ForEach(listItem => Console.WriteLine(listItem));
                Console.WriteLine("------------------");

                for (int i = _callbackChannels.Count - 1; i >= 0; i--)
                {
                    if (((ICommunicationObject)_callbackChannels[i]).State != CommunicationState.Opened)
                    {
                        Console.WriteLine("Detected Non-Open Callback Channel: {0}", _callbackChannels[i].GetHashCode());
                        _callbackChannels.RemoveAt(i);
                        continue;
                    }

                    try
                    {
                        _callbackChannels[i].SendUpdatedList(_messages);
                        Console.WriteLine("Pushed Updated List on Callback Channel: {0}", _callbackChannels[i].GetHashCode());
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Service threw exception while communicating on Callback Channel: {0}", _callbackChannels[i].GetHashCode());
                        Console.WriteLine("Exception Type: {0} Description: {1}", ex.GetType(), ex.Message);
                        _callbackChannels.RemoveAt(i);
                    }
                }
            }
        }
    }
}
