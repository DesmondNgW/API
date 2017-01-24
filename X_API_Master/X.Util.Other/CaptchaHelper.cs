using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using X.Util.Core.Common;
using X.Util.Entities;
using X.Util.Extend.Cryption;

namespace X.Util.Other
{
    public class CaptchaHelper
    {
        private static CaptchaModel CaptchaOptions()
        {
            var options = new CaptchaModel
            {
                Width = 150,
                Height = 30,
                Colors = new[] { Color.Black, Color.Red, Color.Blue, Color.Green, Color.Orange, Color.Brown, Color.DarkBlue },
                BackGroundColor = Color.White,
                Characters = "23456789ABCDEFGHJKLMNPRSTWXY",
                Fonts = new[] { "Times New Roman", "MS Mincho", "Book Antiqua", "Gungsuh", "PMingLiU", "Impact" },
                Value = string.Empty
            };
            for (var i = 0; i < 6; i++)
            {
                options.Value += options.Characters[StringConvert.SysRandom.Next(options.Characters.Length)];
            }
            return options;
        }

        private static TextImage TextImageOptions()
        {
            return new TextImage
            {
                Width = 200,
                Height = 40,
                Color = Color.Blue,
                BackGroundColor = Color.White,
                Font = new Font("Microsoft yaHei", 16),
                Value = "TextImageModel",
                Format = ImageFormat.Png
            };
        }

        /// <summary>
        /// 1*1 像素图片
        /// </summary>
        public static byte[] BitMap()
        {
            var bmp = new Bitmap(1, 1);
            byte[] result;
            var ms = new MemoryStream();
            try
            {
                bmp.Save(ms, ImageFormat.Jpeg);
                result = ms.ToArray();
            }
            finally
            {
                bmp.Dispose();
                ms.Close();
                ms.Dispose();
            }
            return result;
        }

        /// <summary>
        /// 简单画图
        /// </summary>
        /// <param name="options"></param>
        public static byte[] TextImage(TextImage options)
        {
            if (options == null) options = TextImageOptions();
            var bmp = new Bitmap(options.Width, options.Height);
            var g = Graphics.FromImage(bmp);
            g.Clear(Color.FromArgb(0, options.BackGroundColor));
            g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            g.DrawString(options.Value, options.Font, new SolidBrush(options.Color), 0, 0);
            var ms = new MemoryStream();
            byte[] result;
            try
            {
                bmp.Save(ms, options.Format);
                result = ms.ToArray();
            }
            finally
            {
                bmp.Dispose();
                g.Dispose();
                ms.Close();
                ms.Dispose();
            }
            return result;
        }

        /// <summary>
        /// 验证码图片
        /// </summary>
        /// <param name="options"></param>
        public static byte[] Captcha(CaptchaModel options)
        {
            if (Equals(options, null)) options = CaptchaOptions();
            int width = options.Width, height = options.Height;
            var value = options.Value;
            var colors = options.Colors;
            var fonts = options.Fonts;
            var bmp = new Bitmap(width, height);
            var g = Graphics.FromImage(bmp);
            g.Clear(options.BackGroundColor);
            g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            for (var i = 0; i < 3; i++)
            {
                var x1 = StringConvert.SysRandom.Next(width);
                var y1 = StringConvert.SysRandom.Next(height);
                var x2 = StringConvert.SysRandom.Next(width);
                var y2 = StringConvert.SysRandom.Next(height);
                var clr = colors[StringConvert.SysRandom.Next(colors.Length)];
                g.DrawLine(new Pen(clr), x1, y1, x2, y2);
            }
            for (var i = 0; i < value.Length; i++)
            {
                var fnt = fonts[StringConvert.SysRandom.Next(fonts.Length)];
                var ft = new Font(fnt, 16);
                var clr = colors[StringConvert.SysRandom.Next(colors.Length)];
                g.DrawString(value[i].ToString(), ft, new SolidBrush(clr), (float)i * 20 + 20, 6);
            }
            for (var i = 0; i < 50; i++)
            {
                var x = StringConvert.SysRandom.Next(bmp.Width);
                var y = StringConvert.SysRandom.Next(bmp.Height);
                var clr = colors[StringConvert.SysRandom.Next(colors.Length)];
                bmp.SetPixel(x, y, clr);
            }
            var ms = new MemoryStream();
            byte[] result;
            try
            {
                bmp.Save(ms, ImageFormat.Jpeg);
                result = ms.ToArray();
            }
            finally
            {
                bmp.Dispose();
                g.Dispose();
                ms.Close();
                ms.Dispose();
            }
            return result;
        }

        /// <summary>
        /// 验证验证码是否正确
        /// </summary>
        /// <param name="options"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        //public static bool VerifyCaptcha(CaptchaModel options, string value)
        //{
        //    if (Equals(options, null)) options = CaptchaOptions();
        //    return BaseCryption.VerifyData(options.CookieName, CookieHelper.GetCookie(context, options.CookieName), HmacType.Sha1);
        //}
    }
}
