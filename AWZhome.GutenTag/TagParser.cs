using System.Text.RegularExpressions;

namespace AWZhome.GutenTag
{
    public static class TagParser
    {
        private static readonly Regex tagPattern = new(
            @"(?<prefix>[^0-9\.]*)(?<major>[0-9]+)(\.(?<minor>[0-9]+)(\.(?<patch>[0-9]+))?)?([-\.](?<tag>\S+))?",
            RegexOptions.Singleline | RegexOptions.Compiled);

        public static VersionInfo Parse(string tag, VersioningConfig versioningConfig)
        {
            var match = tagPattern.Match(tag);
            if (match.Success)
            {
                var prefix = match.Groups["prefix"]?.Value;
                var major = match.Groups["major"]?.Value;
                var minor = match.Groups["minor"]?.Value;
                var patch = match.Groups["patch"]?.Value;
                var preReleaseTag = match.Groups["tag"]?.Value;

                if ((prefix == versioningConfig.DevTagPrefix) || (prefix == versioningConfig.ReleaseTagPrefix))
                {
                    var versionInfo = new VersionInfo()
                    {
                        BasedOnTag = tag
                    };


                    if (prefix == versioningConfig.DevTagPrefix)
                    {
                        versionInfo.IsBasedOnDevMark = true;
                    }

                    if (int.TryParse(major, out int majorNum))
                    {
                        versionInfo.Major = majorNum;
                    }
                    if ((minor != null) && int.TryParse(minor, out int minorNum))
                    {
                        versionInfo.Minor = minorNum;
                    }
                    if ((patch != null) && int.TryParse(patch, out int patchNum))
                    {
                        versionInfo.Patch = patchNum;
                    }
                    if (!string.IsNullOrEmpty(preReleaseTag))
                    {
                        versionInfo.PreReleaseTag = preReleaseTag;
                    }

                    return versionInfo;
                }
            }

            return null;
        }
    }
}
