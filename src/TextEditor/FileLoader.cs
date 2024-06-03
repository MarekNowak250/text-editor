using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

namespace TextEditor
{
    internal class FileLoader
    {
        public IList<List<DocumentChar>> LoadFile(string path) 
        {
            var serializedText = new List<List<DocumentChar>>();
            var text = File.ReadAllLines(path);
            for (int i = 0; i < text.Length; i++)
            {
                var line = new List<DocumentChar>();
                foreach (var c in text[i])
                {
                    line.Add(new DocumentChar(c, i, line.Count));
                }
                serializedText.Add(line);
            }

            return serializedText;
        }

        public void SaveFile(string path, IList<List<DocumentChar>> rowChars)
        {
            var rows = new List<string>();
            foreach(var line in rowChars)
            {
                var row = "";

                foreach(var character in line)
                {
                    row += character.Character;
                }

                rows.Add(row);
            }

            File.WriteAllLines(path, rows);
        }
    }
}
