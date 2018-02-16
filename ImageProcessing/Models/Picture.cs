    using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessing.Models
{
    public class FocusObject
    {
        public string Name { get; set; }
        public Place Place { get; set; }
    }

    public class Place
    {
        public PartObject[,] Matrix { get; set; }
        public int Cols { get; set; }
        public int Rows { get; set; }
    }

    public class PartObject
    {
        public int Score { get; set; }
    }
}
