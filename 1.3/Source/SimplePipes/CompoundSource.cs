using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace UdderlyEvelyn.SimplePipes
{
    public class CompoundSource : CompoundResourceUser
    {
        public float[] OriginalResourceTotal;
        public float[] Remaining;
        public bool[] LimitedAmount;
        public bool[] Empty;

        public CompoundSource()
        {
            for (int i = 0; i < Resources.Length; i++)
                if (LimitedAmount[i])
                    Remaining[i] = OriginalResourceTotal[i];
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
