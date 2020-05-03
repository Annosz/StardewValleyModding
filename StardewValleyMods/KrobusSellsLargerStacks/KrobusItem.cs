using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KrobusSellsLargerStacks
{
    public class KrobusItem
    {
        public int ItemId { get; set; }
        public int ItemQuantity { get; set; }

        public KrobusItem(int itemId, int itemQuantity)
        {
            ItemId = itemId;
            ItemQuantity = itemQuantity;
        }
    }
}
