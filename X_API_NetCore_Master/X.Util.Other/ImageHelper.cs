using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using X.Util.Core.Log;
using X.Util.Entities;

namespace X.Util.Other
{
    public class ImageHelper
    {
        /// <summary>
        /// 生成缩略图
        /// </summary>
        /// <param name="originalImagePath">源图路径（物理路径）</param>
        /// <param name="width">缩略图宽度</param>
        /// <param name="height">缩略图高度</param>
        /// <param name="mode">生成缩略图的方式</param>    
        public static byte[] MakeThumbnail(string originalImagePath, int width, int height, string mode)
        {
            var originalImage = Image.FromFile(originalImagePath);
            int towidth = width, toheight = height, x = 0, y = 0, ow = originalImage.Width, oh = originalImage.Height;
            switch (mode)
            {
                case "HW":  //指定高宽缩放（可能变形）                
                    break;
                case "W":   //指定宽，高按比例                    
                    toheight = originalImage.Height * width / originalImage.Width;
                    break;
                case "H":   //指定高，宽按比例
                    towidth = originalImage.Width * height / originalImage.Height;
                    break;
                case "Cut": //指定高宽裁减（不变形）                
                    if (originalImage.Width / (double)originalImage.Height > (double)towidth / toheight)
                    {
                        oh = originalImage.Height;
                        ow = originalImage.Height * towidth / toheight;
                        y = 0;
                        x = (originalImage.Width - ow) / 2;
                    }
                    else
                    {
                        ow = originalImage.Width;
                        oh = originalImage.Width * height / towidth;
                        x = 0;
                        y = (originalImage.Height - oh) / 2;
                    }
                    break;
                default:
                    break;
            }
            var bitmap = new Bitmap(towidth, toheight);
            var g = Graphics.FromImage(bitmap);
            g.InterpolationMode = InterpolationMode.High;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.Clear(Color.Transparent);
            g.DrawImage(originalImage, new Rectangle(0, 0, towidth, toheight), new Rectangle(x, y, ow, oh), GraphicsUnit.Pixel);
            var ms = new MemoryStream();
            byte[] result = default;
            try
            {
                bitmap.Save(ms, ImageFormat.Jpeg);
                result = ms.ToArray();
            }
            catch (Exception e)
            {
                Logger.Client.Error(Logger.Client.GetMethodInfo(MethodBase.GetCurrentMethod(), new object[] { originalImagePath, width, height, mode }), e, LogDomain.Util);
            }
            finally
            {
                originalImage.Dispose();
                bitmap.Dispose();
                g.Dispose();
            }
            return result;
        }

        /// <summary>
        /// 图片水印处理方法
        /// </summary>
        /// <param name="path">需要加载水印的图片路径（绝对路径）</param>
        /// <param name="waterpath">水印图片（绝对路径）</param>
        /// <param name="location">水印位置（传送正确的代码）</param>
        public static byte[] ImageWatermark(string path, string waterpath, string location)
        {
            byte[] result = default;
            var extension = Path.GetExtension(path);
            if (extension == ".jpg" || extension == ".bmp" || extension == ".jpeg")
            {
                var img = Image.FromFile(path);
                var waterimg = Image.FromFile(waterpath);
                var g = Graphics.FromImage(img);
                var locationObj = GetLocation(location, img, waterimg);
                g.DrawImage(waterimg, new Rectangle(int.Parse(locationObj.Item1.ToString()), int.Parse(locationObj.Item2.ToString()), waterimg.Width, waterimg.Height));
                waterimg.Dispose();
                g.Dispose();
                var ms = new MemoryStream();
                try
                {
                    img.Save(ms, ImageFormat.Jpeg);
                    result = ms.ToArray();
                }
                catch (Exception e)
                {
                    Logger.Client.Error(Logger.Client.GetMethodInfo(MethodBase.GetCurrentMethod(), new object[] { path, waterpath, location }), e, LogDomain.Util);
                }
                finally
                {
                    img.Dispose();
                }
            }
            return result;
        }

