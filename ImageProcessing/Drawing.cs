using ImageProcessing.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessing
{
    public static class Drawing
    {
        public static System.Drawing.Image DrawRectangles(System.Drawing.Image image, List<GraphicObject> graphicObjects)
        {
            using (Graphics graphics = Graphics.FromImage(image))
            {
                Color color = Color.Red;
                Pen pen = new Pen(color);

                foreach (var i in graphicObjects)
                {
                    foreach (var j in i.Rectangles)
                    {
                        Rectangle rect = new Rectangle(j.X, j.Y, j.Width, j.Height);

                        graphics.DrawRectangle(pen, rect);
                    }
                }
                return image;
            }
        }
        public static System.Drawing.Image DrawLabels(System.Drawing.Image image, List<LabelObject> labels)
        {
            using (Graphics graphics = Graphics.FromImage(image))
            {
                int height = 20;
                int fontSize = (image.Height > 4000) ? 30 : (image.Height > 2000) ? 24 : (image.Height > 1000) ? 20 : (image.Height > 500) ? 16 : (image.Height > 100) ? 10 : 6;
                Color color = Color.Red;
                Pen pen = new Pen(color);

                for (int i = 0; i < labels.Count && i*height < image.Height; i++)
                {
                    Font font = new Font("Aerial", fontSize, FontStyle.Bold);
                    SolidBrush brush = new SolidBrush(color);
                    Point point = new Point(10, i * height);

                    graphics.DrawString(String.Format("{0} = {1}", labels[i].Name, (int)(labels[i].Score*100)), font, brush, point);
                }
                return image;
            }
        }
        public static List<System.Drawing.Image> DevideImage(PartInfo image)
        {
            int colHeight = Division(image.Image.Width, 50);
            int rowHeight = Division(image.Image.Height, 50);
            int cols = image.Image.Width / colHeight;
            int rows = image.Image.Height / rowHeight;
            
            List<System.Drawing.Image> list = new List<System.Drawing.Image>();
            Graphics g = Graphics.FromImage(image.Image);
            Brush redBrush = new SolidBrush(Color.Red);
            Pen pen = new Pen(redBrush, 3);
            
            for (int i = 0; i < image.Image.Width; i = (image.Image.Width / 3) + i)
            {
                for (int y = 0; y < image.Image.Height; y = (image.Image.Height / 3) + y)
                {
                    Rectangle r = new Rectangle(i, y, image.Image.Width / 3, image.Image.Height / 3);
                    g.DrawRectangle(pen, r);
                    if (i > 0 && y > 0)
                    {
                        if (i + r.Width < image.Image.Width && y + r.Height < image.Image.Height)
                        {
                            list.Add(cropImage(image.Image, r));
                        }
                    }
                }
            }
            g.Dispose();
            return list;

            //int width = image.Image.Width / 2;
            //int height = image.Image.Height / 2;

            //List<PartInfo> parts = new List<PartInfo>();
            //parts.Capacity = 4;

            //for (int i = 0; i > 50; i++)
            //{
            //    for (int j = 0; height > 50; j++)
            //    {
            //        var graphics = Graphics.FromImage(parts[index].Image);
            //        int index = i * 2 + j;
            //        parts.Add(new PartInfo()
            //        {
            //            Image = new Bitmap(width, height),
            //            AmountOfTrees = image.AmountOfTrees + 1,
            //            XNumber = i + 1,
            //            YNumber = j + 1
            //        });



            //        graphics.DrawImage(image.Image, new Rectangle(0, 0, width, height), new Rectangle(i * width, j * height, width, height), GraphicsUnit.Pixel);
            //        graphics.Dispose();
            //    }
            //}
            //return parts;
        }

        private static System.Drawing.Image cropImage(System.Drawing.Image img, Rectangle cropArea)
        {
            Bitmap bmpImage = new Bitmap(img);
            Bitmap bmpCrop = bmpImage.Clone(cropArea, System.Drawing.Imaging.PixelFormat.DontCare);
            return (System.Drawing.Image)(bmpCrop);
        }

        private static int Division(int size, int requirement)
        {
            int res = size;
            while (res / 2 > requirement)
            {
                res /= 2;
            }
            return res;
        }
    }
}
