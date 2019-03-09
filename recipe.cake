#load nuget:https://www.myget.org/F/gep13/api/v2?package=Cake.VsCode.Recipe&prerelease

Environment.SetVariableNames();

BuildParameters.SetParameters(context: Context,
                            buildSystem: BuildSystem,
                            title: "chocolatey-vscode",
                            repositoryOwner: "gep13",
                            repositoryName: "chocolatey-vscode",
                            appVeyorAccountName: "GaryEwanPark",
                            shouldRunGitVersion: true);

BuildParameters.PrintParameters(Context);

BuildParameters.Tasks.CleanTask.Does(() => {
    var directoriesToClean = GetDirectories("./src/**/bin") +
                             GetDirectories("./src/**/obj");

    CleanDirectories(directoriesToClean);
});

const string projectFile = "./src/Chocolatey.Language.Server/Chocolatey.Language.Server.csproj";
string compileConfiguration = Argument("configuration", "Release");

Setup<DotNetCoreMSBuildSettings>((context) => {
    var msBuildSettings = new DotNetCoreMSBuildSettings()
        .WithProperty("Version", BuildParameters.Version.SemVersion)
        .WithProperty("AssemblyVersion", BuildParameters.Version.Version)
        .WithProperty("FileVersion", BuildParameters.Version.Version)
        .WithProperty("AssemblyInformationalVersion", BuildParameters.Version.InformationalVersion);

    if (!IsRunningOnWindows())
    {
        var frameworkPathOverride = new FilePath(typeof(object).Assembly.Location).GetDirectory().FullPath + "/";

        // Use FrameworkPathOverride when not running on Windows
        Information("Build will use FrameworkPathOverride={0} since not building on Windows.", frameworkPathOverride);
        msBuildSettings.WithProperty("FrameworkPathOverride", frameworkPathOverride);
    }

    return msBuildSettings;
});

Task("Restore-Language-Server")
    .IsDependentOn("Clean")
    .Does(() => {
        DotNetCoreRestore(projectFile, new DotNetCoreRestoreSettings {
            // To allow caching of .net core packages
            PackagesDirectory = "./packages"
        });
    });

Task("Build-Language-Server")
    .IsDependentOn("Restore-Language-Server")
    .Does<DotNetCoreMSBuildSettings>(buildSettings =>
{
    DotNetCoreBuild(projectFile, new DotNetCoreBuildSettings {
        Configuration = compileConfiguration,
        NoIncremental = true,
        NoRestore = true,
        MSBuildSettings = buildSettings
    });
});

Task("Publish-Language-Server")
    .IsDependentOn("Build-Language-Server")
    .Does<DotNetCoreMSBuildSettings>(buildSettings =>
{
    DotNetCorePublish(projectFile, new DotNetCorePublishSettings {
        Configuration = compileConfiguration,
        NoBuild = true,
        NoRestore = true,
        //SelfContained = true,
        OutputDirectory = "./out/.server",
        MSBuildSettings = buildSettings
    });
});

BuildParameters.Tasks.PackageExtensionTask.IsDependentOn("Publish-Language-Server");

Build.Run();
