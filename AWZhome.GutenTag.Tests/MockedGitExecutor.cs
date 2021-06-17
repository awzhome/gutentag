using System.Collections.Generic;
using System.Linq;

namespace AWZhome.GutenTag.Tests
{
    public class MockedGitExecutor
    {
        public const string HasCleanWorkingCopyCmdLine = "status --short";
        public const string CurrentBranchCmdLine = "rev-parse --abbrev-ref HEAD";

        public string SimpleDescribeCmdLine => $"describe --first-parent --match \"{devTagPrefix}*\" --match \"{releaseTagPrefix}*\"";

        public string DescribeOnlyPatchMatchesCmdLine =>
           "describe --first-parent " + string.Join(' ', new[] {
               $"{devTagPrefix}[0-9999]", $"{releaseTagPrefix}[0-9999]", $"{devTagPrefix}[0-9999].[0-9999]", $"{releaseTagPrefix}[0-9999].[0-9999]", $"{devTagPrefix}[0-9999].[0-9999].[0-9999]", $"{releaseTagPrefix}[0-9999].[0-9999].[0-9999]"
           }.Select(m => $"--match \"{m}\""));

        public string DescribeOnlyMinorMatchesCmdLine =>
           "describe --first-parent " + string.Join(' ', new[] {
               $"{devTagPrefix}[0-9999]", $"{releaseTagPrefix}[0-9999]", $"{devTagPrefix}[0-9999].[0-9999]", $"{releaseTagPrefix}[0-9999].[0-9999]", $"{devTagPrefix}[0-9999].[0-9999].0", $"{releaseTagPrefix}[0-9999].[0-9999].0"
           }.Select(m => $"--match \"{m}\""));

        public Dictionary<string, IEnumerable<string>> MockedOutput { get; } = new();

        private readonly string releaseTagPrefix = "v";
        private readonly string devTagPrefix = "dev-";

        public MockedGitExecutor(VersioningConfig versioningConfig)
        {
            releaseTagPrefix = versioningConfig.ReleaseTagPrefix;
            devTagPrefix = versioningConfig.DevTagPrefix;
        }

        public bool HasCleanWorkingCopy
        {
            set => MockedOutput[HasCleanWorkingCopyCmdLine] = value ? Enumerable.Empty<string>() : new[] { "file", "anotherfile" };
        }

        public string CurrentBranch
        {
            set => MockedOutput[CurrentBranchCmdLine] = new[] { value };
        }

        public string ResultOnSimpleDescribe
        {
            set => MockedOutput[SimpleDescribeCmdLine] = new[] { value };
        }

        public string ResultOnDescribeOnlyPatchMatches
        {
            set => MockedOutput[DescribeOnlyPatchMatchesCmdLine] = new[] { value };
        }

        public (string Excluding, string Result) ResultOnDescribeOnlyPatchMatchesWithExclude
        {
            set => MockedOutput[DescribeOnlyPatchMatchesCmdLine + $" --exclude \"{value.Excluding}\""] = new[] { value.Result };
        }

        public string ResultOnDescribeOnlyMinorMatches
        {
            set => MockedOutput[DescribeOnlyMinorMatchesCmdLine] = new[] { value };
        }

        public (string Excluding, string Result) ResultOnDescribeOnlyMinorMatchesWithExclude
        {
            set => MockedOutput[DescribeOnlyMinorMatchesCmdLine + $" --exclude \"{value.Excluding}\""] = new[] { value.Result };
        }

        public IEnumerable<string> Execute(string commandLine) =>
            MockedOutput.GetValueOrDefault(commandLine.Trim());

        public static implicit operator GitExecutor(MockedGitExecutor mockedGitExcecutor)
        {
            return mockedGitExcecutor.Execute;
        }
    }
}
