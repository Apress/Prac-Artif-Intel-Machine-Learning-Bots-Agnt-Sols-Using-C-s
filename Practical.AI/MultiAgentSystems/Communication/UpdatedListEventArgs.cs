using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Practical.AI.MultiAgentSystems.Communication
{
    public class UpdatedListEventArgs : EventArgs
    {
        private List<string> _items;

        public List<string> ItemList
        {
            get { return _items; }
            set { _items = value; }
        }

        public UpdatedListEventArgs(List<string> items)
        {
            _items = items;
        }

    }
}
