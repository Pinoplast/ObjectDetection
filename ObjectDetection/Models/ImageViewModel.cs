using System;
using System.Collections.Generic;
using System.Linq;

namespace ObjectDetection.Models
{
    public class ImageViewModel
    {
        public string Name { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public long Size { get; set; }
    }
}