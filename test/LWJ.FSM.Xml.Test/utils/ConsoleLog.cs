using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LWJ.FSM.Xml.Test
{
    public class ConsoleLog : IFSMLogger
    {
        public void Log(string type, string messageFormat, params object[] args)
        {
            messageFormat = messageFormat ?? string.Empty;
            Console.WriteLine("{0}: {1}", type, string.Format(messageFormat, args));
        }
    }
}
