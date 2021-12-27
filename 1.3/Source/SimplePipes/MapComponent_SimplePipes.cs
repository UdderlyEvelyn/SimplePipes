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
                                //if other pipe has no connection to circuit now than split circuits
                            }
                        }
                }
            if (pipe.Circuit.Pipes.Count == 1) //If this is the last pipe in the circuit..
                Circuits.Remove(pipe.Circuit); //Deregister it.
            pipe.Circuit.Pipes.Remove(pipe); //Remove the pipe from it.
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
