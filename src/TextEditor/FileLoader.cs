using System.Collections.Generic;
using System.IO;

namespace TextEditor
{
    internal class FileLoader
    {
        public (IList<List<DocumentChar>> rowChars, int charCount) LoadFile(string path) 
        {
            var serializedText = new List<List<DocumentChar>>();
            var text = File.ReadAllLines(path);
            int charCount = 0;

            for (int i = 0; i < text.Length; i++)
            {
                var line = new List<DocumentChar>();
                foreach (var c in text[i])
                {
                    line.Add(new DocumentChar(c, i, line.Count));
                    charCount++;
                }
                serializedText.Add(line);
            }

            return (serializedText, charCount);
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
