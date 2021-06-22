using System;

namespace AWZhome.GutenTag
{
    public enum VersionType
    {
        Release,
        PreRelease,
        CIBuild
    }

    public class ProjectVersion : IComparable<ProjectVersion>
    {
        public int Major { get; set; }
        public int Minor { get; set; }
        public int Patch { get; set; }
        public string PreReleaseTag { get; set; }
        public int BuildNumber { get; set; }
        public int Revision { get; set; }
        public bool IsBasedOnDevMark { get; set; } = false;
        public string BasedOnGitTag { get; set; }

        public VersionType Type
        {
            get
            {
                if (BuildNumber == 0)
                {
                    if (string.IsNullOrEmpty(PreReleaseTag))
                    {
                        return VersionType.Release;
                    }
                    else
                    {
                        return VersionType.PreRelease;
                    }
                }
                else
                {
                    return VersionType.CIBuild;
                }
            }
        }

        public int CompareTo(ProjectVersion other)
        {
            if (BuildNumber == 0 && other?.BuildNumber == 0)
            {
                if (!IsBasedOnDevMark && other.IsBasedOnDevMark)
                {
                    return -1;
                }
                else if (IsBasedOnDevMark && !other.IsBasedOnDevMark)
                {
                    return 1;
                }
            }

            int majorCompare = Major.CompareTo(other?.Major);
            if (majorCompare != 0)
                return majorCompare;

            int minorCompare = Minor.CompareTo(other?.Minor);
            if (minorCompare != 0)
                return minorCompare;

            int patchCompare = Patch.CompareTo(other?.Patch);
            if (patchCompare != 0)
                return patchCompare;

            int revisionCompare = BuildNumber.CompareTo(other?.BuildNumber);
            if (revisionCompare != 0)
                return revisionCompare;

            string preReleaseTag = PreReleaseTag ?? string.Empty;
            string otherPreReleaseTag = other?.PreReleaseTag ?? string.Empty;
            if (preReleaseTag != otherPreReleaseTag)
                return preReleaseTag.CompareTo(otherPreReleaseTag);

            return 0;
        }

        public static ProjectVersion DefaultInitial => new() { Minor = 1, IsBasedOnDevMark = true };
    }
}