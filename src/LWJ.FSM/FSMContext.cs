using LWJ.FSM.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LWJ.FSM
{
    public class FSMContext : IFSMContext
    {
        private Dictionary<string, ParamData> paramerts;
        private FSMContext parent;

        private IFSMExpressionProvider exprProvider;

        public FSMContext()
            : this(null)
        {
        }

        public FSMContext(FSMContext parent)
        {
            this.parent = parent;
        }

        public FSMContext Parent => parent;

        public object this[string name] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }



        public IFSMExpressionProvider ExpressionProvider
        {
            get => exprProvider == null && parent != null ? parent.ExpressionProvider : exprProvider;
            set => exprProvider = value;
        }


        #region Parameter

        public bool ContainsParameter(string name)
        {
            var current = this;
            while (current != null)
            {
                if (current.paramerts != null && current.paramerts.ContainsKey(name))
                    return true;
                current = current.parent;
            }
            return false;
        }

        public object GetParameter(string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            return GetParamData(name).value;
        }

        public void SetParameter(string name, object value)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            GetParamData(name).value = value;
        }


        public Type GetParameterType(string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            return GetParamData(name).type;
        }

        public IEnumerable<string> EnumerateParameters()
        {
            var current = this;
            Dictionary<string, ParamData> ps;
            while (current != null)
            {
                ps = current.paramerts;
                if (ps != null)
                {
                    foreach (var name in ps.Keys)
                        yield return name;
                }
                current = current.parent;
            }
        }


        internal ParamData GetParamData(string name)
        {
            var current = this;
            Dictionary<string, ParamData> ps;
            ParamData data = null;
            while (current != null)
            {
                ps = current.paramerts;

                if (ps != null && ps.TryGetValue(name, out data))
                    break;
                current = current.parent;
            }

            if (data == null)
                throw new FSMParameterException(Resource1.FSM_NotFoundParameter, name);

            return data;
        }

        public void AddParameter(Type type, string name)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (name == null) throw new ArgumentNullException(nameof(name));
            object defaultValue;
            if (type.IsValueType)
                defaultValue = Activator.CreateInstance(type);
            else
                defaultValue = null;
            AddParameter(type, name, defaultValue);
        }

        public void AddParameter(Type type, string name, object defaultValue)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (name == null) throw new ArgumentNullException(nameof(name));
            AddParameter(new ParamData(type, name, defaultValue, null));
        }

        public void AddParameter(Type type, string name, Func<object> defaultValue)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (defaultValue == null) throw new ArgumentNullException(nameof(defaultValue));
            AddParameter(new ParamData(type, name, null, defaultValue));
        }

        private void AddParameter(ParamData data)
        {
            if (paramerts == null)
                paramerts = new Dictionary<string, ParamData>();
            paramerts[data.name] = data;
        }

        #endregion


        public void Reset()
        {
            if (paramerts != null)
            {
                object value;
                foreach (var p in paramerts.Values.ToArray())
                {
                    if (p.getDefaultValue != null)
                        value = p.getDefaultValue();
                    else
                        value = p.defaultValue;
                    SetParameter(p.name, value);
                }
            }
        }



    }
    internal class ParamData
    {
        public object value;
        public string name;
        public object defaultValue;
        public Type type;
        public Func<object> getDefaultValue;

        public ParamData(Type type, string name, object defaultValue, Func<object> getDefaultValue)
        {
            this.type = type;
            this.name = name;
            this.defaultValue = defaultValue;
            this.getDefaultValue = getDefaultValue;
        }

    }
}
