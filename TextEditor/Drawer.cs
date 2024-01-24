using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace TextEditor
{
    internal class Drawer
    {
        private CharFactory _charFactory;
        private readonly Canvas _canvas;

        public Drawer(CharFactory charFactory, Canvas canvas)
        {
            _charFactory = charFactory;
            _canvas = canvas;
        }

        public void Draw(ReadOnlyDictionary<int, List<DocumentChar>> chars)
        {
            _canvas.Children.Clear();

            foreach (var row in chars)
            {
                int y = 12 + row.Key * 20;
                double x = 12;
                foreach (var character in row.Value)
                {
                    BitmapImage? newCharRender = _charFactory.GetCharRender(character.Character);

                    _canvas.Children.Add(new Image()
                    {
                        Source = newCharRender,
                        Margin = new Thickness(x, y, 0, 0),
                    });

                    x += newCharRender.Width;
                }
            }
        }
    }
}
