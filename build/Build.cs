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
        "main" => new() { Tag = "preview", IncrementPatch = false },
        _ => new()
    };

    Versioning Versioning => new(new VersioningConfig(), BranchVersioningConfig, NukeGitAdapter.Executor);

    Target ShowVersion => _ => _
        .Executes(() =>
        {
            var version = Versioning.GetProjectVersion();
            Console.WriteLine($"Project version is {version.AsString()} (Assembly version: {version.AsNumericVersion()})");
        });

    Target Versionize => _ => _
        .DependsOn(ShowVersion)
        .Executes(() =>
        {
            var version = Versioning.GetProjectVersion();
            var writer = new BuildVersionWriter(version);
            writer.WriteToVsProject(WorkingDirectory / "AWZhome.GutenTag" / "AWZhome.GutenTag.csproj");
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
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetBuild(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .EnableNoRestore());
        });
}
