using System;
using System.Collections.Generic;

namespace Pandora.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class StatusExtraInfoAttribute : Attribute
    {
        public IEnumerable<string> Values { get; set; }
        public StatusExtraInfoAttribute(params string[] values)
        {
            Values = values;
        }
    }
}