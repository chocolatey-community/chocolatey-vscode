import { window, commands, workspace, QuickPickItem, ExtensionContext, Uri } from "vscode";
import * as chocolateyCli from "./ChocolateyCliManager";
import * as chocolateyOps from "./ChocolateyOperation";
import * as path from "path";
import * as fs from "fs";

var chocolateyManager : chocolateyCli.ChocolateyCliManager;
var installed : boolean = false;

export function activate(context: ExtensionContext): void {
    // register Commands
    context.subscriptions.push(
        commands.registerCommand("chocolatey.new", (arg: any) => execute("new", arg)),
        commands.registerCommand("chocolatey.pack", () => execute("pack")),
        commands.registerCommand("chocolatey.delete", () => deleteNupkgs()),
        commands.registerCommand("chocolatey.push", () => execute("push")),
        commands.registerCommand("chocolatey.installTemplates", () => execute("installTemplates")),
        commands.registerCommand("chocolatey.apikey", () => execute("apikey")),
        commands.registerCommand("chocolatey.open", async (uri: string) => await commands.executeCommand('vscode.open', Uri.parse(uri)))
    );
}

function deleteNupkgs():void {
    // check if there is an open folder in workspace
    if (workspace.rootPath === undefined) {
        window.showErrorMessage("You have not yet opened a folder.");
        return;
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
    // check if there is an open folder in workspace
    if (workspace.rootPath === undefined) {
        return window.showErrorMessage("You have not yet opened a folder.");
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
            ecmd.call(chocolateyManager, arg);
            return;
        } catch (e) {
            // well, clearly we didn't call a function
            console.log(e);
            return;
        }
    }

    return;
}
