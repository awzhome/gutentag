using Xunit;

namespace AWZhome.GutenTag.Tests
{
    public class VersioningTests
    {
        [Fact]
        public void MajorRelease()
        {
            VersioningConfig versioningConfig = new();
            static BranchVersioning branchConfig(string _) => new() { IncrementedPart = IncrementedPart.Patch };
            var git = new TestVcsAdapter(versioningConfig)
            {
                CurrentBranch = "main",
                IsCleanWorkingCopy = true,
                ChronologicAncestorTags = new[] { ("v2.0", 0), ("v1.0", 10) },
            };
            Versioning versioning = new(branchConfig, git);
            var version = versioning.GetVersionInfo();

            Assert.Equal("v2.0", version.BasedOnTag);
            Assert.Equal(2, version.Major);
            Assert.Equal(0, version.Minor);
            Assert.Equal(0, version.Patch);
            Assert.Equal(0, version.BuildNumber);
            Assert.Equal(0, version.Revision);
            Assert.Null(version.PreReleaseTag);
            Assert.False(version.IsBasedOnDevMark);
            Assert.Equal(VersionType.Release, version.Type);
        }

        [Fact]
        public void MajorRelease_DifferentPrefix()
        {
            VersioningConfig versioningConfig = new() { ReleaseTagPrefix = "ver-" };
            static BranchVersioning branchConfig(string _) => new() { IncrementedPart = IncrementedPart.Patch };
            var git = new TestVcsAdapter(versioningConfig)
            {
                CurrentBranch = "main",
                IsCleanWorkingCopy = true,
                ChronologicAncestorTags = new[] { ("ver-2.0", 0), ("ver-1.0", 10) },
            };
            Versioning versioning = new(branchConfig, git);
            var version = versioning.GetVersionInfo();

            Assert.Equal("ver-2.0", version.BasedOnTag);
            Assert.Equal(2, version.Major);
            Assert.Equal(0, version.Minor);
            Assert.Equal(0, version.Patch);
            Assert.Equal(0, version.BuildNumber);
            Assert.Equal(0, version.Revision);
            Assert.Null(version.PreReleaseTag);
            Assert.False(version.IsBasedOnDevMark);
            Assert.Equal(VersionType.Release, version.Type);
        }

        [Fact]
        public void PatchRelease()
        {
            VersioningConfig versioningConfig = new();
            static BranchVersioning branchConfig(string _) => new() { IncrementedPart = IncrementedPart.Patch };
            var git = new TestVcsAdapter(versioningConfig)
            {
                CurrentBranch = "main",
                IsCleanWorkingCopy = true,
                ChronologicAncestorTags = new[] { ("v2.0.1", 0), ("v2.0", 10) },
            };
            Versioning versioning = new(branchConfig, git);
            var version = versioning.GetVersionInfo();

            Assert.Equal("v2.0.1", version.BasedOnTag);
            Assert.Equal(2, version.Major);
            Assert.Equal(0, version.Minor);
            Assert.Equal(1, version.Patch);
            Assert.Equal(0, version.BuildNumber);
            Assert.Equal(0, version.Revision);
            Assert.Null(version.PreReleaseTag);
            Assert.False(version.IsBasedOnDevMark);
            Assert.Equal(VersionType.Release, version.Type);
        }

        [Fact]
        public void MajorRelease_MinorIncrement_DirtyWorkingCopy()
        {
            VersioningConfig versioningConfig = new();
            static BranchVersioning branchConfig(string _) => new() { IncrementedPart = IncrementedPart.Minor };
            var git = new TestVcsAdapter(versioningConfig)
            {
                CurrentBranch = "main",
                IsCleanWorkingCopy = false,
                ChronologicAncestorTags = new[] { ("v2.0", 0), ("v1.0", 10) },
            };
            Versioning versioning = new(branchConfig, git);
            var version = versioning.GetVersionInfo();

            Assert.Equal("v2.0", version.BasedOnTag);
            Assert.Equal(2, version.Major);
            Assert.Equal(1, version.Minor);
            Assert.Equal(0, version.Patch);
            Assert.Equal(1, version.BuildNumber);
            Assert.Equal(1, version.Revision);
            Assert.Equal("main", version.PreReleaseTag);
            Assert.False(version.IsBasedOnDevMark);
            Assert.Equal(VersionType.CIBuild, version.Type);
        }

