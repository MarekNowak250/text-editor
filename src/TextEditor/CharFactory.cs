using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace TextEditor
{
    public class CharFactory
    {
        public float FontSize => _font.Size;
        private ConcurrentDictionary<char, System.Windows.Media.Imaging.BitmapImage> _letters = new();
        private Font _font;

        public CharFactory(Font font, bool prerenderCommon = false)
        {
            _font = font;
            if (prerenderCommon)
                PrerenderCommonChars();
        }

        public System.Windows.Media.Imaging.BitmapImage GetCharRender(char character, Font font = null)
        {
            if(font == null)
                font = _font;

            System.Windows.Media.Imaging.BitmapImage representation = null!;
            if(!_letters.TryGetValue(character, out representation!))
            {
                representation = DrawText(character.ToString(), font, Color.Black, Color.Transparent);
                representation.Freeze();
                _letters.TryAdd(character, representation); 
            }

            return representation;
        }

        private void PrerenderCommonChars()
        {
            var commonChars = new char[] { 'a', 'b' ,'c', 'd', 'e','f','g','h','i','j','k', 
                'l','m','n','o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', 
                '1', '2', '3', '4' ,'5','6','7','8','9','0', '-', '{','}' };

            foreach (char c in commonChars)
            {
                _ = GetCharRender(c);
            }
        }

        private System.Windows.Media.Imaging.BitmapImage DrawText(String text, Font font, Color textColor, Color backColor)
        {
            //first, create a dummy bitmap just to get a graphics object
            Bitmap img = new Bitmap(1, 1);
            Graphics drawing = Graphics.FromImage(img);

            //measure the string to see how big the image needs to be
            SizeF textSize = drawing.MeasureString(text, font);

            //free up the dummy image and old graphics object
            img.Dispose();
            drawing.Dispose();

            //temoprary fix for too much space between chars
            textSize.Width -= 1.2F;

            if (textSize.Width < 1)
                textSize.Width = 1;

            if (textSize.Height < 1)
                textSize.Height = 1;


            //create a new image of the right size
            img = new Bitmap((int)textSize.Width, (int)textSize.Height);

            drawing = Graphics.FromImage(img);

            //paint the background
            drawing.Clear(backColor);

            //create a brush for the text
            Brush textBrush = new SolidBrush(textColor);

            drawing.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            drawing.InterpolationMode = InterpolationMode.HighQualityBicubic;
            drawing.PixelOffsetMode = PixelOffsetMode.HighQuality;
            drawing.SmoothingMode = SmoothingMode.HighQuality;
            drawing.CompositingQuality = CompositingQuality.HighQuality;

            drawing.DrawString(text, font, textBrush, 0, 0);

            drawing.Save();

            textBrush.Dispose();
            drawing.Dispose();

            return ImageToBitmapImage(img);
        }

        private System.Windows.Media.Imaging.BitmapImage ImageToBitmapImage(Image image)
        {
            using (var ms = new MemoryStream())
            {
                image.Save(ms, ImageFormat.Png);
                ms.Position = 0;

                var bi = new System.Windows.Media.Imaging.BitmapImage();
                bi.BeginInit();
                bi.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad;
                bi.StreamSource = ms;
                bi.EndInit();
                return bi;
            }
        }
    }
}
