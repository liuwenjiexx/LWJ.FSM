using System;
using System.Xml;

namespace LWJ.FSM.Model.Xml
{
    public class InvalidConvertException : FSMReadException
    {
        public InvalidConvertException(XmlNode node, string parseText, Type convertToType, Exception innerException = null)
               : this(null, node, parseText, convertToType, innerException)
        {
        }

        public InvalidConvertException(string message, XmlNode node, string parseText, Type convertToType, Exception innerException = null)
            : base(message ?? Resource1.Parse_InvalidConvert, node, innerException)
        {
            this.ParseText = parseText;
            this.ConvertToType = convertToType;
        }

        public string ParseText { get; private set; }

        public Type ConvertToType { get; private set; }

        public override string Message
        {
            get
            {
                string message = base.Message;

                message += Environment.NewLine + Resource1.Parse_ParseText.FormatArgs(ParseText);
                if (ConvertToType != null)
                    message += Environment.NewLine + Resource1.Parse_ConvertTypeName.FormatArgs(ConvertToType.FullName);
                return message;
            }
        }

    }
}
