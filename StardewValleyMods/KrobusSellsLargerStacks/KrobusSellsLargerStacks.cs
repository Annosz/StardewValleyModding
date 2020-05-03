using Newtonsoft.Json.Linq;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KrobusSellsLargerStacks
{
    public class KrobusSellsLargerStacks : Mod
    {
        private ModConfig Config;

        public override void Entry(IModHelper helper)
        {
            Config = Helper.ReadConfig<ModConfig>();

            helper.Events.Display.MenuChanged += OnMenuChanged;
        }

        private void OnMenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (e.NewMenu is ShopMenu menu && Game1.currentLocation.Name == "Sewer")
            {
                foreach (var krobusItem in GetKrobusItemsFromConfig(Config))
                {
                    var sellable = menu.itemPriceAndStock.FirstOrDefault(kv => (kv.Key as StardewValley.Object).ParentSheetIndex == krobusItem.ItemId);

                    if (!sellable.Equals(new KeyValuePair<ISalable, int[]>()))
                    {
                        var sellableItem = sellable.Key;
                        var priceAndStock = sellable.Value;

                        sellableItem.Stack = krobusItem.ItemQuantity;
                        priceAndStock[priceAndStock.Length - 1] = krobusItem.ItemQuantity;
                        menu.itemPriceAndStock[sellableItem] = priceAndStock;
                    }
                }
            }
        }

        public List<KrobusItem> GetKrobusItemsFromConfig(ModConfig modConfig)
        {
            List<KrobusItem> krobusItems = new List<KrobusItem>();

            foreach (var property in modConfig.GetType().GetProperties())
            {
                switch (property.Name)
                {
                    case "DarkEssenceQuantity":
                        krobusItems.Add(new KrobusItem(769, int.Parse(property.GetValue(modConfig, null)?.ToString())));
                        break;
                    case "MixedSeedsQuantity":
                        krobusItems.Add(new KrobusItem(770, int.Parse(property.GetValue(modConfig, null)?.ToString())));
                        break;
                    default:
                        break;
                }
            }

            return krobusItems;
        }
    }
}