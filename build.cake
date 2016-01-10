///////////////////////////////////////////////////////////////////////////////
// ADDINS
///////////////////////////////////////////////////////////////////////////////
#addin Cake.Json
#addin Cake.VsCode

///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////

var target          = Argument<string>("target", "Default");
var configuration   = Argument<string>("configuration", "Release");

///////////////////////////////////////////////////////////////////////////////
// GLOBAL VARIABLES
///////////////////////////////////////////////////////////////////////////////

var packageJsonFile            = "./package.json";
var version                    = "0.1.0";
var buildDirectory             = Directory("./.build");

///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////

public class PackageJsonObject
{
    public string version { get; set; }
}

Setup(() =>
{
    // Executed BEFORE the first task.
    Information("Running tasks...");
});

Teardown(() =>
{
    // Executed AFTER the last task.
    Information("Finished running tasks.");
});

///////////////////////////////////////////////////////////////////////////////
// TASK DEFINITIONS
///////////////////////////////////////////////////////////////////////////////

Task("Create-Build-Directory")
	.Does(() =>
{
    if (!DirectoryExists(buildDirectory))
    {
        CreateDirectory(buildDirectory);
    }
});

Task("Find-Version-Number")
    .Does(() =>
{
    var packageJson = DeserializeJsonFromFile<PackageJsonObject>(MakeAbsolute((FilePath)packageJsonFile));
    version = packageJson.version;
    
    Information("Running GitReleaseManager for version: {0}", version);
});

Task("Package-Extension")
    .IsDependentOn("Create-Build-Directory")
    .IsDependentOn("Find-Version-Number") 
    .Does(() =>
{
    var packageFile = File("chocolatey-vscode-" + version + ".vsix");
    
    VscePackage(new VscePackageSettings() {
        OutputFilePath = buildDirectory + packageFile,
    });
});

Task("Create-Release-Notes")
    .IsDependentOn("Find-Version-Number") 
    .IsDependentOn("Package-Extension")  
    .Does(() =>
{
    var userName = EnvironmentVariable("GITHUB_USERNAME");
    var password = EnvironmentVariable("GITHUB_PASSWORD");

    GitReleaseManagerCreate(userName, password, "gep13", "chocolatey-vscode", new GitReleaseManagerCreateSettings {
        Milestone         = version,
        Assets            = string.Format("{0}/chocolatey-vscode-{1}.vsix", Context.Environment.WorkingDirectory, version), 
        Name              = version,
        Prerelease        = false,
        TargetCommitish   = "master"
    });
});

Task("Publish-Extension")
    .IsDependentOn("Create-Release-Notes")
    .Does(() =>
{
    var personalAccessToken = EnvironmentVariable("VSCE_PAT");
    var userName = EnvironmentVariable("GITHUB_USERNAME");
    var password = EnvironmentVariable("GITHUB_PASSWORD");
    var packageFile = File("chocolatey-vscode-" + version + ".vsix");
    
    VscePublish(new VscePublishSettings(){
        PersonalAccessToken = personalAccessToken,
        Package = buildDirectory + packageFile
    });
    
    GitReleaseManagerPublish(userName, password, "gep13", "chocolatey-vscode", version, new GitReleaseManagerPublishSettings {
    });
    
    GitReleaseManagerClose(userName, password, "gep13", "chocolatey-vscode", version, new GitReleaseManagerCloseMilestoneSettings {
    });
});

Task("Default")
    .IsDependentOn("Package-Extension");

///////////////////////////////////////////////////////////////////////////////
// EXECUTION
///////////////////////////////////////////////////////////////////////////////

RunTarget(target);