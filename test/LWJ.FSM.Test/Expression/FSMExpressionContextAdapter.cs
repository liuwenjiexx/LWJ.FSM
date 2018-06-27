using LWJ.Expressions;
using System;
using System.Collections.Generic;
namespace LWJ.FSM.Test
{
    class FSMExpressionContextAdapter : IExpressionContext
    {
        private FSMContext ctx;

        public object this[string name] { get => ctx[name]; set => ctx[name] = value; }

        public FSMExpressionContextAdapter(FSMContext ctx)
        {
            this.ctx = ctx;
        }

        public bool ContainsVariable(string name)
        {
            return ctx.ContainsParameter(name);
        }

        public IEnumerable<string> EnumerateVariables()
        {
            return ctx.EnumerateParameters();
        }

        public void SetVariable(string name, object value)
        {
            ctx.SetParameter(name, value);
        }

        public Type GetVariableType(string name)
        {
            return ctx.GetParameterType(name);
        }

        public object GetVariable(string name)
        {
            return ctx.GetParameter(name);
        }
    }

}
