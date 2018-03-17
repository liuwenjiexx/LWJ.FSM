using LWJ.Expressions;
using System;
using System.Collections.Generic;
using System.Xml;
using System.Linq;

namespace LWJ.FSM.Model.Xml
{
    public class FSMXmlReader
    {
        private LWJ.Expressions.Xml.XmlExpressionReader exprReader;
        private Dictionary<string, Dictionary<string, Func<FSMXmlReader, Action>>> actionReaders;
        private Dictionary<Type, Func<FSMXmlReader, Action>> actionTypeReaders;
        private XmlNamespaceManager nsmgr;
        private static Dictionary<string, Func<FSMXmlReader, Action>> cachedActionReaders;
        private static Dictionary<Type, Func<FSMXmlReader, Action>> cachedActionTypeReaders;
        private static Dictionary<string, string> cachedTypeNames;
        private static LWJ.Expressions.Xml.XmlExpressionReader defaultExprReader;
        private static readonly object lockObj = new object();
        private readonly object lockThisObj = new object();
        public const string RootNodeName = "fsm";
        public const string TransitionNodeName = "transition";
        public const string FSMNamespace = "urn:schema-lwj:fsm";
        private FSMContext context;
        private Stack<XmlNode> nodes;


        public FSMXmlReader()
            : this(null)
        {

        }

        public FSMXmlReader(Expressions.Xml.XmlExpressionReader exprReader)
        {
            this.exprReader = exprReader;
            lock (lockObj)
            {
                InitStaticMember();
            }
        }

        public XmlNode CurrentNode => nodes.Peek();

        LWJ.Expressions.Xml.XmlExpressionReader GetExprReader()
        {
            var reader = exprReader;
            if (reader == null)
            {
                if (defaultExprReader == null)
                    defaultExprReader = new Expressions.Xml.XmlExpressionReader();

                reader = defaultExprReader;

            }
            return reader;
        }


        public State Read(string xml, FSMContext context = null)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            return Read(doc.DocumentElement, context);
        }

