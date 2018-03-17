using System;
using LWJ.FSM.Model;

namespace LWJ.FSM
{
    public class ExitEventArgs : EventArgs
    {
        internal ExitEventArgs(TransitionalTarget exitState, string @event)
        {
            this.ExitState = exitState;
        }

        public TransitionalTarget ExitState { get; private set; }
    }

}
