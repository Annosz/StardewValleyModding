using Harmony;
using StardewModdingAPI;
using System;

namespace SleeplessFisherman
{
    public class SleeplessFisherman : Mod
    {
        public override void Entry(IModHelper helper)
        {
            var harmony = HarmonyInstance.Create(this.ModManifest.UniqueID);

            harmony.Patch(
               original: AccessTools.Method(typeof(StardewValley.Character), nameof(StardewValley.Character.doEmote), new Type[] { typeof(int), typeof(bool), typeof(bool) }),
               prefix: new HarmonyMethod(typeof(CharacterPatches), nameof(CharacterPatches.doEmote_Prefix))
            );
        }
    }
}