using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;

namespace UdderlyEvelyn.SimplePipes
{
    public class FluidSink : FluidUser
    {
        public float LastTickPulled = 0;
        public float TicksPerPull;
        public bool Supplied = false;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref LastTickPulled, "LastTickPulled");
            Scribe_Values.Look(ref TicksPerPull, "TicksPerPull");
            Scribe_Values.Look(ref Supplied, "Supplied");
        }
    }
}
