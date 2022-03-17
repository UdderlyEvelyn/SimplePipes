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
        public List<ICompoundPipe> Pipes = new();
        public float[] Capacities;
        public float[] Contents;
        public Resource[] Resources;
        public event Action<Resource, float> ExcessiveCapacity;
        public event Action<Resource, float> InsufficientContent;
        public event Action<CompoundCircuit> OnAbsorb;
        public event Action<CompoundCircuit> OnAbsorbed;
        public bool Initialized = false;

        public virtual void Initialize()
        {
            Initialized = true;
        }

        public virtual void Merge(CompoundCircuit circuit)
        {
            for (int i = 0; i < Resources.Length; i++)
                if (Resources[i].ID != circuit.Resources[i].ID)
                    return; //Mismatch, don't merge.
            if (circuit.Pipes != null)
            {
                foreach (var pipe in circuit.Pipes) //Loop through those pipes..
                    pipe.Circuit = this; //Assign them to this circuit.
                Pipes.AddRange(circuit.Pipes);
                for (int i = 0; i < Resources.Length; i++)
                {
                    Capacities[i] += circuit.Capacities[i];
                    Contents[i] += circuit.Contents[i];
                }
            }
            if (OnAbsorb != null)
                OnAbsorb(circuit);
            if (circuit.OnAbsorbed != null)
                circuit.OnAbsorbed(this);
        }

        /// <summary>
        /// Pulls resources of a single type out of the circuit manually.
        /// </summary>
        /// <param name="value">how much to pull out</param>
        /// <returns>whether the circuit had enough resources to satisfy the request</returns>
        public bool Pull(Resource resource, float value)
        {
            var i = Array.IndexOf(Resources, resource);
            if (value <= Contents[i])
            {
                Contents[i] -= value;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Pushes resources of a single type into the circuit manually.
        /// </summary>
        /// <param name="value">how much to push in</param>
        /// <returns>whether the circuit had enough capacity to satisfy the request</returns>
        public bool Push(Resource resource, float value)
        {
            var i = Array.IndexOf(Resources, resource);
            var newTotal = Contents[i] + value;
            if (newTotal <= Capacities[i])
            {
                Contents[i] = newTotal;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Pulls resources out of the circuit manually.
        /// </summary>
        /// <param name="value">how much to pull out</param>
        /// <returns>whether the circuit had enough resources to satisfy the request</returns>
        public bool Pull(float[] values)
        {
            float[] newValues = new float[values.Length];
            for (int i = 0; i < values.Length; ++i)
                if ((newValues[i] = Contents[i] - values[i]) < 0)
                {
                    if (InsufficientContent != null)
                        InsufficientContent(Resources[i], newValues[i]);
                    return false;
                }
            Contents = newValues;
            return true;
        }

        /// <summary>
        /// Pushes resources into the circuit manually.
        /// </summary>
        /// <param name="value">how much to push in</param>
        /// <returns>whether the circuit had enough capacity to satisfy the request</returns>
        public bool Push(float[] values)
        {
            float[] newValues = new float[values.Length];
            for (int i = 0; i < values.Length; ++i)
                if ((newValues[i] = Contents[i] + values[i]) > Capacities[i])
                {
                    if (ExcessiveCapacity != null)
                        ExcessiveCapacity(Resources[i], Capacities[i] - newValues[i]);
                    return false;
                }
            Contents = newValues;
            return true;
        }

        internal void _raiseExcessiveCapacity(Resource resource, float amount)
        {
            ExcessiveCapacity(resource, amount);
        }

        internal void _raiseInsufficientContent(Resource resource, float amount)
        {
            InsufficientContent(resource, amount);
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