        public State Read(XmlNode node, FSMContext context = null)
        {
            State result;
            lock (lockThisObj)
            {

                nsmgr = new XmlNamespaceManager(node.OwnerDocument.NameTable);
                nsmgr.AddNamespace("s", FSMNamespace);
                nodes = new Stack<XmlNode>();
                if (context == null)
                    this.context = new FSMContext();
                else
                    this.context = new FSMContext(context);
                ReadStartNode(node);
                result = ReadState();
                ReadEndNode();
                this.context = null;
                this.nodes = null;
                this.nsmgr = null;

            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodeName"></param>
        /// <param name="ns">node namespace</param>
        /// <param name="actionType"></param>
        /// <param name="parser"></param>
        public void AddActionReader(string nodeName, string ns, Type actionType, Func<FSMXmlReader, Action> parser)
        {
            if (actionType == null) throw new ArgumentNullException(nameof(actionType));
            if (parser == null) throw new ArgumentNullException(nameof(parser));

            if (actionReaders == null)
            {
                actionReaders = new Dictionary<string, Dictionary<string, Func<FSMXmlReader, Action>>>();
                actionTypeReaders = new Dictionary<Type, Func<FSMXmlReader, Action>>();
            }

            Dictionary<string, Func<FSMXmlReader, Action>> reader2;
            if (!actionReaders.TryGetValue(ns, out reader2))
            {
                reader2 = new Dictionary<string, Func<FSMXmlReader, Action>>();
                actionReaders[ns] = reader2;
            }

            reader2[nodeName] = parser;
            actionTypeReaders[actionType] = parser;
        }

        public void ReadStartNode(XmlNode node)
        {
            nodes.Push(node);
        }

        public void ReadEndNode()
        {
            nodes.Pop();
        }

        public State ReadState()
        {
            var state = new State();
            PushScope();
            ReadTransitionalState(state);
            PopScope();
            return state;
        }

        public ParallelState ReadParallelState()
        {
            var parallel = new ParallelState();
            PushScope();
            ReadTransitionalState(parallel);
            PopScope();
            return parallel;
        }

        public EntryState ReadEntryState()
        {
            var state = new EntryState();
            PushScope();
            ReadTransitionalTarget(state);
            foreach (XmlNode child in FilterChildNodeType(FSMNamespace))
            {
                ReadStartNode(child);
                switch (child.LocalName)
                {
                    case "transition":
                        state.AddTransition(ReadTransition());
                        break;
                }
                ReadEndNode();
            }

            PopScope();
            return state;
        }

        public ExitState ReadExitState()
        {
            var state = new ExitState();
            PushScope();
            ReadTransitionalTarget(state);
            PopScope();
            return state;
        }

        public void PushScope()
        {
            this.context = new FSMContext(context);
        }

        public void PopScope()
        {
            this.context = context.Parent;
        }

        public void ReadTransitionalTarget(TransitionalTarget state)
        {
            //if (CurrentNode.Name == RootNodeName)
            state.Name = ReadAttributeValue<string>("name", null);
            //else
            //    state.Name = ReadAttributeValue<string>("name");



            foreach (XmlNode child in FilterChildNodeType(FSMNamespace))
            {
                ReadStartNode(child);
                switch (child.LocalName)
                {
                    case "params":
                        foreach (var p in ReadParameters())
                        {
                            state.AddParamerter(p);
                            context.AddParameter(p.Type, p.Name);
                        }
                        break;
                    case "onEntry":
                        state.AddOnEntry(ReadStateOnEntry());
                        break;
                    case "onExit":
                        state.AddOnExit(ReadStateOnExit());
                        break;
                }
                ReadEndNode();
            }

            foreach (var action in ReadActions(FilterChildNodeType().Where(o => IsAction(o))))
            {
                state.AddAction(action);
            }

        }

        public void ReadEnterableState(EnterableState state)
        {
            ReadTransitionalTarget(state);

            //foreach (XmlNode child in FilterChildNodeType(FSMNamespace))
            //{
            //    ReadStartNode(child);
            //    switch (child.LocalName)
            //    {

            //    }
            //    ReadEndNode();
            //}
        }


        public void ReadTransitionalState(TransitionalState state)
        {
            ReadEnterableState(state);

            state.Initial = ReadAttributeValue<string>("initial", null);

            foreach (XmlNode child in FilterChildNodeType(FSMNamespace))
            {
                ReadStartNode(child);
                switch (child.LocalName)
                {
                    case "transition":
                        state.AddTransition(ReadTransition());
                        break;
                    //case "onInitial":
                    //    state.OnInitial = ReadStateOnInitial();
                    //    break;
                    case "entry":
                        state.EntryState = ReadEntryState();
                        break;
                    case "exit":
                        state.ExitState = ReadExitState();
                        break;
                    case "state":
                        state.AddChild(ReadState());
                        break;
                    case "parallel":
                        state.AddChild(ReadParallelState());
                        break;
                }
                ReadEndNode();
            }
        }

        public void ReadExecutable(Executable executable)
        {
            foreach (var action in ReadActions())
                executable.AddAction(action);
        }
        //public OnInitial ReadStateOnInitial()
        //{
        //    OnInitial evt = new OnInitial();
        //    foreach (var t in FilterChildNodeType())
        //    {
        //        ReadStartNode(t);
        //        if (t.Name == TransitionNodeName)
        //            evt.AddTransition(ReadTransition());
        //        else
        //            evt.AddAction(ReadAction());
        //        ReadEndNode();
        //    }

        //    return evt;
        //}
        public OnEntry ReadStateOnEntry()
        {
            OnEntry evt = new OnEntry();
            ReadExecutable(evt);
            return evt;
        }

        public OnExit ReadStateOnExit()
        {
            OnExit evt = new OnExit();
            ReadExecutable(evt);
            return evt;
        }

        public Action ReadAction(Type actionType)
        {
            var parser = GetActionParser(actionType);
            if (parser == null)
                return null;
            var action = parser(this);
            return action;
        }

        public Action ReadAction()
        {
            var parser = GetActionParser();
            if (parser == null)
                return null;
            var action = parser(this);
            return action;
        }
        private IEnumerable<Action> ReadActions(IEnumerable<XmlNode> nodes, int index = -1)
        {
            Action action = null;

            int n = 0;
            foreach (var item in nodes)
            {
                if (index != -1 && n < index)
                    continue;
                n++;
                ReadStartNode(item);
                action = ReadAction();
                ReadEndNode();
                if (action != null)
                    yield return action;

            }
        }


        private void ReadActions(IEnumerable<XmlNode> nodes, ActionsContainer container, int index = -1)
        {
            foreach (var action in ReadActions(nodes, index))
            {
                container.AddAction(action);
            }
        }

        private IEnumerable<Action> ReadActions()
        {
            Action action;
            foreach (var child in FilterChildNodeType())
            {
                ReadStartNode(child);
                action = ReadAction();
                ReadEndNode();
                if (action != null)
                    yield return action;
            }
        }

        #region parse action

        private static ExpressionAction ReadExpressionAction(FSMXmlReader reader)
        {
            ExpressionAction action = new ExpressionAction();
            action.Expr = reader.ReadExpression(true);

            return action;
        }

        private static CancelAcion ReadCancelAction(FSMXmlReader reader)
        {
            CancelAcion cancelAction = new CancelAcion();
            cancelAction.Event = reader.ReadAttributeValue<string>("event");
            return cancelAction;
        }


        //private void ReadIfAction(IfAcion ifAction)
        //{

        //}

        private static IfAcion ReadIfAction(FSMXmlReader reader)
        {
            IfAcion ifAction = new IfAcion();
            var node = reader.CurrentNode;
            var condNode = node.SelectSingleNode("s:cond", reader.nsmgr);
            if (condNode != null)
            {
                reader.ReadStartNode(condNode);
                ifAction.Cond = reader.ReadFirstChildExpression();
                reader.ReadEndNode();
            }
            var thenNode = node.SelectSingleNode("s:then", reader.nsmgr);

            if (thenNode != null)
            {
                reader.ReadStartNode(thenNode);
                reader.ReadActions(reader.FilterChildNodeType(), ifAction);
                reader.ReadEndNode();
            }


            //ReadIfAction(ifAction);

            foreach (XmlNode elseIfNode in node.SelectNodes("s:elseIf", reader.nsmgr))
            {
                reader.ReadStartNode(elseIfNode);
                //IfAcion elseIfAction = new IfAcion();
                //ReadIfAction(elseIfAction);
                IfAcion elseIfAction = ReadIfAction(reader);
                ifAction.AddElseIf(elseIfAction);
                reader.ReadEndNode();
            }
            var elseNode = node.SelectSingleNode("s:else", reader.nsmgr);

            if (elseNode != null)
            {
                reader.ReadStartNode(elseNode);
                ActionsContainer elseAction = new ActionsContainer();
                reader.ReadActions(reader.FilterChildNodeType(), elseAction);
                ifAction.Else = elseAction;
                reader.ReadEndNode();
            }
            return ifAction;
        }

        private static LogAcion ReadLogAction(FSMXmlReader reader)
        {
            LogAcion logAction = new LogAcion();
            logAction.Type = reader.ReadAttributeValue<string>("type", null);
            logAction.Message = reader.ReadAttributeValue<string>("msg", null);
            logAction.Format = reader.ReadAttributeValue<string>("format", null);
            logAction.Arguments = reader.ReadChildExpressions().ToArray();
            return logAction;
        }

        private static ForeachAcion ReadForeachAction(FSMXmlReader reader)
        {
            ForeachAcion foreachAction = new ForeachAcion();

            var childs = reader.FilterChildNodeType();
            var itemsNode = childs.FirstOrDefault();
            if (itemsNode.LocalName == "items")
            {
                reader.ReadStartNode(itemsNode);
                foreachAction.Items = reader.ReadFirstChildExpression();
                reader.ReadEndNode();
            }
            else
            {
                throw new FSMReadException("Not Child Node items", reader.CurrentNode);
            }
            reader.PushScope();
            reader.context.AddParameter(typeof(int), "index");
            reader.context.AddParameter(typeof(object), "item");
            reader.ReadActions(childs.Skip(1), foreachAction);
            reader.PopScope();
            return foreachAction;
        }

        private static RaiseAction ReadRaiseAction(FSMXmlReader reader)
        {
            RaiseAction raise = new RaiseAction();
            raise.Event = reader.ReadAttributeValue<string>("event");

            foreach (var child in reader.FilterChildNodeType(FSMNamespace))
            {
                reader.ReadStartNode(child);
                switch (child.LocalName)
                {
                    case "data":
                        raise.DataExpr = reader.ReadFirstChildExpression(null);
                        break;
                }
                reader.ReadEndNode();
            }

            return raise;
        }

        private static AssignAction ReadAssignAction(FSMXmlReader reader)
        {
            AssignAction act = new AssignAction();
            act.Name = reader.ReadAttributeValue<string>("name");
            act.Expr = reader.ReadChildExpressions().FirstOrDefault();
            if (act.Expr == null)
                act.Value = reader.ReadAttributeValue<string>("value");
            return act;
        }

        #endregion

         

        public Transition ReadTransition()
        {
            Transition transition = new Transition();
            transition.Target = ReadAttributeValue<string>("target");
            transition.Event = ReadAttributeValue<string>("event", null);

            var firstNode = FilterChildNodeType().FirstOrDefault();
            if (firstNode != null)
            {
                if (firstNode.LocalName == "cond")
                {
                    ReadStartNode(firstNode);
                    transition.Cond = ReadFirstChildExpression(null);
                    ReadEndNode();
                    foreach (var action in ReadActions(FilterChildNodeType().Skip(1)))
                    {
                        transition.AddAction(action);
                    }
                }
                else
                {
                    foreach (var action in ReadActions(FilterChildNodeType()))
                    {
                        transition.AddAction(action);
                    }
                }
            }
            return transition;
        }




        public Expression ReadExpression(bool isBlock = false)
        {
            CompileContext ctx = new CompileContext(context.Adapter);
            return GetExprReader().Read(CurrentNode, isBlock, ctx);
        }
        public Expression ReadFirstChildExpression(bool isBlock = false)
        {
            var exprNode = FilterChildNodeType().FirstOrDefault();
            if (exprNode == null)
                throw new FSMReadException("Require Child Expr Node", CurrentNode);
            ReadStartNode(exprNode);
            var expr = ReadExpression(isBlock);
            ReadEndNode();
            return expr;
        }

        private Expression ReadFirstChildExpression(Expression defaultValue, bool isBlock = false)
        {
            var exprNode = FilterChildNodeType().FirstOrDefault();
            if (exprNode == null)
                return defaultValue;
            return ReadFirstChildExpression(isBlock);
        }
        public IEnumerable<Expression> ReadChildExpressions()
        {
            var exprReader = GetExprReader();
            foreach (XmlNode exprNode in FilterChildNodeType())
            {
                CompileContext ctx = new CompileContext(context.Adapter);
                var expr = exprReader.Read(exprNode, false, ctx);
                yield return expr;
            }
        }



        public IEnumerable<Parameter> ReadParameters()
        {

            foreach (XmlNode child in FilterChildNodeType(FSMNamespace))
            {
                if (child.LocalName == "param")
                {
                    ReadStartNode(child);
                    var p = ReadParameter();
                    ReadEndNode();
                    yield return p;
                }
            }

        }

        public Parameter ReadParameter()
        {
            string name = ReadAttributeValue<string>("name");
            Type type = ReadAttributeValue<Type>("type");
            object defaultValue = ReadAttributeValue("value", type, null);
            Expression defaultValueExpr = ReadFirstChildExpression(null);

            var p = new Parameter(type, name, defaultValue, defaultValueExpr);
            return p;
        }

        private IEnumerable<XmlNode> FilterChildNodeType()
        {
            foreach (XmlNode child in CurrentNode.ChildNodes)
            {
                if (child.NodeType == XmlNodeType.Element)
                    yield return child;
            }
        }

        private IEnumerable<XmlNode> FilterChildNodeType(string ns)
        {
            foreach (XmlNode child in CurrentNode.ChildNodes)
            {
                if (child.NodeType != XmlNodeType.Element)
                    continue;
                if (child.NamespaceURI != ns)
                    continue;
                yield return child;
            }
        }

        //public string ParseAttributeValue(XmlNode node, string name, string defaultValue)
        //{
        //    if (name == null) throw new ArgumentNullException(nameof(name));

        //    var attr = node.Attributes.GetNamedItem(name);
        //    if (attr == null)
        //        return defaultValue;
        //    return attr.Value;
        //}
        public T ReadAttributeValue<T>(string name, T defaultValue)
        {
            return (T)ReadAttributeValue(name, typeof(T), defaultValue);
        }
        public T ReadAttributeValue<T>(string name)
        {
            return (T)ReadAttributeValue(name, typeof(T));
        }
        public object ReadAttributeValue(string name, Type type)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            var node = CurrentNode;
            var attr = node.Attributes.GetNamedItem(name);
            if (attr == null) throw new AttributeReadException(Resource1.Parse_MissingAttribute, node, name);

            return ChangeType(node, attr.Value, type);
        }

        public object ReadAttributeValue(string name, Type type, object defaultValue)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            var node = CurrentNode;
            var attr = node.Attributes.GetNamedItem(name);
            if (attr == null) return defaultValue;

            return ChangeType(node, attr.Value, type);
        }

