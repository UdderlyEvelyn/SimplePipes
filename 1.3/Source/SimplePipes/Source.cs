using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace UdderlyEvelyn.SimplePipes
{
    public class Source : ResourceUser
    {
        public float OriginalResourceTotal;
        public float Remaining;
        public bool LimitedAmount;
        public bool Empty = false;

        public Source()
        {
            if (LimitedAmount)
                Remaining = OriginalResourceTotal;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref OriginalResourceTotal, "OriginalResourceTotal");
            Scribe_Values.Look(ref Remaining, "Remaining");
            Scribe_Values.Look(ref LimitedAmount, "LimitedAmount");
            Scribe_Values.Look(ref Empty, "Empty");
        }
    }
}
