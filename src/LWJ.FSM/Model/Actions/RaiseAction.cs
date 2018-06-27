using System;

namespace LWJ.FSM.Model
{

    /// <summary>
    /// call <see cref="FSMachine.SendEvent(FSMEvent)"/>
    /// </summary>
    public class RaiseAction : Action
    {
        private string eventName;

        public RaiseAction()
        {
        }

        /// <summary>
        /// send event name
        /// </summary>
        public string Event { get => eventName; set => eventName = value; }

        /// <summary>
        /// send event data
        /// </summary>
        public object DataExpr { get; set; }

        //public Type DataType { get; set; }

        //public string Data { get; set; }

        public override void Execute(FSMExecutionContext ctx)
        {
            object data = null;
            if (DataExpr != null)
            {
                data = ctx.EvalExpression(DataExpr);
            } 
            ctx.StateMachine.SendEvent(eventName, data);
        }

    }

}
