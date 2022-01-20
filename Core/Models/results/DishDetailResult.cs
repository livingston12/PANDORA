using System.Collections.Generic;
using Pandora.Core.Models.Entities;

namespace Pandora.Core.Models.Results
{
    public sealed class DishDetailResult : ResultError
    {
        public DishDetailResult(IDictionary<string, IEnumerable<string>> errors) : base(errors)
        {
        }

        public DishesDetailEntity Detail { get; set; }
    }
}