using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace AWZhome.GutenTag
{
    public class BuildVersionWriter
    {
        public static UTF8Encoding UTF8WithoutBOM => new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

        private readonly BuildVersion version;

        public BuildVersionWriter(BuildVersion version)
        {
            this.version = version;
        }

        public void WriteToVsProject(params string[] files) => 
            WriteToVsProject((IEnumerable<string>) files);

        public void WriteToVsProject(IEnumerable<string> files)
        {
            WriteVersionToFiles("<Version>$$$</Version>", version.AsString(), files);
            WriteVersionToFiles("<AssemblyVersion>$$$</AssemblyVersion>", version.AsAssemblyVersion(), files);
            WriteVersionToFiles("<FileVersion>$$$</FileVersion>", version.AsAssemblyVersion(), files);
        }

        public void WriteToInnoSetupScript(params string[] files) => 
            WriteToInnoSetupScript((IEnumerable<string>) files);

        public void WriteToInnoSetupScript(IEnumerable<string> files)
        {
            WriteVersionToFiles("#define APP_VERSION \"$$$\"", version.AsAssemblyVersion(), files);
            WriteVersionToFiles("#define APP_FULL_VERSION \"$$$\"", version.AsString(), files);
        }

        public static void WriteVersionToFiles(string template, string version, params string[] files) => 
            WriteVersionToFiles(template, version, (IEnumerable<string>) files);

        public static void WriteVersionToFiles(string template, string version, IEnumerable<string> files)
        {
            foreach (var file in files)
            {
                string origText = File.ReadAllText(file, Encoding.UTF8);
                string newText = Regex.Replace(origText,
                    template.Replace("$$$", "(.*)"),
                    template.Replace("$$$", version));
                File.WriteAllText(file, newText, UTF8WithoutBOM);
            }
        }
    }
}
