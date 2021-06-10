using System;
using Xunit;

namespace AWZhome.GutenTag.Tests
{
    public class GitParserTests
    {
        [Fact]
        public void SimpleTag_DefaultPrefixes()
        {
            VersioningConfig config = new();
            var version = GitTagParser.Parse("v2.0", config);

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
        public void SimpleDevTag_DefaultPrefixes()
        {
            VersioningConfig config = new();
            var version = GitTagParser.Parse("dev-2.0", config);

            Assert.Equal("dev-2.0", version.BasedOnGitTag);
            Assert.Equal(2, version.Major);
            Assert.Equal(0, version.Minor);
            Assert.Equal(0, version.Patch);
            Assert.Equal(0, version.PreReleaseNumber);
            Assert.Equal(0, version.Revision);
            Assert.Null(version.PreReleaseTag);
            Assert.True(version.IsDevMark);
        }


        [Fact]
        public void SimpleTag_CustomPrefixes()
        {
            VersioningConfig config = new() { ReleaseTagPrefix = "z", DevTagPrefix = "d-" };
            var version = GitTagParser.Parse("z2.0", config);

            Assert.Equal("z2.0", version.BasedOnGitTag);
            Assert.Equal(2, version.Major);
            Assert.Equal(0, version.Minor);
            Assert.Equal(0, version.Patch);
            Assert.Equal(0, version.PreReleaseNumber);
            Assert.Equal(0, version.Revision);
            Assert.Null(version.PreReleaseTag);
            Assert.False(version.IsDevMark);
        }

        [Fact]
        public void SimpleDevTag_CustomPrefixes()
        {
            VersioningConfig config = new() { ReleaseTagPrefix = "z", DevTagPrefix = "d-" };
            var version = GitTagParser.Parse("d-2.0", config);

            Assert.Equal("d-2.0", version.BasedOnGitTag);
            Assert.Equal(2, version.Major);
            Assert.Equal(0, version.Minor);
            Assert.Equal(0, version.Patch);
            Assert.Equal(0, version.PreReleaseNumber);
            Assert.Equal(0, version.Revision);
            Assert.Null(version.PreReleaseTag);
            Assert.True(version.IsDevMark);
        }

        [Fact]
        public void MajorOnly()
        {
            VersioningConfig config = new();
            var version = GitTagParser.Parse("v2", config);

            Assert.Equal("v2", version.BasedOnGitTag);
            Assert.Equal(2, version.Major);
            Assert.Equal(0, version.Minor);
            Assert.Equal(0, version.Patch);
            Assert.Equal(0, version.PreReleaseNumber);
            Assert.Equal(0, version.Revision);
            Assert.Null(version.PreReleaseTag);
            Assert.False(version.IsDevMark);
        }

        [Fact]
        public void MajorOnly_DevMark()
        {
            VersioningConfig config = new();
            var version = GitTagParser.Parse("dev-2", config);

            Assert.Equal("dev-2", version.BasedOnGitTag);
            Assert.Equal(2, version.Major);
            Assert.Equal(0, version.Minor);
            Assert.Equal(0, version.Patch);
            Assert.Equal(0, version.PreReleaseNumber);
            Assert.Equal(0, version.Revision);
            Assert.Null(version.PreReleaseTag);
            Assert.True(version.IsDevMark);
        }

        [Fact]
        public void MajorManorZeroPatch()
        {
            VersioningConfig config = new();
            var version = GitTagParser.Parse("v2.0.0", config);

            Assert.Equal(2, version.Major);
            Assert.Equal(0, version.Minor);
            Assert.Equal(0, version.Patch);
            Assert.Equal(0, version.PreReleaseNumber);
            Assert.Equal(0, version.Revision);
            Assert.Null(version.PreReleaseTag);
            Assert.False(version.IsDevMark);
        }

        [Fact]
        public void MajorManorNonZeroPatch()
        {
            VersioningConfig config = new();
            var version = GitTagParser.Parse("v2.0.1", config);

            Assert.Equal(2, version.Major);
            Assert.Equal(0, version.Minor);
            Assert.Equal(1, version.Patch);
            Assert.Equal(0, version.PreReleaseNumber);
            Assert.Equal(0, version.Revision);
            Assert.Null(version.PreReleaseTag);
            Assert.False(version.IsDevMark);
        }

        [Fact]
        public void BranchTag_NoPatch()
        {
            VersioningConfig config = new();
            var version = GitTagParser.Parse("v2.0-main", config);

            Assert.Equal(2, version.Major);
            Assert.Equal(0, version.Minor);
            Assert.Equal(0, version.Patch);
            Assert.Equal(0, version.PreReleaseNumber);
            Assert.Equal(0, version.Revision);
            Assert.Equal("main", version.PreReleaseTag);
            Assert.False(version.IsDevMark);
        }

        [Fact]
        public void BranchTag_WithPatch()
        {
            VersioningConfig config = new();
            var version = GitTagParser.Parse("v2.0.1-main", config);

            Assert.Equal(2, version.Major);
            Assert.Equal(0, version.Minor);
            Assert.Equal(1, version.Patch);
            Assert.Equal(0, version.PreReleaseNumber);
            Assert.Equal(0, version.Revision);
            Assert.Equal("main", version.PreReleaseTag);
            Assert.False(version.IsDevMark);
        }

        [Fact]
        public void BranchTag_WithPatch_DevMark()
        {
            VersioningConfig config = new();
            var version = GitTagParser.Parse("dev-2.0.1-main", config);

            Assert.Equal(2, version.Major);
            Assert.Equal(0, version.Minor);
            Assert.Equal(1, version.Patch);
            Assert.Equal(0, version.PreReleaseNumber);
            Assert.Equal(0, version.Revision);
            Assert.Equal("main", version.PreReleaseTag);
            Assert.True(version.IsDevMark);
        }

        [Fact]
        public void Revision_Simple()
        {
            VersioningConfig config = new();
            var version = GitTagParser.Parse("v2.0-5-abcdefg", config);

            Assert.Equal(2, version.Major);
            Assert.Equal(0, version.Minor);
            Assert.Equal(0, version.Patch);
            Assert.Equal(5, version.PreReleaseNumber);
            Assert.Equal(5, version.Revision);
            Assert.Null(version.PreReleaseTag);
            Assert.False(version.IsDevMark);
        }

        [Fact]
        public void Revision_WithPatch()
        {
            VersioningConfig config = new();
            var version = GitTagParser.Parse("v2.0.1-5-abcdefg", config);

            Assert.Equal(2, version.Major);
            Assert.Equal(0, version.Minor);
            Assert.Equal(1, version.Patch);
            Assert.Equal(5, version.PreReleaseNumber);
            Assert.Equal(5, version.Revision);
            Assert.Null(version.PreReleaseTag);
            Assert.False(version.IsDevMark);
        }

        [Fact]
        public void Revision_WithBranchTag()
        {
            VersioningConfig config = new();
            var version = GitTagParser.Parse("v2.0.1-main-5-abcdefg", config);

            Assert.Equal(2, version.Major);
            Assert.Equal(0, version.Minor);
            Assert.Equal(1, version.Patch);
            Assert.Equal(5, version.PreReleaseNumber);
            Assert.Equal(5, version.Revision);
            Assert.Equal("main", version.PreReleaseTag);
            Assert.False(version.IsDevMark);
        }

        [Fact]
        public void Revision_WithBranchTag_DevMark()
        {
            VersioningConfig config = new();
            var version = GitTagParser.Parse("dev-2.0.1-main-5-abcdefg", config);

            Assert.Equal(2, version.Major);
            Assert.Equal(0, version.Minor);
            Assert.Equal(1, version.Patch);
            Assert.Equal(5, version.PreReleaseNumber);
            Assert.Equal(5, version.Revision);
            Assert.Equal("main", version.PreReleaseTag);
            Assert.True(version.IsDevMark);
        }
    }
}
