using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace UdderlyEvelyn.SimplePipes
{
    public class CompoundCircuit : IExposable
    {
        public List<CompoundPipe> Pipes;
        public float[] Capacities;
        public float[] Contents;
        public Resource[] Resources;

        public CompoundCircuit(IEnumerable<CompoundPipe> pipes = null)
        {
            if (pipes != null)
                pipes = new List<CompoundPipe>(pipes);
            else
                pipes = new List<CompoundPipe>();
        }

        public virtual void Merge(CompoundCircuit circuit)
        {
            for (int i = 0; i < Resources.Length; i++)
                if (Resources[i].ID != circuit.Resources[i].ID)
                    return; //Mismatch, don't merge.
            foreach (var pipe in circuit.Pipes) //Loop through those pipes..
                pipe.Circuit = this; //Assign them to this circuit.
            Pipes.AddRange(circuit.Pipes);
            for (int i = 0; i < Resources.Length; i++)
            {
                Capacities[i] += circuit.Capacities[i];
                Contents[i] += circuit.Contents[i];
            }
        }

        void IExposable.ExposeData()
        {
            Scribe_Collections.Look(ref Pipes, "Pipes");
            Scribe_Values.Look(ref Capacities, "Capacities");
            Scribe_Values.Look(ref Contents, "Contents");
            Scribe_Values.Look(ref Resources, "Resources");
        }
    }
}
