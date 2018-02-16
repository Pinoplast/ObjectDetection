using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessing.Models
{
    public class Image
    {
        public string content { get; set; }
    }

    public class Feature
    {
        public string type { get; set; }
        public int maxResults { get; set; }
    }

    public class Request
    {
        public Image image { get; set; }
        public IList<Feature> features { get; set; }
    }
    public class ListRequests
    {
        public List<Request> requests { get; set; }
    }
}
