using System;

namespace LWJ.FSM.Model
{

    /// <summary>
    /// <see cref="FSMachine.Logger"/>
    /// </summary>
    public class LogAcion : Action
    {
        private object[] exprs;
        private string message;
        private string type;
        private string format;

        /// <summary>
        /// log type
        /// </summary>
        public string Type { get => type; set => type = value; }

        /// <summary>
        /// message format <see cref="String.Format(string, object[])"/>
        /// </summary>
        public string Format { get => format; set => format = value; }

        /// <summary>
        /// log message
        /// </summary>
        public string Message { get => message; set => message = value; }

        /// <summary>
        /// expr
        /// <see cref="Format"/> arguments
        /// </summary>
        public object[] Arguments { get => exprs; set => exprs = value; }

        public override void Execute(FSMExecutionContext ctx)
        {
            string msg = null;
            var exprs = this.exprs;
            if (exprs != null && exprs.Length > 0)
            {
                if (format == null)
                {
                    var o = ctx.EvalExpression(exprs[0]);
                    if (o != null)
                        msg = o.ToString();
                }
                else
                {
                    var args = new object[exprs.Length];
                    for (int i = 0, len = exprs.Length; i < len; i++)
                        args[i] = ctx.EvalExpression(exprs[i]);
                    msg = format.FormatArgs(args);
                }
            }
            else
            {
                if (format == null)
                    msg = this.message;
                else
                    msg = format.FormatArgs(this.message);
            }
            ctx.StateMachine.Log(Type, msg);
        }
    }

}
