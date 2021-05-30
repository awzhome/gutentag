namespace AWZhome.GutenTag
{
    static class GitTagParser
    {
        public static BuildVersion Parse(string gitTag, VersioningConfig versioningConfig)
        {
            if (gitTag == null || gitTag.StartsWith("fatal"))
            {
                return BuildVersion.DefaultInitial;
            }

            BuildVersion resultVersion = new();

            /*
             * git describe tag formats:
             * 
             * v2.0 (simple tag)
             * v2.0-sometag (tag with "tag" in semver understanding)
             * v2.0-sometag-30-g41e086cb (tag with semver-tag and number of commits after the tag)
             */
            var parts = gitTag.Split('-');
            if (parts.Length > 0)
            {
                resultVersion.BasedOnGitTag = parts[0];
            }

            if (parts.Length > 1 && parts[0] == "dev")
            {
                resultVersion.IsDevMark = true;
                parts = parts[1..];
                resultVersion.BasedOnGitTag += "-" + parts[0];
            }

            if (parts.Length > 0)
            {
                var baseVersionParts = parts[0].Split('.');
                if (baseVersionParts.Length > 0)
                {
                    string major = baseVersionParts[0];
                    if (major.StartsWith(versioningConfig.ReleaseTagPrefix))
                    {
                        major = major[versioningConfig.ReleaseTagPrefix.Length..];
                    }
                    if (int.TryParse(major, out int majorNum))
                    {
                        resultVersion.Major = majorNum;
                    }
                }
                if (baseVersionParts.Length > 1)
                {
                    if (int.TryParse(baseVersionParts[1], out int minorNum))
                    {
                        resultVersion.Minor = minorNum;
                    }
                }
                if (baseVersionParts.Length > 2)
                {
                    if (int.TryParse(baseVersionParts[2], out int patchNum))
                    {
                        resultVersion.Patch = patchNum;
                    }
                }
            }
            if (parts.Length == 2 || parts.Length == 4)
            {
                resultVersion.Tag = parts[1];
                resultVersion.BasedOnGitTag += "-" + parts[1];
            }
            if (parts.Length > 2)
            {
                if (int.TryParse(parts[^2], out int revisionNum))
                {
                    resultVersion.Revision = revisionNum;
                }
            }

            resultVersion.AssemblyRevision = resultVersion.Revision;

            return resultVersion;
        }
    }
}