using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley.Menus;
using System.Collections.Generic;
using System.Linq;

namespace KrobusSellsLargerStacks
{
    public class KrobusSellsLargerStacks : Mod
    {
        private ModConfig config;
        private bool wasUpdatedToday;

        public override void Entry(IModHelper helper)
        {
            config = Helper.ReadConfig<ModConfig>();

            helper.Events.Display.MenuChanged += OnMenuChanged;
            helper.Events.GameLoop.DayStarted += OnDayStarted;
        }

        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            wasUpdatedToday = false;
        }

        private void OnMenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (!wasUpdatedToday && e.NewMenu is ShopMenu menu && menu.ShopId == "ShadowShop")
            {
                var krobusItems = GetKrobusItemsFromConfig(config);
                var matches = krobusItems
                    .Join(
                        menu.itemPriceAndStock,
                        i => i.QualifiedItemId,
                        i => i.Key.QualifiedItemId,
                        (k, i) => (key: i.Key, krobusItem: k, shopItem: i.Value))
                    .ToList();
                foreach (var (key, krobusItem, shopItem) in matches)
                {
                    var changedItem = shopItem; // Struct, so we have to do this.
                    changedItem.Stock = krobusItem.ItemQuantity;
                    menu.itemPriceAndStock[key] = changedItem;
                }
                wasUpdatedToday = true;
            }
        }

        public static IEnumerable<KrobusItem> GetKrobusItemsFromConfig(ModConfig modConfig)
        {
            return [
                new(modConfig.DarkEssenceQuantity, "(O)769"),
                new(modConfig.SolarEssenceQuantity, "(O)768"),
                new(modConfig.SlimeQuantity, "(O)766"),
                new(modConfig.OmniGeodeQuantity, "(O)749"),
                new(modConfig.MixedSeedsQuantity, "(O)770"),
                new(modConfig.IridiumSprinklerQuantity, "(O)645"),
                new(modConfig.BatWingQuantity, "(O)767"),
                new(modConfig.MagnetQuantity, "(O)703"),
                new(modConfig.FishQuantity, type: "Fish"),
                new(modConfig.FoodQuantity, type: "Cooking"),
            ];
        }
    }
}