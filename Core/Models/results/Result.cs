using System.Collections.Generic;

namespace Pandora.Core.Models.Results
{
public class ResultError
    {
        public ResultError(IDictionary<string, IEnumerable<string>> errors)
        {
            Errors = errors;
        }
        public IDictionary<string, IEnumerable<string>> Errors { get; private set; }
        public string StatusCode { get; set; }
    }
}
