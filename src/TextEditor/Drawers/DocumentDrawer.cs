using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TextEditor
{
    internal class DocumentDrawer : IDisposable
    {
        int _padding = 12;
        private CharFactory _charFactory;
        float _spaceBetween => _charFactory.FontSize + 8;
        private readonly Cursor _cursor;
        private readonly Canvas _canvas;
        private IList<List<DocumentChar>> _chars;
        private readonly IDisplayWindow _displayWindow;
        private bool _cursorVisible = true;
        PeriodicTimer _blinkingTimer = null!;
        private object _drawLock = new();

        public DocumentDrawer(Cursor cursor, Canvas canvas,
            IList<List<DocumentChar>> chars, IDisplayWindow displayWindow, CharFactory factory = null!)
        {
            _charFactory = factory ?? new CharFactory(new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular));
            _cursor = cursor;
            _canvas = canvas;
            _chars = chars;
            _displayWindow = displayWindow;
            _ = BlinkCursorTimer();
        }

        public void ChangeCharFactory(CharFactory factory)
        {
            if (factory == null)
                return;
            _charFactory = factory;
        }

        public void Rerender(IList<List<DocumentChar>> chars = null)
        {
            if( chars != null)
                _chars = chars;
            Draw(_chars, GetMaxHeight(_canvas), GetMaxWidth(_canvas));
        }

        public void RerenderRow(int row)
        {
            if (row < 0 || row >= _chars.Count)
                return;

            DrawRow(row);
        }

        public (int maxRowCount, int maxColumnCount) GetMaxRowColCount(Canvas canvas)
        {
            int maxColumnCount = (int)Math.Floor((canvas.ActualWidth - _padding) / (_spaceBetween * 0.8));
            int maxRowCount = (int)Math.Floor((canvas.ActualHeight - _padding) / _spaceBetween);

            return (maxRowCount, maxColumnCount);
        }

        private double GetMaxHeight(Canvas canvas)
        {
            return canvas.ActualHeight - _padding * 2;
        }

        public double GetMaxWidth(Canvas canvas)
        {
            return canvas.ActualWidth - _padding * 2;
        }

        private async Task BlinkCursorTimer()
        {
            _blinkingTimer = new PeriodicTimer(TimeSpan.FromSeconds(0.5));
            while (await _blinkingTimer.WaitForNextTickAsync(CancellationToken.None))
            {
                BlinkCursor();
            }
        }

        private void BlinkCursor()
        {
            BitmapSource combinedLetters = null!;

            int startCol = _displayWindow.StartCol;
            var maxWidth = GetMaxWidth(_canvas);

            int y = _padding;
            double x = _padding;

            lock (_drawLock)
            {
                _cursorVisible = !_cursorVisible;
                if (_canvas.Children.Count > 0)
                    _canvas.Children.RemoveAt(_cursor.Row - _displayWindow.StartRow);

                if (_cursor.Column == 0 && _cursorVisible)
                    combinedLetters = RenderCursorOnEmptyImage();

                combinedLetters = RenderRow(combinedLetters, _chars[_cursor.Row], maxWidth, _cursorVisible, startCol);

                _canvas.Children.Insert(_cursor.Row - _displayWindow.StartRow, new Image()
                {
                    Source = combinedLetters,
                    Margin = new Thickness(x, y + ((_cursor.Row - _displayWindow.StartRow) * _spaceBetween), 0, 0),
                });
            }
        }

        private void DrawRow(int row)
        {
            BitmapSource combinedLetters = null!;

            var maxWidth = GetMaxWidth(_canvas);
            int startCol = _displayWindow.StartCol;

            int y = _padding;
            double x = _padding;

            lock (_drawLock)
            {
                if (_canvas.Children.Count > 0 )
                    _canvas.Children.RemoveAt(row - _displayWindow.StartRow);

                combinedLetters = RenderRow(combinedLetters, _chars[row], maxWidth, row == _cursor.Row, startCol);

                _canvas.Children.Insert(row - _displayWindow.StartRow, new Image()
                {
                    Source = combinedLetters,
                    Margin = new Thickness(x, y + ((row - _displayWindow.StartRow) * _spaceBetween), 0, 0),
                });
            }
        }

        private void Draw(IList<List<DocumentChar>> chars, double maxHeight, double maxWidth)
        {
            lock (_drawLock)
            {
                _canvas.Children.Clear();

                int startRow = _displayWindow.StartRow;
                int startCol = _displayWindow.StartCol;
                double x = _padding;
                int maxRows = (int)Math.Floor((_canvas.ActualHeight - _padding) / _spaceBetween) + startRow;
                maxRows = maxRows >= chars.Count ? chars.Count : maxRows;
                
                var tasks = new Task[maxRows - startRow];
                ConcurrentDictionary<int, BitmapSource> rowImagePairs = new ConcurrentDictionary<int, BitmapSource>();

                for (int i = startRow; i < maxRows; i++)
                {
                    int j = i;
                    // concurrently draw rows
                    tasks[j - startRow] = (Task.Run(() =>
                    {
                        BitmapSource combinedLetters = null!;

                        combinedLetters = RenderRow(combinedLetters, chars[j], maxWidth, j == _cursor.Row, startCol);
                        combinedLetters?.Freeze();
                        rowImagePairs.TryAdd(j - startRow, combinedLetters);
                    }));
                }

                Task.WaitAll(tasks.ToArray());
                foreach(var data in rowImagePairs.OrderBy(x=> x.Key)) 
                {
                    float y = _padding + data.Key * _spaceBetween;
                    _canvas.Children.Add(new Image()
                    {
                        Source = data.Value,
                        Margin = new Thickness(x, y, 0, 0),
                    });
                }
            }
        }

        BitmapSource RenderRow(BitmapSource combinedLetters, IList<DocumentChar> row, double maxWidth,
                            bool renderCursor, int startCol)
        {
            for (int j = startCol; j < row.Count; j++)
            {
                BitmapSource? newCharRender = _charFactory
                    .GetCharRender(row[j].Character);

                if (combinedLetters is null)
                    combinedLetters = newCharRender;
                else if (combinedLetters.Width > maxWidth)
                    return combinedLetters;
                else
                    combinedLetters = StitchBitmaps(combinedLetters, newCharRender);

                if (renderCursor && _cursor.Column - 1 == j)
                    combinedLetters = Overlay(combinedLetters, _cursor.GetRender(), (int)combinedLetters.Width);
            }

            return combinedLetters;
        }

        private BitmapSource StitchBitmaps(BitmapSource b1, BitmapSource b2)
        {
            var width = b1.PixelWidth + b2.PixelWidth;
            var height = Math.Max(b1.PixelHeight, b2.PixelHeight);
            var wb = new WriteableBitmap(width, height, 96, 96, b1.Format, b1.Palette);
            var stride1 = (b1.PixelWidth * b1.Format.BitsPerPixel) / 8;
            var stride2 = (b2.PixelWidth * b2.Format.BitsPerPixel) / 8;
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

        private BitmapSource RenderCursorOnEmptyImage()
        {
            PixelFormat pf = PixelFormats.Bgr32;
            int rawStride = (1 * pf.BitsPerPixel + 1) / 8;
            byte[] rawImage = new byte[rawStride * 20];
            var i = BitmapSource.Create(1, 15, 96, 96, pf, null, rawImage, rawStride);
            var combinedLetters = Overlay(i, _cursor.GetRender(), 0);

            return combinedLetters;
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

        public void Dispose()
        {
            if (_blinkingTimer != null)
                _blinkingTimer.Dispose();
        }
    }
}
