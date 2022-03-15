using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UdderlyEvelyn.SimplePipes
{
    public interface ISink : IResourceUser
    {
        public float PulledPerTick { get; set; }

        public int LastTickPulled { get; set; }

        public int TicksPerPull { get; set; }

        public bool Supplied { get; set; }
    }
}