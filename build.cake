///////////////////////////////////////////////////////////////////////////////
// ADDINS
///////////////////////////////////////////////////////////////////////////////
#addin Cake.Json

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

Task("Find-Version-Number")
    .Does(() =>
{
    var packageJson = DeserializeJsonFromFile<PackageJsonObject>(MakeAbsolute((FilePath)packageJsonFile));
    version = packageJson.version;
    
    Information("Running GitReleaseManager for version: {0}", version);
});

Task("Create-Release-Notes")
    .IsDependentOn("Find-Version-Number")  
    .Does(() =>
{
    var userName = EnvironmentVariable("GITHUB_USERNAME");
    var password = EnvironmentVariable("GITHUB_PASSWORD");

    GitReleaseManagerCreate(userName, password, "gep13", "chocolatey-vscode", new GitReleaseManagerCreateSettings {
        Milestone         = version,
        Name              = version,
        Prerelease        = false,
        TargetCommitish   = "master"
    });
});

Task("Default")
    .IsDependentOn("Create-Release-Notes");

///////////////////////////////////////////////////////////////////////////////
// EXECUTION
///////////////////////////////////////////////////////////////////////////////

RunTarget(target);