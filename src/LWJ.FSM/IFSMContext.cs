using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LWJ.FSM
{
    public interface IFSMContext
    {
        object this[string name] { get; set; }

        bool ContainsParameter(string name);

        void SetParameter(string name, object value);

        object GetParameter(string name);

    }
}
