using Xunit;

namespace AWZhome.GutenTag.Tests
{
    public class VersioningTests
    {
        [Fact]
        public void MajorRelease()
        {
            VersioningConfig versioningConfig = new();
            static BranchVersioning branchConfig(string _) => new() { IncrementPatch = true };
            MockedGitExecutor git = new(versioningConfig)
            {
                CurrentBranch = "main",
                HasCleanWorkingCopy = true,
                ResultOnSimpleDescribe = "v2.0",
                ResultOnDescribeOnlyPatchMatches = "v2.0"
            };
            Versioning versioning = new(versioningConfig, branchConfig, git);
            var version = versioning.GetProjectVersion();

            Assert.Equal("v2.0", version.BasedOnGitTag);
            Assert.Equal(2, version.Major);
            Assert.Equal(0, version.Minor);
            Assert.Equal(0, version.Patch);
            Assert.Equal(0, version.PreReleaseNumber);
            Assert.Equal(0, version.Revision);
            Assert.Null(version.PreReleaseTag);
            Assert.False(version.IsDevMark);
        }

        [Fact]
        public void MajorRelease_DifferentPrefix()
        {
            VersioningConfig versioningConfig = new() { ReleaseTagPrefix = "ver-" };
            static BranchVersioning branchConfig(string _) => new() { IncrementPatch = true };
            MockedGitExecutor git = new(versioningConfig)
            {
                CurrentBranch = "main",
                HasCleanWorkingCopy = true,
                ResultOnSimpleDescribe = "ver-2.0",
                ResultOnDescribeOnlyPatchMatches = "ver-2.0"
            };
            Versioning versioning = new(versioningConfig, branchConfig, git);
            var version = versioning.GetProjectVersion();

            Assert.Equal("ver-2.0", version.BasedOnGitTag);
            Assert.Equal(2, version.Major);
            Assert.Equal(0, version.Minor);
            Assert.Equal(0, version.Patch);
            Assert.Equal(0, version.PreReleaseNumber);
            Assert.Equal(0, version.Revision);
            Assert.Null(version.PreReleaseTag);
            Assert.False(version.IsDevMark);
        }

        [Fact]
        public void PatchRelease()
        {
            VersioningConfig versioningConfig = new();
            static BranchVersioning branchConfig(string _) => new() { IncrementPatch = true };
            MockedGitExecutor git = new(versioningConfig)
            {
                CurrentBranch = "main",
                HasCleanWorkingCopy = true,
                ResultOnSimpleDescribe = "v2.0.1",
                ResultOnDescribeOnlyPatchMatches = "v2.0.1"
            };
            Versioning versioning = new(versioningConfig, branchConfig, git);
            var version = versioning.GetProjectVersion();

            Assert.Equal("v2.0.1", version.BasedOnGitTag);
            Assert.Equal(2, version.Major);
            Assert.Equal(0, version.Minor);
            Assert.Equal(1, version.Patch);
            Assert.Equal(0, version.PreReleaseNumber);
            Assert.Equal(0, version.Revision);
            Assert.Null(version.PreReleaseTag);
            Assert.False(version.IsDevMark);
        }

        [Fact]
        public void MajorRelease_MinorIncrement_DirtyWorkingCopy()
        {
            VersioningConfig versioningConfig = new();
            static BranchVersioning branchConfig(string _) => new() { IncrementPatch = false };
            MockedGitExecutor git = new(versioningConfig)
            {
                CurrentBranch = "main",
                HasCleanWorkingCopy = false,
                ResultOnSimpleDescribe = "v2.0",
                ResultOnDescribeOnlyMinorMatches = "v2.0"
            };
            Versioning versioning = new(versioningConfig, branchConfig, git);
            var version = versioning.GetProjectVersion();

            Assert.Equal("v2.0", version.BasedOnGitTag);
            Assert.Equal(2, version.Major);
            Assert.Equal(1, version.Minor);
            Assert.Equal(0, version.Patch);
            Assert.Equal(1, version.PreReleaseNumber);
            Assert.Equal(1, version.Revision);
            Assert.Equal("main", version.PreReleaseTag);
            Assert.False(version.IsDevMark);
        }