        [Fact]
        public void MajorRelease_PatchIncrement_DirtyWorkingCopy()
        {
            VersioningConfig versioningConfig = new();
            static BranchVersioning branchConfig(string _) => new() { IncrementedPart = IncrementedPart.Patch };
            var git = new TestVcsAdapter(versioningConfig)
            {
                CurrentBranch = "main",
                IsCleanWorkingCopy = false,
                ChronologicAncestorTags = new[] { ("v2.0", 0), ("v1.0", 10) },
            };
            Versioning versioning = new(branchConfig, git);
            var version = versioning.GetVersionInfo();

            Assert.Equal("v2.0", version.BasedOnTag);
            Assert.Equal(2, version.Major);
            Assert.Equal(0, version.Minor);
            Assert.Equal(1, version.Patch);
            Assert.Equal(1, version.BuildNumber);
            Assert.Equal(1, version.Revision);
            Assert.Equal("main", version.PreReleaseTag);
            Assert.False(version.IsBasedOnDevMark);
            Assert.Equal(VersionType.CIBuild, version.Type);
        }

        [Fact]
        public void CommitAfterMajorRelease_MinorIncrement_CleanWorkingCopy()
        {
            VersioningConfig versioningConfig = new();
            static BranchVersioning branchConfig(string _) => new() { IncrementedPart = IncrementedPart.Minor };
            var git = new TestVcsAdapter(versioningConfig)
            {
                CurrentBranch = "main",
                IsCleanWorkingCopy = true,
                ChronologicAncestorTags = new[] { ("v2.0", 1), ("v1.0", 10) },
            };
            Versioning versioning = new(branchConfig, git);
            var version = versioning.GetVersionInfo();

            Assert.Equal("v2.0", version.BasedOnTag);
            Assert.Equal(2, version.Major);
            Assert.Equal(1, version.Minor);
            Assert.Equal(0, version.Patch);
            Assert.Equal(1, version.BuildNumber);
            Assert.Equal(1, version.Revision);
            Assert.Equal("main", version.PreReleaseTag);
            Assert.False(version.IsBasedOnDevMark);
            Assert.Equal(VersionType.CIBuild, version.Type);
        }

        [Fact]
        public void CommitAfterMajorRelease_MinorIncrement_DirtyWorkingCopy()
        {
            VersioningConfig versioningConfig = new();
            static BranchVersioning branchConfig(string _) => new() { IncrementedPart = IncrementedPart.Minor };
            var git = new TestVcsAdapter(versioningConfig)
            {
                CurrentBranch = "main",
                IsCleanWorkingCopy = false,
                ChronologicAncestorTags = new[] { ("v2.0", 1), ("v1.0", 10) },
            };
            Versioning versioning = new(branchConfig, git);
            var version = versioning.GetVersionInfo();

            Assert.Equal("v2.0", version.BasedOnTag);
            Assert.Equal(2, version.Major);
            Assert.Equal(1, version.Minor);
            Assert.Equal(0, version.Patch);
            Assert.Equal(2, version.BuildNumber);
            Assert.Equal(2, version.Revision);
            Assert.Equal("main", version.PreReleaseTag);
            Assert.False(version.IsBasedOnDevMark);
            Assert.Equal(VersionType.CIBuild, version.Type);
        }

        [Fact]
        public void CommitAfterMajorRelease_PatchIncrement_CleanWorkingCopy()
        {
            VersioningConfig versioningConfig = new();
            static BranchVersioning branchConfig(string _) => new() { IncrementedPart = IncrementedPart.Patch };
            var git = new TestVcsAdapter(versioningConfig)
            {
                CurrentBranch = "main",
                IsCleanWorkingCopy = true,
                ChronologicAncestorTags = new[] { ("v2.0", 1), ("v1.0", 10) },
            };
            Versioning versioning = new(branchConfig, git);
            var version = versioning.GetVersionInfo();

            Assert.Equal("v2.0", version.BasedOnTag);
            Assert.Equal(2, version.Major);
            Assert.Equal(0, version.Minor);
            Assert.Equal(1, version.Patch);
            Assert.Equal(1, version.BuildNumber);
            Assert.Equal(1, version.Revision);
            Assert.Equal("main", version.PreReleaseTag);
            Assert.False(version.IsBasedOnDevMark);
            Assert.Equal(VersionType.CIBuild, version.Type);
        }

