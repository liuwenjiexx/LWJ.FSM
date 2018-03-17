using System;

namespace LWJ.FSM
{
    public class FSMParameterException : FSMException
    {
        public FSMParameterException(string paramName)
            : this(null, paramName, null)
        {
        }

        public FSMParameterException(string message, string paramName)
            : this(message, paramName, null)
        {
        }

        public FSMParameterException(string message, string paramName,  Exception innerException)
            : base(message ?? Resource1.FSM_ParameterException, innerException)
        {
            this.ParamName = paramName;
        }

        public string ParamName { get; private set; }

        public override string Message
        {
            get
            {
                string message = base.Message;
                if (!string.IsNullOrEmpty(ParamName))
                    message += Environment.NewLine + Resource1.FSM_ParamName.FormatArgs(ParamName);
                return message;
            }
        }

    }
}
