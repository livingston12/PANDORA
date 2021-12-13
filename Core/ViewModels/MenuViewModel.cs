using System;

namespace Pandora.Core.ViewModels
{
    public class MenuViewModel
    {
        public int MenuId { get; set; }
        public string Menu { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public int? RestaurantId { get; set; }


    }
}