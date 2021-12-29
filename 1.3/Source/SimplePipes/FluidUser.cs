using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace UdderlyEvelyn.SimplePipes
{
    public class FluidUser : Pipe
    {
        public float AmountPerTick;

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            map.GetComponent<MapComponent_SimplePipes>().RegisterUser(this);
        }

        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {
            Map.GetComponent<MapComponent_SimplePipes>().DeregisterUser(this);
            base.Destroy(mode);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref AmountPerTick, "AmountPerTick");
        }
    }
}
