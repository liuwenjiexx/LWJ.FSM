using LWJ.FSM.Model;
using System;

namespace LWJ.FSM
{

    public class TransitionEventArgs : EventArgs
    {
        private FSMState fromState;
        private FSMState toState;
        private string @event;
        private Transition transition;

        internal TransitionEventArgs(FSMState from, FSMState to, Transition transition, string @event)
        {
            this.fromState = from;
            this.toState = to;
            this.transition = transition;
            this.@event = @event;
        }


        public Transition Transition { get => transition;}
        public string Event { get => @event;}
        public FSMState ToState { get => toState; }
        public FSMState FromState { get => fromState; }

        public override string ToString()
        {
            return string.Format("<{0}> => <{1}>",
                fromState == null ? "" : fromState.Name,
                toState == null ? "" : toState.Name);
        }

    }

}
