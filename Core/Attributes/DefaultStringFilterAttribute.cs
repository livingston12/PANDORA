using System;

namespace Pandora.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    internal class DefaultStringFilterAttribute : DefaultFilterAttribute
    {
        public string Search { get; set; }

        /// <summary>
        /// </summary>
        /// <param name="search">Default info to search</param>
        internal DefaultStringFilterAttribute(string search)
        {
            Search = search;
        }
    }
}