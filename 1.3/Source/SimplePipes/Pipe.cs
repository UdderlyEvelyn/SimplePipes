using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace UdderlyEvelyn.SimplePipes
{
    public class Pipe : Building
    {
        public Fluid Fluid;
        public float Capacity;
        public Circuit Circuit;

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
            Scribe_Values.Look(ref Fluid, "Fluid");
            Scribe_Values.Look(ref Capacity, "Capacity");
            Scribe_Values.Look(ref Circuit, "Circuit");
        }
    }
}
