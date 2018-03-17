using LWJ.FSM.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace LWJ.FSM.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void AA()
        {
            List<int> list = new List<int>();

            list.Add(0);
            list.Add(1);
            list.Add(2);

            ReadOnlyCollection<int> list2 = list.AsReadOnly();

            Assert.AreEqual(1, list2[1]);

            list.Add(3);
            Assert.AreEqual(3, list2[3]);

        }

        [TestMethod]
        public void Base()
        {
            State root = new State();
            root.Initial = "s1";

            State s1 = new State();
            s1.Name = "s1";

            root.AddChild(s1);

            FSMachine fsm = new FSMachine();
            fsm.SetRoot(root);
            Assert.IsNull(fsm.Current);
            fsm.Start();
            var current = fsm.Current;
            Assert.IsNotNull(current);
            Assert.AreEqual("s1", current.Name);
        }
        [TestMethod]
        public void Empty()
        {
            State root = new State();

            FSMachine fsm = new FSMachine();
            fsm.SetRoot(root);
            Assert.IsFalse(fsm.Root.IsActive);
            Assert.IsNull(fsm.Current);

            fsm.Start();
            Assert.IsNull(fsm.Current);
            Assert.IsTrue(fsm.Root.IsActive);
        }


    }

}
