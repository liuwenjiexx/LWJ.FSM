using LWJ.FSM.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static LWJ.Expressions.Expression;

namespace LWJ.FSM.Test
{
    [TestClass]
    public class ParameterTest
    {

        public static FSMachine GetFSM()
        {
            FSMContext context = new FSMContext();
            context.ExpressionProvider = new FSMExpressionProvider();
            FSMachine fsm = new FSMachine(context);
            return fsm;
        }


        [TestMethod]
        public void Parameter()
        {
            State root = new State();
            root.Initial = "s1";
            root.AddParamerter<int>("a");
            root.AddParamerter<int>("b");

            FSMachine fsm = new FSMachine();
            fsm.SetRoot(root);
            //Assert.IsFalse(fsm.Root.ContainsParameter("a"));
            fsm.Start();
            Assert.IsTrue(fsm.Root.ContainsParameter("a"));

            fsm["a"] = 1;
            fsm["b"] = 2;
            Assert.AreEqual(1, fsm["a"]);
            Assert.AreEqual(2, fsm["b"]);
            try
            {
                fsm.Root.GetParameter("1");
                Assert.Fail();
            }
            catch (FSMParameterException ex)
            {
            }
        }

        [TestMethod]
        public void Parameter_ExprEval()
        {
            State root = new State();
            root.Initial = "s1";
            root.AddParamerter<int>("a");
            root.AddParamerter<int>("b");
            root.AddParamerter<int>("result");

            //FSMachine fsm = GetFSM();
            FSMachine fsm = new FSMachine();
            fsm.SetRoot(root);
            fsm.Start();
            Assert.IsTrue(fsm.Root.ContainsParameter("a"), "not contains param a");

            //fsm["a"] = 1;
            //fsm["b"] = 2;
            //fsm.Root.EvalExpression(Assign(Variable("result"), Add(Variable("a"), Variable("b"))));
            //Assert.AreEqual(1, fsm["a"]);
            //Assert.AreEqual(2, fsm["b"]);
            //Assert.AreEqual(3, fsm["result"]);
        }

        [TestMethod]
        public void Parameter_Child()
        {
            State root = new State("root");
            root.Initial = "s1";
            root.AddParamerter<int>("a");
            State s1 = new State("s1");
            s1.AddParamerter<int>("b");
            root.AddChild(s1);
            root.AddParamerter<int>("result");

            FSMachine fsm = GetFSM();
            fsm.SetRoot(root);
            //Assert.IsFalse(fsm.Root.ContainsParameter("a"));
            fsm.Start();
            Assert.IsTrue(fsm.Root.ContainsParameter("a"));
            Assert.IsFalse(fsm.Root.ContainsParameter("b"));

            fsm["a"] = 1;
            fsm.GetState("s1")["b"] = 2;
            Assert.IsTrue(fsm.GetState("s1").ContainsParameter("b"));
            fsm.GetState("s1").GetContext().EvalExpression(Assign(Variable<int>("result"), Add(Variable<int>("a"), Variable<int>("b"))));
            Assert.AreEqual(1, fsm["a"]);
            Assert.AreEqual(2, fsm.GetState("s1")["b"]);
            Assert.AreEqual(3, fsm.GetState("s1")["result"]);
        }


    }
}
