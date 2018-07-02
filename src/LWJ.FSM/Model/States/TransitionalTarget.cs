using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace LWJ.FSM.Model
{
    public abstract class TransitionalTarget
    {

        private EnterableState parent;

        private List<Parameter> parameters = new List<Parameter>();
        private List<Action> actions = new List<Action>();
        private List<OnEntry> onEnters = new List<OnEntry>();
        private List<OnExit> onExits = new List<OnExit>();
        private List<IFSMStateListener> listeners = new List<IFSMStateListener>();

        public virtual string Name { get; set; }

        public EnterableState Parent { get => parent; set => parent = value; }

        public virtual bool IsAtomicState => true;

        public ReadOnlyCollection<Parameter> Parameters
            => parameters.AsReadOnly();


        public ReadOnlyCollection<Action> Actions
            => actions.AsReadOnly();

        public ReadOnlyCollection<OnEntry> OnEnters
            => onEnters.AsReadOnly();

        public ReadOnlyCollection<OnExit> OnExits
            => onExits.AsReadOnly();

        public ReadOnlyCollection<IFSMStateListener> Listeners
            => listeners.AsReadOnly();

        public void AddParamerter<T>(string name)
            => AddParamerter(typeof(T), name);


        public void AddParamerter(Type type, string name)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (name == null) throw new ArgumentNullException(nameof(name));
            AddParamerter(new Parameter(type, name));
        }

        public void AddParamerter(Parameter parameter)
        {
            if (parameter == null) throw new ArgumentNullException(nameof(parameter));
            parameters.Add(parameter);
        }
        public void RemoveParamerter(Parameter parameter)
        {
            if (parameter == null) throw new ArgumentNullException(nameof(parameter));
            parameters.Remove(parameter);
        }

        public void AddAction(Action action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            if (!actions.Contains(action))
            {
                actions.Add(action);
            }
        }

        public void RemoveAction(Action action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            actions.Remove(action);
        }



        public void AddOnEntry(OnEntry onEntry)
        {
            if (onEntry == null) throw new ArgumentNullException(nameof(onEntry));
            if (!onEnters.Contains(onEntry))
            {
                onEnters.Add(onEntry);
                onEntry.Parent = this;
            }
        }

        public void RemoveOnEntry(OnEntry onEntry)
        {
            if (onEntry == null) throw new ArgumentNullException(nameof(onEntry));
            if (onEnters.Remove(onEntry))
            {
                onEntry.Parent = null;
            }
        }


        public void AddOnExit(OnExit onExit)
        {
            if (onExit == null) throw new ArgumentNullException(nameof(onExit));
            if (!onExits.Contains(onExit))
            {
                onExit.Parent = this;
                onExits.Add(onExit);
            }
        }

        public void RemoveOnExit(OnExit onExit)
        {
            if (onExit == null) throw new ArgumentNullException(nameof(onExit));

            if (onExits.Remove(onExit))
            {
                onExit.Parent = null;
            }
        }


        public void AddListener(IFSMStateListener listener)
        {
            if (listener != null && !listeners.Contains(listener))
            {
                listeners.Add(listener);
            }
        }
        public void RemoveListener(IFSMStateListener listener)
        {
            if (listener != null && listeners.Contains(listener))
            {
                listeners.Remove(listener);
            }
        }

        public override string ToString()
        {
            return "{0}: {1}".FormatArgs(nameof(TransitionalTarget), Name);
        }


    }
}