        [Fact]
        public void MajorRelease_PatchIncrement_DirtyWorkingCopy()
        {
            VersioningConfig versioningConfig = new();
            static BranchVersioning branchConfig(string _) => new() { IncrementPatch = true };
            MockedGitExecutor git = new(versioningConfig)
            {
                CurrentBranch = "main",
                HasCleanWorkingCopy = false,
                ResultOnSimpleDescribe = "v2.0",
                ResultOnDescribeOnlyPatchMatches = "v2.0"
            };
            Versioning versioning = new(versioningConfig, branchConfig, git);
            var version = versioning.GetProjectVersion();

            Assert.Equal("v2.0", version.BasedOnGitTag);
            Assert.Equal(2, version.Major);
            Assert.Equal(0, version.Minor);
            Assert.Equal(1, version.Patch);
            Assert.Equal(1, version.PreReleaseNumber);
            Assert.Equal(1, version.Revision);
            Assert.Equal("main", version.PreReleaseTag);
            Assert.False(version.IsDevMark);
        }

        [Fact]
        public void CommitAfterMajorRelease_MinorIncrement_CleanWorkingCopy()
        {
            VersioningConfig versioningConfig = new();
            static BranchVersioning branchConfig(string _) => new() { IncrementPatch = false };
            MockedGitExecutor git = new(versioningConfig)
            {
                CurrentBranch = "main",
                HasCleanWorkingCopy = true,
                ResultOnSimpleDescribe = "v2.0-1-abcdef",
                ResultOnDescribeOnlyMinorMatches = "v2.0-1-abcdef"
            };
            Versioning versioning = new(versioningConfig, branchConfig, git);
            var version = versioning.GetProjectVersion();

            Assert.Equal("v2.0", version.BasedOnGitTag);
            Assert.Equal(2, version.Major);
            Assert.Equal(1, version.Minor);
            Assert.Equal(0, version.Patch);
            Assert.Equal(1, version.PreReleaseNumber);
            Assert.Equal(1, version.Revision);
            Assert.Equal("main", version.PreReleaseTag);
            Assert.False(version.IsDevMark);
        }

        [Fact]
        public void CommitAfterMajorRelease_MinorIncrement_DirtyWorkingCopy()
        {
            VersioningConfig versioningConfig = new();
            static BranchVersioning branchConfig(string _) => new() { IncrementPatch = false };
            MockedGitExecutor git = new(versioningConfig)
            {
                CurrentBranch = "main",
                HasCleanWorkingCopy = false,
                ResultOnSimpleDescribe = "v2.0-1-abcdef",
                ResultOnDescribeOnlyMinorMatches = "v2.0-1-abcdef"
            };
            Versioning versioning = new(versioningConfig, branchConfig, git);
            var version = versioning.GetProjectVersion();

            Assert.Equal("v2.0", version.BasedOnGitTag);
            Assert.Equal(2, version.Major);
            Assert.Equal(1, version.Minor);
            Assert.Equal(0, version.Patch);
            Assert.Equal(2, version.PreReleaseNumber);
            Assert.Equal(2, version.Revision);
            Assert.Equal("main", version.PreReleaseTag);
            Assert.False(version.IsDevMark);
        }

        [Fact]
        public void CommitAfterMajorRelease_PatchIncrement_CleanWorkingCopy()
        {
            VersioningConfig versioningConfig = new();
            static BranchVersioning branchConfig(string _) => new() { IncrementPatch = true };
            MockedGitExecutor git = new(versioningConfig)
            {
                CurrentBranch = "main",
                HasCleanWorkingCopy = true,
                ResultOnSimpleDescribe = "v2.0-1-abcdef",
                ResultOnDescribeOnlyPatchMatches = "v2.0-1-abcdef"
            };
            Versioning versioning = new(versioningConfig, branchConfig, git);
            var version = versioning.GetProjectVersion();

            Assert.Equal("v2.0", version.BasedOnGitTag);
            Assert.Equal(2, version.Major);
            Assert.Equal(0, version.Minor);
            Assert.Equal(1, version.Patch);
            Assert.Equal(1, version.PreReleaseNumber);
            Assert.Equal(1, version.Revision);
            Assert.Equal("main", version.PreReleaseTag);
            Assert.False(version.IsDevMark);
        }

