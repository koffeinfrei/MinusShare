using System.IO;
using Koffeinfrei.Base;

namespace Koffeinfrei.MinusShare
{
    /// <summary>
    /// The UI file liste consists of these items.
    /// </summary>
    public class FileListItem
    {
        public string Name { get; private set; }

        public string FullName { get; private set; }

        public string FileSize { get; private set; }

        public FileListItem(string file)
        {
            FileInfo info = new FileInfo(file);
            Name = info.Name;
            FullName = info.FullName;
            FileSize = info.Length.ToFileSize();
        }
    }
}