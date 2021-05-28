using System.Collections.Generic;

namespace AWZhome.GutenTag
{
    public interface IGitAdapter
    {
        public IEnumerable<string> Execute(string commandLine);
    }
}
