using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace LWJ.FSM.Model
{

    public sealed class EntryState : TransitionalTarget
    {

        private List<Transition> transitions = new List<Transition>();

        public const string StateName = "state.entry";

        public override string Name
        {
            get => StateName;
            set { }
        }

        public ReadOnlyCollection<Transition> Transitions
            => transitions.AsReadOnly();

        public void AddTransition(Transition transition)
        {
            if (transition == null) throw new ArgumentNullException(nameof(transition));

            if (!transitions.Contains(transition))
            {
                transitions.Add(transition);
                transition.Parent = (TransitionalState)Parent;
            }
        }

        public void RemoveTransition(Transition transition)
        {
            if (transition == null) throw new ArgumentNullException(nameof(transition));

            if (transitions.Remove(transition))
            {
                transition.Parent = null;
            }
        }



    }


}
