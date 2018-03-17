using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace LWJ.FSM.Model
{
    public abstract class Executable
    {
        private TransitionalTarget parent;

        private List<Action> actions = new List<Action>();

        public ReadOnlyCollection<Action> Actions => actions.AsReadOnly();

        public TransitionalTarget Parent { get => parent; set => SetParent(value); }


        protected virtual void SetParent(TransitionalTarget parent)
        {
            this.parent = parent;
        }

        public virtual void AddAction(Action action)
        {
            actions.Add(action);
        }

        public virtual void RemoveAction(Action action)
        {
            actions.Remove(action);
        }
         
    }
}
