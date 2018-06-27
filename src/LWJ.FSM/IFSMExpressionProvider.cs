using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LWJ.FSM
{
    public interface IFSMExpressionProvider
    {
        object ReadExpr(FSMContext context, string expr, bool isBlock);
        object ReadExpr(FSMContext context, System.Xml.XmlNode exprNode, bool isBlock);
        object EvalExpression(FSMContext context,object expr);
    }
}
