using System.Drawing;
using System.Drawing.Imaging;

namespace X.Util.Entities
{
    public class TextImage
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public Font Font { get; set; }
        public string Value { get; set; }
        public Color BackGroundColor { get; set; }
        public Color Color { get; set; }
        public ImageFormat Format { get; set; }
    }
}
