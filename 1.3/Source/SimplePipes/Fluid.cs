using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UdderlyEvelyn.SimplePipes
{
    public class Fluid : IEquatable<Fluid>
    {
        uint ID;
        string Name;
        FluidType Type = FluidType.None;

        bool IEquatable<Fluid>.Equals(Fluid other)
        {
            return this.ID == other.ID;
        }
    }
}
