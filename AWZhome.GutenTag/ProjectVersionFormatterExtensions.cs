using System.Text;

namespace AWZhome.GutenTag
{
    public static class ProjectVersionFormatterExtensions
    {
        public static string AsString(this ProjectVersion version)
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
            if (version.Tag != null)
            {
                sb.Append('-');
                sb.Append(version.Tag);
                if (version.Revision != 0)
                {
                    sb.Append('.');
                    sb.Append(version.Revision);
                }
            }

            return sb.ToString();
        }

        public static string AsAssemblyVersion(this ProjectVersion version)
        {
            return $"{version.Major}.{version.Minor}.{version.Patch}.{version.AssemblyRevision}";
        }
    }
}