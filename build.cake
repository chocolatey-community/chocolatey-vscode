//////////////////////////////////////////////////////////////////////
// ADDINS
//////////////////////////////////////////////////////////////////////

#addin "nuget:?package=MagicChunks&version=2.0.0.119"
#addin "nuget:?package=Cake.VsCode&version=0.8.0"
#addin "nuget:?package=Cake.Npm&version=0.10.0"
#addin "nuget:?package=Cake.AppVeyor&version=1.1.0.9"
#addin "nuget:?package=Cake.Wyam&version=1.7.4"
#addin "nuget:?package=Cake.Git&version=0.19.0"
#addin "nuget:?package=Cake.Kudu&version=0.8.0"
#addin "nuget:?package=Cake.Gitter&version=0.10.0"
#addin "nuget:?package=Cake.Twitter&version=0.9.0"

//////////////////////////////////////////////////////////////////////
// TOOLS
//////////////////////////////////////////////////////////////////////

#tool "nuget:?package=gitreleasemanager&version=0.7.0"
#tool "nuget:?package=GitVersion.CommandLine&version=3.6.5"
#tool "nuget:?package=Wyam&version=1.7.4"
#tool "nuget:?package=KuduSync.NET&version=1.4.0"

// Load other scripts.
#load "./build/parameters.cake"
#load "./build/wyam.cake"
#load "./build/gitter.cake"
#load "./build/twitter.cake"

//////////////////////////////////////////////////////////////////////
// PARAMETERS
//////////////////////////////////////////////////////////////////////

BuildParameters parameters = BuildParameters.GetParameters(Context, BuildSystem);
bool publishingError = false;

///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////

Setup(context =>
{
    parameters.SetBuildVersion(
        BuildVersion.CalculatingSemanticVersion(
            context: Context,
            parameters: parameters
        )
    );

    Information("Building version {0} of chocolatey-vscode ({1}, {2}) using version {3} of Cake. (IsTagged: {4})",
        parameters.Version.SemVersion,
        parameters.Configuration,
        parameters.Target,
        parameters.Version.CakeVersion,
        parameters.IsTagged);
});

Teardown(context =>
{
    Information("Starting Teardown...");

    if(context.Successful)
    {
        if(!parameters.IsLocalBuild && !parameters.IsPullRequest && parameters.IsMasterRepo && (parameters.IsMasterBranch || ((parameters.IsReleaseBranch || parameters.IsHotFixBranch))) && parameters.IsTagged)
        {
            if(parameters.CanPostToTwitter)
            {
                SendMessageToTwitter();
            }

            if(parameters.CanPostToGitter)
            {
                SendMessageToGitterRoom();
            }
        }
    }

    Information("Finished running tasks.");
});

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    CleanDirectories(new[] { "./build-results" });
});

Task("Npm-Install")
    .Does(() =>
{
    var settings = new NpmInstallSettings();
    settings.LogLevel = NpmLogLevel.Silent;
    NpmInstall(settings);
});

Task("Install-TypeScript")
    .Does(() =>
{
    var settings = new NpmInstallSettings();
    settings.Global = true;
    settings.AddPackage("typescript", "2.9.2");
    settings.LogLevel = NpmLogLevel.Silent;
    NpmInstall(settings);
});

Task("Install-Vsce")
    .Does(() =>
{
    var settings = new NpmInstallSettings();
    settings.Global = true;
    settings.AddPackage("vsce", "1.43.0");
    settings.LogLevel = NpmLogLevel.Silent;
    NpmInstall(settings);
});

Task("Create-Release-Notes")
    .Does(() =>
{
    GitReleaseManagerCreate(parameters.GitHub.UserName, parameters.GitHub.Password, "gep13", "chocolatey-vscode", new GitReleaseManagerCreateSettings {
        Milestone         = parameters.Version.Milestone,
        Name              = parameters.Version.Milestone,
        Prerelease        = true,
        TargetCommitish   = "master"
    });
});

Task("Update-Project-Json-Version")
    .WithCriteria(() => !parameters.IsLocalBuild)
    .Does(() =>
{
    var projectToPackagePackageJson = "package.json";
    Information("Updating {0} version -> {1}", projectToPackagePackageJson, parameters.Version.SemVersion);

    TransformConfig(projectToPackagePackageJson, projectToPackagePackageJson, new TransformationCollection {
        { "version", parameters.Version.SemVersion }
    });
});

Task("Package-Extension")
    .IsDependentOn("Update-Project-Json-Version")
    .IsDependentOn("Npm-Install")
    .IsDependentOn("Install-TypeScript")
    .IsDependentOn("Install-Vsce")
    .IsDependentOn("Clean")
    .Does(() =>
{
    var buildResultDir = Directory("./build-results");
    var packageFile = File("chocolatey-vscode-" + parameters.Version.SemVersion + ".vsix");

    VscePackage(new VscePackageSettings() {
        OutputFilePath = buildResultDir + packageFile
    });
});

