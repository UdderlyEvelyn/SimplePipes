using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UdderlyEvelyn.SimplePipes
{
    public interface ISink : IResourceUser
    {
        public float LastTickPulled { get; set; }

        public float TicksPerPull { get; set; }

        public bool Supplied { get; set; }
    }
}