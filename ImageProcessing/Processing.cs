using ImageProcessing.Models;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessing
{
    public class Processing
    {
        PartInfo _partInfo = new PartInfo();
        public Processing(string fileName)
        {
            System.Drawing.Image image = System.Drawing.Image.FromFile(fileName);

            _partInfo.Image = image;
            _partInfo.AmountOfTrees = 0;
            _partInfo.Objects = new List<LabelObject>();
            _partInfo.XNumber = 1;
            _partInfo.YNumber = 1;


        }

        public List<LabelAnnotation> Request(System.Drawing.Image image)
        {
            
            HttpResponseMessage message = new HttpResponseMessage();
            var client = new RestClient("https://vision.googleapis.com/");

            var restRequest = new RestRequest("v1/images:annotate?key={key}", Method.POST);
            restRequest.AddUrlSegment("key", "AIzaSyDtL-Xheow5RTA0ZhNXF5_cBddSGcOgNVM");

            ListRequests requests = new ListRequests()
            {
                requests = new List<Request>()
                    {
                        new Request()
                        {

                            features = new List<Feature>()
                            {
                                new Feature()
                                {
                                    maxResults = 10,
                                    type = "LABEL_DETECTION"
                                }

                            },
                            image = new Models.Image()
                            {
                                content = ImageToBase64(image)
                            }
                        }
                    }
            };
            
            var json = JsonConvert.SerializeObject(requests);

            restRequest.AddParameter("application/json", json, "utf8", ParameterType.RequestBody);
            IRestResponse restResponse = client.Execute(restRequest);

            var list = JsonConvert.DeserializeObject<ResponsesList>(restResponse.Content);
            
            List<LabelAnnotation> res = new List<LabelAnnotation>();
            foreach (var i in list.responses.Select(x => x.labelAnnotations.Where(y => y.score > 0.80)))
            {
                res.AddRange(i);
            }

            return res;
        }

        public KeyValuePair<System.Drawing.Image, List<LabelAnnotation>> Process()
        {
            List<LabelAnnotation> labels = new List<LabelAnnotation>();
            
            var r = Request(_partInfo.Image);

            _partInfo.Objects = r.Select(x => new LabelObject()
            {
                Name = x.description,
                Score = x.score
            }).ToList();

            foreach (var i in r)
            {
                labels.Add(i);
            }

            //var rects = CheckEqual(ref labels, _partInfo);

            //var img = Drawing.DrawRectangles(_partInfo.Image, rects);

            var image = _partInfo.Image;

            image = Drawing.DrawLabels(image, r.Select(x => new LabelObject()
            {
                Name = x.description,
                Score = x.score
            }).ToList());

            return new KeyValuePair<System.Drawing.Image, List<LabelAnnotation>>(image, r);
        }

        private List<GraphicObject> CheckEqual(ref List<LabelAnnotation> labels, PartInfo image)
        {
            List<PartInfo> parts = DevideImage(image);
            List<GraphicObject> rectangles = new List<GraphicObject>();
            foreach (var p in parts)
            {
                var response = Request(p.Image);
                p.Objects = response.Select(x => new LabelObject()
                {
                    Name = x.description,
                    Score = x.score
                }).ToList();
                foreach (var i in labels)
                {
                    var label = response.FirstOrDefault(x => x.description == i.description);
                    
                    if (label != null 
                        && image.Objects.Where(x => x.Name == i.description).Count() > 0 
                        && _partInfo.Objects.Where(x => x.Name == i.description).Count() > 0 
                        && (_partInfo.Objects.FirstOrDefault(x => x.Name == i.description).Score - label.score) < 0.2 
                        && (label.score) >= image.Objects.FirstOrDefault(x => x.Name == i.description).Score)
                    {
                        var res = CheckEqual(ref labels, p);
                        
                        if (res.Count > 0)
                        {
                            rectangles.AddRange(res);
                        }
                        else
                        {
                            rectangles.Add(new GraphicObject()
                            {
                                Name = i.description,
                                Rectangles = new List<Rectangle>()
                                {
                                    new Rectangle(p.XNumber*p.Image.Width, p.YNumber*p.Image.Height, p.Image.Width, p.Image.Height)
                                }
                            });
                        }
                    }
                }
            }
            return rectangles;
        }

        private List<PartInfo> DevideImage(PartInfo image)
        {
            int width = image.Image.Width / 2;
            int height = image.Image.Height / 2;
            List<PartInfo> parts = new List<PartInfo>();
            parts.Capacity = 4;
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    int index = i * 2 + j;
                    parts.Add(new PartInfo(){
                        Image = new Bitmap(width, height),
                        AmountOfTrees = image.AmountOfTrees+1,
                        XNumber = i+1,
                        YNumber = j+1
                    }
                    );

                    var graphics = Graphics.FromImage(parts[index].Image);
                    graphics.DrawImage(image.Image, new Rectangle(0, 0, width, height), new Rectangle(i * width, j * height, width, height), GraphicsUnit.Pixel);
                    graphics.Dispose();
                }
            }
            return parts;
        }
        
        private string ImageToBase64(System.Drawing.Image image)
        {
            string base64String = "";
            using (MemoryStream m = new MemoryStream())
            {
                try
                {
                    image.Save(m, ImageFormat.Bmp);
                    byte[] imageBytes = m.ToArray();

                    // Convert byte[] to Base64 String
                    base64String = Convert.ToBase64String(imageBytes);
                }
                catch (Exception e)
                {
                    var exception = e;
                }

            }
            return base64String;
        }

    }
}
