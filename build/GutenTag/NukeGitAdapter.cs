using System.Collections.Generic;
using System.Linq;
using static Nuke.Common.Tools.Git.GitTasks;

namespace AWZhome.GutenTag.Nuke
{
    public class NukeGitAdapter : IGitAdapter
    {
        public IEnumerable<string> Execute(string commandLine) =>
            Git(commandLine, null, null, default, false, false, false).Select(o => o.Text);
    }
}
