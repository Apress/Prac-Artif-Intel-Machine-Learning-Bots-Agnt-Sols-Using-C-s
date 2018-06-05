using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading;

namespace Practical.AI.MultiAgentSystems.Communication.Acl
{
    public class FibaAcl
    {
        public AgentCommunicationServiceClient Communication { get; set; }

        public FibaAcl(AgentCommunicationServiceClient communication)
        {
            Communication = communication;
        }

        public void Message(Performative p, string senderId, string receiverId, string content)
        {
            switch (p)
            {
                case Performative.Inform:
                    ThreadPool.QueueUserWorkItem(delegate { Communication.Send(senderId, receiverId, "inform[content:" + content + ";]"); });
                    break;
                case Performative.Cfp:
                    ThreadPool.QueueUserWorkItem(delegate { Communication.Send(senderId, receiverId, "cfp[content:" + content + ";]"); });
                    break;
                case Performative.Proposal:
                    ThreadPool.QueueUserWorkItem(delegate { Communication.Send(senderId, receiverId, "proposal[from:" + senderId + ";content:" + content + "]"); });
                    break;
            }
        }

        public static string GetPerformative(string task)
        {
            return task.Substring(0, task.IndexOf('['));
        }

        public static string GetInnerMessage(string task)
        {
            return task.Substring(task.IndexOf('[') + 1, task.LastIndexOf(']') - task.IndexOf('[') - 1);
        }

        public static Dictionary<string, string> MessageToDict(string innerMessage)
        {
            var result = new Dictionary<string, string>();
            var items = innerMessage.Split(';');
            var contentItems = new List<string>();

            foreach (var item in items)
                if (!string.IsNullOrEmpty(item))
                    contentItems.AddRange(item.Split(':'));

            for (int i = 0; i < contentItems.Count; i += 2)
                result.Add(contentItems[i], contentItems[i + 1]);

            return result;
        }

        public static Dictionary<string, string> MessagesToDict(string message)
        {
            return MessageToDict(GetInnerMessage(message));
        }

        public static List<Tuple<double, Tuple<int, int>>> GetContent(List<Dictionary<string, string>> messagesFromContractor)
        {
            var result = new List<Tuple<double, Tuple<int, int>>>();

            foreach (var msg in messagesFromContractor)
            {
                var content = msg["content"];
                var values = content.Split(',');
                result.Add(new Tuple<double, Tuple<int, int>>(double.Parse(values[0]),
                    new Tuple<int, int>(int.Parse(values[1]), int.Parse(values[2]))));
            }

            return result;
        }
    }

    public enum Performative
    {
        Accept, Cfp, Inform, Proposal
    }
}
