using System;
using System.Collections.Generic;
using System.Linq;

namespace AWZhome.GutenTag
{
    public abstract class GitAdapter : VcsAdapter
    {
        protected GitAdapter(VersioningConfig versioningConfig) 
            : base(versioningConfig)
        {
        }

        public override string GetCurrentBranch()
        {
            return ExecuteGit("rev-parse --abbrev-ref HEAD")?.SingleOrDefault();
        }

        public override IEnumerable<string> GetChrologicAncestorTags()
        {
            return ExecuteGit("tag --sort=-creatordate --merged HEAD");
        }

        public override string GetTagAtHead()
        {
            return ExecuteGit("tag --points-at HEAD")?.SingleOrDefault();
        }


        public override int GetCommitsCount(string tag = null)
        {
            var commandLine = string.IsNullOrEmpty(tag) ? "rev-list --count HEAD" : $"rev-list --count {tag}..HEAD";
            var commitsSinceTag = ExecuteGit(commandLine)?.SingleOrDefault();
            if ((commitsSinceTag != null) && int.TryParse(commitsSinceTag, out int count))
            {
                return count;
            }

            return 0;
        }

        public override bool HasCleanWorkingCopy()
        {
            return !ExecuteGit("status --short")?.Any() ?? false;
        }

        public abstract IEnumerable<string> ExecuteGit(string commandLine);
    }
}
