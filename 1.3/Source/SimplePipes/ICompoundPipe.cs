using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace UdderlyEvelyn.SimplePipes
{
    public interface ICompoundPipe
    {
        public Type CircuitType { get; set; }

        public Resource[] Resources { get; set; }

        public float[] Capacities { get; set; }

        public CompoundCircuit Circuit { get; set; }

        public Thing Thing { get; }
    }
}