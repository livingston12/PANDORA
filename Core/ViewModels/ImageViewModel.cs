using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pandora.Core.ViewModels
{
    public class ImageViewModel
    {
        public bool uploaded { get; set; }
        public string message { get; set; }
        public string type { get; set; }
    }
}