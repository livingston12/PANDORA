using System;

namespace Pandora.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class StatusSpecificCheckAttribute : StatusCheckAttribute
    {
        
        public string Title { get; set; }
        public object ValidValue { get; set; }

        public StatusSpecificCheckAttribute(string message = "", string team = "")
            : base(message, team)
        {
        }
    }
}