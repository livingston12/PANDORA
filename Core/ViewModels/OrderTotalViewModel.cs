using System.Collections.Generic;

namespace Pandora.Core.ViewModels
{
    public class OrderTotalViewModel
    {
        public List<decimal?> Totals { get; set; } = new List<decimal?>();
        public List<string> Labels { get; set; } = new List<string>();
    }
}