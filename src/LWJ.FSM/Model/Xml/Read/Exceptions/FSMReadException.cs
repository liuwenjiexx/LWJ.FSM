using System;
using System.Xml;

namespace LWJ.FSM.Model.Xml
{
    public class FSMReadException : Exception
    {
        public FSMReadException(string message, XmlNode node, Exception innerException = null)
            : base(message, innerException)
        {
            this.Node = node;
            if (node != null)
                NodeName = node.LocalName;
        }


        public XmlNode Node { get; private set; }

        public string NodeName { get; private set; }

        public override string Message
        {
            get
            {
                string message = base.Message;
                if (!string.IsNullOrEmpty(NodeName))
                    message += Environment.NewLine + Resource1.Parse_NodeName.FormatArgs(NodeName);
                return message;
            }
        }
    }
}
