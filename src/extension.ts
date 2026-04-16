import { window, commands, workspace, QuickPickItem, ExtensionContext, OutputChannel, Uri } from "vscode";
import * as chocolateyCli from "./ChocolateyCliManager";
import * as chocolateyOps from "./ChocolateyOperation";
import { getWorkspaceRoot } from "./config";
import * as path from "path";
import * as fs from "fs";

let chocolateyManager: chocolateyCli.ChocolateyCliManager | undefined;
let installed: boolean | undefined;
let outputChannel: OutputChannel;

export function activate(context: ExtensionContext): void {
    outputChannel = window.createOutputChannel("Chocolatey");
    context.subscriptions.push(outputChannel);

    context.subscriptions.push(
        commands.registerCommand("chocolatey.new", (arg?: Uri) => runManagerCommand(m => m.new(arg))),
        commands.registerCommand("chocolatey.pack", () => runManagerCommand(m => m.pack())),
        commands.registerCommand("chocolatey.delete", () => deleteNupkgs()),
        commands.registerCommand("chocolatey.push", () => runManagerCommand(m => m.push())),
        commands.registerCommand("chocolatey.installTemplates", () => runManagerCommand(m => m.installTemplates())),
        commands.registerCommand("chocolatey.apikey", () => runManagerCommand(m => m.apikey())),
        commands.registerCommand(
            "chocolatey.open",
            (uri: string) => commands.executeCommand("vscode.open", Uri.parse(uri))
        )
    );
}

export function deactivate(): void {
    // Nothing to clean up explicitly: the output channel is owned by
    // context.subscriptions and is disposed by VS Code on shutdown.
}

/**
 * Lazily instantiates the ChocolateyCliManager and checks that the
 * Chocolatey CLI is installed, then runs the supplied command against
 * the manager.  Shows a user-facing error message if any precondition
 * fails and returns silently.
 */
function runManagerCommand(action: (manager: chocolateyCli.ChocolateyCliManager) => void): void {
    if (!getWorkspaceRoot()) {
        window.showErrorMessage("You have not yet opened a folder.");
        return;
    }

    if (!chocolateyManager) {
        chocolateyManager = new chocolateyCli.ChocolateyCliManager();
    }

    if (installed === undefined) {
        installed = chocolateyOps.isChocolateyCliInstalled();
    }

    if (!installed) {
        window.showErrorMessage("Chocolatey is not installed");
        return;
    }

    try {
        action(chocolateyManager);
    } catch (err) {
        outputChannel.appendLine(`Command failed: ${err instanceof Error ? err.message : String(err)}`);
    }
}

function deleteNupkgs(): void {
    if (!getWorkspaceRoot()) {
        window.showErrorMessage("You have not yet opened a folder.");
        return;
    }

    workspace.findFiles("**/*.nupkg").then((nupkgFiles) => {
        if (nupkgFiles.length === 0) {
            window.showErrorMessage("There are no nupkg files in the current workspace.");
            return;
        }

        const quickPickItems: Array<QuickPickItem> = nupkgFiles.map((filePath) => {
            return {
                label: path.basename(filePath.fsPath),
                description: filePath.fsPath
            };
        });

        if (quickPickItems.length > 1) {
            quickPickItems.unshift({ label: "All nupkg files" });
        }

        window.showQuickPick(quickPickItems, {
            placeHolder: "Available nupkg files..."
        }).then((nupkgSelection) => {
            if (!nupkgSelection) {
                return;
            }

            if (nupkgSelection.label === "All nupkg files") {
                quickPickItems.forEach((quickPickItem) => {
                    if (quickPickItem.label === "All nupkg files") {
                        return;
                    }
                    deleteNupkgAt(quickPickItem.description);
                });
            } else {
                deleteNupkgAt(nupkgSelection.description);
            }
        });
    });
}

function deleteNupkgAt(fsPath: string | undefined): void {
    if (fsPath && fs.existsSync(fsPath)) {
        fs.unlinkSync(fsPath);
        outputChannel.appendLine(`Deleted file: ${fsPath}`);
    }
}
