using System.Text;

namespace LWJ.FSM.Xml.Test
{
    public class LogMessageTrace : IFSMLogger
    {
        private StringBuilder sb = new StringBuilder();

        public void Log(string type, string messageFormat, params object[] args)
        {
            messageFormat = messageFormat ?? string.Empty;
            if (sb.Length != 0)
                sb.Append("_");
            sb.AppendFormat(messageFormat, args);
        }

        public override string ToString()
        {
            return sb.ToString();
        }
    }
}