        [Fact]
        public void CommitOnDevMark()
        {
            VersioningConfig versioningConfig = new();
            static BranchVersioning branchConfig(string _) => new() { IncrementedPart = IncrementedPart.Patch };
            var git = new TestVcsAdapter(versioningConfig)
            {
                CurrentBranch = "main",
                IsCleanWorkingCopy = true,
                ChronologicAncestorTags = new[] { ("dev-2.0", 0), ("v1.0", 10) },
            };
            Versioning versioning = new(branchConfig, git);
            var version = versioning.GetVersionInfo();

            Assert.Equal("dev-2.0", version.BasedOnTag);
            Assert.Equal(2, version.Major);
            Assert.Equal(0, version.Minor);
            Assert.Equal(0, version.Patch);
            Assert.Equal(1, version.BuildNumber);
            Assert.Equal(1, version.Revision);
            Assert.Equal("main", version.PreReleaseTag);
            Assert.True(version.IsBasedOnDevMark);
            Assert.Equal(VersionType.CIBuild, version.Type);
        }

        [Fact]
        public void CommitAfterDevMark()
        {
            VersioningConfig versioningConfig = new();
            static BranchVersioning branchConfig(string _) => new() { IncrementedPart = IncrementedPart.Patch };
            var git = new TestVcsAdapter(versioningConfig)
            {
                CurrentBranch = "main",
                IsCleanWorkingCopy = true,
                ChronologicAncestorTags = new[] { ("dev-2.0", 1), ("v1.0", 10) },

            };
            Versioning versioning = new(branchConfig, git);
            var version = versioning.GetVersionInfo();

            Assert.Equal("dev-2.0", version.BasedOnTag);
            Assert.Equal(2, version.Major);
            Assert.Equal(0, version.Minor);
            Assert.Equal(0, version.Patch);
            Assert.Equal(2, version.BuildNumber);
            Assert.Equal(2, version.Revision);
            Assert.Equal("main", version.PreReleaseTag);
            Assert.True(version.IsBasedOnDevMark);
            Assert.Equal(VersionType.CIBuild, version.Type);
        }

        [Fact]
        public void MajorReleaseAfterDevMarkForSameVersion()
        {
            VersioningConfig versioningConfig = new();
            static BranchVersioning branchConfig(string _) => new() { IncrementedPart = IncrementedPart.Patch };
            var git = new TestVcsAdapter(versioningConfig)
            {
                CurrentBranch = "main",
                IsCleanWorkingCopy = true,
                ChronologicAncestorTags = new[] { ("v2.0", 0), ("dev-2.0", 5), ("v1.0", 10) },
            };
            Versioning versioning = new(branchConfig, git);
            var version = versioning.GetVersionInfo();

            Assert.Equal("v2.0", version.BasedOnTag);
            Assert.Equal(2, version.Major);
            Assert.Equal(0, version.Minor);
            Assert.Equal(0, version.Patch);
            Assert.Equal(0, version.BuildNumber);
            Assert.Equal(6, version.Revision);
            Assert.Null(version.PreReleaseTag);
            Assert.False(version.IsBasedOnDevMark);
            Assert.Equal(VersionType.Release, version.Type);
        }

        [Fact]
        public void MajorReleaseAfterDevMarkForSameVersion_DifferentPrefixes()
        {
            VersioningConfig versioningConfig = new() { ReleaseTagPrefix = "ver-", DevTagPrefix = "start-" };
            static BranchVersioning branchConfig(string _) => new() { IncrementedPart = IncrementedPart.Patch };
            var git = new TestVcsAdapter(versioningConfig)
            {
                CurrentBranch = "main",
                IsCleanWorkingCopy = true,
                ChronologicAncestorTags = new[] { ("ver-2.0", 0), ("start-2.0", 5), ("ver-1.0", 10) },
            };
            Versioning versioning = new(branchConfig, git);
            var version = versioning.GetVersionInfo();

            Assert.Equal("ver-2.0", version.BasedOnTag);
            Assert.Equal(2, version.Major);
            Assert.Equal(0, version.Minor);
            Assert.Equal(0, version.Patch);
            Assert.Equal(0, version.BuildNumber);
            Assert.Equal(6, version.Revision);
            Assert.Null(version.PreReleaseTag);
            Assert.False(version.IsBasedOnDevMark);
            Assert.Equal(VersionType.Release, version.Type);
        }

