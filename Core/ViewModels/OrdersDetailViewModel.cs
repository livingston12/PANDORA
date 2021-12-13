namespace Pandora.Core.ViewModels
{
    public class OrdersDetailViewModel
    {
        public int OrderDetailId { get; set; }
        public int Quantity { get; set; }
        public decimal? Price { get; set; }
        public decimal? Total { get; set; }
        public int? DishId { get; set; }
    }
}