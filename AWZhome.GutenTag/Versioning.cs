using System.Collections.Generic;
using System.Linq;

namespace AWZhome.GutenTag
{
    public class VersioningConfig
    {
        public string ReleaseTagPrefix { get; init; } = "v";
        public string DevTagPrefix { get; init; } = "dev-";
    }

    public class BranchVersioning
    {
        public string Tag { get; set; }
        public bool IncrementPatch { get; set; } = true;
    }

    public delegate BranchVersioning BranchSpecificConfig(string branchName);

    public delegate IEnumerable<string> GitExecutor(string commandLine);

    public class Versioning
    {
        private readonly VersioningConfig versioningConfig;
        private readonly BranchSpecificConfig branchConfig;
        private readonly GitExecutor gitExecutor;

        public Versioning(VersioningConfig versioningConfig, BranchSpecificConfig branchConfig, GitExecutor gitExecutor)
        {
            this.versioningConfig = versioningConfig;
            this.branchConfig = branchConfig;
            this.gitExecutor = gitExecutor;
        }

        public ProjectVersion GetProjectVersion()
        {
            var releaseTagPrefix = versioningConfig.ReleaseTagPrefix;
            var devTagPrefix = versioningConfig.DevTagPrefix;

            string currentBranch = GitCurrentBranch();
            var branchVersioning = branchConfig(currentBranch) ?? new();
            var currentVersion = GitDescribe(new[] { $"{devTagPrefix}*", $"{releaseTagPrefix}*" });

            var correctedMatchPatterns = branchVersioning.IncrementPatch ?
                    new[] { $"{devTagPrefix}[0-9999]", $"{releaseTagPrefix}[0-9999]", $"{devTagPrefix}[0-9999].[0-9999]", $"{releaseTagPrefix}[0-9999].[0-9999]", $"{devTagPrefix}[0-9999].[0-9999].[0-9999]", $"{releaseTagPrefix}[0-9999].[0-9999].[0-9999]" } :
                    new[] { $"{devTagPrefix}[0-9999]", $"{releaseTagPrefix}[0-9999]", $"{devTagPrefix}[0-9999].[0-9999]", $"{releaseTagPrefix}[0-9999].[0-9999]", $"{devTagPrefix}[0-9999].[0-9999].0", $"{releaseTagPrefix}[0-9999].[0-9999].0" };
            var correctedVersion = GitDescribe(correctedMatchPatterns);

            if (currentVersion.IsBasedOnDevMark)
            {
                currentVersion.BuildNumber++;
                currentVersion.Revision++;
            }
            if (correctedVersion.IsBasedOnDevMark)
            {
                correctedVersion.BuildNumber++;
                correctedVersion.Revision++;
            }

            if (!GitHasCleanWorkingCopy())
            {
                currentVersion.BuildNumber++;
                currentVersion.Revision++;
                correctedVersion.BuildNumber++;
                correctedVersion.Revision++;
            }

            if (currentVersion.BuildNumber != 0)
            {
                currentVersion = correctedVersion;

                currentVersion.PreReleaseTag ??= branchVersioning.Tag ?? PreReleaseNormalizer.FromGitBranch(currentBranch);

                if (!currentVersion.IsBasedOnDevMark)
                {
                    if (branchVersioning.IncrementPatch)
                    {
                        currentVersion.Patch++;
                    }
                    else
                    {
                        currentVersion.Minor++;
                    }
                }
            }
            else
            {
                var versionWithoutCurrentTag = GitDescribe(correctedMatchPatterns,
                    (currentVersion.BasedOnGitTag != null) ? new[] { currentVersion.BasedOnGitTag } : null);

                if (versionWithoutCurrentTag.Major == currentVersion.Major
                    && versionWithoutCurrentTag.Minor == currentVersion.Minor
                    && versionWithoutCurrentTag.Patch == currentVersion.Patch)
                {
                    currentVersion.Revision = versionWithoutCurrentTag.BuildNumber + 1;
                }

            }

            return currentVersion;
        }

        private bool GitHasCleanWorkingCopy()
        {
            return !gitExecutor?.Invoke("status --short")?.Any() ?? false;
        }

        private string GitCurrentBranch()
        {
            return gitExecutor?.Invoke("rev-parse --abbrev-ref HEAD")?.SingleOrDefault();
        }

        private ProjectVersion GitDescribe(string[] matches = null, string[] excludes = null)
        {
            string matchParam = "";
            if (matches != null)
            {
                matchParam += string.Join(' ', matches.Select(m => $"--match \"{m}\""));
            }
            string excludeParam = "";
            if (excludes != null)
            {
                excludeParam += string.Join(' ', excludes.Select(e => $"--exclude \"{e}\""));
            }
            return GitTagParser.Parse(gitExecutor?.Invoke($"describe --tags --first-parent {matchParam} {excludeParam}")?.FirstOrDefault(), versioningConfig);
        }
    }
}