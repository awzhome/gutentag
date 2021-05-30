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
    public static int Main() => Execute<Build>(x => x.compile);

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

    Target show_version => _ => _
        .Executes(() =>
        {
            var version = Versioning.GetProjectVersion();
            Console.WriteLine($"Project version is {version.AsString()} (Assembly version: {version.AsAssemblyVersion()})");
        });

    Target versionize => _ => _
        .DependsOn(show_version)
        .Executes(() =>
        {
            var version = Versioning.GetProjectVersion();
            var writer = new BuildVersionWriter(version);
            writer.WriteToVsProject(WorkingDirectory / "AWZhome.GutenTag" / "AWZhome.GutenTag.csproj");
        });

    Target clean => _ => _
        .Before(restore)
        .Executes(() =>
        {
            EnsureCleanDirectory(OutputDirectory);
        });

    Target restore => _ => _
        .Executes(() =>
        {
            DotNetRestore(s => s
                .SetProjectFile(Solution));
        });

    Target compile => _ => _
        .DependsOn(restore)
        .Executes(() =>
        {
            DotNetBuild(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .EnableNoRestore());
        });
}
