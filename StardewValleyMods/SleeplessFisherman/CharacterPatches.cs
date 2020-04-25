using StardewModdingAPI;
using StardewValley;
using StardewValley.Tools;
using System;

namespace SleeplessFisherman
{
    class CharacterPatches
    {
        private static IMonitor Monitor;

        public static void Initialize(IMonitor monitor)
        {
            Monitor = monitor;
        }

        public static bool doEmote_Prefix(Character __instance, int whichEmote, bool playSound, bool nextEventCommand = true)
        {
            try
            {
                if ((__instance is Farmer) && ((__instance as Farmer).CurrentTool is FishingRod) 
                    && ((__instance as Farmer).CurrentTool as FishingRod).isFishing)
                    return false;
                return true;
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed in {nameof(doEmote_Prefix)}:\n{ex}", LogLevel.Error);
                return true;
            }
        }
    }
}
