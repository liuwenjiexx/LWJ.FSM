using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LWJ.FSM;
using System.Reflection;
using System.Text;
using System.Threading;

namespace LWJ.FSM.Xml.Test
{
    [TestClass]
    public partial class TimeWatchTest
    {
        public static FSMachine GetFSM()
        {
            return BaseTest.GetFSM();
        }
        [TestMethod]
        public void Watch_05S()
        {
            var fsm = GetFSM();
            fsm.LoadXml(TestUtils.LoadText("xml.demo.TimeWatch.xml"));

            fsm.Logger = new ConsoleLog();
            fsm.Start();

            fsm.SendEvent("watch.start");
            fsm.Update();
            fsm.Update(0.2f);
            fsm.Update(0.2f);
            fsm.Update(0.2f);
            fsm.SendEvent("watch.stop");
            fsm.Update();

           
            Assert.IsTrue((float)fsm["time"] > 0.5);
        }

    }

}