        [Fact]
        public void CommitOnDevMark()
        {
            VersioningConfig versioningConfig = new();
            static BranchVersioning branchConfig(string _) => new() { IncrementPatch = true };
            MockedGitExecutor git = new(versioningConfig)
            {
                CurrentBranch = "main",
                HasCleanWorkingCopy = true,
                ResultOnSimpleDescribe = "dev-2.0",
                ResultOnDescribeOnlyPatchMatches = "dev-2.0"
            };
            Versioning versioning = new(versioningConfig, branchConfig, git);
            var version = versioning.GetProjectVersion();

            Assert.Equal("dev-2.0", version.BasedOnGitTag);
            Assert.Equal(2, version.Major);
            Assert.Equal(0, version.Minor);
            Assert.Equal(0, version.Patch);
            Assert.Equal(1, version.PreReleaseNumber);
            Assert.Equal(1, version.Revision);
            Assert.Equal("main", version.PreReleaseTag);
            Assert.True(version.IsDevMark);
        }

        [Fact]
        public void CommitAfterDevMark()
        {
            VersioningConfig versioningConfig = new();
            static BranchVersioning branchConfig(string _) => new() { IncrementPatch = true };
            MockedGitExecutor git = new(versioningConfig)
            {
                CurrentBranch = "main",
                HasCleanWorkingCopy = true,
                ResultOnSimpleDescribe = "dev-2.0-1-abcdef",
                ResultOnDescribeOnlyPatchMatches = "dev-2.0-1-abcdef"
            };
            Versioning versioning = new(versioningConfig, branchConfig, git);
            var version = versioning.GetProjectVersion();

            Assert.Equal("dev-2.0", version.BasedOnGitTag);
            Assert.Equal(2, version.Major);
            Assert.Equal(0, version.Minor);
            Assert.Equal(0, version.Patch);
            Assert.Equal(2, version.PreReleaseNumber);
            Assert.Equal(2, version.Revision);
            Assert.Equal("main", version.PreReleaseTag);
            Assert.True(version.IsDevMark);
        }

        [Fact]
        public void MajorReleaseAfterDevMarkForSameVersion()
        {
            VersioningConfig versioningConfig = new();
            static BranchVersioning branchConfig(string _) => new() { IncrementPatch = true };
            MockedGitExecutor git = new(versioningConfig)
            {
                CurrentBranch = "main",
                HasCleanWorkingCopy = true,
                ResultOnSimpleDescribe = "v2.0",
                ResultOnDescribeOnlyPatchMatches = "v2.0",
                ResultOnDescribeOnlyPatchMatchesWithExclude = (Excluding: "v2.0", Result: "dev-2.0-5-abcdef")
            };
            Versioning versioning = new(versioningConfig, branchConfig, git);
            var version = versioning.GetProjectVersion();

            Assert.Equal("v2.0", version.BasedOnGitTag);
            Assert.Equal(2, version.Major);
            Assert.Equal(0, version.Minor);
            Assert.Equal(0, version.Patch);
            Assert.Equal(0, version.PreReleaseNumber);
            Assert.Equal(6, version.Revision);
            Assert.Null(version.PreReleaseTag);
            Assert.False(version.IsDevMark);
        }

