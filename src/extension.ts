import { window, commands, workspace, QuickPickItem, ExtensionContext } from "vscode";
import { LanguageClient, LanguageClientOptions, ServerOptions } from 'vscode-languageclient';
import { Trace } from 'vscode-jsonrpc';
import * as chocolateyCli from "./ChocolateyCliManager";
import * as chocolateyOps from "./ChocolateyOperation";
import * as path from "path";
import * as fs from "fs";

var chocolateyManager : chocolateyCli.ChocolateyCliManager;
var installed : boolean = false;

export function activate(context: ExtensionContext): void {
    // register Commands
    commands.registerCommand("chocolatey.new", () => execute("new"));
    commands.registerCommand("chocolatey.pack", () => execute("pack"));
    commands.registerCommand("chocolatey.delete", () => deleteNupkgs());
    commands.registerCommand("chocolatey.push", () => execute("push"));
    commands.registerCommand("chocolatey.installTemplates", () => execute("installTemplates"));
    commands.registerCommand("chocolatey.apikey", () => execute("addapikey"));

    let serverExe = 'dotnet';

    // If the extension is launched in debug mode then the debug server options are used
    // Otherwise the run options are used
    let serverOptions: ServerOptions = {
        // TODO: For the time being, this path is hard-coded
        // A decision has to be made about how the Language Server is going to be placed on the file server for execution
        run: { command: serverExe, args: ['/Users/svalding/Documents/Git/chocolatey-vscode/src/Chocolatey.Language.Server/chocolatey-vscode/src/Chocolatey.Language.Server/bin/Debug/netcoreapp2.1/Chocolatey.Language.Server.dll'] },
        debug: { command: serverExe, args: ['/Users/svalding/Documents/Git/chocolatey-vscode/src/Chocolatey.Language.Server/chocolatey-vscode/src/Chocolatey.Language.Server/bin/Debug/netcoreapp2.1/Chocolatey.Language.Server.dll'] }
    }

    // Options to control the language client
    let clientOptions: LanguageClientOptions = {
        // Register the server for plain text documents
        documentSelector: [
            {
                pattern: '**/*.nuspec',
            }
        ],
        synchronize: {
            configurationSection: 'nuspec',
            fileEvents: workspace.createFileSystemWatcher('**/*.nuspec')
        },
    }

    // Create the language client and start the client.
    const client = new LanguageClient('nuspec', 'nuspec', serverOptions, clientOptions);
    client.trace = Trace.Verbose;
    let disposable = client.start();

    // Push the disposable to the context's subscriptions so that the
    // client can be deactivated on extension deactivation
    context.subscriptions.push(disposable);
}

function deleteNupkgs():void {
    // Check if there is an open folder in workspace
    if (workspace.rootPath === undefined) {
        window.showErrorMessage("You have not yet opened a folder.");
    }

    workspace.findFiles("**/*.nupkg").then((nupkgFiles) => {
        if(nupkgFiles.length ===0) {
            window.showErrorMessage("There are no nupkg files in the current workspace.");
            return;
        }

        let quickPickItems: Array<QuickPickItem> =  nupkgFiles.map((filePath) => {
            return {
                label: path.basename(filePath.fsPath),
                description: filePath.fsPath
            };
        });

        if(quickPickItems.length > 1) {
            quickPickItems.unshift({label: "All nupkg files"});
        }

        window.showQuickPick(quickPickItems, {
            placeHolder: "Available nupkg files..."
          }).then((nupkgSelection) => {
            if(!nupkgSelection) {
                return;
            }

            if(nupkgSelection.label === "All nupkg files") {
                quickPickItems.forEach((quickPickItem) => {
                    if(quickPickItem.label === "All nupkg files") {
                        return;
                    }

                    if(quickPickItem.description && fs.existsSync(quickPickItem.description)) {
                        fs.unlinkSync(quickPickItem.description);
                        console.log("Deleted file: " + quickPickItem.description);
                    }
                });
            } else {
                if(nupkgSelection.description && fs.existsSync(nupkgSelection.description)) {
                    fs.unlinkSync(nupkgSelection.description);
                        console.log("Deleted file: " + nupkgSelection.description);
                }
            }
        });
    });
}

function execute(cmd?: string | undefined, arg?: any[] | undefined): Thenable<string | undefined> | undefined {
    // Check if there is an open folder in workspace
    if (workspace.rootPath === undefined) {
        window.showErrorMessage("You have not yet opened a folder.");
    }

    if (!chocolateyManager) {
        chocolateyManager = new chocolateyCli.ChocolateyCliManager();
    }

    if (!installed) {
        installed = chocolateyOps.isChocolateyCliInstalled();
    }

    if (!cmd) {
        return;
    }

    // ensure Chocolatey is installed
    if (!installed) {
        return window.showErrorMessage("Chocolatey is not installed");
    }

    // check if there is an open folder in workspace
    if (workspace.rootPath === undefined) {
        return window.showErrorMessage("You have not yet opened a folder.");
    }

    let ecmd: any = chocolateyManager[cmd];
    if (typeof ecmd === "function") {
        try {
            ecmd.apply(chocolateyManager, arg);
            return;
        } catch (e) {
            // well, clearly we didn't call a function
            console.log(e);
            return;
        }
    }

    return;
}
