using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace UdderlyEvelyn.SimplePipes
{
    public class Pipe : Building, IPipe
    {
        protected Resource _resource;
        protected float _capacity;
        protected Circuit _circuit;

        public Thing Thing => this;

        public Resource Resource
        {
            get => _resource;
            set => _resource = value;
        }

        public float Capacity
        {
            get => _capacity;
            set => _capacity = value;
        }

        public Circuit Circuit
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
            Scribe_Values.Look(ref _resource, "Resource");
            Scribe_Values.Look(ref _capacity, "Capacity");
            Scribe_Values.Look(ref _circuit, "Circuit");
        }
    }
}
