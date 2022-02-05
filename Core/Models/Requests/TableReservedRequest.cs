using System.Collections.Generic;

namespace Pandora.Core.Models.Requests
{
    public class TableReservedRequest
    {
        public int TableId { get; set; }
        public IEnumerable<OrderDetail> OrderDetail { get; set; }
    }
    public class OrderDetail
    {
        public int DishId { get; set; }
        public int Quantity { get; set; }
    }

}