using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Practical.AI.MultiAgentSystems.Communication;

namespace Practical.AI.MultiAgentSystems.GUI
{
    public partial class Messaging : Form
    {
        public const string ServiceEndpointUri = "http://localhost:9090/AgentCommunicationService";
        public AgentCommunicationServiceClient Proxy { get; set; }
        public List<string> Messages { get; set; }
        public int MessagesReceived { get; set; }
        private string _id;

        public Messaging(string id)
        {
            _id = id;
            Messages = new List<string>();
            InitializeComponent();
            InitializeClient();
        }

        private void InitializeClient()
        {
            if (Proxy != null)
            {
                try
                {
                    Proxy.Close();
                }
                catch
                {
                    Proxy.Abort();
                }
            }

            var callback = new AgentCommunicationCallback();
            callback.ServiceCallbackEvent += HandleServiceCallbackEvent;

            var instanceContext = new InstanceContext(callback);
            var dualHttpBinding = new WSDualHttpBinding(WSDualHttpSecurityMode.None);
            var endpointAddress = new EndpointAddress(ServiceEndpointUri);
            Proxy = new AgentCommunicationServiceClient(instanceContext, dualHttpBinding, endpointAddress);
            Proxy.Open();
            Proxy.Connect(_id);
        }

        public void HandleServiceCallbackEvent(object sender, UpdatedListEventArgs e)
        {
            Messages = new List<string>(e.ItemList);
            if (Messages.Count > 0)
                messageList.DataSource = Messages;
        }
    }
}
