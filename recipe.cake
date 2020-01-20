#load nuget:https://www.myget.org/F/gep13/api/v2?package=Cake.VsCode.Recipe&prerelease

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