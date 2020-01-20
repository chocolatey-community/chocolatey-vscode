#load nuget:?package=Cake.VsCode.Recipe&version=0.1.0

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