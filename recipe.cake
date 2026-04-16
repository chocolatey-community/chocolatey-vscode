#load nuget:?package=Cake.VsCode.Recipe&version=1.0.0

Environment.SetVariableNames();

BuildParameters.SetParameters(context: Context,
                            buildSystem: BuildSystem,
                            title: "chocolatey-vscode",
                            repositoryOwner: "chocolatey-community",
                            repositoryName: "chocolatey-vscode",
                            appVeyorAccountName: "chocolateycommunity",
                            shouldRunGitVersion: true,
                            marketPlacePublisher: "gep13",
                            preferDotNetGlobalToolUsage: true);

// We remove the installation of typescript
// as it conflicts with the version used
// in this repository.
var packageTask = (CakeTask)BuildParameters.Tasks.PackageExtensionTask.Task;
var taskToRemove = packageTask.Dependencies.First(x => x.Name == BuildParameters.Tasks.InstallTypeScriptTask.Task.Name);
packageTask.Dependencies.Remove(taskToRemove);

// Run ESLint against the TypeScript sources.
var lintTask = Task("Lint")
    .IsDependentOn("Npm-Install")
    .Does(() =>
{
    NpmRunScript("lint");
});

// Run the unit and integration test suites via `npm test`.  Depends on
// Lint so a single Cake invocation from CI exercises both gates.
var testTask = Task("Test")
    .IsDependentOn("Npm-Install")
    .IsDependentOn("Lint")
    .Does(() =>
{
    NpmRunScript("test");
});

// Packaging (and therefore publishing) the extension now requires the
// test suites to pass first.
BuildParameters.Tasks.PackageExtensionTask.IsDependentOn("Test");

BuildParameters.PrintParameters(Context);

Build.Run();