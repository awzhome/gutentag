using System;

namespace AWZhome.GutenTag
{
    public class ProjectVersion : IComparable<ProjectVersion>
    {
        public int Major { get; set; }
        public int Minor { get; set; }
        public int Patch { get; set; }
        public string PreReleaseTag { get; set; }
        public int PreReleaseNumber { get; set; }
        public int Revision { get; set; }
        public bool IsDevMark { get; set; } = false;
        public string BasedOnGitTag { get; set; }

        public int CompareTo(ProjectVersion other)
        {
            if (PreReleaseNumber == 0 && other?.PreReleaseNumber == 0)
            {
                if (!IsDevMark && other.IsDevMark)
                {
                    return -1;
                }
                else if (IsDevMark && !other.IsDevMark)
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

            int revisionCompare = PreReleaseNumber.CompareTo(other?.PreReleaseNumber);
            if (revisionCompare != 0)
                return revisionCompare;

            string preReleaseTag = PreReleaseTag ?? string.Empty;
            string otherPreReleaseTag = other?.PreReleaseTag ?? string.Empty;
            if (preReleaseTag != otherPreReleaseTag)
                return preReleaseTag.CompareTo(otherPreReleaseTag);

            return 0;
        }

        public static ProjectVersion DefaultInitial => new() { Minor = 1, IsDevMark = true };
    }
}