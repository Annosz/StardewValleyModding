using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using Newtonsoft.Json.Linq;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
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
        }

        private void GameLoop_SaveLoaded(object sender, SaveLoadedEventArgs e, IModHelper helper)
        {
            helper.Events.Display.RenderedWorld += Display_RenderedWorld; ;
        }

        private void Display_RenderedWorld(object sender, RenderedWorldEventArgs e)
        {
            if (Game1.currentLocation == null)
                return;

            var highlightableObjects = Game1.currentLocation.objects.Values.Where(o =>
                ((Config.HighlightJars && (o as StardewValley.Object).parentSheetIndex == 15)
                    || (Config.HighlightKegs && (o as StardewValley.Object).parentSheetIndex == 12))
                && o.minutesUntilReady <= 0 && !o.readyForHarvest
                ).ToList();

            foreach (var highlightableObject in highlightableObjects)
            {
                Vector2 local = Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(highlightableObject.TileLocation.X * 64), (float)(Math.Abs(highlightableObject.TileLocation.Y * 64 - 64))));
                Rectangle destinationRectangle = new Rectangle((int)local.X + (highlightableObject.shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0), (int)local.Y + (highlightableObject.shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0) - 32, 64, 64);
                e.SpriteBatch.Draw(Game1.emoteSpriteSheet, destinationRectangle, new Rectangle(exclamationEmote * 16 % Game1.emoteSpriteSheet.Width, exclamationEmote * 16 / Game1.emoteSpriteSheet.Width * 16, 16, 16), Color.White * 0.95f, 0.0f, Vector2.Zero, SpriteEffects.None, (float)((highlightableObject.TileLocation.Y - 1) * 64) / 10000f);
            }
        }
    }
}