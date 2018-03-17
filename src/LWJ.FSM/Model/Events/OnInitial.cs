//using LWJ.Expressions;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;

//namespace LWJ.FSM.Model
//{
//    public class OnInitial : Executable
//    {
//        private List<Transition> transitions;

//        public ReadOnlyCollection<Transition> Transitions => transitions.AsReadOnly();

//        protected override void SetParent(TransitionalTarget parent)
//        {
//            base.SetParent(parent);
//            if (transitions != null)
//            {
//                foreach (var t in transitions)
//                    t.Parent = (TransitionalState)Parent;
//            }
//        }

//        public void AddTransition(Transition transition)
//        {
//            if (transitions == null)
//                transitions = new List<Transition>();
//            transitions.Add(transition);
//            transition.Parent = (TransitionalState)Parent;
//        }

//        public Transition AddTransition(string target, string eventName, Expression cond)
//        {
//            Transition transition = new Transition(target, eventName, cond);
//            AddTransition(transition);
//            return transition;
//        }

//        public void RemoveTransition(Transition transition)
//        {
//            if (transitions == null || transition == null)
//                return;
//            if (transitions.Remove(transition))
//            {
//                transition.Parent = null;
//            }
//        }

//        internal void Execute(FSMExecutionContext ctx)
//        {
//            ctx.ExecuteAction(Actions);

//            if (transitions != null)
//            {

//                foreach (var transition in transitions)
//                {
//                    if (transition.Execute(ctx))
//                        break;
//                }
//            }
//        }

//    }
//}
