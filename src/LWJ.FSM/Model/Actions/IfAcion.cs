using System;
using System.Collections.Generic;
using LWJ.Expressions;
using System.Collections.ObjectModel;

namespace LWJ.FSM.Model
{


    public class IfAcion : ActionsContainer
    {

        private Expression cond;

        private List<IfAcion> elseIfs = new List<IfAcion>();

        private ActionsContainer @else;

        /// <summary>
        /// if test value
        /// </summary>
        public Expression Cond { get => cond; set => cond = value; }

        /// <summary>
        /// elseIf action
        /// </summary>
        public ReadOnlyCollection<IfAcion> ElseIfs => elseIfs.AsReadOnly();

        /// <summary>
        /// else action
        /// </summary>
        public ActionsContainer Else { get => @else; set => @else = value; }

        public void AddElseIf(IfAcion elseIf)
        {
            elseIfs.Add(elseIf);
        }

        public void RemoveElseIf(IfAcion elseIf)
        {
            elseIfs.Remove(elseIf);
        }


        public override void Execute(FSMExecutionContext ctx)
        {
            if (ctx.EvalExpressionBool(Cond))
            {
                base.Execute(ctx);
                return;
            }

            if (ElseIfs != null)
            {
                foreach (var elseIf in ElseIfs)
                {
                    if (ctx.EvalExpressionBool(elseIf.Cond))
                    {
                        elseIf.Execute(ctx);
                        return;
                    }
                }
            }

            if (Else != null)
            {
                Else.Execute(ctx);
            }

        }
    }
}
