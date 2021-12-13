using Pandora.Core.Models.Enums;
using System;

namespace Pandora.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    internal class DefaultSortPropertyAttribute : Attribute
    {
        public string Name { get; set; }
        public SortDirection SortDirection { get; set; }

        internal DefaultSortPropertyAttribute(string propertyName
                , SortDirection sortDirection = SortDirection.DESC)
        {
            Name = propertyName;
            SortDirection = sortDirection;
        }
    }
}
