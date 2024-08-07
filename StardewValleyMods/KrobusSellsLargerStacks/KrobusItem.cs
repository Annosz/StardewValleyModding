namespace KrobusSellsLargerStacks
{
    public class KrobusItem
    {
        public int ItemQuantity { get; set; }
        public string QualifiedItemId { get; set; }
        public string Type { get; set; }

        public KrobusItem(int itemQuantity, string qualifiedItemId = "", string type = "")
        {
            ItemQuantity = itemQuantity;
            QualifiedItemId = qualifiedItemId;
            Type = type;
        }
    }
}
