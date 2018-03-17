using System.Collections.Generic;

namespace LWJ.FSM.Model
{

    public sealed class ExitState : TransitionalTarget
    { 
        public const string StateName = "state.exit";

        public override string Name
        {
            get => StateName;
            set { }
        }



    }

}
