using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace UdderlyEvelyn.SimplePipes
{
    public class CompoundPipe : Building, ICompoundPipe
    {
        protected Resource[] _resources;
        protected float[] _capacities;
        protected CompoundCircuit _circuit;

        public Thing Thing => this;

        public Resource[] Resources
        {
            get => _resources;
            set => _resources = value;
        }

        public float[] Capacities
        {
            get => _capacities;
            set => _capacities = value;
        }

        public CompoundCircuit Circuit
        {
            get => _circuit;
            set => _circuit = value;
        }

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
            Scribe_Values.Look(ref _resources, "Resources");
            Scribe_Values.Look(ref _capacities, "Capacities");
            Scribe_Values.Look(ref _circuit, "Circuit");
        }
    }
}
