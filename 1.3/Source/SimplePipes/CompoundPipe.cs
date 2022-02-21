using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace UdderlyEvelyn.SimplePipes
{
    public class CompoundPipe : Building
    {
        public Resource[] Resources;
        public float[] Capacities;
        public CompoundCircuit Circuit;

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            map.GetComponent<MapComponent_SimplePipes>().RegisterPipe(this);
        }

        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {
            Map.GetComponent<MapComponent_SimplePipes>().DeregisterPipe(this);
            base.Destroy(mode);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref Resources, "Resources");
            Scribe_Values.Look(ref Capacities, "Capacities");
            Scribe_Values.Look(ref Circuit, "Circuit");
        }
    }
}
