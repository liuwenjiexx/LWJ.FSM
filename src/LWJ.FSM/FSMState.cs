using LWJ.FSM.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;


namespace LWJ.FSM
{

    /// <summary>
    /// 状态机状态
    /// </summary>
    public class FSMState : IEnumerable<FSMState>
    {
        private string name;
        private FSMachine machine;

        internal FSMState(FSMachine machine, TransitionalTarget state)
            : this(machine, state, null)
        {
        }

        private FSMState(FSMachine machine, TransitionalTarget state, FSMState parent)
        {
            this.machine = machine ?? throw new ArgumentNullException(nameof(machine));
            this.state = state ?? throw new ArgumentNullException(nameof(state));
            activeStates = new LinkedList<FSMState>();
            name = this.state.Name;
            if (parent != null)
            {
                if (state.Name == null) throw new FSMException(Resource1.FSM_StateNameNull);

                this.parent = parent;
                this.context = new FSMContext(parent.context);
                execContext = new FSMExecutionContext(machine, state, context);
            }
            else
            {
                this.context = new FSMContext(machine.GlobalContext);
                execContext = new FSMExecutionContext(machine, state, context);
            }
            if (state is ParallelState)
                stateType = StateType.ParallelState;
            else if (state is TransitionalState)
                stateType = StateType.TransitionalState;
            else if (state is EnterableState)
                stateType = StateType.EnterableState;
            else
                stateType = StateType.TransitionalTarget;

            if (state.Parameters != null)
            {
                state.Parameters.Where((p) =>
                {
                    if (p.DefaultValueExpr != null)
                        context.AddParameter(p.Type, p.Name, () => execContext.EvalExpression(p.DefaultValueExpr));
                    else
                        context.AddParameter(p.Type, p.Name, p.DefaultValue);
                    return false;
                }).ToArray();
            }

            var ts = state as TransitionalState;
            if (ts != null)
            {
                if (parent != null)
                {
                    transitions = ts.Transitions;
                }

                if (ts.Children.Count > 0)
                {
                    entry = new FSMState(machine, ts.EntryState ?? new EntryState(), this);
                    exit = AddChild(ts.ExitState ?? new ExitState());

                    foreach (var child in ts.Children)
                    {
                        AddChild(child);
                    }
                    if (!string.IsNullOrEmpty(ts.Initial))
                    {
                        initial = GetChild(ts.Initial);
                    }
                }
                else
                {
                    if (ts.EntryState != null || ts.ExitState != null)
                    {
                        entry = new FSMState(machine, ts.EntryState ?? new EntryState(), this);

                        exit = AddChild(ts.ExitState ?? new ExitState());
                    }
                }
            }
            else if (state is EntryState)
            {
                transitions = ((EntryState)state).Transitions;
            }
        }

        public FSMachine StateMachine => machine;

        public string Name => name;
        private FSMState parent;
        private Dictionary<string, FSMState> children;

        private TransitionalTarget state;
        internal FSMContext context;
        private FSMExecutionContext execContext;
        private Transition activeTransition;
        private StateType stateType;
        private FSMState entry;
        private FSMState exit;
        private FSMState initial;
        private ReadOnlyCollection<Transition> transitions;
        private LinkedList<FSMState> activeStates;
        private bool isEntryReset;

        enum StateType
        {
            TransitionalTarget = 0x1,
            EnterableState = 0x2,
            TransitionalState = 0x4,
            ParallelState = EnterableState | 0x80,
        }


        public object this[string paramName]
        {
            get { return GetParameter(paramName); }
            set { SetParameter(paramName, value); }
        }

        /// <summary>
        /// first active state
        /// </summary>
        public FSMState Current => activeStates.Count > 0 ? activeStates.First.Value : null;

        public IEnumerable<FSMState> ActiveStates
        {
            get
            {
                foreach (var item in activeStates)
                    yield return item;
            }
        }

        public int ActiveCount => activeStates.Count;

        public FSMState Parent => parent;

        internal Transition ActiveTransition { get => activeTransition; }

        private bool isActive;
        internal bool isInitial;
        public bool IsActive => isActive;

        public TransitionalTarget State => state;

        internal void SetActiveTransition(Transition activeTransition)
        {
            this.activeTransition = activeTransition;
        }

        private FSMState AddChild(TransitionalTarget state)
        {
            if (children == null)
                children = new Dictionary<string, FSMState>();
            string name = state.Name;

            if (state.Name == null) throw new FSMException(Resource1.FSM_StateNameNull);
            if (children.ContainsKey(name)) throw new FSMStateException(this.name, name);

            var s = new FSMState(machine, state, this);

            children[name] = s;
            return s;
        }

        #region Parameter

        public bool ContainsParameter(string name)
            => context.ContainsParameter(name);

        public object GetParameter(string name)
            => context.GetParameter(name);

        public void SetParameter(string name, object value)
            => context.SetParameter(name, value);


        #endregion




