using System;
using LWJ.FSM.Model;

namespace LWJ.FSM
{
    public class EntryEventArgs : EventArgs
    {
        internal EntryEventArgs(TransitionalTarget entryState, string @event)
        {
            this.EntryState = entryState;
        }

        public TransitionalTarget EntryState { get; private set; }
    }
}
