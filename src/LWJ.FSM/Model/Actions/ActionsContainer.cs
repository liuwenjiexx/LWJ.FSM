using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;


namespace LWJ.FSM.Model
{

    public class ActionsContainer : Action
    {
        private List<Action> actions = new List<Action>();

        public ReadOnlyCollection<Action> Actions
            => actions.AsReadOnly();


        public virtual void AddAction(Action action)
        {
            actions.Add(action);
        }

        public virtual void RemoveAction(Action action)
        {
            actions.Remove(action);
        }

        public override void Execute(FSMExecutionContext ctx)
        {
            if (actions != null)
            {
                foreach (var action in actions)
                    action.Execute(ctx);
            }
        }

    }
}
