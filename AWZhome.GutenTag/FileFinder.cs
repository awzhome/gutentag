using System.Collections.Generic;
using System.IO;

namespace AWZhome.GutenTag
{
    public static class FileFinder
    {
        public static IEnumerable<string> FindFiles(string directory, string pattern) =>
            Directory.EnumerateFiles(directory, pattern, new EnumerationOptions() { RecurseSubdirectories = true });
    }
}