        [Fact]
        public void PatchReleaseAfterMajorRelease_MinorIncrement()
        {
            VersioningConfig versioningConfig = new();
            static BranchVersioning branchConfig(string _) => new() { IncrementedPart = IncrementedPart.Minor };
            var git = new TestVcsAdapter(versioningConfig)
            {
                CurrentBranch = "main",
                IsCleanWorkingCopy = true,
                ChronologicAncestorTags = new[] { ("v2.0.1", 0), ("v2.0", 5), ("dev-2.0", 7), ("v1.0", 10) },
            };
            Versioning versioning = new(branchConfig, git);
            var version = versioning.GetVersionInfo();

            Assert.Equal("v2.0.1", version.BasedOnTag);
            Assert.Equal(2, version.Major);
            Assert.Equal(0, version.Minor);
            Assert.Equal(1, version.Patch);
            Assert.Equal(0, version.BuildNumber);
            Assert.Equal(0, version.Revision);
            Assert.Null(version.PreReleaseTag);
            Assert.False(version.IsBasedOnDevMark);
            Assert.Equal(VersionType.Release, version.Type);
        }

        [Fact]
        public void AfterPatchReleaseAfterMajorRelease_PatchIncrement()
        {
            VersioningConfig versioningConfig = new();
            static BranchVersioning branchConfig(string _) => new() { IncrementedPart = IncrementedPart.Patch };
            var git = new TestVcsAdapter(versioningConfig)
            {
                CurrentBranch = "main",
                IsCleanWorkingCopy = true,
                ChronologicAncestorTags = new[] { ("v2.0.1", 2), ("v2.0", 5), ("dev-2.0", 7), ("v1.0", 10) },
            };
            Versioning versioning = new(branchConfig, git);
            var version = versioning.GetVersionInfo();

            Assert.Equal("v2.0.1", version.BasedOnTag);
            Assert.Equal(2, version.Major);
            Assert.Equal(0, version.Minor);
            Assert.Equal(2, version.Patch);
            Assert.Equal(2, version.BuildNumber);
            Assert.Equal(2, version.Revision);
            Assert.Equal("main", version.PreReleaseTag);
            Assert.False(version.IsBasedOnDevMark);
            Assert.Equal(VersionType.CIBuild, version.Type);
        }

        [Fact]
        public void AfterPatchReleaseAfterMajorRelease_MinorIncrement()
        {
            VersioningConfig versioningConfig = new();
            static BranchVersioning branchConfig(string _) => new() { IncrementedPart = IncrementedPart.Minor };
            var git = new TestVcsAdapter(versioningConfig)
            {
                CurrentBranch = "main",
                IsCleanWorkingCopy = true,
                ChronologicAncestorTags = new[] { ("v2.0.1", 2), ("v2.0", 5), ("dev-2.0", 7), ("v1.0", 10) },
            };
            Versioning versioning = new(branchConfig, git);
            var version = versioning.GetVersionInfo();

            Assert.Equal("v2.0", version.BasedOnTag);
            Assert.Equal(2, version.Major);
            Assert.Equal(1, version.Minor);
            Assert.Equal(0, version.Patch);
            Assert.Equal(5, version.BuildNumber);
            Assert.Equal(5, version.Revision);
            Assert.Equal("main", version.PreReleaseTag);
            Assert.False(version.IsBasedOnDevMark);
            Assert.Equal(VersionType.CIBuild, version.Type);
        }

