using System.Text.RegularExpressions;

namespace AWZhome.GutenTag
{
    static class PreReleaseNormalizer
    {
        private static readonly Regex invalidPreReleaseCharsPattern = new("[^\\p{L}0-9]", RegexOptions.Compiled);

        public static string FromGitBranch(string branch)
        {
            return invalidPreReleaseCharsPattern.Replace(branch, ".");
        }
    }
}
