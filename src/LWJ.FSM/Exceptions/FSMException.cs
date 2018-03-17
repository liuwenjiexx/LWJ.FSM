using System;

namespace LWJ.FSM
{
    public class FSMException : Exception
    {
        public FSMException(string message)
            : this(message, null)
        {
        }

        public FSMException(string message, Exception innerException)
            : base(message ?? Resource1.FSM_Exception, innerException)
        {
        }

    }


}
