using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UdderlyEvelyn.SimplePipes
{
    public class Circuit
    {
        public List<Pipe> Pipes;
        public float Capacity;
        public float Content;
        public Fluid Fluid;

        public Circuit(IEnumerable<Pipe> pipes = null)
        {
            if (pipes != null)
                Pipes = new List<Pipe>(pipes);
            else
                Pipes = new List<Pipe>();
        }

        public void Merge(Circuit circuit)
        {
            foreach (Pipe pipe in circuit.Pipes) //Loop through those pipes..
                pipe.Circuit = this; //Assign them to this circuit.
            Pipes.AddRange(circuit.Pipes);
            Capacity += circuit.Capacity;
            Content += circuit.Content;
        }
    }
}