        internal void OnUpdate()
        {
            var state = this.state;
            var ctx = execContext;

            var actions = state.Actions;
            if (actions != null)
            {
                foreach (var action in actions)
                {
                    action.Execute(execContext);
                }
            }

            var transitions = this.transitions;
            if (transitions != null)
            {
                foreach (var transition in transitions)
                {
                    transition.OnUpdate(execContext);
                }
            }

            if (state is ParallelState)
            {
                if (children != null)
                {
                    foreach (var child in children.Values)
                    {
                        child.OnUpdate();
                    }
                }
            }
            else
            {

                foreach (var s in activeStates)
                {
                    s.OnUpdate();
                }

            }

        }

        internal void OnEvent()
        {
            var ctx = execContext;

            if (!isActive)
                return;

            if (transitions != null)
            {
                foreach (var transition in transitions)
                {
                    if (transition.CanTransition(ctx))
                    {
                        ctx.Event.Handle();
                        ctx.ExecuteAction(transition.Actions);

                        parent.ChangeState(transition.Target, transition);
                        break;
                    }
                }
            }


            if (!isActive)
                return;

            var activeStates = this.activeStates;
            LinkedListNode<FSMState> current = activeStates.First;
            FSMState s;
            while (current != null)
            {
                s = current.Value;
                s.OnEvent();
                if (!isActive)
                    break;
                current = current.Next;
            }


        }


        public void Reset()
        {
            execContext.Reset();
        }

        internal virtual void OnEntry()
        {
            if (parent != null)
            {
                if (state is EntryState)
                {
                    if (parent.isEntryReset)
                    {
                        parent.Reset();
                        parent.isEntryReset = false;
                    }
                }

                parent.activeStates.AddLast(this);
            }

            Reset();


            isActive = true;

            if (state.OnEnters != null)
            {
                foreach (var onEntry in state.OnEnters)
                    execContext.ExecuteAction(onEntry.Actions);
            }

            machine.OnEntry(this, execContext.Event.EventName);


            OnEvent();


            if (state is ExitState)
            {
                if (parent != null)
                {
                    parent.isEntryReset = true;
                    if (parent.entry != null)
                        parent.ChangeState(parent.entry, null);
                }
            }

            if (isActive)
            {

                if (!state.IsAtomicState)
                {
                    if (state is ParallelState)
                    {
                        foreach (var child in children.Values)
                        {
                            child.OnEntry();
                        }
                    }
                    else
                    {
                        if (initial != null)
                        {
                            ChangeState(initial, null);
                        }
                        else
                        {
                            if (entry != null)
                            {
                                entry.OnEntry();
                            }
                        }
                    }
                }

            }


        }



        internal virtual void OnExit()
        {
            activeTransition = null;
            isActive = false;


            //children exit
            var current = this.activeStates.First;
            if (current != null)
            {
                FSMState s;
                while (current != null)
                {
                    s = current.Value;
                    current = current.Next;
                    s.OnExit();
                }
            }

            if (parent != null)
            {
                parent.activeStates.Remove(this);
            }

            if (state.OnExits != null)
            {
                foreach (var onExit in state.OnExits)
                    execContext.ExecuteAction(onExit.Actions);
            }
            machine.OnExit(this, execContext.Event.EventName);


        }


        private void ChangeState(string name, Transition transition)
        {

            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
            FSMState toState;

            toState = GetChild(name);
            ChangeState(toState, transition);
        }

        private void ChangeState(FSMState toState, Transition transition)
        {
            FSMState fromState;

            if (stateType == StateType.ParallelState)
            {
                fromState = null;
            }
            else
            {
                fromState = Current;
            }
            ChangeState(fromState, toState, transition);
        }
        private void ChangeState(FSMState fromState, FSMState toState, Transition transition)
        {
            if (transition != null)
            {
                string @event;
                @event = execContext.Event.EventName;
                TransitionEventArgs e = new TransitionEventArgs(fromState, toState, transition, @event);
                machine.OnTransition(e);
            }

            if (fromState != null)
            {
                fromState.OnExit();
            }

            if (toState != null)
            {
                toState.OnEntry();
            }

        }

        public FSMState GetChild(string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            FSMState state;
            if (children == null || !children.TryGetValue(name, out state))
                throw new FSMStateException(Resource1.FSM_NotFoundChildState.FormatArgs(name), this.name);
            return state;
        }

        public FSMState TryGetChildState(string name)
        {
            FSMState state;
            if (children == null || !children.TryGetValue(name, out state))
                return null;
            return state;
        }


        private FSMState FindState(string name)
        {
            FSMState current = this;
            FSMState state = null;

            while (current != null)
            {
                state = current.GetChild(name);
                if (state != null)
                    break;
                current = current.parent;
            }
            return state;
        }

        public FSMExecutionContext GetContext()
        {
            if (!isActive)
                return null;
            return execContext;
        }

        public IEnumerator<FSMState> GetEnumerator()
        {
            if (children != null)
            {
                foreach (var child in children.Values)
                    yield return child;
            }
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            if (children != null)
            {
                foreach (var child in children.Values)
                    yield return child;
            }
        }


        public override string ToString()
        {
            string str = "state:{0}".FormatArgs(name);
            str += ", active:{0}".FormatArgs(ActiveCount);
            return str;
        }


    }
}
