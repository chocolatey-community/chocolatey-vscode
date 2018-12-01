///////////////////////////////////////////////////////////////////////////////
// TASK DEFINITIONS
///////////////////////////////////////////////////////////////////////////////

Task("Clean-Documentation")
    .Does(() =>
{
    EnsureDirectoryExists(parameters.WyamPublishDirectoryPath);
});

Task("Publish-Documentation")
    .IsDependentOn("Clean-Documentation")
    .WithCriteria(() => DirectoryExists(parameters.WyamRootDirectoryPath))
    .Does(() =>
{
        // Check to see if any documentation has changed
        var sourceCommit = GitLogTip("./");
        Information("Source Commit Sha: {0}", sourceCommit.Sha);
        var filesChanged = GitDiff("./", sourceCommit.Sha);
        Information("Number of changed files: {0}", filesChanged.Count);
        var docFileChanged = false;

        var wyamDocsFolderDirectoryName = parameters.WyamRootDirectoryPath.GetDirectoryName();

        foreach(var file in filesChanged)
        {
            var forwardSlash = '/';
            Verbose("Changed File OldPath: {0}, Path: {1}", file.OldPath, file.Path);
            if(file.OldPath.Contains(string.Format("{0}{1}", wyamDocsFolderDirectoryName, forwardSlash)) ||
                file.Path.Contains(string.Format("{0}{1}", wyamDocsFolderDirectoryName, forwardSlash)) ||
                file.Path.Contains("config.wyam"))
            {
            docFileChanged = true;
            break;
            }
        }

        if(docFileChanged)
        {
            Information("Detected that documentation files have changed, so running Wyam...");

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

            PublishDocumentation();
        }
        else
        {
            Information("No documentation has changed, so no need to generate documentation");
        }
    }
)
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

Task("Force-Publish-Documentation")
    .IsDependentOn("Clean-Documentation")
    .WithCriteria(() => DirectoryExists(parameters.WyamRootDirectoryPath))
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

    PublishDocumentation();
});

public void PublishDocumentation()
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

            if(GitHasStagedChanges(publishFolder))
            {
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
    }
    else
    {
        Warning("Unable to publish documentation, as not all Wyam Configuration is present");
    }
}