        /// <summary>
        /// 图片水印位置处理方法
        /// </summary>
        /// <param name="location">水印位置</param>
        /// <param name="img">需要添加水印的图片</param>
        /// <param name="waterimg">水印图片</param>
        private static Tuple<int, int> GetLocation(string location, Image img, Image waterimg)
        {
            int x, y;
            switch (location)
            {
                case "LT":
                    x = 10;
                    y = 10;
                    break;
                case "T":
                    x = img.Width / 2 - waterimg.Width / 2;
                    y = img.Height - waterimg.Height;
                    break;
                case "RT":
                    x = img.Width - waterimg.Width;
                    y = 10;
                    break;
                case "LC":
                    x = 10;
                    y = img.Height / 2 - waterimg.Height / 2;
                    break;
                case "C":
                    x = img.Width / 2 - waterimg.Width / 2;
                    y = img.Height / 2 - waterimg.Height / 2;
                    break;
                case "RC":
                    x = img.Width - waterimg.Width;
                    y = img.Height / 2 - waterimg.Height / 2;
                    break;
                case "LB":
                    x = 10;
                    y = img.Height - waterimg.Height;
                    break;
                case "B":
                    x = img.Width / 2 - waterimg.Width / 2;
                    y = img.Height - waterimg.Height;
                    break;
                default:
                    x = img.Width - waterimg.Width;
                    y = img.Height - waterimg.Height;
                    break;

            }
            return new Tuple<int, int>(x, y);
        }

        /// <summary>
        /// 文字水印处理方法
        /// </summary>
        /// <param name="path">图片路径（绝对路径）</param>
        /// <param name="size">字体大小</param>
        /// <param name="letter">水印文字</param>
        /// <param name="color">颜色</param>
        /// <param name="location">水印位置</param>
        public static byte[] LetterWatermark(string path, int size, string letter, Color color, string location)
        {
            byte[] result = default;
            var extension = Path.GetExtension(path);
            if (extension == ".jpg" || extension == ".bmp" || extension == ".jpeg")
            {
                var img = Image.FromFile(path);
                var gs = Graphics.FromImage(img);
                var locationObj = GetLocation(location, img, size, letter.Length);
                Font font = new Font("宋体", size);
                Brush br = new SolidBrush(color);
                gs.DrawString(letter, font, br, locationObj.Item1, locationObj.Item2);
                gs.Dispose();
                var ms = new MemoryStream();
                try
                {
                    img.Save(ms, ImageFormat.Jpeg);
                    result = ms.ToArray();
                }
                catch (Exception e)
                {
                    Logger.Client.Error(Logger.Client.GetMethodInfo(MethodBase.GetCurrentMethod(), new object[] { path, size, letter, color, location }), e, LogDomain.Util);
                }
                finally
                {
                    img.Dispose();
                }
            }
            return result;
        }

        /// <summary>
        /// 文字水印位置的方法
        /// </summary>
        /// <param name="location">位置代码</param>
        /// <param name="img">图片对象</param>
        /// <param name="width">宽(当水印类型为文字时,传过来的就是字体的大小)</param>
        /// <param name="height">高(当水印类型为文字时,传过来的就是字符的长度)</param>
        private static Tuple<float, float> GetLocation(string location, Image img, int width, int height)
        {
            float x = 10, y = 10;
            switch (location)
            {
                case "LT":
                    break;
                case "T":
                    x = img.Width / 2 - (width * height) / 2;
                    break;
                case "RT":
                    x = img.Width - width * height;
                    break;
                case "LC":
                    y = img.Height / 2;
                    break;
                case "C":
                    x = img.Width / 2 - (width * height) / 2;
                    y = img.Height / 2;
                    break;
                case "RC":
                    x = img.Width - height;
                    y = img.Height / 2;
                    break;
                case "LB":
                    y = img.Height - width - 5;
                    break;
                case "B":
                    x = img.Width / 2 - (width * height) / 2;
                    y = img.Height - width - 5;
                    break;
                default:
                    x = img.Width - width * height;
                    y = img.Height - width - 5;
                    break;
            }
            return new Tuple<float, float>(x, y);
        }
    }
}
