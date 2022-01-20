using System.Collections.Generic;
using Pandora.Core.Models.Dtos;

namespace Pandora.Core.Models.Results
{
    public sealed class UpdateResult : ResultError
    {
        public UpdateResult(IDictionary<string, IEnumerable<string>> errors) : base(errors)
        {
        }
        public bool IsUpdate
        {
            get { return Errors.Count == 0; }
        }
    }
}