using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace UdderlyEvelyn.SimplePipes
{
    public class CompoundResourceUser : CompoundPipe, ICompoundResourceUser
    {
        protected float[] _amountPerTick;
        protected bool _enabled = true;

        public float[] AmountPerTick
        {
            get => _amountPerTick;
            set => _amountPerTick = value;
        }

        public bool Enabled
        {
            get => _enabled;
            set => _enabled = value;
        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            MapComponentCache<MapComponent_SimplePipes>.GetFor(Map).RegisterUser(this);
        }

        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {
            MapComponentCache<MapComponent_SimplePipes>.GetFor(Map).DeregisterUser(this);
            base.Destroy(mode);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref _amountPerTick, "AmountPerTick");
            Scribe_Values.Look(ref _enabled, "Enabled");
        }
    }
}