        //string GetAttributeValue(XmlNode node, string name)
        //{
        //    if (name == null) throw new ArgumentNullException(nameof(name));

        //    var attr = node.Attributes.GetNamedItem(name);
        //    if (attr == null) throw new MissingAttributeException(node, name);

        //    return attr.Value;
        //}

        object ChangeType(XmlNode node, string text, Type valueType)
        {
            object value;
            try
            {
                if (valueType == typeof(Type))
                {
                    text = GetTypeName(text);
                    value = Type.GetType(text, true);
                }
                else
                {
                    value = System.Convert.ChangeType(text, valueType);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidConvertException(node, node.InnerText, valueType, ex);
            }
            return value;
        }
        string GetTypeName(string typeName)
        {
            if (typeName != null)
            {
                string name;
                if (cachedTypeNames.TryGetValue(typeName, out name))
                    return name;
            }

            return typeName;
        }

        static void AddDefaultActionReader(string nodeName, Type actionType, Func<FSMXmlReader, Action> parser)
        {
            cachedActionReaders[nodeName] = parser;
            cachedActionTypeReaders[actionType] = parser;
        }

        static void InitStaticMember()
        {

            if (cachedTypeNames != null)
                return;
            cachedActionReaders = new Dictionary<string, Func<FSMXmlReader, Action>>();
            cachedActionTypeReaders = new Dictionary<Type, Func<FSMXmlReader, Action>>();


            AddDefaultActionReader("expr", typeof(ExpressionAction), ReadExpressionAction);
            AddDefaultActionReader("if", typeof(IfAcion), ReadIfAction);
            AddDefaultActionReader("raise", typeof(RaiseAction), ReadRaiseAction);
            AddDefaultActionReader("log", typeof(LogAcion), ReadLogAction);
            AddDefaultActionReader("cancel", typeof(CancelAcion), ReadCancelAction);
            AddDefaultActionReader("foreach", typeof(ForeachAcion), ReadForeachAction);
            AddDefaultActionReader("assign", typeof(AssignAction), ReadAssignAction);


            cachedTypeNames = new Dictionary<string, string>();

            cachedTypeNames["bool"] = typeof(bool).FullName;
            cachedTypeNames["string"] = typeof(string).FullName;
            cachedTypeNames["int32"] = typeof(int).FullName;
            cachedTypeNames["int64"] = typeof(long).FullName;
            cachedTypeNames["float32"] = typeof(float).FullName;
            cachedTypeNames["float64"] = typeof(double).FullName;
            cachedTypeNames["type"] = typeof(Type).FullName;
            cachedTypeNames["datetime"] = typeof(DateTime).FullName;

        }

