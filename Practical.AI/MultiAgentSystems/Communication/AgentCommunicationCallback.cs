using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Practical.AI.MultiAgentSystems.Communication
{
    [CallbackBehavior(UseSynchronizationContext = false, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class AgentCommunicationCallback : IAgentCommunicationCallback
    {
        private readonly SynchronizationContext _syncContext = AsyncOperationManager.SynchronizationContext;
        public event EventHandler<UpdatedListEventArgs> ServiceCallbackEvent;
        public string Id { get; set; }

        public void SendUpdatedList(List<string> items)
        {
            _syncContext.Post(OnServiceCallbackEvent, new UpdatedListEventArgs(items));
        }

        private void OnServiceCallbackEvent(object state)
        {
            EventHandler<UpdatedListEventArgs> handler = ServiceCallbackEvent;
            var e = state as UpdatedListEventArgs;

            if (handler != null)
                handler(this, e);
        }
    }
}
