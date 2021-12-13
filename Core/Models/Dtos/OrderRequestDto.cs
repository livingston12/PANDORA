using System;
using System.Diagnostics.CodeAnalysis;

namespace Pandora.Core.Models.Dtos
{
    [ExcludeFromCodeCoverage]
    public sealed class OrderRequestDto
    {
        public int? DishId { get; set; }
        public DateTime PlacementDate { get; set; }
        public string Note { get; set; }
    }
}