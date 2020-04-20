using Harmony;
using StardewModdingAPI;

namespace CheaperBeachBridgeRepair
{
    public class SleeplessFisherman : Mod
    {
        public override void Entry(IModHelper helper)
        {
            var harmony = HarmonyInstance.Create(this.ModManifest.UniqueID);

            harmony.Patch(
               original: AccessTools.Method(typeof(StardewValley.Farmer), nameof(StardewValley.Farmer.doSleepEmote)),
               prefix: new HarmonyMethod(typeof(FarmerPatches), nameof(FarmerPatches.doSleepEmote_Prefix))
            );
        }
    }
}