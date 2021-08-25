using System.Collections.Generic;
using System.Linq;

namespace AWZhome.GutenTag.Tests
{
    class TestVcsAdapter : VcsAdapter
    {
        public TestVcsAdapter(VersioningConfig versioningConfig)
            : base(versioningConfig)
        {
        }


        public string CurrentBranch { get; set; }

        public override string GetCurrentBranch() => CurrentBranch;

        public IEnumerable<(string Tag, int CommitsToHead)> ChronologicAncestorTags { get; set; }

        public override IEnumerable<string> GetChrologicAncestorTags() => ChronologicAncestorTags?.Select(t => t.Tag) ?? Enumerable.Empty<string>();

        public override int GetCommitsCount(string tag) =>
            ChronologicAncestorTags?.Where(t => t.Tag == tag)?.Select(t => t.CommitsToHead)?.FirstOrDefault() ?? 0;
        
        public bool IsCleanWorkingCopy { get; set; } = true;

        public override bool HasCleanWorkingCopy() => IsCleanWorkingCopy;

        public override string GetTagAtHead() => 
            ChronologicAncestorTags?.Where(t => t.CommitsToHead == 0)?.Select(t => t.Tag)?.FirstOrDefault();
    }
}
