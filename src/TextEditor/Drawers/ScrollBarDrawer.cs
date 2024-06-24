using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace TextEditor
{
    internal class ScrollBarDrawer
    {
        int _padding = 12;
        int _spaceBetween = 20;
        private readonly Canvas _canvas;

        public ScrollBarDrawer(Canvas canvas)
        {
            _canvas = canvas;
        }

        public void Rerender(int charLines, int cursorRow, int startDisplayRow)
        {
            _canvas.Children.Clear();

            var displayRect = GetDisplayRectRender(charLines, startDisplayRow);
            var cursorRect = GetCursorRender(charLines, cursorRow);

            _canvas.Children.Add(displayRect);
            _canvas.Children.Add(cursorRect);
        }

        private System.Windows.Shapes.Rectangle GetDisplayRectRender(int charLines, int startRow)
        {
            var showPercentage = GetShowPercentage(charLines);
            var positionPercentage = GetPositionPercentage(charLines, startRow);

            var width = _canvas.ActualWidth;
            var height = Math.Floor(showPercentage * _canvas.ActualHeight / 100);
            if (height < 1)
                height = 1;

            var y = Math.Floor(positionPercentage * _canvas.ActualHeight / 100);
            if (y < 0)
                y = 0;
            else if( y + height > _canvas.ActualHeight)
                y = _canvas.ActualHeight - height;

            var rect = new System.Windows.Shapes.Rectangle();
            rect.Margin = new Thickness(4, y, 0, 0);
            rect.Height = height;
            rect.Width = _canvas.ActualWidth - 8;
            rect.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#CCC"));

            return rect;
        }

        private System.Windows.Shapes.Rectangle GetCursorRender(int charLines, int cursorRow)
        {
            var cursorPercentage = GetPositionPercentage(charLines, cursorRow);
            var cursorRect = new System.Windows.Shapes.Rectangle();
            var y = Math.Floor(cursorPercentage * _canvas.ActualHeight / 100 - 2);
            cursorRect.Margin = new Thickness(0, y, 0, 0);
            cursorRect.Height = 2;
            cursorRect.Width = _canvas.ActualWidth;
            cursorRect.Fill = new SolidColorBrush(Colors.Gray);

            return cursorRect;
        }

        private double GetShowPercentage(int charLines)
        {
            var maxRowCount = Math.Floor((_canvas.ActualHeight - _padding) / _spaceBetween);
            return Math.Floor(maxRowCount / charLines * 100);
        }

        private double GetPositionPercentage(int charLines, int cursorRow)
        {
            return Math.Floor((double)cursorRow / charLines * 100);
        }
    }
}
