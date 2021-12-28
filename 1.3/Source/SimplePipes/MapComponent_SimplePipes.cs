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
        public List<FluidSink> FluidSinks = new List<FluidSink>();
        public List<FluidSource> FluidSources = new List<FluidSource>();

        public MapComponent_SimplePipes(Map map) : base(map)
        {

        }

        public void RegisterPipe(Pipe pipe)
        {
            ActOnAdjacentPipes(pipe, otherPipe =>
            {
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
            });
            //The above would have to break out of the loop.
            int pipeX = pipe.Position.x;
            int pipeY = pipe.Position.y;
            for (int x = pipeX - 1; x <= pipeX + 1; x++)
                for (int y = pipeY - 1; y <= pipeY + 1; y++)
                {
                    if (x == pipeX && y == pipeY) //If it's the pipe's position itself.
                        continue; //Skip.
                    var things = map.thingGrid.ThingsAt(new IntVec3(x, y, 0));
                    foreach (var thing in things)
                        if (thing is Pipe)
                        {
                            var otherPipe = (Pipe)thing;
                            if (otherPipe.Fluid == pipe.Fluid) //If it's the same fluid type..
                            {
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
                }
            if (pipe.Circuit == null) //If we didn't find a circuit after all that..
            {
                pipe.Circuit = new Circuit(new[] { pipe }) { Capacity = pipe.Capacity, Fluid = pipe.Fluid }; //New circuit.
                Circuits.Add(pipe.Circuit);
            }
        }

        public void DeregisterPipe(Pipe pipe)
        {
            ActOnAdjacentPipes(pipe, otherPipe =>
            {
                var adjacentPipes = GetAdjacentPipes(otherPipe);
                var foundCircuitPipe = false;
                Circuit circuit = null;
                for (int i = 0; i < adjacentPipes.Length; i++)
                {
                    var pipeInQuestion = adjacentPipes[i];
                    if (pipeInQuestion != pipe) //Skip the pipe we're yeeting..
                    {
                        if (pipeInQuestion.Circuit == otherPipe.Circuit) //If the pipe has the same circuit as otherPipe..
                        {
                            foundCircuitPipe = true; //We found one!
                            break; //We're good, stop here.
                        }
                        else if (pipeInQuestion.Circuit != null && circuit == null) //Else if the pipe has a different circuit and we don't already have an alternative circuit..
                            circuit = pipeInQuestion.Circuit; //Store it so we can swap to that circuit if there's no more keeping us in this one.
                    }
                        
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
            });
        }

        public Pipe[] GetAdjacentPipes(Pipe pipe, bool sameFluidOnly = true)
        {
            var pipes = new Pipe[8];
            int pipeX = pipe.Position.x;
            int pipeY = pipe.Position.y;
            for (int x = pipeX - 1; x <= pipeX + 1; x++)
                for (int y = pipeY - 1; y <= pipeY + 1; y++)
                {
                    if (x == pipeX && y == pipeY) //If it's the pipe's position itself.
                        continue; //Skip.
                    var arrayX = x - pipeX + 1; //Adjust to array position..
                    var arrayY = y - pipeY + 1; //Adjust to array position..
                    var things = map.thingGrid.ThingsAt(new IntVec3(x, y, 0));
                    foreach (var thing in things)
                        if (thing is Pipe)
                        {
                            var otherPipe = (Pipe)thing;
                            if (sameFluidOnly && pipe.Fluid == otherPipe.Fluid)
                                pipes[3 * y + x] = otherPipe; //3 per row * Y coordinate + X coordinate offset
                        }
                }
            return pipes;
        }

        public void ActOnAdjacentPipes(Pipe pipe, Action<Pipe> action, bool sameFluidOnly = true)
        {
            var pipes = new Pipe[8];
            int pipeX = pipe.Position.x;
            int pipeY = pipe.Position.y;
            for (int x = pipeX - 1; x <= pipeX + 1; x++)
                for (int y = pipeY - 1; y <= pipeY + 1; y++)
                {
                    if (x == pipeX && y == pipeY) //If it's the pipe's position itself.
                        continue; //Skip.
                    var things = map.thingGrid.ThingsAt(new IntVec3(x, y, 0));
                    foreach (var thing in things)
                        if (thing is Pipe)
                        {
                            var otherPipe = (Pipe)thing;
                            if (sameFluidOnly && pipe.Fluid == otherPipe.Fluid)
                                action(otherPipe);
                        }
                }
        }

        public void RegisterUser(FluidUser user)
        {
            if (user is FluidSource)
                FluidSources.Add((FluidSource)user);
            else if (user is FluidSink)
                FluidSinks.Add((FluidSink)user);
            else
                Log.Error("[Simple Pipes] Attempted to register FluidUser that was neither a sink nor a source.");
        }

        public void DeregisterUser(FluidUser user)
        {
            if (user is FluidSource)
                FluidSources.Remove((FluidSource)user);
            else if (user is FluidSink)
                FluidSinks.Remove((FluidSink)user);
            else
                Log.Error("[Simple Pipes] Attempted to deregister FluidUser that was neither a sink nor a source.");
        }

        public void RecalculateCircuits()
        {
            var pipes = map.listerBuildings.AllBuildingsColonistOfClass<Pipe>();
            foreach (var pipe in pipes)
            {
                //Stuff
            }
        }

        public override void MapComponentTick()
        {
            foreach (var source in FluidSources)
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
            foreach (var sink in FluidSinks)
            {
                if (sink.TicksPerPull > 0) //If it has a TicksPerPull..
                {
                    var tick = Find.TickManager.TicksGame;
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
        }
    }
}
