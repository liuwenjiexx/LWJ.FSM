using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;

namespace LWJ.FSM.Xml.Test
{

    [TestClass]
    public class BaseTest
    {
        [TestMethod]
        public void Initial()
        {
            FSMachine fsm = new FSMachine();
            fsm.LoadXml(TestUtils.LoadText("xml.initial.xml"));

            Assert.IsNull(fsm.Current);
            Assert.IsNull(fsm.Root.Current);
            fsm.Start();
            Assert.AreEqual(fsm.Root.Current, fsm.Current);
            Assert.IsNotNull(fsm.Current);
            Assert.AreEqual("s1", fsm.Current.Name);
        }

        [TestMethod]
        public void Entry()
        {
            FSMachine fsm = new FSMachine();
            fsm.LoadXml(TestUtils.LoadText("xml.entry.xml"));

            Assert.IsNull(fsm.Current);
            Assert.IsNull(fsm.Root.Current);
            fsm.Start();
            Assert.AreEqual(fsm.Root.Current, fsm.Current);
            Assert.IsNotNull(fsm.Current);
            Assert.AreEqual("s1", fsm.Current.Name);
        }

        [TestMethod]
        public void Entry_Trace()
        {
            FSMachine fsm = new FSMachine();
            fsm.LoadXml(TestUtils.LoadText("xml.entry_trace.xml"));
            fsm.Logger = new LogMessageTrace();
            fsm.Start();
            Assert.AreEqual("entry.onentry_entry.transition_entry.onexit_s1.entry.onentry", fsm.Logger.ToString(), "start");
            fsm.Logger = new LogMessageTrace();
            fsm.Update();
            Assert.AreEqual("update_s1.update_s1.entry.update",
                fsm.Logger.ToString());

            fsm.Logger = new LogMessageTrace();
            fsm.SendEvent("to.exit");
            fsm.Update();
            Assert.AreEqual("update_s1.update_s1.entry.update_s1.entry.onexit_exit.onentry_exit.onexit_entry.onentry_entry.transition_entry.onexit_s1.entry.onentry",
                fsm.Logger.ToString(), "update");
        }
        [TestMethod]
        public void Entry_Reset()
        {
            FSMachine fsm = new FSMachine();
            fsm.LoadXml(TestUtils.LoadText("xml.entry_reset.xml"));
            fsm.Start();
            fsm["n"] = 1;
            Assert.AreEqual("s1", fsm.Current.Name);
            Assert.AreEqual(1, fsm["n"]);
            fsm.SendEvent("to.exit");
            fsm.Update();
            Assert.AreEqual(0, fsm["n"]);
        }

        [TestMethod]
        public void Params()
        {
            FSMachine fsm = new FSMachine();
            fsm.LoadXml(TestUtils.LoadText("xml.params.xml"));
            fsm.Start();

            Assert.AreEqual(0, fsm["i_0"]);
            Assert.AreEqual(0L, fsm["l_0"]);
            Assert.AreEqual(1, fsm["i_1"]);
            Assert.AreEqual(2, fsm["i_2"]);

            Assert.AreEqual(0f, fsm["f_0"]);
            Assert.AreEqual(0d, fsm["d_0"]);
            Assert.AreEqual(0.1f, fsm["f_01"]);
            Assert.AreEqual(0.2f, fsm["f_02"]);

            Assert.AreEqual(false, fsm["b_false"]);
            Assert.AreEqual(true, fsm["b_true"]);
            Assert.AreEqual(false, fsm["b_false2"]);
            Assert.AreEqual(true, fsm["b_true2"]);

            Assert.AreEqual(null, fsm["s_null"]);
            Assert.AreEqual("abc", fsm["s_abc"]);
            Assert.AreEqual("123", fsm["s_123"]);

            Assert.AreEqual(null, fsm["t_null"]);
            Assert.AreEqual(typeof(string), fsm["t_string"]);
            Assert.AreEqual(typeof(int), fsm["t_int"]);

            DateTime dt = new DateTime(2017, 1, 2, 3, 4, 5);
            Assert.AreEqual(dt, fsm["datetime_value"]);
            Assert.AreEqual(dt, fsm["datetime_expr"]);

            var s1 = fsm.GetState("s1");
            Assert.AreEqual(fsm["i_1"], s1["ref_i_1"]);
            try
            {
                fsm.Root.GetParameter("no_param");
                Assert.Fail();
            }
            catch (FSMException ex) { }
        }

