using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace TextEditor
{
    internal class Renderer
    {
        int _padding = 12;
        int _spaceBetween = 20;
        private CharFactory _charFactory;
        private readonly Cursor _cursor;
        private readonly Canvas _canvas;
        private readonly IList<List<DocumentChar>> _chars;
        MoveOnDisplay mover;

        public Renderer(CharFactory charFactory, Cursor cursor, Canvas canvas, IList<List<DocumentChar>> chars)
        {
            _charFactory = charFactory;
            _cursor = cursor;
            _canvas = canvas;
            _chars = chars;
            mover = new MoveOnDisplay(_cursor, chars);
        }

        public void Rerender(Direction direction)
        {
            int maxPerColumn = (int)Math.Floor((_canvas.ActualWidth - _padding) / _spaceBetween);
            int maxPerRow = (int)Math.Floor((_canvas.ActualHeight - _padding) / _spaceBetween);

            mover.Move(direction, maxPerRow, maxPerColumn);
            Draw(_chars.AsReadOnly(), maxPerColumn, maxPerRow);
        }

        public void Draw(ReadOnlyCollection<List<DocumentChar>> chars, 
            int maxPerColumn, int maxPerRow)
        {
            _canvas.Children.Clear();

            int startRow = mover.StartRow;
            int startCol = mover.StartCol;
            int endRow = startRow + maxPerRow;
            int y = _padding;

            foreach (var row in chars.Skip(startRow).Take(maxPerRow))
            {
                double x = _padding;
                var charsToRender = row.Skip(startCol).Take(maxPerColumn).ToList();

                BitmapSource combinedLetters = null;
                foreach (var character in charsToRender)
                {
                    BitmapImage? newCharRender = _charFactory.GetCharRender(character.Character);
                    if (combinedLetters is null)
                    {
                        combinedLetters = newCharRender;
                    }
                    else
                    {
                        combinedLetters = StitchBitmaps(combinedLetters, newCharRender);
                    }
                }
                _canvas.Children.Add(new Image()
                {
                    Source = combinedLetters,
                    Margin = new Thickness(x, y, 0, 0),
                });
                y += _spaceBetween;
            }
        }

        public BitmapSource StitchBitmaps(BitmapSource b1, BitmapSource b2)
        {
            var width = b1.PixelWidth + b2.PixelWidth;
            var height = Math.Max(b1.PixelHeight, b2.PixelHeight);
            var wb = new WriteableBitmap(width, height, 96, 96, b1.Format, null);
            var stride1 = (b1.PixelWidth * b1.Format.BitsPerPixel + 7) / 8;
            var stride2 = (b2.PixelWidth * b2.Format.BitsPerPixel + 7) / 8;
            var size = b1.PixelHeight * stride1;
            size = Math.Max(size, b2.PixelHeight * stride2);

            var buffer = new byte[size];
            b1.CopyPixels(buffer, stride1, 0);
            wb.WritePixels(
                new Int32Rect(0, 0, b1.PixelWidth, b1.PixelHeight),
                buffer, stride1, 0);

            b2.CopyPixels(buffer, stride2, 0);
            wb.WritePixels(
                new Int32Rect(b1.PixelWidth, 0, b2.PixelWidth, b2.PixelHeight),
                buffer, stride2, 0);

            return wb;
        }
    }
}
