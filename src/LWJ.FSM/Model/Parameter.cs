using LWJ.Expressions;
using System;

namespace LWJ.FSM.Model
{
    public class Parameter
    {

        public string Name { get; set; }


        public Type Type { get; set; }


        public object DefaultValue { get; set; }

        public Expressions.Expression DefaultValueExpr { get; set; }

        public Parameter(Type type, string name)
            : this(type, name, null, null)
        {

        }

        public Parameter(Type type, string name, object defaultValue, Expression defaultValueExpr)
        {
            this.Type = type;
            this.Name = name;
            if (defaultValueExpr == null)
            {
                if (defaultValue == null)
                {
                    if (type.IsValueType)
                        DefaultValue = Activator.CreateInstance(type);
                    else
                        DefaultValue = null;
                }
                else
                {
                    DefaultValue = defaultValue;
                }
            }
            else
            {
                this.DefaultValueExpr = defaultValueExpr;
            }
        }

        public override string ToString()
        {
            return "Name={0}, Type={1}".FormatArgs(Name, Type.FullName);
        }

    }

}
