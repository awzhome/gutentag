using System;
using Xunit;

namespace AWZhome.GutenTag.Tests
{
    public class TagParserTests
    {
        [Fact]
        public void SimpleTag_DefaultPrefixes()
        {
            VersioningConfig config = new();
            var version = TagParser.Parse("v2.0", config);

            Assert.Equal("v2.0", version.BasedOnTag);
            Assert.Equal(2, version.Major);
            Assert.Equal(0, version.Minor);
            Assert.Equal(0, version.Patch);
            Assert.Equal(0, version.BuildNumber);
            Assert.Equal(0, version.Revision);
            Assert.Null(version.PreReleaseTag);
            Assert.False(version.IsBasedOnDevMark);
        }

        [Fact]
        public void SimpleDevTag_DefaultPrefixes()
        {
            VersioningConfig config = new();
            var version = TagParser.Parse("dev-2.0", config);

            Assert.Equal("dev-2.0", version.BasedOnTag);
            Assert.Equal(2, version.Major);
            Assert.Equal(0, version.Minor);
            Assert.Equal(0, version.Patch);
            Assert.Equal(0, version.BuildNumber);
            Assert.Equal(0, version.Revision);
            Assert.Null(version.PreReleaseTag);
            Assert.True(version.IsBasedOnDevMark);
        }


        [Fact]
        public void SimpleTag_CustomPrefixes()
        {
            VersioningConfig config = new() { ReleaseTagPrefix = "z", DevTagPrefix = "d-" };
            var version = TagParser.Parse("z2.0", config);

            Assert.Equal("z2.0", version.BasedOnTag);
            Assert.Equal(2, version.Major);
            Assert.Equal(0, version.Minor);
            Assert.Equal(0, version.Patch);
            Assert.Equal(0, version.BuildNumber);
            Assert.Equal(0, version.Revision);
            Assert.Null(version.PreReleaseTag);
            Assert.False(version.IsBasedOnDevMark);
        }

        [Fact]
        public void SimpleDevTag_CustomPrefixes()
        {
            VersioningConfig config = new() { ReleaseTagPrefix = "z", DevTagPrefix = "d-" };
            var version = TagParser.Parse("d-2.0", config);

            Assert.Equal("d-2.0", version.BasedOnTag);
            Assert.Equal(2, version.Major);
            Assert.Equal(0, version.Minor);
            Assert.Equal(0, version.Patch);
            Assert.Equal(0, version.BuildNumber);
            Assert.Equal(0, version.Revision);
            Assert.Null(version.PreReleaseTag);
            Assert.True(version.IsBasedOnDevMark);
        }

        [Fact]
        public void MajorOnly()
        {
            VersioningConfig config = new();
            var version = TagParser.Parse("v2", config);

            Assert.Equal("v2", version.BasedOnTag);
            Assert.Equal(2, version.Major);
            Assert.Equal(0, version.Minor);
            Assert.Equal(0, version.Patch);
            Assert.Equal(0, version.BuildNumber);
            Assert.Equal(0, version.Revision);
            Assert.Null(version.PreReleaseTag);
            Assert.False(version.IsBasedOnDevMark);
        }

        [Fact]
        public void MajorOnly_DevMark()
        {
            VersioningConfig config = new();
            var version = TagParser.Parse("dev-2", config);

            Assert.Equal("dev-2", version.BasedOnTag);
            Assert.Equal(2, version.Major);
            Assert.Equal(0, version.Minor);
            Assert.Equal(0, version.Patch);
            Assert.Equal(0, version.BuildNumber);
            Assert.Equal(0, version.Revision);
            Assert.Null(version.PreReleaseTag);
            Assert.True(version.IsBasedOnDevMark);
        }

        [Fact]
        public void MajorManorZeroPatch()
        {
            VersioningConfig config = new();
            var version = TagParser.Parse("v2.0.0", config);

            Assert.Equal("v2.0.0", version.BasedOnTag);
            Assert.Equal(2, version.Major);
            Assert.Equal(0, version.Minor);
            Assert.Equal(0, version.Patch);
            Assert.Equal(0, version.BuildNumber);
            Assert.Equal(0, version.Revision);
            Assert.Null(version.PreReleaseTag);
            Assert.False(version.IsBasedOnDevMark);
        }

