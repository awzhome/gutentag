using System.Collections.Generic;
using System.Linq;

namespace AWZhome.GutenTag
{
    public abstract class VcsAdapter
    {
        private readonly VersioningConfig versioningConfig;

        public VcsAdapter(VersioningConfig versioningConfig)
        {
            this.versioningConfig = versioningConfig;
        }

        public abstract string GetCurrentBranch();

        public abstract IEnumerable<string> GetChrologicAncestorTags();

        public abstract string GetTagAtHead();

        public abstract int GetCommitsCount(string sinceTag = null);

        public abstract bool HasCleanWorkingCopy();

        public IEnumerable<VersionInfo> GetChronologicVersionInfos()
        {
            return GetChrologicAncestorTags().Select(tag => TagParser.Parse(tag, versioningConfig)).Where(v => v != null);
        }
    }
}
