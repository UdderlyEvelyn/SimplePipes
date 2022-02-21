using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace UdderlyEvelyn.SimplePipes
{
    public class Resource : IEquatable<Resource>, IExposable
    {
        public uint ID;
        public string Name;
        public ResourceType Type = ResourceType.None;

        bool IEquatable<Resource>.Equals(Resource other)
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
