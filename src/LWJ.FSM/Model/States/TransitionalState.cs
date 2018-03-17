using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;

namespace LWJ.FSM.Model
{
    public abstract class TransitionalState : EnterableState
    { 
        private List<EnterableState> children = new List<EnterableState>();
        private List<Transition> transitions;
        private EntryState entryState;
        private ExitState exitState;
        private string initial;


        public TransitionalState()
        {
            transitions = new List<Transition>();
        }

        public string Initial { get => initial; set => initial = value; }
 

        public ReadOnlyCollection<EnterableState> Children
            => children.AsReadOnly();


        public ReadOnlyCollection<Transition> Transitions
            => transitions.AsReadOnly();


        public override bool IsAtomicState
        {
            get { return false; }
        }

        public EntryState EntryState
        {
            get => entryState;
            set
            {
                entryState = value;
                entryState.Parent = this;
            }
        }

        public ExitState ExitState
        {
            get => exitState;
            set
            {
                exitState = value;
                exitState.Parent = this;
            }
        }

        public void AddChild(EnterableState child)
        {
            if (child.Parent == this)
                return;
            if (child.Parent != null)
            {
                ((TransitionalState)child.Parent).RemoveChild(child);
            }
            child.Parent = this;
            children.Add(child);
        }

        public void RemoveChild(EnterableState child)
        {
            if (child.Parent != this)
                return;
            children.Remove(child);
            child.Parent = null;
        }




        public void AddTransition(Transition transition)
        {
            if (transition == null) throw new ArgumentNullException(nameof(transition));

            if (!transitions.Contains(transition))
            {
                transitions.Add(transition);
                transition.Parent = this;
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
         

        public override string ToString()
        {
            return "{0}: {1}".FormatArgs(nameof(TransitionalState), Name);
        }


    }

}
