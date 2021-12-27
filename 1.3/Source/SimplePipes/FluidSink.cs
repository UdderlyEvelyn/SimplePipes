using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UdderlyEvelyn.SimplePipes
{
    public class FluidSink : FluidUser
    {
        public float LastTickPulled = 0;
        public float TicksPerPull;
        public bool Supplied = false;
    }
}