        [TestMethod]
        public void Transition_Target()
        {
            FSMachine fsm = new FSMachine();
            fsm.LoadXml(TestUtils.LoadText("xml.transition_target.xml"));
            fsm.Start();

            Assert.AreEqual("s2", fsm.Current.Name);
        }

        [TestMethod]
        public void Transition_Event()
        {
            FSMachine fsm = new FSMachine();
            fsm.LoadXml(TestUtils.LoadText("xml.transition_event.xml"));
            fsm.Start();
            fsm.SendEvent("to.s2");
            fsm.Update();
            Assert.IsNotNull(fsm.Current);
            Assert.AreEqual("s2", fsm.Current.Name);
        }
        [TestMethod]
        public void Transition_Event_Back()
        {
            FSMachine fsm = new FSMachine();
            fsm.LoadXml(TestUtils.LoadText("xml.transition_event_back.xml"));
            fsm.SendEvent("to.s2");
            fsm.Start();
            Assert.IsNotNull(fsm.Current);
            Assert.AreEqual("s1", fsm.Current.Name);
        }
        [TestMethod]
        public void Transition_Cond()
        {
            FSMachine fsm = new FSMachine();
            fsm.LoadXml(TestUtils.LoadText("xml.transition_cond.xml"));
            fsm.Start();
            Assert.AreEqual("s1", fsm.Current.Name);
            fsm["a"] = true;
            fsm.Update();
            Assert.IsNotNull(fsm.Current);
            Assert.AreEqual("s2", fsm.Current.Name);
        }

        [TestMethod]
        public void Transition_Event_Cond()
        {
            FSMachine fsm = new FSMachine();
            fsm.LoadXml(TestUtils.LoadText("xml.transition_event_cond.xml"));
            fsm.Start();
            Assert.AreEqual("s1", fsm.Current.Name);
            fsm.SendEvent("to.s2");
            fsm.Update();
            Assert.AreEqual("s1", fsm.Current.Name);
            fsm.SendEvent("to.s2");
            fsm["a"] = true;
            fsm.Update();
            Assert.IsNotNull(fsm.Current);
            Assert.AreEqual("s2", fsm.Current.Name);
        }

        [TestMethod]
        public void Transition_Trace()
        {
            FSMachine fsm = new FSMachine();
            fsm.LoadXml(TestUtils.LoadText("xml.transition_trace.xml"));
            StringBuilder sb = new StringBuilder();
            fsm.Logger = new LogMessageTrace();
            fsm.StateTransition += (o, e) =>
            {
                string from = e.FromState == null ? "null" : e.FromState.Name;
                string to = e.ToState == null ? "null" : e.ToState.Name;
                Assert.IsNotNull(e.Transition);
                sb.Append("_" + from + ">" + to);
            };
            fsm.Start();

            Console.WriteLine(sb.ToString());

            Assert.AreEqual("s1.onEntry_s1.transition_s1.onExit_s2.onEntry_s2.transition_s2.onExit_s3.onEntry",
                fsm.Logger.ToString());

            Assert.AreEqual("_s1>s2_s2>s3", sb.ToString());
        }
        [TestMethod]
        public void Transition_Trace2()
        {
            FSMachine fsm = new FSMachine();
            fsm.LoadXml(TestUtils.LoadText("xml.transition_trace2.xml"));
            fsm.Logger = new LogMessageTrace();

            fsm.Start();

            Assert.AreEqual("s1.entry_s1.transition_s1.exit_s2.entry_s2.transition_s2.exit_s1.entry_s1.transition_s1.exit_s2.entry_s2.transition_s2.exit_s1.entry",
                fsm.Logger.ToString());
        }

    }
}