Task("Create-Chocolatey-Package")
    .IsDependentOn("Package-Extension")
    .Does(() =>
{
    // TODO: Automatically update the release notes in the nuspec file
    // TODO: Automatically update the description from the Readme.md file

    var nuspecFile = File("./chocolatey/chocolatey-vscode.nuspec");

    EnsureDirectoryExists(parameters.ChocolateyPackages);
    var extensionFile = MakeAbsolute((FilePath)("./build-results/chocolatey-vscode-" + parameters.Version.SemVersion + ".vsix"));
    CopyFile("LICENSE", "./chocolatey/tools/LICENSE.txt");
    var files = GetFiles("./chocolatey/tools/**/*").Select(f => new ChocolateyNuSpecContent {
                  Source = MakeAbsolute((FilePath)f).ToString(),
                  Target = "tools"
                }).ToList();
    files.Add(new ChocolateyNuSpecContent { Source = extensionFile.ToString(), Target = "tools/chocolatey-vscode.vsix" });

    ChocolateyPack(nuspecFile, new ChocolateyPackSettings {
        Version = parameters.Version.SemVersion,
        OutputDirectory = parameters.ChocolateyPackages,
        WorkingDirectory = "./chocolatey",
        Files = files.ToArray()
    });
});

Task("Upload-AppVeyor-Artifacts")
    .IsDependentOn("Package-Extension")
    .IsDependentOn("Create-Chocolatey-Package")
    .WithCriteria(() => parameters.IsRunningOnAppVeyor)
.Does(() =>
{
    var buildResultDir = Directory("./build-results");
    var packageFile = File("chocolatey-vscode-" + parameters.Version.SemVersion + ".vsix");
    var chocolateyPackageFile = File("chocolatey-vscode." + parameters.Version.SemVersion + ".nupkg");
    AppVeyor.UploadArtifact(buildResultDir + packageFile);
    AppVeyor.UploadArtifact(buildResultDir + chocolateyPackageFile);
});

Task("Publish-GitHub-Release")
    .IsDependentOn("Package-Extension")
    .IsDependentOn("Create-Chocolatey-Package")
    .WithCriteria(() => parameters.ShouldPublish)
    .Does(() =>
{
    var packageFiles = GetFiles("./build-results/*.vsix")
                     + GetFiles(parameters.ChocolateyPackages + "/*.nupkg");

    foreach (var package in packageFiles.Select(f => MakeAbsolute(f)))
    {
        GitReleaseManagerAddAssets(parameters.GitHub.UserName, parameters.GitHub.Password, "gep13", "chocolatey-vscode", parameters.Version.Milestone, package.ToString());
    }

    GitReleaseManagerClose(parameters.GitHub.UserName, parameters.GitHub.Password, "gep13", "chocolatey-vscode", parameters.Version.Milestone);
})
.OnError(exception =>
{
    Information("Publish-GitHub-Release Task failed, but continuing with next Task...");
    publishingError = true;
});

Task("Publish-Chocolatey-Package")
    .IsDependentOn("Create-Chocolatey-Package")
    .WithCriteria(() => parameters.ShouldPublish)
    .Does(() =>
{
    foreach (var package in GetFiles(parameters.ChocolateyPackages + "/*.nupkg"))
    {
        ChocolateyPush(package, new ChocolateyPushSettings {
            ApiKey = parameters.Chocolatey.ApiKey,
            Source = parameters.Chocolatey.SourceUrl
        });
    }
})
.OnError(exception =>
{
    Information("Publish-Chocolatey-Package Task failed, but continuing with next Task...");
    publishingError = true;
});

Task("Publish-Extension")
    .IsDependentOn("Package-Extension")
    .WithCriteria(() => parameters.ShouldPublish)
    .Does(() =>
{
    var buildResultDir = Directory("./build-results");
    var packageFile = File("chocolatey-vscode-" + parameters.Version.SemVersion + ".vsix");

    VscePublish(new VscePublishSettings(){
        PersonalAccessToken = parameters.Marketplace.Token,
        Package = buildResultDir + packageFile
    });
})
.OnError(exception =>
{
    Information("Publish-Extension Task failed, but continuing with next Task...");
    publishingError = true;
});

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Package-Extension");

Task("Appveyor")
    .IsDependentOn("Upload-AppVeyor-Artifacts")
    .IsDependentOn("Publish-Documentation")
    .IsDependentOn("Publish-GitHub-Release")
    .IsDependentOn("Publish-Extension")
    .IsDependentOn("Publish-Chocolatey-Package")
    .Finally(() =>
{
    if(publishingError)
    {
        throw new Exception("An error occurred during the publishing of chocolatey-vscode.  All publishing tasks have been attempted.");
    }
});

Task("ReleaseNotes")
    .IsDependentOn("Create-Release-Notes");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(parameters.Target);
