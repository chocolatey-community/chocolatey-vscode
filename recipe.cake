#load nuget:?package=Cake.VsCode.Recipe&version=0.1.0

Environment.SetVariableNames();

BuildParameters.SetParameters(context: Context,
                            buildSystem: BuildSystem,
                            title: "chocolatey-vscode",
                            repositoryOwner: "chocolatey-community",
                            repositoryName: "chocolatey-vscode",
                            appVeyorAccountName: "chocolateycommunity",
                            shouldRunGitVersion: true,
                            shouldDeployGraphDocumentation: false);

// We remove the installation of typescript
// as it conflicts with the version used
// in this repository.
var packageTask = (CakeTask)BuildParameters.Tasks.PackageExtensionTask.Task;
var taskToRemove = packageTask.Dependencies.First(x => x.Name == BuildParameters.Tasks.InstallTypeScriptTask.Task.Name);
packageTask.Dependencies.Remove(taskToRemove);
                            
BuildParameters.PrintParameters(Context);

Build.Run();