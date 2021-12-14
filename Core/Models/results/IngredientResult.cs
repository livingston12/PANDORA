using System.Collections.Generic;
using Pandora.Core.Models.Dtos;
using Pandora.Core.Models.Entities;

namespace Pandora.Core.Models.Results
{
public sealed class IngredientResult : ResultError
{
    public IngredientResult(IDictionary<string, IEnumerable<string>> errors) : base(errors)
    {
    }

    public IngredientEntity Ingredient { get; set; }
}
}