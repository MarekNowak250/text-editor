using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Dynamic;
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
        private readonly MoveOnDisplay _moveOnDisplay;

        public Renderer(CharFactory charFactory, Cursor cursor, Canvas canvas,
            IList<List<DocumentChar>> chars, MoveOnDisplay moveOnDisplay)
        {
            _charFactory = charFactory;
            _cursor = cursor;
            _canvas = canvas;
            _chars = chars;
            _moveOnDisplay = moveOnDisplay;
        }

        public void Rerender()
        {
            var maxCount = GetMaxRowColCount(_canvas);

            Draw(_chars.AsReadOnly(), maxCount.maxColumnCount, maxCount.maxRowCount);
        }

        public (int maxRowCount, int maxColumnCount) GetMaxRowColCount(Canvas canvas)
        {
            int maxColumnCount = (int)Math.Floor((canvas.ActualWidth - _padding) / _spaceBetween);
            int maxRowCount = (int)Math.Floor((canvas.ActualHeight - _padding) / _spaceBetween);

            return (maxRowCount, maxColumnCount);
        }

        public void Draw(ReadOnlyCollection<List<DocumentChar>> chars, 
            int maxColumnCount, int maxRowCount)
        {
            _canvas.Children.Clear();

            int startRow = _moveOnDisplay.StartRow;
            int startCol = _moveOnDisplay.StartCol;
            int endRow = startRow + maxRowCount;
            endRow = endRow <= chars.Count() ? endRow : chars.Count();
            int y = _padding;

            for(int i =startRow; i < endRow; i++)
            {
                double x = _padding;
                var columnCount = maxColumnCount <= _chars[i].Count() ? maxColumnCount : _chars[i].Count();
                var charsToRender = _chars[i].Skip(startCol).Take(columnCount+2).ToList();

                if(charsToRender.Count == 0 && _cursor.Row == i) 
                {
                    _canvas.Children.Add(new Image()
                    {
                        Source = _charFactory.GetCharRender(_cursor.Character),
                        Margin = new Thickness(x, y, 0, 0),
                    });
                    continue;
                }

                BitmapSource combinedLetters = null!;
                for(int j=0; j < charsToRender.Count; j++)
                {

                    BitmapImage? newCharRender = _charFactory
                        .GetCharRender(charsToRender[j].Character);

                    if (combinedLetters is null)
                    {
                        combinedLetters = newCharRender;
                    }
                    else
                    {
                        combinedLetters = StitchBitmaps(combinedLetters, newCharRender);
                    }

                    if (_cursor.Column == (startCol + j) && _cursor.Row == i)
                    {
                        combinedLetters = StitchBitmaps(combinedLetters, _charFactory
                            .GetCharRender(_cursor.Character));
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
