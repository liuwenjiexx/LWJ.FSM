using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;

namespace LWJ.FSM.Model
{

    public abstract class EnterableState : TransitionalTarget
    {

        //bool isAtomicState;
        //bool isSimple;
        //bool isComposite;
        //bool isRegion;



        public override string ToString()
        {
            return "{0}: {1}".FormatArgs(nameof(EnterableState), Name);
        }

    }
}
