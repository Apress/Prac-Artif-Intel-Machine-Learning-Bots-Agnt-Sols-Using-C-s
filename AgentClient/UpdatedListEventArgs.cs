using System;
using System.Collections.Generic;

namespace AgentClient
{
    public class UpdatedListEventArgs : EventArgs
    {
        private List<string> _items;

        public List<string> MessageList
        {
            get { return _items; }
            set { _items = value; }
        }

        public UpdatedListEventArgs(List<string> messages)
        {
            MessageList = messages;
        }

    }
}
