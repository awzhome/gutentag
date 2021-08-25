using AWZhome.GutenTag;
using AWZhome.GutenTag.Nuke;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.DotNet;
using System;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

[CheckBuildProjectConfigurations]
[ShutdownDotNetAfterServerBuild]
class Build : NukeBuild
{
    public static int Main() => Execute<Build>(x => x.Compile);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Solution] readonly Solution Solution;
    [GitRepository] readonly GitRepository GitRepository;

    AbsolutePath OutputDirectory => RootDirectory / "output";

    BranchSpecificConfig BranchVersioningConfig => b => b switch
    {
        "main" => new() { Tag = "preview", IncrementedPart = IncrementedPart.Minor },
        _ => new()
    };

    Versioning Versioning => new(BranchVersioningConfig, new NukeGitAdapter(new VersioningConfig()));

    Target ShowVersion => _ => _
        .Executes(() =>
        {
            var version = Versioning.GetVersionInfo();
            Console.WriteLine($"Project version is {version.AsString()} (Assembly version: {version.AsNumericVersion()})");
        });

    Target Versionize => _ => _
        .DependsOn(ShowVersion)
        .Executes(() =>
        {
            var version = Versioning.GetVersionInfo();
            var writer = new VersionInfoWriter(version);
            writer.WriteToVsProject(
                WorkingDirectory / "AWZhome.GutenTag" / "AWZhome.GutenTag.csproj",
                WorkingDirectory / "AWZhome.GutenTag.Tests" / "AWZhome.GutenTag.Tests.csproj");
        });

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            EnsureCleanDirectory(OutputDirectory);
        });

    Target Restore => _ => _
        .Executes(() =>
        {
            DotNetRestore(s => s
                .SetProjectFile(Solution));
        });

    Target Compile => _ => _
        .DependsOn(Restore, Versionize)
        .Executes(() =>
        {
            DotNetBuild(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .EnableNoRestore());
        });

    Target Test => _ => _
        .Executes(() =>
        {
            DotNetTest(s => s
                .SetProjectFile("AWZhome.GutenTag.Tests")
            );
        });
}
