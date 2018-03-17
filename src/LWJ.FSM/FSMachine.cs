using LWJ.FSM.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace LWJ.FSM
{
    public class FSMachine
    {

        private FSMState root;
        private FSMContext globalContext;

        private LinkedList<FSMEvent> events;
        private bool isStarted;
        private FSMEvent currentEvent;
        private IFSMLogger logger;
        private Dictionary<TransitionalTarget, FSMState> state_fsmstates;
        private FSMTime time;
        private DateTime startTime;
        private readonly static object lockObj = new object();

        public const string EventVarName = "$e";
        public const string TimeVarName = "$time";

        public FSMachine()
          : this(null)
        {

        }

        public FSMachine(FSMContext globalContext)
        {
            this.events = new LinkedList<FSMEvent>();
            if (globalContext == null)
                globalContext = new FSMContext();
            this.globalContext = globalContext;

            time = new FSMTime();
            globalContext.AddParameter(typeof(FSMTime), TimeVarName);
            globalContext.SetParameter(TimeVarName, time);

            globalContext.AddParameter(typeof(FSMEvent), EventVarName);

        }

        /// <summary>
        /// access root parameter
        /// </summary>
        public object this[string paramName]
        {
            get => Root[paramName];
            set => Root[paramName] = value;
        }

        /// <summary>
        /// root state
        /// </summary>
        /// <exception cref="MemberAccessException"/>
        public FSMState Root
        {
            get
            {
                var r = root;
                if (r == null) throw new MemberAccessException(Resource1.FSM_NotSetRootState);
                return r;
            }
        }

        /// <summary>
        /// global context
        /// </summary> 
        public FSMContext GlobalContext => globalContext;



        public bool IsStarted => isStarted;

        /// <summary>
        /// Root.Current
        /// </summary>
        public FSMState Current => Root.Current;

        public IEnumerable<FSMState> ActiveStates => Root.ActiveStates;

        public int ActiveCount => Root.ActiveCount;

        public FSMEvent CurrentEvent
        {
            get
            {
                return currentEvent;
            }
        }

        public FSMTime Time => time;

        public IFSMLogger Logger { get => logger; set => logger = value; }

        public event EventHandler<TransitionEventArgs> StateTransition;

        public event EventHandler<EntryEventArgs> StateEntry;

        public event EventHandler<ExitEventArgs> StateExit;


        public void SetRoot(Model.State root)
        {
            if (root == null) throw new ArgumentNullException(nameof(root));
            if (this.root != null) throw new FSMException(Resource1.FSM_RootAlreadyExists);

            this.root = new FSMState(this, root);
            state_fsmstates = new Dictionary<TransitionalTarget, FSMState>();
            var s = this.root;

            CacheState(this.root);
        }

        private void CacheState(FSMState s)
        {
            state_fsmstates[s.State] = s;
            foreach (var child in s)
            {
                CacheState(child);
            }
        }


        public void LoadXml(string xml)
        {
            if (this.root != null) throw new FSMException(Resource1.FSM_RootAlreadyExists);

            var reader = new Model.Xml.FSMXmlReader();
            var root = reader.Read(xml, GlobalContext);
            SetRoot(root);
        }


        public void Start()
        {

            if (!isStarted)
            {
                if (root == null) throw new FSMException(Resource1.FSM_NotSetRootState);

                isStarted = true;

                time.Time = 0;
                time.DeltaTime = 0;
                startTime = DateTime.Now;

                events.Clear();
                SetEvent(FSMEvent.MakeEmpty());

                root.OnEntry();
                UpdateEvent();
            }
        }

        #region Parameter

        //public bool ContainsParameter(string name)
        //    => Root.ContainsParameter(name);


        //public object GetParameter(string name)
        //    => Root.GetParameter(name);

        //public void SetParameter(string name, object value)
        //    => Root.SetParameter(name, value);


        #endregion



        public void SendEvent(string eventName, object data = null)
        {
            if (eventName == null) throw new ArgumentNullException(nameof(eventName));
            FSMEvent e = new FSMEvent(eventName, data);
            SendEvent(e);
        }

        public void SendEvent(FSMEvent e)
        {
            if (e == null) throw new ArgumentNullException(nameof(e));

            lock (lockObj)
            {
                events.AddLast(e);
            }

        }

        public void CancelEvent(string eventName)
        {
            LinkedListNode<FSMEvent> current;
            FSMEvent e;

            lock (lockObj)
            {
                current = events.Last;
                while (current != null)
                {
                    e = current.Value;
                    if (e.EventName == eventName)
                    {
                        current.List.Remove(current);
                        break;
                    }
                    current = current.Previous;
                }
            }
        }

        private void SetEvent(FSMEvent evt)
        {
            currentEvent = evt;
            globalContext.SetParameter(EventVarName, evt);
        }

        public void Update()
            => Update(0);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="deltaTime">seconds</param>
        public void Update(float deltaTime)
        {
            if (!isStarted)
                throw new FSMException(Resource1.FSM_NotStarted);

            if (deltaTime < 0)
                deltaTime = 0;
            time.DeltaTime = deltaTime;
            time.Time += deltaTime;

            SetEvent(FSMEvent.MakeEmpty());
            root.OnUpdate();

            SendEvent(FSMEvent.MakeEmpty());

            UpdateEvent();

        }

        private void UpdateEvent()
        {
            LinkedListNode<FSMEvent> current;
            FSMEvent evt;
            while (true)
            {
                lock (lockObj)
                {
                    current = events.First;
                    if (current == null)
                        break;
                    evt = current.Value;
                    events.RemoveFirst();
                }
                SetEvent(evt);
                Root.OnEvent();
                SetEvent(null);
            }
        }

        internal void OnTransition(TransitionEventArgs e)
        {

            var evt = StateTransition;
            if (evt != null)
            {
                evt(this, e);
            }
        }

        internal void OnEntry(FSMState entryState, string @event)
        {
            var evt = StateEntry;
            if (evt != null)
            {
                EntryEventArgs e = new EntryEventArgs(entryState.State, @event);
                evt(this, e);
            }
        }
        internal void OnExit(FSMState exitState, string @event)
        {
            var evt = StateExit;
            if (evt != null)
            {
                ExitEventArgs e = new ExitEventArgs(exitState.State, @event);
                evt(this, e);
            }
        }

        internal FSMState GetFSMState(TransitionalTarget state)
        {
            FSMState s;
            if (!state_fsmstates.TryGetValue(state, out s))
                throw new FSMException("Not Found State");
            return s;
        }

        public FSMState GetState(string name)
        {
            if (root.Name == name)
                return root;
            FSMState result;

            result = GetState(root, name);
            return result;
        }

        private FSMState GetState(FSMState parent, string name)
        {
            FSMState result;
            result = parent.TryGetChildState(name);
            if (result != null)
                return result;
            foreach (var child in parent)
            {
                result = GetState(child, name);
                if (result != null)
                    break;
            }
            return result;
        }

        public void Log(string type, string messageFormat, params object[] args)
        {
            var logger = this.logger;
            if (logger != null)
                logger.Log(type, messageFormat, args);
        }

    }


}
