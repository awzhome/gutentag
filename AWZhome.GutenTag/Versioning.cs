using System.Collections.Generic;
using System.Linq;

namespace AWZhome.GutenTag
{
    public class VersioningConfig
    {
        public string ReleaseTagPrefix { get; init; } = "v";
        public string DevTagPrefix { get; init; } = "dev-";
    }

    public enum IncrementedPart
    {
        Minor,
        Patch
    }

    public class BranchVersioning
    {
        public string Tag { get; set; }
        public IncrementedPart IncrementedPart { get; set; } = IncrementedPart.Patch;
    }

    public delegate BranchVersioning BranchSpecificConfig(string branchName);

    public class Versioning
    {
        private readonly BranchSpecificConfig branchConfig;
        private readonly VcsAdapter vcsAdapter;

        public Versioning(BranchSpecificConfig branchConfig, VcsAdapter vcsAdapter)
        {
            this.branchConfig = branchConfig;
            this.vcsAdapter = vcsAdapter;
        }

        private VersionInfo SelectBaseVersion(IEnumerable<VersionInfo> chronologicVersionInfos, BranchVersioning branchVersioning)
        {
            var baseVersion = (branchVersioning.IncrementedPart switch
            {
                IncrementedPart.Minor => chronologicVersionInfos.Where(v => (v.Patch == 0) && (v.PreReleaseTag == null)),
                _ => chronologicVersionInfos.Where(v => v.PreReleaseTag == null),
            }).Max();
            baseVersion.BuildNumber = vcsAdapter.GetCommitsCount(baseVersion.BasedOnTag);
            baseVersion.Revision = baseVersion.BuildNumber;
            return baseVersion;
        }

        public VersionInfo GetVersionInfo()
        {
            string currentBranch = vcsAdapter.GetCurrentBranch();
            var branchVersioning = branchConfig(currentBranch) ?? new();

            var chronologicVersionInfos = vcsAdapter.GetChronologicVersionInfos();
            var headTag = vcsAdapter.GetTagAtHead();

            VersionInfo resultVersion = VersionInfo.DefaultInitial;
            if (chronologicVersionInfos.Any())
            {
                if ((headTag != null) && (chronologicVersionInfos.FirstOrDefault()?.BasedOnTag == headTag))
                {
                    resultVersion = chronologicVersionInfos.FirstOrDefault();
                }
                else
                {
                    resultVersion = SelectBaseVersion(chronologicVersionInfos, branchVersioning);
                }
            }
            else
            {
                resultVersion.BuildNumber = vcsAdapter.GetCommitsCount();
                resultVersion.Revision = resultVersion.BuildNumber;
            }


            if (resultVersion.IsBasedOnDevMark)
            {
                resultVersion.BuildNumber++;
                resultVersion.Revision++;
            }

            if (!vcsAdapter.HasCleanWorkingCopy())
            {
                resultVersion.BuildNumber++;
                resultVersion.Revision++;
            }

            if (resultVersion.BuildNumber != 0)
            {
                resultVersion.PreReleaseTag ??= branchVersioning.Tag ?? PreReleaseNormalizer.FromGitBranch(currentBranch);

                if (!resultVersion.IsBasedOnDevMark)
                {
                    switch (branchVersioning.IncrementedPart)
                    {
                        case IncrementedPart.Patch:
                            resultVersion.Patch++;
                            break;
                        default:
                            resultVersion.Minor++;
                            break;
                    }
                }
            }
            else
            {
                var baseVersionWithoutHead = SelectBaseVersion(chronologicVersionInfos.Where(v => v.BasedOnTag != headTag), branchVersioning);
                if (baseVersionWithoutHead.Major == resultVersion.Major
                    && baseVersionWithoutHead.Minor == resultVersion.Minor
                    && baseVersionWithoutHead.Patch == resultVersion.Patch)
                {
                    resultVersion.Revision = baseVersionWithoutHead.BuildNumber + 1;
                }
            }

            return resultVersion;
        }
    }
}