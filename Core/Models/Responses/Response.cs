using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Pandora.Core.Models.Responses
{
    public class Response<T>
    {
        public IEnumerable<T> List { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalPage => PageSize == 0 ? 1 : (int)Math.Ceiling((decimal)Total / PageSize);

        [JsonPropertyName("total")]
        public int Total { get; set; }
    }
}