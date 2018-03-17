using LWJ.Expressions;

namespace LWJ.FSM.Model
{
    /// <summary>
    /// <see cref="LWJ.Expressions.BlockExpression"/>
    /// </summary>
    public class ExpressionAction : Action
    {
        private Expression expr;

        /// <summary>
        /// <see cref="Expressions.Expression"/>
        /// </summary>
        public Expression Expr { get => expr; set => expr = value; }

        public override void Execute(FSMExecutionContext ctx)
        {
            if (expr != null)
            {
                ctx.EvalExpression(expr);
            }
        }
    }
}