        [Fact]
        public void MajorPreReleaseAfterDevMarkForSameVersion_DifferentPreReleaseTag()
        {
            VersioningConfig versioningConfig = new();
            static BranchVersioning branchConfig(string _) => new() { IncrementedPart = IncrementedPart.Patch };
            var git = new TestVcsAdapter(versioningConfig)
            {
                CurrentBranch = "main",
                IsCleanWorkingCopy = true,
                ChronologicAncestorTags = new[] { ("v2.0-beta", 0), ("dev-2.0", 5), ("v1.0", 10) },
            };
            Versioning versioning = new(branchConfig, git);
            var version = versioning.GetVersionInfo();

            Assert.Equal("v2.0-beta", version.BasedOnTag);
            Assert.Equal(2, version.Major);
            Assert.Equal(0, version.Minor);
            Assert.Equal(0, version.Patch);
            Assert.Equal(0, version.BuildNumber);
            Assert.Equal(6, version.Revision);
            Assert.Equal("beta", version.PreReleaseTag);
            Assert.False(version.IsBasedOnDevMark);
            Assert.Equal(VersionType.PreRelease, version.Type);
        }

        [Fact]
        public void MajorPreReleaseAfterDevMarkForSameVersion_SamePreReleaseTag()
        {
            VersioningConfig versioningConfig = new();
            static BranchVersioning branchConfig(string _) => new() { IncrementedPart = IncrementedPart.Patch };
            var git = new TestVcsAdapter(versioningConfig)
            {
                CurrentBranch = "rc",
                IsCleanWorkingCopy = true,
                ChronologicAncestorTags = new[] { ("v2.0-rc", 0), ("dev-2.0", 5), ("v1.0", 10) },
            };
            Versioning versioning = new(branchConfig, git);
            var version = versioning.GetVersionInfo();

            Assert.Equal("v2.0-rc", version.BasedOnTag);
            Assert.Equal(2, version.Major);
            Assert.Equal(0, version.Minor);
            Assert.Equal(0, version.Patch);
            Assert.Equal(0, version.BuildNumber);
            Assert.Equal(6, version.Revision);
            Assert.Equal("rc", version.PreReleaseTag);
            Assert.False(version.IsBasedOnDevMark);
            Assert.Equal(VersionType.PreRelease, version.Type);
        }

        [Fact]
        public void AfterMajorPreReleaseAfterDevMarkForSameVersion_DifferentPreReleaseTag()
        {
            VersioningConfig versioningConfig = new();
            static BranchVersioning branchConfig(string _) => new() { IncrementedPart = IncrementedPart.Patch };
            var git = new TestVcsAdapter(versioningConfig)
            {
                CurrentBranch = "main",
                IsCleanWorkingCopy = true,
                ChronologicAncestorTags = new[] { ("v2.0-beta", 1), ("dev-2.0", 5), ("v1.0", 10) },
            };
            Versioning versioning = new(branchConfig, git);
            var version = versioning.GetVersionInfo();

            Assert.Equal("dev-2.0", version.BasedOnTag);
            Assert.Equal(2, version.Major);
            Assert.Equal(0, version.Minor);
            Assert.Equal(0, version.Patch);
            Assert.Equal(6, version.BuildNumber);
            Assert.Equal(6, version.Revision);
            Assert.Equal("main", version.PreReleaseTag);
            Assert.True(version.IsBasedOnDevMark);
            Assert.Equal(VersionType.CIBuild, version.Type);
        }

        [Fact]
        public void AfterMajorPreReleaseAfterDevMarkForSameVersion_SamePreReleaseTag()
        {
            VersioningConfig versioningConfig = new();
            static BranchVersioning branchConfig(string _) => new() { IncrementedPart = IncrementedPart.Patch };
            var git = new TestVcsAdapter(versioningConfig)
            {
                CurrentBranch = "rc",
                IsCleanWorkingCopy = true,
                ChronologicAncestorTags = new[] { ("v2.0-rc", 1), ("dev-2.0", 5), ("v1.0", 10) },
            };
            Versioning versioning = new(branchConfig, git);
            var version = versioning.GetVersionInfo();

            Assert.Equal("dev-2.0", version.BasedOnTag);
            Assert.Equal(2, version.Major);
            Assert.Equal(0, version.Minor);
            Assert.Equal(0, version.Patch);
            Assert.Equal(6, version.BuildNumber);
            Assert.Equal(6, version.Revision);
            Assert.Equal("rc", version.PreReleaseTag);
            Assert.True(version.IsBasedOnDevMark);
            Assert.Equal(VersionType.CIBuild, version.Type);
        }

