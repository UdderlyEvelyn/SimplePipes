using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace UdderlyEvelyn.SimplePipes
{
    public class Fluid : IEquatable<Fluid>, IExposable
    {
        uint ID;
        string Name;
        FluidType Type = FluidType.None;

        bool IEquatable<Fluid>.Equals(Fluid other)
        {
            return this.ID == other.ID;
        }

        public void ExposeData()
        {
            Scribe_Values.Look(ref ID, "ID");
            Scribe_Values.Look(ref Name, "Name");
            Scribe_Values.Look(ref Type, "Type");
        }
    }
}
