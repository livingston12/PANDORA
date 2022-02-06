using System.Collections.Generic;
using Pandora.Core.Models.Entities;

namespace Pandora.Core.Models.Results
{
    public sealed class CategoryResult : ResultError
    {
        public CategoryResult(IDictionary<string, IEnumerable<string>> errors) : base(errors)
        {
        }

        public CategoryEntity Category { get; set; }
    }
}