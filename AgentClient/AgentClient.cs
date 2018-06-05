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

namespace AgentClient
{
    public partial class AgentClient : Form
    {
        private const string ServiceEndpointUri = "http://localhost:9090/AgentCommunicationService/";
        private AgentCommunicationServiceClient _proxy;

        public AgentClient()
        {
            InitializeComponent();
            InitializeClient();
        }

        private void InitializeClient()
        {
            if (_proxy != null)
            {
                try
                {
                    _proxy.Close();
                }
                catch
                {
                    _proxy.Abort();
                }
            }

            var callback = new AgentCommunicationCallback();
            callback.ServiceCallbackEvent += HandleServiceCallbackEvent;

            var instanceContext = new InstanceContext(callback);
            var dualHttpBinding = new WSDualHttpBinding(WSDualHttpSecurityMode.None);
            var endpointAddress = new EndpointAddress(ServiceEndpointUri);
            _proxy = new AgentCommunicationServiceClient(instanceContext, dualHttpBinding, endpointAddress);
            _proxy.Open();
            _proxy.Subscribe();
        }

        private void HandleServiceCallbackEvent(object sender, UpdatedListEventArgs e)
        {
            List<string> list = e.MessageList;

            if (list != null && list.Count > 0)
                messageList.DataSource = list;
         }

        private void SendBtnClick(object sender, EventArgs e)
        {
            _proxy.Send("", "", wordBox.Text.Trim());
            wordBox.Clear();
        }
    }
}
