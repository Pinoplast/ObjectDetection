using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessing.Models
{
    public class GraphicObject
    {
        public string Name { get; set; }
        public List<Rectangle> Rectangles { get; set; }
    }
}
