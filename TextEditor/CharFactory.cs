﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace TextEditor
{
    internal class CharFactory
    {
        private Dictionary<char, System.Windows.Media.Imaging.BitmapImage> _letters = new();
        private readonly Font _font;

        public CharFactory(Font font)
        {
            _font = font;
        }

        public System.Windows.Media.Imaging.BitmapImage GetCharRender(char character)
        {
            System.Windows.Media.Imaging.BitmapImage representation = null;
            if(!_letters.TryGetValue(character, out representation))
            {
                representation = DrawText(character.ToString(), _font, Color.Black, Color.Transparent);
                _letters.Add(character, representation); 
            }

            return representation;
        }

        private System.Windows.Media.Imaging.BitmapImage DrawText(String text, Font font, Color textColor, Color backColor)
        {
            //first, create a dummy bitmap just to get a graphics object
            Image img = new Bitmap(1, 1);
            Graphics drawing = Graphics.FromImage(img);

            //measure the string to see how big the image needs to be
            SizeF textSize = drawing.MeasureString(text, font);

            //free up the dummy image and old graphics object
            img.Dispose();
            drawing.Dispose();

            //create a new image of the right size
            img = new Bitmap((int)textSize.Width, (int)textSize.Height);

            drawing = Graphics.FromImage(img);

            //paint the background
            drawing.Clear(backColor);

            //create a brush for the text
            Brush textBrush = new SolidBrush(textColor);

            drawing.DrawString(text, font, textBrush, 0, 0);

            drawing.Save();

            textBrush.Dispose();
            drawing.Dispose();

            return ImageToBitmapImage(img);
        }

        private System.Windows.Media.Imaging.BitmapImage ImageToBitmapImage(System.Drawing.Image image)
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
