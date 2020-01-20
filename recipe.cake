#load nuget:?package=Cake.VsCode.Recipe

Environment.SetVariableNames();

BuildParameters.SetParameters(context: Context,
                            buildSystem: BuildSystem,
                            title: "chocolatey-vscode",
                            repositoryOwner: "chocolatey-community",
                            repositoryName: "chocolatey-vscode",
                            appVeyorAccountName: "chocolateycommunity",
                            shouldRunGitVersion: true);

BuildParameters.PrintParameters(Context);

Build.Run();