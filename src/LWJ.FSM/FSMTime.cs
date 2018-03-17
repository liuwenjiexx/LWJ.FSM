using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LWJ.FSM
{

    public class FSMTime
    {

        private double time;
        private float deltaTime;

        internal FSMTime()
        {
        }

        public double Time { get => time; internal set => time = value; }

        public float DeltaTime { get => deltaTime; internal set => deltaTime = value; }
          

    }

}
