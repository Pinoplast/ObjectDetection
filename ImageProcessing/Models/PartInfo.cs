using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessing.Models
{
    public class PartInfo
    {
        public System.Drawing.Image Image { get; set; }
        public int XNumber { get; set; }
        public int YNumber { get; set; }
        public int AmountOfTrees { get; set; }
        public List<LabelObject> Objects { get; set; }
    }
}
