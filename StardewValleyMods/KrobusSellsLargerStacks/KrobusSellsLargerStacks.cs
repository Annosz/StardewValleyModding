using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using System.Linq;

namespace KrobusSellsLargerStacks
{
    public class KrobusSellsLargerStacks : Mod
    {
        public override void Entry(IModHelper helper)
        {
            helper.Events.Display.MenuChanged += this.OnMenuChanged;
        }

        private void OnMenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (e.NewMenu is ShopMenu menu && Game1.currentLocation.Name == "Sewer")
            {
                var sellable = menu.itemPriceAndStock.FirstOrDefault(kv => kv.Key.Name == "Mixed Seeds");
                var sellableItem = sellable.Key;
                var priceAndStock = sellable.Value;

                sellableItem.Stack = 42;
                priceAndStock[priceAndStock.Length - 1] = 42;
                menu.itemPriceAndStock[sellableItem] = priceAndStock;
            }

        }
    }
}