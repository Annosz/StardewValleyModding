using StardewModdingAPI;
using StardewValley;
using System;
using xTile.Dimensions;

namespace CheaperBeachBridgeRepair
{
    class FarmerPatches
    {
        private static IMonitor Monitor;

        public static void Initialize(IMonitor monitor)
        {
            Monitor = monitor;
        }

        public static bool doSleepEmote_Prefix(Farmer who)
        {
            try
            {
                if (who.FishingLevel == 10)
                    return false;
                return true;
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed in {nameof(doSleepEmote_Prefix)}:\n{ex}", LogLevel.Error);
                return true;
            }
        }
    }
}