        Func<FSMXmlReader, Action> GetActionParser()
        {
            XmlNode node = CurrentNode;
            string nodeName = node.LocalName;
            string ns = node.NamespaceURI;
            Dictionary<string, Func<FSMXmlReader, Action>> readers2;
            Func<FSMXmlReader, Action> result = null;

            if (actionReaders != null)
            {
                if (actionReaders.TryGetValue(ns, out readers2))
                {
                    readers2.TryGetValue(nodeName, out result);
                }
            }

            if (result == null && ns == FSMNamespace)
                cachedActionReaders.TryGetValue(nodeName, out result);

            if (result == null)
                throw new FSMReadException(Resource1.Parse_InvalidNodeName, node);

            return result;
        }
        Func<FSMXmlReader, Action> GetActionParser(Type actionType)
        {
            Func<FSMXmlReader, Action> result;
            if (actionReaders == null || !actionTypeReaders.TryGetValue(actionType, out result))
            {
                if (!cachedActionTypeReaders.TryGetValue(actionType, out result))
                    //    return null;
                    throw new FSMReadException(Resource1.Parse_InvalidNodeName, CurrentNode);
            }
            return result;
        }
        bool IsAction(XmlNode node)
        {
            string name = node.LocalName;
            if (actionReaders != null && actionReaders.ContainsKey(name))
                return true;
            if (cachedActionReaders != null && cachedActionReaders.ContainsKey(name))
                return true;
            return false;
        }