        [Fact]
        public void MajorReleaseAfterDevMarkForSameVersion_DifferentPrefixes()
        {
            VersioningConfig versioningConfig = new() { ReleaseTagPrefix = "ver-", DevTagPrefix = "start-" };
            static BranchVersioning branchConfig(string _) => new() { IncrementPatch = true };
            MockedGitExecutor git = new(versioningConfig)
            {
                CurrentBranch = "main",
                HasCleanWorkingCopy = true,
                ResultOnSimpleDescribe = "ver-2.0",
                ResultOnDescribeOnlyPatchMatches = "ver-2.0",
                ResultOnDescribeOnlyPatchMatchesWithExclude = (Excluding: "ver-2.0", Result: "start-2.0-5-abcdef")
            };
            Versioning versioning = new(versioningConfig, branchConfig, git);
            var version = versioning.GetProjectVersion();

            Assert.Equal("ver-2.0", version.BasedOnGitTag);
            Assert.Equal(2, version.Major);
            Assert.Equal(0, version.Minor);
            Assert.Equal(0, version.Patch);
            Assert.Equal(0, version.PreReleaseNumber);
            Assert.Equal(6, version.Revision);
            Assert.Null(version.PreReleaseTag);
            Assert.False(version.IsDevMark);
        }

        [Fact]
        public void PatchReleaseAfterMajorRelease_MinorIncrement()
        {
            VersioningConfig versioningConfig = new();
            static BranchVersioning branchConfig(string _) => new() { IncrementPatch = false };
            MockedGitExecutor git = new(versioningConfig)
            {
                CurrentBranch = "main",
                HasCleanWorkingCopy = true,
                ResultOnSimpleDescribe = "v2.0.1",
                ResultOnDescribeOnlyMinorMatches = "v2.0-5-abcdef"
            };
            Versioning versioning = new(versioningConfig, branchConfig, git);
            var version = versioning.GetProjectVersion();

            Assert.Equal("v2.0.1", version.BasedOnGitTag);
            Assert.Equal(2, version.Major);
            Assert.Equal(0, version.Minor);
            Assert.Equal(1, version.Patch);
            Assert.Equal(0, version.PreReleaseNumber);
            Assert.Equal(0, version.Revision);
            Assert.Null(version.PreReleaseTag);
            Assert.False(version.IsDevMark);
        }

        [Fact]
        public void AfterPatchReleaseAfterMajorRelease_PatchIncrement()
        {
            VersioningConfig versioningConfig = new();
            static BranchVersioning branchConfig(string _) => new() { IncrementPatch = true };
            MockedGitExecutor git = new(versioningConfig)
            {
                CurrentBranch = "main",
                HasCleanWorkingCopy = true,
                ResultOnSimpleDescribe = "v2.0.1-2-abcdef",
                ResultOnDescribeOnlyPatchMatches = "v2.0.1-2-abcdef"
            };
            Versioning versioning = new(versioningConfig, branchConfig, git);
            var version = versioning.GetProjectVersion();

            Assert.Equal("v2.0.1", version.BasedOnGitTag);
            Assert.Equal(2, version.Major);
            Assert.Equal(0, version.Minor);
            Assert.Equal(2, version.Patch);
            Assert.Equal(2, version.PreReleaseNumber);
            Assert.Equal(2, version.Revision);
            Assert.Equal("main", version.PreReleaseTag);
            Assert.False(version.IsDevMark);
        }

        [Fact]
        public void AfterPatchReleaseAfterMajorRelease_MinorIncrement()
        {
            VersioningConfig versioningConfig = new();
            static BranchVersioning branchConfig(string _) => new() { IncrementPatch = false };
            MockedGitExecutor git = new(versioningConfig)
            {
                CurrentBranch = "main",
                HasCleanWorkingCopy = true,
                ResultOnSimpleDescribe = "v2.0.1-2-abcdef",
                ResultOnDescribeOnlyMinorMatches = "v2.0-5-abcdef"
            };
            Versioning versioning = new(versioningConfig, branchConfig, git);
            var version = versioning.GetProjectVersion();

            Assert.Equal("v2.0", version.BasedOnGitTag);
            Assert.Equal(2, version.Major);
            Assert.Equal(1, version.Minor);
            Assert.Equal(0, version.Patch);
            Assert.Equal(5, version.PreReleaseNumber);
            Assert.Equal(5, version.Revision);
            Assert.Equal("main", version.PreReleaseTag);
            Assert.False(version.IsDevMark);
        }
    }
}
