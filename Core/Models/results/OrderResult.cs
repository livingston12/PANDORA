using System.Collections.Generic;
using Pandora.Core.Models.Dtos;

namespace Pandora.Core.Models.Results
{
    public sealed class OrderResult : ResultError
    {
        public OrderResult(IDictionary<string, IEnumerable<string>> errors) : base(errors)
        {
        }
        public string OrderId { get; set; }
        public int? RestaurantId { get; set; }

        public IEnumerable<OrderRequestDto> Orders { get; set; }
    }
}