using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LWJ.FSM.Model
{
    public class AssignAction : Action
    {

        private string name;
        private string value;
        private object expr;

        /// <summary>
        /// variable name
        /// </summary>
        public string Name { get => name; set => name = value; }
        /// <summary>
        /// value expression
        /// </summary>
        public object Expr { get => expr; set => expr = value; }

        /// <summary>
        /// value
        /// </summary>
        public string Value { get => value; set => this.value = value; }


        public override void Execute(FSMExecutionContext ctx)
        {
            object paramValue;
            if (expr == null)
            {
                paramValue = value;
            }
            else
            {
                paramValue = ctx.EvalExpression(expr);
            }
            Type paramType = ctx.Context.GetParameterType(name);

            if (paramValue != null)
            {
                Type valType = paramValue.GetType();
                if (!paramType.IsAssignableFrom(valType))
                {
                    paramValue = Convert.ChangeType(paramValue, paramType);
                }
            }
            else
            {
                if (paramType.IsValueType)
                {
                    if (paramValue == null)
                    {
                        paramValue = Activator.CreateInstance(paramType);
                    }
                }
            }

            ctx.Context.SetParameter(name, paramValue);
        }
    }
}
