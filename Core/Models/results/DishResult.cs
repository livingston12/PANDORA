using System.Collections.Generic;
using Pandora.Core.Models.Entities;

namespace Pandora.Core.Models.Results
{
    public sealed class DishResult : ResultError
    {
        public DishResult(IDictionary<string, IEnumerable<string>> errors) : base(errors)
        {
        }

        public DishesEntity Dish { get; set; }
    }
}