using System.Text;

namespace AWZhome.GutenTag
{
    public static class VersionInfoFormatterExtensions
    {
        public static string AsString(this VersionInfo version)
        {
            StringBuilder sb = new();
            sb.Append(version.Major);
            sb.Append('.');
            sb.Append(version.Minor);
            if (version.Patch != 0)
            {
                sb.Append('.');
                sb.Append(version.Patch);
            }
            if (version.PreReleaseTag != null)
            {
                sb.Append('-');
                sb.Append(version.PreReleaseTag);
                if (version.BuildNumber != 0)
                {
                    sb.Append('.');
                    sb.Append(version.BuildNumber);
                }
            }

            return sb.ToString();
        }

        public static string AsNumericVersion(this VersionInfo version)
        {
            return $"{version.Major}.{version.Minor}.{version.Patch}.{version.Revision}";
        }
    }
}