        //class Scope : IExpressionContext
        //{
        //    private Dictionary<string, Type> variables;
        //    public Scope parent;

        //    public object this[string name] { get => variables[name]; set => variables[name] = value; }

        //    public Scope()
        //    {
        //    }

        //    public Scope(Scope parent)
        //    {
        //        this.parent = parent;
        //    }

        //    public void AddVariable(Type type, string name)
        //    {
        //        if (variables == null)
        //            variables = new Dictionary<string, Type>();
        //        variables[name] = type;
        //    }

        //    public Type GetVariableType(string name)
        //    {
        //        if (variables != null)
        //        {
        //            Type type;
        //            if (variables.TryGetValue(name, out type))
        //                return type;
        //        }
        //        if (parent != null)
        //            return parent.GetVariableType(name);

        //        throw new VariableException(Resource1.Read_VarNotDefined, name);
        //    }

        //    public bool ContainsVariable(string name)
        //    {
        //        if (variables != null)
        //        {
        //            if (variables.ContainsKey(name))
        //                return true;
        //        }
        //        if (parent != null)
        //            return parent.ContainsVariable(name);
        //        return false;
        //    }



        //    public void SetVariable(string name, object value)
        //    {
        //        throw new NotImplementedException();
        //    }

        //    public object GetVariable(string name)
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

    }


}
