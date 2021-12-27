using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UdderlyEvelyn.SimplePipes
{
    public class FluidSource : FluidUser
    {
        public float OriginalFluidTotal;
        public float Remaining;
        public bool LimitedAmount;
        public bool Empty = false;

        public FluidSource()
        {
            if (LimitedAmount)
                Remaining = OriginalFluidTotal;
        }
    }
}
