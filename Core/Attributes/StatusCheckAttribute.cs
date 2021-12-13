using System;

namespace Pandora.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class StatusCheckAttribute : Attribute
    {
        public string Tip { get; set; }
        public string Team { get; set; }

        public StatusCheckAttribute(string message = "", string team = "")
        {
            Tip = message;
            Team = team;
        }
    }
}