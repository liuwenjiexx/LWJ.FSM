using System;

namespace LWJ.FSM
{
    public class FSMStateException : FSMException
    {
        public FSMStateException(string stateName)
            : this(null, stateName, null)
        {
        }

        public FSMStateException(string message, string stateName)
            : this(message, stateName, null)
        {
        }

        public FSMStateException(string message, string stateName, Exception innerException)
            : base(message ?? Resource1.FSM_StateException, innerException)
        {
            this.StateName = stateName;
        }

        public string StateName { get; private set; }

        public override string Message
        {
            get
            {
                string message = base.Message;
                if (!string.IsNullOrEmpty(StateName))
                    message += Environment.NewLine + Resource1.FSM_StateName.FormatArgs(StateName);
                return message;
            }
        }
    }
}
