namespace Pandora.Core.ViewModels
{
    public enum InventoryType
    {
        increase,
        decrease,
        current
    }

    public class IngredientViewModel
    {
        public int IngredientId { get; set; }
        public string Ingredient { get; set; }
        public decimal? Price { get; set; }
        public int? Quantity { get; set; }
        public int? RestaurantId { get; set; }
        public bool? IsGarrison { get; set; } = false;
        public bool? isMisePlace { get; set; } = false;
    }

    public class IngredientUpdateInventory
    {
        public int IngredientId { get; set; }
        public int? Quantity { get; set; } = 0;
        public InventoryType Type { get; set; } = InventoryType.increase;
    }
}