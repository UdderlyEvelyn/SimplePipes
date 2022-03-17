using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;

namespace UdderlyEvelyn.SimplePipes
{
    public interface IPipe
    {
        public Type CircuitType { get; set; }

        public Resource Resource { get; set; }

        public float Capacity { get; set; }

        public Circuit Circuit { get; set; }

        public Thing Thing { get; }
    }
}