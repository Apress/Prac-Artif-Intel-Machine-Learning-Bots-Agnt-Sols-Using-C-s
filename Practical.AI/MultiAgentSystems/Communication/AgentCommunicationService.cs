using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Practical.AI.MultiAgentSystems.Communication
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "AgentCommunication" in both code and config file together.
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class AgentCommunicationService : IAgentCommunicationService
    {
        private static readonly List<IAgentCommunicationCallback> CallbackChannels = new List<IAgentCommunicationCallback>();
        private static readonly List<string> Messages = new List<string>();
        private static readonly object SycnRoot = new object();
        private static readonly List<string> ChannelIds = new List<string>();
        private static Dictionary<string, List<string>> MessagesPerChannel = new Dictionary<string, List<string>>(); 

        public void Connect(string id)
        {
            try
            {
                var callbackChannel =
                    OperationContext.Current.GetCallbackChannel<IAgentCommunicationCallback>();

                lock (SycnRoot)
                {
                    if (!CallbackChannels.Contains(callbackChannel))
                    {
                        CallbackChannels.Add(callbackChannel);
                        ChannelIds.Add(id);
                        MessagesPerChannel.Add(id, new List<string>());
                        Console.WriteLine("Added Callback Channel: {0}", callbackChannel.GetHashCode());
                        callbackChannel.SendUpdatedList(Messages);
                    }
                }
            }
            catch (Exception)
            {
                
            }
        }

        public void Send(string from, string to, string message)
        {
                Messages.Add(message);
                MessagesPerChannel[to].Add(message);

                Console.WriteLine("-- Multi-Agent Service --");
                Messages.ForEach(Console.WriteLine);
                Console.WriteLine("------------------");

                for (int i = CallbackChannels.Count - 1; i >= 0; i--)
                {
                    if ((CallbackChannels[i] as ICommunicationObject).State != CommunicationState.Opened)
                    {
                        Console.WriteLine("Detected Non-Open Callback Channel: {0}", CallbackChannels[i].GetHashCode());
                        CallbackChannels.RemoveAt(i);
                        continue;
                    }
                    try
                    {
                        if (ChannelIds[i] == to)
                        {
                            CallbackChannels[i].SendUpdatedList(MessagesPerChannel[to]);
                            Console.WriteLine("Pushed Updated List on Callback Channel: {0}",
                                              CallbackChannels[i].GetHashCode());
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Service threw exception while communicating on Callback Channel: {0}", CallbackChannels[i].GetHashCode());
                        Console.WriteLine("Exception Type: {0} Description: {1}", ex.GetType(), ex.Message);
                        CallbackChannels.RemoveAt(i);
                    }
            }
        }
    }
}
