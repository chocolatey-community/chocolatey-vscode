import { window, QuickPickItem, workspace } from "vscode";
import { ChocolateyOperation } from "./ChocolateyOperation";
import * as path from "path";

export class ChocolateyCliManager {
    public new(): void {
        window.showInputBox({
            prompt: "Name for new Chocolatey Package?"
        }).then((result) => {
            if (!result || result === "") {
                return;
            }

            let newOp: ChocolateyOperation = new ChocolateyOperation(["new", result]);
            newOp.run();
        });
    }

    public pack(): void {
        workspace.findFiles("**/*.nuspec").then((nuspecFiles) => {
            if(nuspecFiles.length ===0) {
                window.showErrorMessage("There are no nuspec files in the current workspace.");
                return;
            }

            let quickPickItems: Array<QuickPickItem> =  nuspecFiles.map((filePath) => {
                return {
                    label: path.basename(filePath.fsPath),
                    description: path.dirname(filePath.fsPath)
                };
            });

            if(quickPickItems.length > 1) {
                quickPickItems.unshift({label: "All nuspec files"});
            }

            window.showQuickPick(quickPickItems, {
                placeHolder: "Available nuspec files..."
              }).then((nuspecSelection) => {
                if(!nuspecSelection) {
                    return;
                }

                window.showInputBox({
                    prompt: "Additional command arguments?"
                }).then((additionalArguments) => {
                    if(nuspecSelection.label === "All nuspec files") {
                        quickPickItems.forEach((quickPickItem) => {
                            if(!additionalArguments || additionalArguments === "") {
                                additionalArguments = "";
                            }

                            if(quickPickItem.label === "All nuspec files") {
                                return;
                            }

                            let cwd: string = quickPickItem.description ? quickPickItem.description : "";
                            // tslint:disable-next-line:max-line-length
                            let packOp: ChocolateyOperation = new ChocolateyOperation(["pack", quickPickItem.label, additionalArguments], { isOutputChannelVisible: true, currentWorkingDirectory: cwd });
                            packOp.run();
                        });
                    } else {
                        if(!additionalArguments || additionalArguments === "") {
                            additionalArguments = "";
                        }

                        let cwd: string = nuspecSelection.description ? nuspecSelection.description : "";
                        // tslint:disable-next-line:max-line-length
                        let packOp: ChocolateyOperation = new ChocolateyOperation(["pack", nuspecSelection.label, additionalArguments], { isOutputChannelVisible: true, currentWorkingDirectory: cwd });
                        packOp.run();
                    }
                });
            });
        });
    }
}
