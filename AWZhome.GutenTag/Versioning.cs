﻿using System.Collections.Generic;
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
            string currentBranch = GitCurrentBranch();
            var branchVersioning = branchConfig(currentBranch) ?? new();
            var currentVersion = GitDescribe(new[] { $"{versioningConfig.DevTagPrefix}*", $"{versioningConfig.ReleaseTagPrefix}*" });

            var correctedMatchPatterns = branchVersioning.IncrementPatch ?
                    new[] { "dev-[0-9999]", "v[0-9999]", "dev-[0-9999].[0-9999]", "v[0-9999].[0-9999]", "dev-[0-9999].[0-9999].[0-9999]", "v[0-9999].[0-9999].[0-9999]", } :
                    new[] { "dev-[0-9999]", "v[0-9999]", "dev-[0-9999].[0-9999]", "v[0-9999].[0-9999]", "dev-[0-9999].[0-9999].0", "v[0-9999].[0-9999].0" };
            var correctedVersion = GitDescribe(correctedMatchPatterns);

            if (currentVersion.IsDevMark)
            {
                currentVersion.PreReleaseNumber++;
                currentVersion.Revision++;
                correctedVersion.PreReleaseNumber++;
                correctedVersion.Revision++;
            }

            if (!GitHasCleanWorkingCopy())
            {
                currentVersion.PreReleaseNumber++;
                currentVersion.Revision++;
                correctedVersion.PreReleaseNumber++;
                correctedVersion.Revision++;
            }

            if (currentVersion.PreReleaseNumber != 0)
            {
                currentVersion = correctedVersion;

                currentVersion.PreReleaseTag ??= branchVersioning.Tag ?? currentBranch;

                if (!currentVersion.IsDevMark)
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
                    currentVersion.Revision = versionWithoutCurrentTag.PreReleaseNumber + 1;
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
            return GitTagParser.Parse(gitExecutor?.Invoke($"describe --first-parent {matchParam} {excludeParam}")?.FirstOrDefault(), versioningConfig);
        }
    }
}