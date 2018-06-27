using LWJ.FSM.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Text;
using System;

namespace LWJ.FSM.Xml.Test
{

    [TestClass]
    public class ActionTest
    {
        public class StringTraceLog : IFSMLogger
        {
            private StringBuilder sb = new StringBuilder();

            public void Log(string type, string message, params object[] args)
            {
                if (sb.Length != 0)
                    sb.Append("_");
                message = message ?? string.Empty;
                sb.Append(type).Append(":").AppendFormat(message, args);
            }

            public override string ToString()
            {
                return sb.ToString();
            }
        }

        [TestMethod]
        public void Log()
        {


            FSMachine fsm = GetExprFSM();
            fsm.LoadXml(TestUtils.LoadText("xml.actions.log.xml"));
            StringTraceLog log = new StringTraceLog();
            fsm.Logger = log;
            fsm.Start();

            Assert.AreEqual(":root.msg_string:root.type.string_format:root.format_:s1.onEntry_expr:s1.type.expr_format:1+2=3",
                log.ToString());
        }


        [TestMethod]
        public void Raise()
        {
            FSMachine fsm = new FSMachine();
            fsm.LoadXml(TestUtils.LoadText("xml.actions.raise.xml"));

            fsm.Start();
            Assert.AreEqual("s3", fsm.Current.Name);
        }

        [TestMethod]
        public void Raise_Data()
        {
            FSMachine fsm = GetExprFSM();
            fsm.LoadXml(TestUtils.LoadText("xml.actions.raise_data.xml"));
            fsm.Logger = new LogMessageTrace();
            fsm.Start();
            Assert.AreEqual("s2", fsm.Current.Name);
            Assert.AreEqual("Hello World", fsm.Logger.ToString());
        }


        [TestMethod]
        public void Cancel()
        {
            FSMachine fsm = new FSMachine();
            fsm.LoadXml(TestUtils.LoadText("xml.actions.cancel.xml"));
            fsm.Start();
            Assert.AreEqual("s1", fsm.Current.Name);
        }

        [TestMethod]
        public void If()
        {
            FSMachine fsm = GetExprFSM();
            fsm.LoadXml(TestUtils.LoadText("xml.actions.if.xml"));
            fsm.Logger = new LogMessageTrace();
            fsm.SendEvent("to.s2");
            fsm.Start();
            Assert.AreEqual("s1", fsm.Current.Name);
            Assert.AreEqual("0", fsm.Logger.ToString());

            fsm.Logger = new LogMessageTrace();
            fsm["n"] = 1;
            fsm.SendEvent("to.s2");
            fsm.Update();
            Assert.AreEqual("s1", fsm.Current.Name);
            Assert.AreEqual("1", fsm.Logger.ToString());

            fsm.Logger = new LogMessageTrace();
            fsm["n"] = 2;
            fsm.SendEvent("to.s2");
            fsm.Update();
            Assert.AreEqual("s1", fsm.Current.Name);
            Assert.AreEqual("2", fsm.Logger.ToString());

            fsm.Logger = new LogMessageTrace();
            fsm["n"] = 3;
            fsm.SendEvent("to.s2");
            fsm.Update();
            Assert.AreEqual("s1", fsm.Current.Name);
            Assert.AreEqual("3", fsm.Logger.ToString());
        }

        [TestMethod]
        public void Foreach()
        {
            FSMachine fsm = GetExprFSM();
            fsm.Logger = new LogMessageTrace();

            int[] intArray = new int[] { 1, 2, 3 };
            fsm.GlobalContext.AddParameter(intArray.GetType(), "array");
            fsm.GlobalContext.SetParameter("array", intArray);

            fsm.LoadXml(TestUtils.LoadText("xml.actions.foreach.xml"));
            fsm.Start();

            Assert.AreEqual("index:0, value:1_index:1, value:2_index:2, value:3", fsm.Logger.ToString());

            fsm =   GetExprFSM();
            fsm.Logger = new LogMessageTrace();

            List<int> listArray = new List<int>(new int[] { 1, 2, 3 });
            fsm.GlobalContext.AddParameter(listArray.GetType(), "array");
            fsm.GlobalContext.SetParameter("array", listArray);
            fsm.LoadXml(TestUtils.LoadText("xml.actions.foreach.xml"));
            fsm.Start();

            Assert.AreEqual("index:0, value:1_index:1, value:2_index:2, value:3", fsm.Logger.ToString());
            //System.Func<A, B> aa=null;
            //System.Func<int, object> cc = null;
            //System.Func<B, A> bb = null;
            //bb = aa;
        }

        public static FSMachine GetExprFSM()
        {
            return BaseTest.GetFSM();
        }
        [TestMethod]
        public void Assign()
        {
            FSMachine fsm = GetExprFSM();
            fsm.LoadXml(TestUtils.LoadText("xml.actions.assign.xml"));
            fsm.Start();
            Assert.AreEqual(1, fsm["int32ByValue"]);
            Assert.AreEqual("Hello World", fsm["stringByExpr"]);
        }


        class A
        {

        }
        class B : A
        {

        }


        [TestMethod]
        public void Custom()
        {
            FSMachine fsm = new FSMachine();
            var reader = new Model.Xml.FSMXmlReader();
            reader.AddActionReader("myAction", "urn:test", typeof(MyAction), (Model.Xml.FSMXmlReader r) =>
             {
                 MyAction action = new MyAction();
                 action.Text = r.ReadAttributeValue<string>("text", null);

                 return action;
             });
            fsm.SetRoot(reader.Read(TestUtils.LoadText("xml.actions.custom.xml")));
            fsm.Logger = new LogMessageTrace();
            fsm.Start();
            Assert.AreEqual("hello world", fsm.Logger.ToString());
        }

        class MyAction : Model.Action
        {
            public string Text { get; set; }
            public override void Execute(FSMExecutionContext ctx)
            {
                ctx.StateMachine.Log(null, Text);
            }
        }

    }
}
