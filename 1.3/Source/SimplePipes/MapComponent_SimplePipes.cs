using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace UdderlyEvelyn.SimplePipes
{
    public class MapComponent_SimplePipes : MapComponent
    {
        public List<Circuit> Circuits = new List<Circuit>();
        public List<Sink> Sinks = new List<Sink>();
        public List<Source> Sources = new List<Source>();

        public List<CompoundCircuit> CompoundCircuits = new List<CompoundCircuit>();
        public List<CompoundSink> CompoundSinks = new List<CompoundSink>();
        public List<CompoundSource> CompoundSources = new List<CompoundSource>();

        public MapComponent_SimplePipes(Map map) : base(map)
        {

        }

        public void RegisterPipe(Pipe pipe)
        {
            List<Pipe> foundPipes = new List<Pipe>();
            List<Thing> things = new List<Thing>();
            GenAdjFast.AdjacentThings8Way(pipe, things);
            for (int i = 0; i < things.Count; i++) //Loop through things adjacent to the pipe we're registering..
            {
                if (things[i] is Pipe otherPipe) //If we find a pipe..
                {
                    foundPipes.Add(otherPipe); //Store this for later!
                    if (pipe.Circuit == null) //If we don't have a circuit yet..
                        pipe.Circuit = otherPipe.Circuit; //Assign it to that circuit.
                    else //We have a circuit already, we're mixing 'em together into one big circuit now!
                    {
                        pipe.Circuit.Merge(otherPipe.Circuit);
                        if (Circuits.Contains(otherPipe.Circuit))
                            Circuits.Remove(otherPipe.Circuit);
                        else
                            Log.Error("[Simple Pipes] Attempted to remove unregistered circuit after merge.");
                    }
                }
            }
            if (pipe.Circuit == null) //If we didn't find a circuit after all that..
            {
                pipe.Circuit = new Circuit(new[] { pipe }) { Capacity = pipe.Capacity, Resource = pipe.Resource }; //New circuit.
                Circuits.Add(pipe.Circuit); //Register circuit.
            }
            foreach (var foundPipe in foundPipes) //Go through any pipes we found earlier.
                if (foundPipe.Circuit == null) //If they don't have a circuit..
                {
                    foundPipe.Circuit = pipe.Circuit; //Assign them to this one.
                    pipe.Circuit.Pipes.Add(foundPipe);
                }

        }

        public void DeregisterPipe(Pipe pipe)
        {
            var foundCircuitPipe = false;
            Circuit circuit = null;
            List<Thing> things = new List<Thing>();
            GenAdjFast.AdjacentThings8Way(pipe, things);
            for (int i = 0; i < things.Count; i++) //Loop through things adjacent to the pipe we're deregistering..
            {
                if (things[i] is Pipe otherPipe) //If we find a pipe..
                {
                    List<Thing> thingsInner = new List<Thing>();
                    GenAdjFast.AdjacentThings8Way(otherPipe, thingsInner);
                    for (int j = 0; j < thingsInner.Count; j++) //Loop through things around the other pipe..?
                    {
                        if (thingsInner[j] is Pipe pipeInQuestion) //If something around the other pipe is also a pipe..
                        {
                            if (pipeInQuestion != pipe) //Skip the pipe we're yeeting..
                            {
                                if (pipeInQuestion.Circuit != null && circuit == null) //If this pipe has a circuit and we don't have one yet..
                                {
                                    foundCircuitPipe = true; //We found one!
                                    break; //Quit looking.
                                }
                                else if (pipeInQuestion.Circuit != null && circuit == null) //Else if the pipe has a different circuit and we don't already have an alternative circuit..
                                    circuit = pipeInQuestion.Circuit; //Store it so we can swap to that circuit if there's no more keeping us in this one.
                            }
                            if (!foundCircuitPipe) //If we didn't find any other connection to the circuit, it's cut off!
                            {
                                if (otherPipe.Circuit.Pipes.Count == 1) //If this is the last pipe in the circuit..
                                    Circuits.Remove(otherPipe.Circuit); //Circuit gone.
                                otherPipe.Circuit.Pipes.Remove(otherPipe); //Remove pipe from circuit.
                                if (circuit != null) //If we found an alternative circuit..
                                {
                                    otherPipe.Circuit = circuit; //Assign this pipe to it..
                                    otherPipe.Circuit.Pipes.Add(otherPipe); //And put it in the list.
                                }
                            }
                        }
                    }
                }
            }
        }

        public void RegisterUser(ResourceUser user)
        {
            if (user is Source)
                Sources.Add((Source)user);
            else if (user is Sink)
                Sinks.Add((Sink)user);
            else
                Log.Error("[Simple Pipes] Attempted to register ResourceUser that was neither a sink nor a source.");
        }

        public void DeregisterUser(ResourceUser user)
        {
            if (user is Source)
                Sources.Remove((Source)user);
            else if (user is Sink)
                Sinks.Remove((Sink)user);
            else
                Log.Error("[Simple Pipes] Attempted to deregister ResourceUser that was neither a sink nor a source.");
        }

        public void RegisterPipe(CompoundPipe pipe)
        {
            List<CompoundPipe> foundPipes = new List<CompoundPipe>();
            List<Thing> things = new List<Thing>();
            GenAdjFast.AdjacentThings8Way(pipe, things);
            for (int i = 0; i < things.Count; i++) //Loop through things adjacent to the pipe we're registering..
            {
                if (things[i] is CompoundPipe otherPipe) //If we find a pipe..
                {
                    foundPipes.Add(otherPipe); //Store this for later!
                    if (pipe.Circuit == null) //If we don't have a circuit yet..
                        pipe.Circuit = otherPipe.Circuit; //Assign it to that circuit.
                    else //We have a circuit already, we're mixing 'em together into one big circuit now!
                    {
                        pipe.Circuit.Merge(otherPipe.Circuit);
                        if (CompoundCircuits.Contains(otherPipe.Circuit))
                            CompoundCircuits.Remove(otherPipe.Circuit);
                        else
                            Log.Error("[Simple Pipes] Attempted to remove unregistered compound circuit after merge.");
                    }
                }
            }
            if (pipe.Circuit == null) //If we didn't find a circuit after all that..
            {
                pipe.Circuit = new CompoundCircuit(new[] { pipe }) { Capacities = pipe.Capacities, Resources = pipe.Resources }; //New circuit.
                CompoundCircuits.Add(pipe.Circuit); //Register circuit.
            }
            foreach (var foundPipe in foundPipes) //Go through any pipes we found earlier.
                if (foundPipe.Circuit == null) //If they don't have a circuit..
                {
                    foundPipe.Circuit = pipe.Circuit; //Assign them to this one.
                    pipe.Circuit.Pipes.Add(foundPipe);
                }

        }

        public void DeregisterPipe(CompoundPipe pipe)
        {
            var foundCircuitPipe = false;
            CompoundCircuit circuit = null;
            List<Thing> things = new List<Thing>();
            GenAdjFast.AdjacentThings8Way(pipe, things);
            for (int i = 0; i < things.Count; i++)
            {
                Thing thing = things[i];
                if (thing is CompoundPipe)
                {
                    var otherPipe = (CompoundPipe)thing;
                    List<Thing> thingsInner = new List<Thing>();
                    GenAdjFast.AdjacentThings8Way(otherPipe, thingsInner);
                    for (int j = 0; j < thingsInner.Count; j++)
                    {
                        Thing thingInner = thingsInner[j];
                        if (thingInner is CompoundPipe pipeInQuestion)
                        {
                            if (pipeInQuestion != pipe) //Skip the pipe we're yeeting..
                            {
                                if (pipeInQuestion.Circuit != null && circuit == null)
                                {
                                    foundCircuitPipe = true;
                                    break;
                                }
                                else if (pipeInQuestion.Circuit != null && circuit == null) //Else if the pipe has a different circuit and we don't already have an alternative circuit..
                                    circuit = pipeInQuestion.Circuit; //Store it so we can swap to that circuit if there's no more keeping us in this one.
                            }
                            if (!foundCircuitPipe) //If we didn't find any other connection to the circuit, it's cut off!
                            {
                                if (otherPipe.Circuit.Pipes.Count == 1) //If this is the last pipe in the circuit..
                                    CompoundCircuits.Remove(otherPipe.Circuit); //Circuit gone.
                                otherPipe.Circuit.Pipes.Remove(otherPipe); //Remove pipe from circuit.
                                if (circuit != null) //If we found an alternative circuit..
                                {
                                    otherPipe.Circuit = circuit; //Assign this pipe to it..
                                    otherPipe.Circuit.Pipes.Add(otherPipe); //And put it in the list.
                                }
                            }
                        }
                    }
                }
            }
        }

        public void RegisterUser(CompoundResourceUser user)
        {
            if (user is CompoundSource)
                CompoundSources.Add((CompoundSource)user);
            else if (user is CompoundSink)
                CompoundSinks.Add((CompoundSink)user);
            else
                Log.Error("[Simple Pipes] Attempted to register CompoundResourceUser that was neither a sink nor a source.");
        }

        public void DeregisterUser(CompoundResourceUser user)
        {
            if (user is CompoundSource)
                CompoundSources.Remove((CompoundSource)user);
            else if (user is CompoundSink)
                CompoundSinks.Remove((CompoundSink)user);
            else
                Log.Error("[Simple Pipes] Attempted to deregister CompoundResourceUser that was neither a sink nor a source.");
        }

        //public void RecalculateCircuits()
        //{
        //    var pipes = map.listerBuildings.AllBuildingsColonistOfClass<Pipe>();
        //    foreach (var pipe in pipes)
        //    {

        //    }
        //}

        public override void MapComponentTick()
        {
            int tick = Find.TickManager.TicksGame;
            foreach (var source in Sources)
            {
                if (source.LimitedAmount && source.Empty) //It's limited and out of stuff..
                    continue; //Skip!
                source.Circuit.Content += source.AmountPerTick; //Contribute fluid..
                if (source.LimitedAmount) //If it's limited..
                {
                    source.Remaining -= source.AmountPerTick; //Reduce remaining fluid..
                    if (source.Remaining <= 0) //If we're out entirely..
                    {
                        source.Empty = true; //Mark it empty so we can skip it in the future.
                        source.Remaining = 0; //Make sure it's not <0.
                    }
                }
            }
            foreach (var source in CompoundSources)
            {
                for (int i = 0; i < source.Resources.Length; i++)
                {
                    var limitedAmount = source.LimitedAmount[i];
                    var amountPerTick = source.AmountPerTick[i];
                    if (limitedAmount && source.Empty[i]) //It's limited and out of stuff..
                        continue; //Skip!
                    source.Circuit.Contents[i] += amountPerTick; //Contribute fluid..
                    if (limitedAmount) //If it's limited..
                    {
                        source.Remaining[i] -= amountPerTick; //Reduce remaining fluid..
                        if (source.Remaining[i] <= 0) //If we're out entirely..
                        {
                            source.Empty[i] = true; //Mark it empty so we can skip it in the future.
                            source.Remaining[i] = 0; //Make sure it's not <0.
                        }
                    }
                }
            }
            foreach (var sink in Sinks)
            {
                if (sink.TicksPerPull > 0) //If it has a TicksPerPull..
                {
                    if (tick - sink.LastTickPulled >= sink.TicksPerPull) //If it's time to pull..
                    {
                        if (sink.Circuit.Content >= sink.AmountPerTick) //There's enough fluid..
                        {
                            sink.Circuit.Content -= sink.AmountPerTick; //Pull
                            sink.LastTickPulled = tick; //We have pulled, so record that.
                            sink.Supplied = true;
                        }
                        else
                            sink.Supplied = false;
                    }
                    else if (sink.Circuit.Content >= sink.AmountPerTick) //There's enough fluid..
                    {
                        sink.Circuit.Content -= sink.AmountPerTick; //Pull
                        sink.Supplied = true;
                    }
                    else
                        sink.Supplied = false;
                }
            }
            foreach (var sink in CompoundSinks)
            {
                for (int i = 0; i < sink.Resources.Length; i++)
                {
                    var ticksPerPull = sink.TicksPerPull[i];
                    var amountPerTick = sink.AmountPerTick[i];
                    if (ticksPerPull > 0) //If it has a TicksPerPull..
                    {
                        if (tick - sink.LastTickPulled[i] >= ticksPerPull) //If it's time to pull..
                        {
                            if (sink.Circuit.Contents[i] >= amountPerTick) //There's enough fluid..
                            {
                                sink.Circuit.Contents[i] -= amountPerTick; //Pull
                                sink.LastTickPulled[i] = tick; //We have pulled, so record that.
                                sink.Supplied[i] = true;
                            }
                            else
                                sink.Supplied[i] = false;
                        }
                        else if (sink.Circuit.Contents[i] >= amountPerTick) //There's enough fluid..
                        {
                            sink.Circuit.Contents[i] -= amountPerTick; //Pull
                            sink.Supplied[i] = true;
                        }
                        else
                            sink.Supplied[i] = false;
                    }
                }
            }
        }

        public override void ExposeData()
        {
            Scribe_Collections.Look(ref Circuits, "Circuits");
            Scribe_Collections.Look(ref Sources, "Sources");
            Scribe_Collections.Look(ref Sinks, "Sinks");
            Scribe_Collections.Look(ref CompoundCircuits, "CompoundCircuits");
            Scribe_Collections.Look(ref CompoundSources, "CompoundSources");
            Scribe_Collections.Look(ref CompoundSinks, "CompoundSinks");
        }
    }
}