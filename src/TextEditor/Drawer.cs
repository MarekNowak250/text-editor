﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
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
            _ = BlinkCursorTimer();
        }

        public async Task BlinkCursorTimer()
        {
            var timer = new PeriodicTimer(TimeSpan.FromSeconds(0.5));
            while (await timer.WaitForNextTickAsync(CancellationToken.None))
            {
                BlinkCursor();
            }
        }

        private bool _cursorVisible = true;

        private void BlinkCursor()
        {
            _cursorVisible = !_cursorVisible;
            var maxCount = GetMaxRowColCount(_canvas);
            int maxColumnCount = maxCount.maxColumnCount;

            int startCol = _moveOnDisplay.StartCol;
            int y = _padding;


            double x = _padding;
            var columnCount = maxColumnCount <= _chars[_cursor.Row].Count() ? maxColumnCount : _chars[_cursor.Row].Count();
            var charsToRender = _chars[_cursor.Row].Skip(startCol).Take(columnCount + 2).ToList();

            _canvas.Children.RemoveAt(_cursor.Row);
            BitmapSource combinedLetters = null!;

            if (_cursor.Column == 0 && _cursorVisible)
                combinedLetters = _charFactory.GetCharRender(_cursor.Character);

            combinedLetters = RenderRow(combinedLetters, charsToRender, _cursorVisible, startCol);

            _canvas.Children.Insert(_cursor.Row, new Image()
            {
                Source = combinedLetters,
                Margin = new Thickness(x, y + (_cursor.Row * _spaceBetween), 0, 0),
            });
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

            for (int i = startRow; i < endRow; i++)
            {
                double x = _padding;
                var columnCount = maxColumnCount <= _chars[i].Count() ? maxColumnCount : _chars[i].Count();
                var charsToRender = _chars[i].Skip(startCol).Take(columnCount + 2).ToList();

                BitmapSource combinedLetters = null!;

                combinedLetters = RenderRow(combinedLetters, charsToRender, i == _cursor.Row, startCol);

                _canvas.Children.Add(new Image()
                {
                    Source = combinedLetters,
                    Margin = new Thickness(x, y, 0, 0),
                });
                y += _spaceBetween;
            }
        }

        BitmapSource RenderRow(BitmapSource combinedLetters, IList<DocumentChar> charsToRender,
                            bool renderCursor = false, int startCol = -1)
        {
            for (int j = 0; j < charsToRender.Count; j++)
            {
                BitmapSource? newCharRender = _charFactory
                    .GetCharRender(charsToRender[j].Character);

                if (combinedLetters is null)
                    combinedLetters = newCharRender;
                else
                    combinedLetters = StitchBitmaps(combinedLetters, newCharRender);

                if (renderCursor && _cursor.Column - 1 == (startCol + j))
                    combinedLetters = Overlay(combinedLetters, _charFactory
                        .GetCharRender(_cursor.Character), (int)combinedLetters.Width);
            }

            return combinedLetters;
        }

        private BitmapSource StitchBitmaps(BitmapSource b1, BitmapSource b2)
        {
            var width = b1.PixelWidth + b2.PixelWidth;
            var height = Math.Max(b1.PixelHeight, b2.PixelHeight);
            var wb = new WriteableBitmap(width, height, 96, 96, b1.Format, b1.Palette);
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

        private BitmapSource Overlay(BitmapSource bmp1, BitmapSource bmp2, int xSpace)
        {
            var drawingVisual = new DrawingVisual();
            var drawingContext = drawingVisual.RenderOpen();
            drawingContext.DrawImage(bmp1, new Rect(new Size(bmp1.Width, bmp1.Height)));
            drawingContext.DrawImage(bmp2, new Rect(xSpace - bmp2.Width * 0.5, 0, bmp2.Width, bmp2.Height));
            drawingContext.Close();
            var mergedImage = new RenderTargetBitmap((int)bmp1.Width, (int)bmp1.Height, 96, 96, PixelFormats.Pbgra32);
            mergedImage.Render(drawingVisual);
            return mergedImage;
        }
    }
}
