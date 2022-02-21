using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;

namespace UdderlyEvelyn.SimplePipes
{
    //This stores MapComponents on a per-map basis so that things don't have to constantly retrieve them.
    public static class MapComponentCache<T> where T : MapComponent
    {
        public static Dictionary<int, T> compCachePerMap = new Dictionary<int, T>();

        public static T GetFor(Map map)
        {
            T comp; //Set up var.
            if (!compCachePerMap.ContainsKey(map.uniqueID)) //If not cached..
                compCachePerMap.Add(map.uniqueID, comp = map.GetComponent<T>()); //Get and cache.
            else
                comp = compCachePerMap[map.uniqueID]; //Retrieve from cache.
            return comp;
        }

        public static void SetFor(Map map, T comp)
        {
            if (!compCachePerMap.ContainsKey(map.uniqueID)) //If not cached..
                compCachePerMap.Add(map.uniqueID, comp); //Cache.
            else
                compCachePerMap[map.uniqueID] = comp; //Reassign cache.
        }
    }
}
