using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace LWJ.FSM.Model
{
    public class Transition
    {

        private string target;
        private string eventName;
        private object cond;
        private TransitionType transitionType;
        private TransitionalState parent;
        private List<Action> actions = new List<Action>();


        public Transition()
        {
        }

        public Transition(string target)
        {
            this.target = target;
        }
        public Transition(string target, string eventName)
        {
            this.target = target;
            this.eventName = eventName;
        }
        public Transition(string target, string eventName, object cond)
        {
            this.target = target;
            this.eventName = eventName;
            this.cond = cond;
        }

        public string Target { get => target; set => target = value; }
        public string Event { get => eventName; set => eventName = value; }

        public object Cond { get => cond; set => cond = value; }

        public TransitionType TransitionType { get => transitionType; set => transitionType = value; }

        public TransitionalState Parent { get => parent; set => parent = value; }

        public ReadOnlyCollection<Action> Actions => actions.AsReadOnly();

        public virtual void AddAction(Action action)
        {
            actions.Add(action);
        }

        public virtual void RemoveAction(Action action)
        {
            actions.Remove(action);
        }

        internal virtual bool CanTransition(FSMExecutionContext ctx)
        {
            var e = ctx.Event;

            if (eventName != null)
            {
                if (e.IsHandled || e.EventName != eventName)
                    return false;
            }

            if (cond != null && !ctx.EvalExpressionBool(cond))
                return false;

            return true;
        }
         


        public virtual void OnUpdate(FSMExecutionContext ctx)
        {

        }

        public override string ToString()
        {
            return "Target={0}, Event={1}".FormatArgs(target, Event);
        }

    }

    public enum TransitionType
    {
        Internal,
        External,
    }

}