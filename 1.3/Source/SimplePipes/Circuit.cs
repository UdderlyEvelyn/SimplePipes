using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace UdderlyEvelyn.SimplePipes
{
    public class Circuit : IExposable
    {
        public List<Pipe> Pipes;
        public float Capacity;
        public float Content;
        public Resource Resource;

        public Circuit(IEnumerable<Pipe> pipes = null)
        {
            if (pipes != null)
                Pipes = new List<Pipe>(pipes);
            else
                Pipes = new List<Pipe>();
        }

        public virtual void Merge(Circuit circuit)
        {
            if (Resource != circuit.Resource)
                return; //Don't.
            foreach (var pipe in circuit.Pipes) //Loop through those pipes..
                pipe.Circuit = this; //Assign them to this circuit.
            Pipes.AddRange(circuit.Pipes);
            Capacity += circuit.Capacity;
            Content += circuit.Content;
        }

        void IExposable.ExposeData()
        {
            Scribe_Collections.Look(ref Pipes, "Pipes");
            Scribe_Values.Look(ref Capacity, "Capacity");
            Scribe_Values.Look(ref Content, "Content");
            Scribe_Values.Look(ref Resource, "Resource");
        }
    }
}
