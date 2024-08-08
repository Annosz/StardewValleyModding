using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using SObject = StardewValley.Object;

namespace HighlightedJars
{
    public class HighlightedJars : Mod
    {
        private const int ExclamationEmoteIndex = 16;

        private static readonly Lazy<Rectangle> ExclamationEmoteSourceRect =
            new(() =>
                new Rectangle(
                    ExclamationEmoteIndex * 16 % Game1.emoteSpriteSheet.Width,
                    ExclamationEmoteIndex * 16 / Game1.emoteSpriteSheet.Width * 16,
                    16,
                    16));

        private readonly PerScreen<HighlightData> dataPerScreen = new();

        private ModConfig config;

        public override void Entry(IModHelper helper)
        {
            config = Helper.ReadConfig<ModConfig>();

            helper.Events.GameLoop.DayStarted += GameLoop_DayStarted;
            helper.Events.GameLoop.SaveLoaded += GameLoop_SaveLoaded;
            helper.Events.GameLoop.ReturnedToTitle += GameLoop_ReturnedToTitle;
            helper.Events.Player.Warped += Player_Warped;
            helper.Events.World.ObjectListChanged += World_ObjectListChanged;
        }

        private void Display_RenderedWorld(object sender, RenderedWorldEventArgs e)
        {
            if (!dataPerScreen.IsActiveForScreen())
            {
                return;
            }

            var data = dataPerScreen.Value;
            if (data.Location != Game1.currentLocation)
            {
                // This should never happen if the events are set up properly. But if it does, don't draw nonsense.
                return;
            }
            foreach (var obj in dataPerScreen.Value.HighlightableObjects)
            {
                if (ShouldHighlight(obj))
                {
                    DrawHighlight(e.SpriteBatch, obj);
                }
            }
        }

        private void GameLoop_DayStarted(object sender, DayStartedEventArgs e)
        {
            ReloadForLocation(Game1.currentLocation);
        }

        private void GameLoop_ReturnedToTitle(object sender, ReturnedToTitleEventArgs e)
        {
            Helper.Events.Display.RenderedWorld -= Display_RenderedWorld;
            dataPerScreen.ResetAllScreens();
        }

        private void GameLoop_SaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            Helper.Events.Display.RenderedWorld += Display_RenderedWorld;
        }

        private void Player_Warped(object sender, WarpedEventArgs e)
        {
            ReloadForLocation(e.NewLocation);
        }

        private void World_ObjectListChanged(object sender, ObjectListChangedEventArgs e)
        {
            foreach (var (_, data) in dataPerScreen.GetActiveValues())
            {
                if (e.Location != data.Location)
                {
                    continue;
                }
                foreach (var removed in e.Removed)
                {
                    data.HighlightableObjects.Remove(removed.Value);
                }
                foreach (var added in e.Added)
                {
                    data.HighlightableObjects.Add(added.Value);
                }
            }
        }

        private static bool CanHighlight(string qualifiedItemId)
        {
            return qualifiedItemId == "(BC)12" || qualifiedItemId == "(BC)15" || qualifiedItemId == "(BC)163";
        }

        private void DrawHighlight(SpriteBatch b, SObject obj)
        {
            var local = Game1.GlobalToLocal(
                Game1.viewport,
                new Vector2((obj.TileLocation.X * 64), (float)(Math.Abs(obj.TileLocation.Y * 64 - 64))));
            switch (config.HighlightType)
            {
                case HighlightType.Highlight:
                    b.Draw(
                        Game1.bigCraftableSpriteSheet,
                        position: new Vector2(local.X + 32, local.Y + 32),
                        sourceRectangle: SObject.getSourceRectForBigCraftable(obj.ParentSheetIndex),
                        color: Color.Red * 0.50f,
                        rotation: 0.0f,
                        origin: new Vector2(8f, 8f),
                        scale: 4f,
                        effects: SpriteEffects.None,
                        layerDepth: ((obj.TileLocation.Y - 1) * 64) / 10000f);
                    break;
                case HighlightType.Bubble:
                default:
                    var destinationRect = new Rectangle((int)local.X, (int)local.Y - 32, 64, 64);
                    b.Draw(
                        Game1.emoteSpriteSheet,
                        destinationRect,
                        sourceRectangle: ExclamationEmoteSourceRect.Value,
                        color: Color.White * 0.95f,
                        rotation: 0.0f,
                        origin: Vector2.Zero,
                        effects: SpriteEffects.None,
                        layerDepth: ((obj.TileLocation.Y - 1) * 64) / 10000f);
                    break;
            }
        }

        private static IEnumerable<SObject> GetHighlightableObjects(GameLocation location)
        {
            return location.objects.Values.Where(obj => CanHighlight(obj.QualifiedItemId));
        }

        private void ReloadForLocation(GameLocation location)
        {
            var highlightableObjects = GetHighlightableObjects(location);
            dataPerScreen.Value = new(location, highlightableObjects);
        }

        private bool ShouldHighlight(SObject obj)
        {
            var isConfiguredForHighlight = obj.QualifiedItemId switch
            {
                "(BC)12" => config.HighlightKegs,
                "(BC)15" => config.HighlightJars,
                "(BC)163" => config.HighlightCasks,
                _ => false,
            };
            return isConfiguredForHighlight && obj.MinutesUntilReady <= 0 && !obj.readyForHarvest.Value;
        }
    }

    class HighlightData(GameLocation location, IEnumerable<SObject> initialHighlightableObjects)
    {
        public GameLocation Location { get; } = location;
        public HashSet<SObject> HighlightableObjects { get; } = [.. initialHighlightableObjects];
    }
}