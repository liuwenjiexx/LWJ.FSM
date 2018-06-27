using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LWJ.FSM;
namespace LWJ.FSM.Xml.Test
{ 
    public partial class TimeWatchTest
    {
        
        [TestMethod]
        public void Sum()
        {
            var fsm = GetFSM();
            fsm.LoadXml(TestUtils.LoadText("xml.demo.sum.xml"));
       
            fsm.Logger = new ConsoleLog();
            fsm.Start();
            int n = 5;
            fsm["n"] = n;
            fsm.SendEvent("start");
            fsm.Update(); 
            var result= RecursiveSum(n);
            Console.WriteLine("result:" + result);
            Assert.AreEqual(result, fsm["result"]);
            System.Collections.IDictionary dic;
             
        }
        static int RecursiveSum(int n)
        {
            return n <= 1 ? 1 : n + RecursiveSum(--n);
        }
    }
}
