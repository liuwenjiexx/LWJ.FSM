namespace LWJ.FSM
{
    public interface IFSMLogger
    {
        void Log(string type, string messageFormat, params object[] args);

    }

}
