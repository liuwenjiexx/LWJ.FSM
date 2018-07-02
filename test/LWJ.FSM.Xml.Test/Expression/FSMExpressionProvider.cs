using LWJ.Expressions;
using LWJ.Expressions.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace LWJ.FSM.Xml.Test
{
    class FSMExpressionProvider : IFSMExpressionProvider
    {
        private XmlExpressionReader exprReader = new Expressions.Xml.XmlExpressionReader();

        private Dictionary<Expression, InvocationDelegate> cachedExprs;


        private InvocationDelegate GetOrCacheExprEval(IExpressionContext context, Expression expr)
        {
            InvocationDelegate eval;
            if (cachedExprs == null)
            {
                cachedExprs = new Dictionary<Expression, InvocationDelegate>();

            }
            if (!cachedExprs.TryGetValue(expr, out eval))
            {
                var compileContext = new CompileContext(context);
                eval = compileContext.Compile(expr);
                cachedExprs[expr] = eval;
            }
            return eval;
        }

        public object ReadExpr(FSMContext context, XmlNode exprNode, bool isBlock)
        {
            CompileContext ctx = new CompileContext(new FSMExpressionContextAdapter(context));

            var expr = exprReader.Read(exprNode, isBlock, ctx);
            return expr;
        }

        public object EvalExpression(FSMContext context, object expr)
        {
            var ctx = new FSMExpressionContextAdapter(context);
            InvocationDelegate eval = null;
            if (expr is string)
            {
                // eval = GetOrCacheExprEval(ctx, expr as string);
            }
            else
            {
                eval = GetOrCacheExprEval(ctx, expr as Expression);
            }
            if (eval == null)
                return null;
            object result = eval(ctx);
            return result;
        }

        public object ReadExpr(FSMContext context, string expr, bool isBlock)
        {
            CompileContext ctx = new CompileContext(new FSMExpressionContextAdapter(context));
            return LWJ.Expressions.Script.ScriptExpressionReader.Instance.Parse(expr, ctx);
        }
    }
}
