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
        public List<IPipe> Pipes = new();
        public float Capacity;
        public float Content;
        public Resource Resource;
        public event Action<float> ExcessiveCapacity;
        public event Action<float> InsufficientContent;
        public event Action<Circuit> OnAbsorbed;
        public event Action<Circuit> OnAbsorb;
        public bool Initialized = false;

        public virtual void Initialize()
        {
            Initialized = true;
        }

        public virtual void Merge(Circuit circuit)
        {
            if (Resource != circuit.Resource)
                return; //Don't.
            if (circuit.Pipes == null)
            {
                foreach (var pipe in circuit.Pipes) //Loop through those pipes..
                    pipe.Circuit = this; //Assign them to this circuit.
                Pipes.AddRange(circuit.Pipes);
                Capacity += circuit.Capacity;
                Content += circuit.Content;
            }
            if (OnAbsorb != null)
                OnAbsorb(circuit);
            if (circuit.OnAbsorbed != null)
                circuit.OnAbsorbed(this);
        }

        /// <summary>
        /// Pulls resources out of the circuit manually.
        /// </summary>
        /// <param name="value">how much to pull out</param>
        /// <returns>whether the circuit had enough resources to satisfy the request</returns>
        public bool Pull(float value)
        {
            var newTotal = Content - value;
            if (newTotal >= 0)
            {
                Content -= value;
                return true;
            }
            if (InsufficientContent != null)
                InsufficientContent(newTotal);
            return false;
        }

        /// <summary>
        /// Pushes resources into the circuit manually.
        /// </summary>
        /// <param name="value">how much to push in</param>
        /// <returns>whether the circuit had enough capacity to satisfy the request</returns>
        public bool Push(float value)
        {
            var newTotal = Content + value;
            if (newTotal <= Capacity)
            {
                Content = newTotal;
                return true;
            }
            if (ExcessiveCapacity != null)
                ExcessiveCapacity(Capacity - newTotal);
            return false;
        }

        internal void _raiseExcessiveCapacity(float amount)
        {
            ExcessiveCapacity(amount);
        }

        internal void _raiseInsufficientContent(float amount)
        {
            InsufficientContent(amount);
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