        [Fact]
        public void MajorManorNonZeroPatch()
        {
            VersioningConfig config = new();
            var version = TagParser.Parse("v2.0.1", config);

            Assert.Equal("v2.0.1", version.BasedOnTag);
            Assert.Equal(2, version.Major);
            Assert.Equal(0, version.Minor);
            Assert.Equal(1, version.Patch);
            Assert.Equal(0, version.BuildNumber);
            Assert.Equal(0, version.Revision);
            Assert.Null(version.PreReleaseTag);
            Assert.False(version.IsBasedOnDevMark);
        }

        [Fact]
        public void PreReleaseTag_NoMinor()
        {
            VersioningConfig config = new();
            var version = TagParser.Parse("v2-main", config);

            Assert.Equal("v2-main", version.BasedOnTag);
            Assert.Equal(2, version.Major);
            Assert.Equal(0, version.Minor);
            Assert.Equal(0, version.Patch);
            Assert.Equal(0, version.BuildNumber);
            Assert.Equal(0, version.Revision);
            Assert.Equal("main", version.PreReleaseTag);
            Assert.False(version.IsBasedOnDevMark);
        }

        [Fact]
        public void PreReleaseTag_NoPatch()
        {
            VersioningConfig config = new();
            var version = TagParser.Parse("v2.0-main", config);

            Assert.Equal("v2.0-main", version.BasedOnTag);
            Assert.Equal(2, version.Major);
            Assert.Equal(0, version.Minor);
            Assert.Equal(0, version.Patch);
            Assert.Equal(0, version.BuildNumber);
            Assert.Equal(0, version.Revision);
            Assert.Equal("main", version.PreReleaseTag);
            Assert.False(version.IsBasedOnDevMark);
        }

        [Fact]
        public void PreReleaseTag_WithPatch()
        {
            VersioningConfig config = new();
            var version = TagParser.Parse("v2.0.1-main", config);

            Assert.Equal("v2.0.1-main", version.BasedOnTag);
            Assert.Equal(2, version.Major);
            Assert.Equal(0, version.Minor);
            Assert.Equal(1, version.Patch);
            Assert.Equal(0, version.BuildNumber);
            Assert.Equal(0, version.Revision);
            Assert.Equal("main", version.PreReleaseTag);
            Assert.False(version.IsBasedOnDevMark);
        }

        [Fact]
        public void PreReleaseTag_WithPatch_DevMark()
        {
            VersioningConfig config = new();
            var version = TagParser.Parse("dev-2.0.1-main", config);

            Assert.Equal("dev-2.0.1-main", version.BasedOnTag);
            Assert.Equal(2, version.Major);
            Assert.Equal(0, version.Minor);
            Assert.Equal(1, version.Patch);
            Assert.Equal(0, version.BuildNumber);
            Assert.Equal(0, version.Revision);
            Assert.Equal("main", version.PreReleaseTag);
            Assert.True(version.IsBasedOnDevMark);
        }

        [Fact]
        public void PreReleaseTag_Numeric()
        {
            VersioningConfig config = new();
            var version = TagParser.Parse("v2.0-5", config);

            Assert.Equal("v2.0-5", version.BasedOnTag);
            Assert.Equal(2, version.Major);
            Assert.Equal(0, version.Minor);
            Assert.Equal(0, version.Patch);
            Assert.Equal(0, version.BuildNumber);
            Assert.Equal(0, version.Revision);
            Assert.Equal("5", version.PreReleaseTag);
            Assert.False(version.IsBasedOnDevMark);
        }

        [Fact]
        public void PreReleaseTag_Complex()
        {
            VersioningConfig config = new();
            var version = TagParser.Parse("v2.0.1-rc.3", config);

            Assert.Equal("v2.0.1-rc.3", version.BasedOnTag);
            Assert.Equal(2, version.Major);
            Assert.Equal(0, version.Minor);
            Assert.Equal(1, version.Patch);
            Assert.Equal(0, version.BuildNumber);
            Assert.Equal(0, version.Revision);
            Assert.Equal("rc.3", version.PreReleaseTag);
            Assert.False(version.IsBasedOnDevMark);
        }

        [Fact]
        public void PreReleaseTag_DotSeparated()
        {
            VersioningConfig config = new();
            var version = TagParser.Parse("v2.0.1.rc3", config);

            Assert.Equal("v2.0.1.rc3", version.BasedOnTag);
            Assert.Equal(2, version.Major);
            Assert.Equal(0, version.Minor);
            Assert.Equal(1, version.Patch);
            Assert.Equal(0, version.BuildNumber);
            Assert.Equal(0, version.Revision);
            Assert.Equal("rc3", version.PreReleaseTag);
            Assert.False(version.IsBasedOnDevMark);
        }
    }
}
