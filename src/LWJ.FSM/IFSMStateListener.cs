using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LWJ.FSM
{

    public interface IFSMStateListener
    {
        void OnEntry(FSMachine fsm, EntryEventArgs e);

        void OnTransition(FSMachine fsm, TransitionEventArgs e);

        void OnExit(FSMachine fsm, ExitEventArgs e);

        void OnUpdate(FSMachine fsm);
    }
}
