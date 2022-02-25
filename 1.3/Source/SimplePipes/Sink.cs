using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;

namespace UdderlyEvelyn.SimplePipes
{
    public class Sink : ResourceUser, ISink
    {
        protected float _pulledPerTick;
        protected float _lastTickPulled = 0;
        protected float _ticksPerPull;
        protected bool _supplied = false;

        public float PulledPerTick
        {
            get => _pulledPerTick;
            set => _pulledPerTick = value;
        }

        public float LastTickPulled
        {
            get => _lastTickPulled;
            set => _lastTickPulled = value;
        }

        public float TicksPerPull
        {
            get => _ticksPerPull;
            set => _ticksPerPull = value;
        }

        public bool Supplied
        {
            get => _supplied;
            set => _supplied = value;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref _pulledPerTick, "PulledPerTick");
            Scribe_Values.Look(ref _lastTickPulled, "LastTickPulled");
            Scribe_Values.Look(ref _ticksPerPull, "TicksPerPull");
            Scribe_Values.Look(ref _supplied, "Supplied");
        }
    }
}
