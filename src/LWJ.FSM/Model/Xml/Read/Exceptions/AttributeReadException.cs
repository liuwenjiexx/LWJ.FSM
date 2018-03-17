using System;
using System.Xml;

namespace LWJ.FSM.Model.Xml
{
    public class AttributeReadException : FSMReadException
    {
        public AttributeReadException(XmlNode node, string attributeName)
               : this(null, node,attributeName)
        {
        }

        public AttributeReadException(string message, XmlNode node, string attributeName)
            : base(message ?? Resource1.Parse_AttributeError, node)
        {
            this.AttributeName = attributeName;
        }

        public string AttributeName { get; private set; }

        public override string Message
        {
            get
            {
                string message = base.Message;
                if (!string.IsNullOrEmpty(AttributeName))
                    message += Environment.NewLine + Resource1.Parse_AttributeName.FormatArgs(AttributeName);
                return message;
            }
        }

    }
}