        [Fact]
        public void AfterMinorPreReleaseAfterMajorDevMark_DifferentPreReleaseTag()
        {
            VersioningConfig versioningConfig = new();
            static BranchVersioning branchConfig(string _) => new() { IncrementedPart = IncrementedPart.Patch };
            var git = new TestVcsAdapter(versioningConfig)
            {
                CurrentBranch = "main",
                IsCleanWorkingCopy = true,
                ChronologicAncestorTags = new[] { ("v2.0-beta", 1), ("dev-2.0", 5), ("v1.0", 10) },
            };
            Versioning versioning = new(branchConfig, git);
            var version = versioning.GetVersionInfo();

            Assert.Equal("dev-2.0", version.BasedOnTag);
            Assert.Equal(2, version.Major);
            Assert.Equal(0, version.Minor);
            Assert.Equal(0, version.Patch);
            Assert.Equal(6, version.BuildNumber);
            Assert.Equal(6, version.Revision);
            Assert.Equal("main", version.PreReleaseTag);
            Assert.True(version.IsBasedOnDevMark);
            Assert.Equal(VersionType.CIBuild, version.Type);
        }

        [Fact]
        public void BranchNormalization_Slashes()
        {
            VersioningConfig versioningConfig = new();
            static BranchVersioning branchConfig(string _) => new() { IncrementedPart = IncrementedPart.Minor };
            var git = new TestVcsAdapter(versioningConfig)
            {
                CurrentBranch = "feature/any",
                IsCleanWorkingCopy = true,
                ChronologicAncestorTags = new[] { ("v2.0", 1), ("v1.0", 10) },
            };
            Versioning versioning = new(branchConfig, git);
            var version = versioning.GetVersionInfo();

            Assert.Equal("v2.0", version.BasedOnTag);
            Assert.Equal(2, version.Major);
            Assert.Equal(1, version.Minor);
            Assert.Equal(0, version.Patch);
            Assert.Equal(1, version.BuildNumber);
            Assert.Equal(1, version.Revision);
            Assert.Equal("feature.any", version.PreReleaseTag);
            Assert.False(version.IsBasedOnDevMark);
            Assert.Equal(VersionType.CIBuild, version.Type);
        }

        [Fact]
        public void BranchNormalization_Dashes()
        {
            VersioningConfig versioningConfig = new();
            static BranchVersioning branchConfig(string _) => new() { IncrementedPart = IncrementedPart.Minor };
            var git = new TestVcsAdapter(versioningConfig)
            {
                CurrentBranch = "feature-any-TASK123",
                IsCleanWorkingCopy = true,
                ChronologicAncestorTags = new[] { ("v2.0", 1), ("v1.0", 10) },
            };
            Versioning versioning = new(branchConfig, git);
            var version = versioning.GetVersionInfo();

            Assert.Equal("v2.0", version.BasedOnTag);
            Assert.Equal(2, version.Major);
            Assert.Equal(1, version.Minor);
            Assert.Equal(0, version.Patch);
            Assert.Equal(1, version.BuildNumber);
            Assert.Equal(1, version.Revision);
            Assert.Equal("feature.any.TASK123", version.PreReleaseTag);
            Assert.False(version.IsBasedOnDevMark);
            Assert.Equal(VersionType.CIBuild, version.Type);
        }

        [Fact]
        public void BranchNormalization_SpecialLetters()
        {
            VersioningConfig versioningConfig = new();
            static BranchVersioning branchConfig(string _) => new() { IncrementedPart = IncrementedPart.Minor };
            var git = new TestVcsAdapter(versioningConfig)
            {
                CurrentBranch = "feätüre-äny-TASK123",
                IsCleanWorkingCopy = true,
                ChronologicAncestorTags = new[] { ("v2.0", 1), ("v1.0", 10) },
            };
            Versioning versioning = new(branchConfig, git);
            var version = versioning.GetVersionInfo();

            Assert.Equal("v2.0", version.BasedOnTag);
            Assert.Equal(2, version.Major);
            Assert.Equal(1, version.Minor);
            Assert.Equal(0, version.Patch);
            Assert.Equal(1, version.BuildNumber);
            Assert.Equal(1, version.Revision);
            Assert.Equal("feätüre.äny.TASK123", version.PreReleaseTag);
            Assert.False(version.IsBasedOnDevMark);
            Assert.Equal(VersionType.CIBuild, version.Type);
        }
    }
}
