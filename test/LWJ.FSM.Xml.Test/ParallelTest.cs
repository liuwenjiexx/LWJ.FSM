using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;


namespace LWJ.FSM.Xml.Test
{
    [TestClass]
    public class ParallelTest 
    {

        [TestMethod]
        public void Parallel()
        {
            FSMachine fsm = new FSMachine();
            fsm.LoadXml(TestUtils.LoadText("xml.parallel.xml"));
            fsm.Logger = new LogMessageTrace();
            fsm.Start();
            Assert.AreEqual("s1", fsm.Current.Name);
            Assert.AreEqual("s1.entry_s11.entry_s12.entry"
                , fsm.Logger.ToString(), "start");

            fsm.Logger = new LogMessageTrace();
            fsm.SendEvent("to.s2");
            fsm.Update();
            Assert.AreEqual("s2", fsm.Current.Name);
            Assert.AreEqual("s11.update_s12.update_s11.exit_s12.exit_s1.exit_s2.entry"
                , fsm.Logger.ToString(), "update");

        }

    }
}
