using System;

namespace LWJ.FSM
{

    public class FSMEvent : ICloneable
    {
        private string eventName;
        private object data;
        private bool isCanceled;
        private bool isHandled;
        private bool isCancelable;
        //private bool isBubbles;

        public const string None = "";

        internal static FSMEvent MakeEmpty() => new FSMEvent("", false);

        public FSMEvent(string eventName)
            : this(eventName, null)
        {
        }

        public FSMEvent(string eventName, object data)
           : this(eventName, data, true)
        {
        }

        public FSMEvent(string eventName, object data, bool cancelable)
        {
            if (eventName == null) throw new ArgumentNullException(nameof(eventName));
            this.eventName = eventName;
            this.data = data;
            this.isCancelable = cancelable;
        }

        public string EventName => eventName;

        public object Data => data;

         
        public bool IsHandled => isHandled;
        public bool IsCancelable => isCancelable;
        //public bool IsBubbles => isBubbles;

        //public void StopPropagation()
        //{
        //    isBubbles = false;
        //}

        public void Cancel()
        {
            isCanceled = true;
            if (!isHandled)
                isHandled = true;
        }

        public void Handle()
        {
            if (!isHandled)
                isHandled = true;
        }

        public virtual object Clone()
        {
            FSMEvent e = new FSMEvent(eventName);
            Clone(e);
            return e;
        }

        protected virtual void Clone(FSMEvent e)
        {
            e.eventName = eventName;
            e.isCancelable = isCancelable;
            e.isCanceled = false;
            e.isHandled = false;
        }



    }

}
