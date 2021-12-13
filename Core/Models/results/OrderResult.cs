using System.Collections.Generic;
using Pandora.Core.Models.Dtos;

public sealed class OrderResult
    {
        public OrderResult(IDictionary<string, IEnumerable<string>> errors)
        {
            Errors = errors;
        }

        public string OrderId { get; set; }
        public string StatusCode { get; set; }
        public string Status { get; set; }
        public int? RestaurantId { get; set; }

        public IDictionary<string, IEnumerable<string>> Errors { get; private set; }
        public IEnumerable<OrderRequestDto> Orders {get; set;}
    }