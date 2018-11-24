#r "System.Net.Http"
using System.Net.Http;
using System.Net.Http.Headers;

///////////////////////////////////////////////////////////////////////////////
// TASK DEFINITIONS
///////////////////////////////////////////////////////////////////////////////

Task("Clean-Documentation")
    .Does(() =>
{
    EnsureDirectoryExists(parameters.WyamPublishDirectoryPath);
});

Task("Build-Documentation")
    .IsDependentOn("Clean-Documentation")
    .Does(() =>
{
    Wyam(new WyamSettings
    {
        Recipe = parameters.WyamRecipe,
        Theme = parameters.WyamTheme,
        OutputPath = MakeAbsolute(Directory("build-results/Documentation")),
        RootPath = parameters.WyamRootDirectoryPath,
        ConfigurationFile = parameters.WyamConfigurationFile,
        PreviewVirtualDirectory = parameters.WebLinkRoot,
        Settings = parameters.WyamSettings
    });
});

Task("Publish-Documentation")
    .IsDependentOn("Build-Documentation")
    .WithCriteria(() => parameters.ShouldPublishDocumentation)
    .WithCriteria(() => DirectoryExists(parameters.WyamRootDirectoryPath))
    .Does(() =>
{
    if(parameters.CanUseWyam)
    {
        var sourceCommit = GitLogTip("./");

        var publishFolder = parameters.WyamPublishDirectoryPath.Combine(DateTime.Now.ToString("yyyyMMdd_HHmmss"));
        Information("Publishing Folder: {0}", publishFolder);
        Information("Getting publish branch...");
        GitClone(parameters.Wyam.DeployRemote, publishFolder, new GitCloneSettings{ BranchName = parameters.Wyam.DeployBranch });

        Information("Sync output files...");
        Kudu.Sync(MakeAbsolute(Directory("build-results/Documentation")), publishFolder, new KuduSyncSettings {
            ArgumentCustomization = args=>args.Append("--ignore").AppendQuoted(".git;CNAME")
        });

        if (GitHasUncommitedChanges(publishFolder))
        {
            Information("Stage all changes...");
            GitAddAll(publishFolder);

            Information("Commit all changes...");
            GitCommit(
                publishFolder,
                sourceCommit.Committer.Name,
                sourceCommit.Committer.Email,
                string.Format("AppVeyor Publish: {0}\r\n{1}", sourceCommit.Sha, sourceCommit.Message)
            );

            Information("Pushing all changes...");
            GitPush(publishFolder, parameters.Wyam.AccessToken, "x-oauth-basic", parameters.Wyam.DeployBranch);
        }
    }
    else
    {
        Warning("Unable to publish documentation, as not all Wyam Configuration is present");
    }
})
.OnError(exception =>
{
    Error(exception.Message);
    Information("Publish-Documentation Task failed, but continuing with next Task...");
    publishingError = true;
});

Task("Preview-Documentation")
    .WithCriteria(() => DirectoryExists(parameters.WyamRootDirectoryPath))
    .Does(() =>
{
    Wyam(new WyamSettings
    {
        Recipe = parameters.WyamRecipe,
        Theme = parameters.WyamTheme,
        OutputPath = MakeAbsolute(Directory("build-results/Documentation")),
        RootPath = parameters.WyamRootDirectoryPath,
        Preview = true,
        Watch = true,
        ConfigurationFile = parameters.WyamConfigurationFile,
        PreviewVirtualDirectory = parameters.WebLinkRoot,
        Settings = parameters.WyamSettings
    });
});
