using System;


namespace LWJ.FSM.Model
{

    /// <summary>
    /// cancel the most recent <see cref="RaiseAction"/> or <see cref="FSMachine.SendEvent(FSMEvent)"/> events
    /// </summary>
    public class CancelAcion : Action
    {
        private string eventName;
        /// <summary>
        /// event name
        /// </summary>
        public string Event { get => eventName; set => eventName = value; }

        public override void Execute(FSMExecutionContext ctx)
        {
            ctx.StateMachine.CancelEvent(Event);
        }
    }


}
