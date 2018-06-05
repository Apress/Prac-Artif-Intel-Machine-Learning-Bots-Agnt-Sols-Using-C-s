using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ServiceModel;
using System.Threading;

namespace AgentClient
{
    [CallbackBehavior(UseSynchronizationContext = false)]
    public class AgentCommunicationCallback : IAgentCommunicationCallback
    {
        private SynchronizationContext _syncContext = AsyncOperationManager.SynchronizationContext;
        public event EventHandler<UpdatedListEventArgs> ServiceCallbackEvent;

        public void SendUpdatedList(List<string> items)
        {
            _syncContext.Post(new SendOrPostCallback(OnServiceCallbackEvent), new UpdatedListEventArgs(items));
        }

        private void OnServiceCallbackEvent(object state)
        {
            EventHandler<UpdatedListEventArgs> handler = ServiceCallbackEvent;
            var e = state as UpdatedListEventArgs;

            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}
