using LWJ.FSM.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;
using static LWJ.Expressions.Expression;


namespace LWJ.FSM.Test
{
    [TestClass]
    public class TransitionTest
    {


        [TestMethod]
        public void Transition_Initial()
        {
            State root = new State("root");
            root.Initial = "s2";

            State s1 = new State("s1");
            root.AddChild(s1);

            State s2 = new State("s2");
            root.AddChild(s2);

            FSMachine fsm = new FSMachine();
            fsm.SetRoot(root);
            fsm.Start();

            Assert.AreEqual("s2", fsm.Current.Name);

        }

        [TestMethod]
        public void Transition_Entry()
        {
            State root = new State("root");
            EntryState entry = new EntryState();
            entry.AddTransition(new Transition("s2"));
            root.EntryState = entry;

            State s1 = new State("s1");
            root.AddChild(s1);

            State s2 = new State("s2");
            root.AddChild(s2);

            FSMachine fsm = new FSMachine();
            fsm.SetRoot(root);
            fsm.Start();

            Assert.AreEqual("s2", fsm.Current.Name);

        }

        [TestMethod]
        public void Transition_Multi()
        {
            State root = new State("root");
            root.Initial = "s1";

            State s1 = new State("s1");
            s1.AddTransition(new Transition("s2"));
            root.AddChild(s1);

            State s2 = new State("s2");
            s2.AddTransition(new Transition("s3"));
            root.AddChild(s2);

            State s3 = new State("s3");
            s3.AddTransition(new Transition("s4"));
            root.AddChild(s3);

            State s4 = new State("s4");
            root.AddChild(s4);

            FSMachine fsm = new FSMachine();
            fsm.SetRoot(root);
            fsm.Start();

            Assert.AreEqual("s4", fsm.Current.Name);

        }

        [ExpectedException(typeof(FSMStateException), AllowDerivedTypes = true)]
        [TestMethod]
        public void Transition_NotTransitionParent()
        {
            State root = new State("root");
            root.Initial = "s1";

            State s1 = new State("s1");
            s1.Initial = "s11";

            State s11 = new State("s11");
            s11.AddTransition(new Transition("s2"));
            s1.AddChild(s11);

            root.AddChild(s1);

            State s2 = new State("s2");
            root.AddChild(s2);

            FSMachine fsm = new FSMachine();
            fsm.SetRoot(root);
            fsm.Start();

            Assert.AreEqual("s1", fsm.Current.Name);

        }


        [TestMethod]
        public void Transition_Event()
        {
            State root = new State("root");
            root.Initial = "s1";

            State s1 = new State("s1");
            s1.AddTransition(new Transition("s2", "to.s2"));
            root.AddChild(s1);

            State s2 = new State("s2");
            root.AddChild(s2);

            StringBuilder sb = new StringBuilder();

            FSMachine fsm = new FSMachine();
            fsm.SetRoot(root);
            fsm.StateTransition += (object o, TransitionEventArgs e) =>
            {
                sb.Append(e.FromState == null ? "null" : e.FromState.Name)
                .Append("=>")
                .Append(e.ToState.Name);
            };


            fsm.Start();
            var current = fsm.Current;
            Assert.IsNotNull(current);
            Assert.AreEqual("s1", current.Name);

            fsm.SendEvent("to.s2");
            fsm.Update();

            current = fsm.Current;
            Assert.IsNotNull(current);
            Assert.AreEqual("s2", current.Name);
            Console.WriteLine(sb.ToString());

        }

        [TestMethod]
        public void Transition_Event_Cond()
        {
            State root = new State("root");
            root.Initial = "s1";
            State s1 = new State("s1");
            s1.AddParamerter(new Parameter(typeof(bool), "isTrue"));
            s1.AddTransition(new Transition("s2", "to.s2", Variable<int>("isTrue")));
            root.AddChild(s1);

            State s2 = new State("s2");
            root.AddChild(s2);

            FSMachine fsm = new FSMachine();
            fsm.SetRoot(root);

            fsm.Start();
            Assert.AreEqual("s1", fsm.Current.Name);

            fsm.SendEvent("to.s2");
            fsm.Update();
            Assert.AreEqual("s1", fsm.Current.Name);

            fsm.GetState("s1").SetParameter("isTrue", true);
            fsm.SendEvent("to.s2");
            fsm.Update();

            Assert.AreEqual("s2", fsm.Current.Name);
        }


    }
}
