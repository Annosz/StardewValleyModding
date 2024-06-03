using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Linq;

namespace HighlightedJars
{
    public class HighlightedJars : Mod
    {
        private ModConfig Config;

        private const int exclamationEmote = 16;

        public override void Entry(IModHelper helper)
        {
            Config = Helper.ReadConfig<ModConfig>();

            helper.Events.GameLoop.SaveLoaded += (s, e) => GameLoop_SaveLoaded(s, e, helper);
            helper.Events.GameLoop.ReturnedToTitle += (s, e) => GameLoop_ReturnedToTitle(s, e, helper);
        }

        private void GameLoop_SaveLoaded(object sender, SaveLoadedEventArgs e, IModHelper helper)
        {
            helper.Events.Display.RenderedWorld += Display_RenderedWorld;
        }

        private void GameLoop_ReturnedToTitle(object sender, ReturnedToTitleEventArgs e, IModHelper helper)
        {
            helper.Events.Display.RenderedWorld -= Display_RenderedWorld;
        }

        private void Display_RenderedWorld(object sender, RenderedWorldEventArgs e)
        {
            if (Game1.currentLocation == null)
                return;

            var highlightableObjects = Game1.currentLocation.objects.Values.Where(o =>
                ((Config.HighlightJars && o.ParentSheetIndex == 15)
                    || (Config.HighlightKegs && o.ParentSheetIndex == 12)
                    || (Config.HighlightCasks && o.ParentSheetIndex == 163))
                && o.MinutesUntilReady <= 0 && !o.readyForHarvest.Value
                ).ToList();

            foreach (var highlightableObject in highlightableObjects)
            {
                Vector2 local = Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(highlightableObject.TileLocation.X * 64), (float)(Math.Abs(highlightableObject.TileLocation.Y * 64 - 64))));
                Rectangle destinationRectangle = new Rectangle((int)local.X, (int)local.Y - 32, 64, 64);

                switch (Config.HighlightType)
                {
                    case "Highlight":
                        e.SpriteBatch.Draw(Game1.bigCraftableSpriteSheet, new Vector2(local.X + 32, local.Y + 32), new Rectangle?(StardewValley.Object.getSourceRectForBigCraftable(highlightableObject.ParentSheetIndex)), Color.Red * 0.50f, 0.0f, new Vector2(8f, 8f), 4f, SpriteEffects.None, (float)((highlightableObject.TileLocation.Y - 1) * 64) / 10000f);
                        break;
                    case "Bubble":
                    default:
                        e.SpriteBatch.Draw(Game1.emoteSpriteSheet, destinationRectangle, new Rectangle(exclamationEmote * 16 % Game1.emoteSpriteSheet.Width, exclamationEmote * 16 / Game1.emoteSpriteSheet.Width * 16, 16, 16), Color.White * 0.95f, 0.0f, Vector2.Zero, SpriteEffects.None, (float)((highlightableObject.TileLocation.Y - 1) * 64) / 10000f);
                        break;
                }
            }
        }
    }
}