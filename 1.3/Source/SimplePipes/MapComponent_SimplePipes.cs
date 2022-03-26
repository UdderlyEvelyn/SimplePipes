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
        //Single-Resource System
        public List<Circuit> Circuits = new();
        public List<IHub> Hubs = new();
        public List<ISource> Sources = new();
        public List<ISink> Sinks = new();
        //Multi-Resource System
        public List<CompoundCircuit> CompoundCircuits = new();
        public List<ICompoundHub> CompoundHubs = new();
        public List<ICompoundSource> CompoundSources = new();
        public List<ICompoundSink> CompoundSinks = new();

        public MapComponent_SimplePipes(Map map) : base(map)
        {

        }

        public void RegisterPipe(IPipe pipe)
        {
            List<Pipe> foundPipes = new List<Pipe>();
            List<Thing> things = new List<Thing>();
            GenAdjFast.AdjacentThings8Way(pipe.Thing, things);
            for (int i = 0; i < things.Count; i++) //Loop through things adjacent to the pipe we're registering..
            {
                if (things[i] is Pipe otherPipe) //If we find a pipe..
                {
                    foundPipes.Add(otherPipe); //Store this for later!
                    if (pipe.Circuit == null) //If we don't have a circuit yet..
                    {
                        pipe.Circuit = otherPipe.Circuit; //Assign it to that circuit.
                        otherPipe.Circuit.Pipes.Add(pipe); //Assign it to that circuit's pipe list.
                    }
                    else if (pipe.CircuitType == otherPipe.CircuitType && pipe.Circuit != otherPipe.Circuit)
                    {
                        Log.Message("[Simple Pipes] Merging networks.");
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
                Log.Message("[Simple Pipes] Creating new network for pipe that could not locate one.");
                pipe.Circuit = (Circuit)Activator.CreateInstance(pipe.CircuitType);
                pipe.Circuit.Pipes.Add(pipe);
                pipe.Circuit.Capacity = pipe.Capacity;
                pipe.Circuit.Resource = pipe.Resource;
                Circuits.Add(pipe.Circuit); //Register circuit.
                pipe.Circuit.Initialize();
            }
            else
                addCapacityToCircuit(pipe);
            foreach (var foundPipe in foundPipes) //Go through any pipes we found earlier.
                if (foundPipe.Circuit == null) //If they don't have a circuit..
                {
                    foundPipe.Circuit = pipe.Circuit; //Assign them to this one.
                    pipe.Circuit.Pipes.Add(foundPipe);
                    addCapacityToCircuit(foundPipe);
                }
            if (pipe is IResourceUser user)
                RegisterUser(user);
        }

        public void DeregisterPipe(IPipe pipe)
        {
            var foundCircuitPipe = false;
            Circuit circuit = null;
            List<Thing> things = new List<Thing>();
            GenAdjFast.AdjacentThings8Way(pipe.Thing, things);
            for (int i = 0; i < things.Count; i++) //Loop through things adjacent to the pipe we're deregistering..
            {
                if (things[i] is IPipe otherPipe) //If we find a pipe..
                {
                    List<Thing> thingsInner = new List<Thing>();
                    GenAdjFast.AdjacentThings8Way(otherPipe.Thing, thingsInner);
                    for (int j = 0; j < thingsInner.Count; j++) //Loop through things around the other pipe..?
                    {
                        if (thingsInner[j] is IPipe pipeInQuestion) //If something around the other pipe is also a pipe..
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
                                removeCapacityFromCircuit(otherPipe);
                                if (circuit != null) //If we found an alternative circuit..
                                {
                                    otherPipe.Circuit = circuit; //Assign this pipe to it..
                                    otherPipe.Circuit.Pipes.Add(otherPipe); //And put it in the list.
                                    addCapacityToCircuit(otherPipe);
                                }
                            }
                        }
                    }
                }
            }
            removeCapacityFromCircuit(pipe);
            if (pipe is IResourceUser user)
                DeregisterUser(user);
        }

        public void RegisterUser(IResourceUser user)
        {
            if (user is IHub hub)
                Hubs.Add(hub);
            else if (user is ISource source)
                Sources.Add(source);
            else if (user is ISink sink)
                Sinks.Add(sink);
            else
                Log.Error("[Simple Pipes] Attempted to register ResourceUser that was neither a sink nor a source.");
        }

        public void DeregisterUser(IResourceUser user)
        {
            if (user is IHub hub)
                Hubs.Remove(hub);
            else if (user is ISource source)
                Sources.Remove(source);
            else if (user is ISink sink)
                Sinks.Remove(sink);
            else
                Log.Error("[Simple Pipes] Attempted to deregister ResourceUser that was neither a sink nor a source.");
        }

        public void RegisterPipe(ICompoundPipe pipe)
        {
            Log.Message("[Simple Pipes] Registering pipe " + pipe.ToString() + ".");
            List<ICompoundPipe> foundPipes = new List<ICompoundPipe>();
            List<Thing> things = new List<Thing>();
            GenAdjFast.AdjacentThings8Way(pipe.Thing, things);
            for (int i = 0; i < things.Count; i++) //Loop through things adjacent to the pipe we're registering..
            {
                if (things[i] is ICompoundPipe otherPipe) //If we find a pipe..
                {
                    foundPipes.Add(otherPipe); //Store this for later!
                    if (pipe.Circuit == null) //If we don't have a circuit yet..
                    {
                        pipe.Circuit = otherPipe.Circuit; //Assign it to that circuit.
                        otherPipe.Circuit.Pipes.Add(pipe); //Add it to the list of pipes for that circuit.
                    }
                    else if (pipe.CircuitType == otherPipe.CircuitType && pipe.Circuit != otherPipe.Circuit)
                    {
                        float pipeCircuitTotalCapacity = 0;
                        float otherPipeCircuitTotalCapacity = 0;
                        for (int j = 0; j < pipe.Circuit.Resources.Length; j++)
                        {
                            pipeCircuitTotalCapacity += pipe.Circuit.Capacities[j];
                            otherPipeCircuitTotalCapacity += otherPipe.Circuit.Capacities[j];
                        }
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
                Log.Message("[Simple Pipes] Creating new network for pipe that could not locate one.");
                pipe.Circuit = (CompoundCircuit)Activator.CreateInstance(pipe.CircuitType);
                pipe.Circuit.Pipes.Add(pipe);
                pipe.Circuit.Capacities = pipe.Capacities;
                pipe.Circuit.Resources = pipe.Resources;
                pipe.Circuit.Contents = new float[pipe.Resources.Length];
                CompoundCircuits.Add(pipe.Circuit); //Register circuit.
                pipe.Circuit.Initialize();
            }
            else
                addCapacityToCompoundCircuit(pipe);
            foreach (var foundPipe in foundPipes) //Go through any pipes we found earlier.
                if (foundPipe.Circuit == null) //If they don't have a circuit..
                {
                    foundPipe.Circuit = pipe.Circuit; //Assign them to this one.
                    pipe.Circuit.Pipes.Add(foundPipe);
                    addCapacityToCompoundCircuit(foundPipe);
                }
            if (pipe is ICompoundResourceUser user)
                RegisterUser(user);
        }

        public void DeregisterPipe(ICompoundPipe pipe)
        {
            var foundCircuitPipe = false;
            CompoundCircuit circuit = null;
            List<Thing> things = new List<Thing>();
            GenAdjFast.AdjacentThings8Way(pipe.Thing, things);
            for (int i = 0; i < things.Count; i++)
            {
                if (things[i] is ICompoundPipe otherPipe)
                {
                    List<Thing> thingsInner = new List<Thing>();
                    GenAdjFast.AdjacentThings8Way(otherPipe.Thing, thingsInner);
                    for (int j = 0; j < thingsInner.Count; j++)
                    {
                        if (thingsInner[j] is ICompoundPipe pipeInQuestion)
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
                                removeCapacityFromCompoundCircuit(otherPipe);
                                if (circuit != null) //If we found an alternative circuit..
                                {
                                    otherPipe.Circuit = circuit; //Assign this pipe to it..
                                    otherPipe.Circuit.Pipes.Add(otherPipe); //And put it in the list.
                                    addCapacityToCompoundCircuit(otherPipe);
                                }
                            }
                        }
                    }
                }
            }
            removeCapacityFromCompoundCircuit(pipe);
            if (pipe is ICompoundResourceUser user)
                DeregisterUser(user);
        }

        public void RegisterUser(ICompoundResourceUser user)
        {
            if (user is ICompoundHub hub)
                CompoundHubs.Add(hub);
            else if (user is ICompoundSource source)
                CompoundSources.Add(source);
            else if (user is ICompoundSink sink)
                CompoundSinks.Add(sink);
            else
                Log.Error("[Simple Pipes] Attempted to register CompoundResourceUser that was neither a hub, a sink nor a source.");
        }

        public void DeregisterUser(ICompoundResourceUser user)
        {
            if (user is ICompoundHub hub)
                CompoundHubs.Remove(hub);
            else if (user is ICompoundSource source)
                CompoundSources.Remove(source);
            else if (user is ICompoundSink sink)
                CompoundSinks.Remove(sink);
            else
                Log.Error("[Simple Pipes] Attempted to deregister CompoundResourceUser that was neither a hub, a sink nor a source.");
        }

        private void addCapacityToCircuit(IPipe pipe)
        {
            pipe.Circuit.Capacity += pipe.Capacity;
        }

        private void addCapacityToCompoundCircuit(ICompoundPipe pipe)
        {
            for (int i = 0; i < pipe.Circuit.Capacities.Length; i++)
                pipe.Circuit.Capacities[i] += pipe.Capacities[i];
        }

        private void removeCapacityFromCircuit(IPipe pipe)
        {
            pipe.Circuit.Capacity -= pipe.Capacity;
        }

        private void removeCapacityFromCompoundCircuit(ICompoundPipe pipe)
        {
            for (int i = 0; i < pipe.Circuit.Capacities.Length; i++)
                pipe.Circuit.Capacities[i] -= pipe.Capacities[i];
        }

        //This needs a refactor, big time.
        //Change to for loops, find a way to refactor to fewer loops, maybe just loop over all pipes and use HashSets for each type to do quick checks in one loop?
        public override void MapComponentTick()
        {
            int tick = Find.TickManager.TicksGame;
            foreach (var hub in Hubs)
            {
                var yield = hub.PushedPerTick;
                if (hub.LimitedAmount && hub.Empty) //It's limited and out of stuff..
                    yield = 0; //Skip!
                bool hasTicksPerPush = false;
                if ((hasTicksPerPush = hub.TicksPerPush > 0) && tick - hub.LastTickPushed < hub.TicksPerPush) //If it has a TicksPerPush and it's not time to push..
                    yield = 0; //Skip, not yet time!
                if (hub.Circuit.Push(yield) && hub.LimitedAmount) //If it's limited..
                {
                    hub.Remaining -= hub.PushedPerTick; //Reduce remaining resources..
                    if (hub.Remaining <= 0) //If we're out entirely..
                    {
                        hub.Empty = true; //Mark it empty so we can skip it in the future.
                        hub.Remaining = 0; //Make sure it's not <0.
                    }
                }
                if (hub.TicksPerPull > 0) //If it has a TicksPerPull..
                {
                    if (tick - hub.LastTickPulled >= hub.TicksPerPull) //If it's time to pull..
                    {
                        if (hub.Circuit.Content >= hub.PulledPerTick) //There's enough resources..
                        {
                            hub.Circuit.Content -= hub.PulledPerTick; //Pull
                            hub.LastTickPulled = tick; //We have pulled, so record that.
                            hub.Supplied = true;
                        }
                        else
                            hub.Supplied = false;
                    }
                    else if (hub.Circuit.Content >= hub.PulledPerTick) //There's enough resources..
                    {
                        hub.Circuit.Content -= hub.PulledPerTick; //Pull
                        hub.Supplied = true;
                    }
                    else
                        hub.Supplied = false;
                }
            }
            foreach (var hub in CompoundHubs)
            {
                for (int i = 0; i < hub.Resources.Length; i++)
                {
                    var limitedAmount = hub.LimitedAmount[i];
                    var yield = hub.PushedPerTick[i];
                    if (limitedAmount && hub.Empty[i]) //It's limited and out of stuff..
                        yield = 0; //Not contributing cuz it's empty!
                    bool hasTicksPerPush = false;
                    if ((hasTicksPerPush = hub.TicksPerPush[i] > 0) && tick - hub.LastTickPushed[i] < hub.TicksPerPush[i]) //If it has a TicksPerPush and it's not time to push..
                        yield = 0; //Skip, not yet time!
                    if (hub.Circuit.Push(hub.Resources[i], yield) && limitedAmount) //If it's limited..
                    {
                        hub.Remaining[i] -= yield; //Reduce remaining resources..
                        if (hub.Remaining[i] <= 0) //If we're out entirely..
                        {
                            hub.Empty[i] = true; //Mark it empty so we can skip it in the future.
                            hub.Remaining[i] = 0; //Make sure it's not <0.
                        }
                    }
                    if (hasTicksPerPush && yield > 0)
                        hub.LastTickPushed[i] = tick; //We have pushed, so record that.
                    var ticksPerPull = hub.TicksPerPull[i];
                    var cost = hub.PulledPerTick[i];
                    if (ticksPerPull > 0) //If it has a TicksPerPull..
                    {
                        if (tick - hub.LastTickPulled[i] >= ticksPerPull) //If it's time to pull..
                        {
                            if (hub.Circuit.Contents[i] >= cost) //There's enough resources..
                            {
                                hub.Circuit.Contents[i] -= cost; //Pull
                                hub.LastTickPulled[i] = tick; //We have pulled, so record that.
                                hub.Supplied[i] = true;
                            }
                            else
                                hub.Supplied[i] = false;
                        }
                        else if (hub.Circuit.Contents[i] >= cost) //There's enough resources..
                        {
                            hub.Circuit.Contents[i] -= cost; //Pull
                            hub.Supplied[i] = true;
                        }
                        else
                            hub.Supplied[i] = false;
                    }
                }
            }
            foreach (var source in Sources)
            {
                if (source.LimitedAmount && source.Empty) //It's limited and out of stuff..
                    continue; //Skip!
                bool hasTicksPerPush = false;
                if ((hasTicksPerPush = source.TicksPerPush > 0) && tick - source.LastTickPushed < source.TicksPerPush) //If it has a TicksPerPush and it's not time to push..
                    continue; //Skip, not yet time!
                if (source.Circuit.Push(source.PushedPerTick) && source.LimitedAmount) //If it pushes and it's limited..
                {
                    source.Remaining -= source.PushedPerTick; //Reduce remaining resources..
                    if (source.Remaining <= 0) //If we're out entirely..
                    {
                        source.Empty = true; //Mark it empty so we can skip it in the future.
                        source.Remaining = 0; //Make sure it's not <0.
                    }
                }
                if (hasTicksPerPush && source.PushedPerTick > 0)
                    source.LastTickPushed = tick; //We have pushed, so record that.
            }
            foreach (var source in CompoundSources)
            {
                for (int i = 0; i < source.Resources.Length; i++)
                {
                    var limitedAmount = source.LimitedAmount[i];
                    var yield = source.PushedPerTick[i];
                    if (limitedAmount && source.Empty[i]) //It's limited and out of stuff..
                        yield = 0; //Skip!
                    bool hasTicksPerPush = false;
                    if ((hasTicksPerPush = source.TicksPerPush[i] > 0) && tick - source.LastTickPushed[i] < source.TicksPerPush[i])//If it has a TicksPerPush and it's not time to push..
                        yield = 0; //Skip, not yet time!
                    if (source.Circuit.Push(source.Resources[i], yield) && limitedAmount) //If it's limited..
                    {
                        source.Remaining[i] -= yield; //Reduce remaining resources..
                        if (source.Remaining[i] <= 0) //If we're out entirely..
                        {
                            source.Empty[i] = true; //Mark it empty so we can skip it in the future.
                            source.Remaining[i] = 0; //Make sure it's not <0.
                        }
                    }
                    if (hasTicksPerPush && yield > 0)
                        source.LastTickPushed[i] = tick;
                }
            }
            foreach (var sink in Sinks)
            {
                bool hasTicksPerPull = false;
                if ((hasTicksPerPull = sink.TicksPerPull > 0) && tick - sink.LastTickPulled >= sink.TicksPerPull) //If it has a TicksPerPull and it's not time to pull..
                    continue; //Skip, not yet time!
                if (sink.Circuit.Content >= sink.PulledPerTick) //There's enough resources..
                {
                    sink.Circuit.Content -= sink.PulledPerTick; //Pull
                    sink.Supplied = true;
                    if (hasTicksPerPull)
                        sink.LastTickPulled = tick; //We have pulled, so record that.
                }
                else
                {
                    sink.Circuit._raiseInsufficientContent(sink.PulledPerTick - sink.Circuit.Content);
                    sink.Supplied = false;
                }
            }
            foreach (var sink in CompoundSinks)
            {
                for (int i = 0; i < sink.Resources.Length; i++)
                {
                    var ticksPerPull = sink.TicksPerPull[i];
                    var cost = sink.PulledPerTick[i];
                    bool hasTicksPerPull = false;
                    if ((hasTicksPerPull = ticksPerPull > 0) && tick - sink.LastTickPulled[i] >= ticksPerPull) //If it has a TicksPerPull and it's not time to pull..
                        continue; //Skip, not yet time!
                    if (sink.Circuit.Contents[i] >= cost) //There's enough resources..
                    {
                        sink.Circuit.Contents[i] -= cost; //Pull
                        sink.LastTickPulled[i] = tick; //We have pulled, so record that.
                        sink.Supplied[i] = true;
                        if (hasTicksPerPull)
                            sink.LastTickPulled[i] = tick; //We have pulled, so record that.
                    }
                    else
                    {
                        sink.Circuit._raiseInsufficientContent(sink.Resources[i], sink.PulledPerTick[i] - sink.Circuit.Contents[i]);
                        sink.Supplied[i] = false;
                    }
                }
            }
        }

        public override void ExposeData()
        {
            Scribe_Collections.Look(ref Circuits, "Circuits", LookMode.Deep);
            Scribe_Collections.Look(ref Sources, "Sources", LookMode.Reference);
            Scribe_Collections.Look(ref Sinks, "Sinks", LookMode.Reference);
            Scribe_Collections.Look(ref Hubs, "Hubs", LookMode.Reference);
            Scribe_Collections.Look(ref CompoundCircuits, "CompoundCircuits", LookMode.Deep);
            Scribe_Collections.Look(ref CompoundSources, "CompoundSources", LookMode.Reference);
            Scribe_Collections.Look(ref CompoundSinks, "CompoundSinks", LookMode.Reference);
            Scribe_Collections.Look(ref CompoundHubs, "CompoundHubs", LookMode.Reference);
        }
    }
}