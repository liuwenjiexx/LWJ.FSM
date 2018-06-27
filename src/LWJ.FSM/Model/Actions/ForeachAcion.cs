using System;
using System.Collections;
using System.Collections.Generic;

namespace LWJ.FSM.Model
{
    public class ForeachAcion : ActionsContainer
    {

        private object items;
        private string item;
        private string index;

        /// <summary>
        /// expr, array value
        /// </summary>
        public object Items { get => items; set => items = value; }


        /// <summary>
        /// item variable name
        /// </summary>
        public string Item { get => item ?? "item"; set => item = value; }

        /// <summary>
        /// index variable name
        /// </summary>
        public string Index { get => index ?? "index"; set => index = value; }

        public override void Execute(FSMExecutionContext ctx)
        {
            if (Items == null)
                return;
            var array = ctx.EvalExpression(Items) as IEnumerable;
            if (array == null)
                return;

            FSMContext ctx2;

            string indexVar = this.Index, itemVar = this.Item;

            Type arrayType = array.GetType();
            bool bb = typeof(IList<>).IsAssignableFrom(arrayType);
            Type itemType = null;
            if (arrayType.IsArray)
                itemType = arrayType.GetElementType();
            else if (arrayType.IsGenericType)
            {
                itemType = arrayType.GetGenericArguments()[0];
            }

            int index = 0;
            foreach (var item in array)
            {
                ctx2 = new FSMContext(ctx.Context);
                ctx2.AddParameter(typeof(int), indexVar);
                ctx2.SetParameter(indexVar, index++);
                if (itemType == null)
                    ctx2.AddParameter(item == null ? typeof(object) : item.GetType(), itemVar);
                else
                    ctx2.AddParameter(itemType, itemVar);
                ctx2.SetParameter(itemVar, item);

                base.Execute(new FSMExecutionContext(ctx.StateMachine, ctx.State, ctx2));
            }

        }
    }
}
