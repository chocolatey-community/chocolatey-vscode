import { window, QuickPickItem, workspace } from "vscode";
import { ChocolateyOperation } from "./ChocolateyOperation";
import * as path from "path";
import * as xml2js from "xml2js";
import * as fs from "fs";
import { getPathToChocolateyConfig } from "./config";

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

    public push(): void {
        // tslint:disable-next-line:max-line-length
        function pushPackage(packages: Array<QuickPickItem>, selectedNupkg: QuickPickItem, allPackages: boolean, source: string, apikey: string): void {
            window.showInputBox({
                prompt: "Additional command arguments?"
            }).then((additionalArguments) => {
                let chocolateyArguments: string[] = [];
                if(source) {
                    chocolateyArguments.push("--source=\"'" + source + "'\"");
                }

                if(apikey) {
                    chocolateyArguments.push("--api-key=\"'" + apikey + "'\"");
                }

                if(allPackages) {
                    packages.forEach((packageToPush) => {
                        if(!additionalArguments || additionalArguments === "") {
                            additionalArguments = "";
                        }

                        let cwd: string = packageToPush.description ? packageToPush.description : "";
                        chocolateyArguments.unshift(packageToPush.label);
                        chocolateyArguments.unshift("push");
                        chocolateyArguments.push(additionalArguments);

                        // tslint:disable-next-line:max-line-length
                        let pushOp: ChocolateyOperation = new ChocolateyOperation(chocolateyArguments, { isOutputChannelVisible: true, currentWorkingDirectory: cwd });
                        pushOp.run();
                    });
                } else {
                    if(!additionalArguments || additionalArguments === "") {
                        additionalArguments = "";
                    }
                    let cwd: string = selectedNupkg.description ? selectedNupkg.description : "";
                    chocolateyArguments.unshift(selectedNupkg.label);
                    chocolateyArguments.unshift("push");
                    chocolateyArguments.push(additionalArguments);

                    // tslint:disable-next-line:max-line-length
                    let pushOp: ChocolateyOperation = new ChocolateyOperation(chocolateyArguments, { isOutputChannelVisible: true, currentWorkingDirectory: cwd });
                    pushOp.run();
                }
            });
        }

        workspace.findFiles("**/*.nupkg").then((nupkgFiles) => {
            if(nupkgFiles.length ===0) {
                window.showErrorMessage("There are no nupkg files in the current workspace.");
                return;
            }

            let quickPickItems: Array<QuickPickItem> =  nupkgFiles.map((filePath) => {
                return {
                    label: path.basename(filePath.fsPath),
                    description: path.dirname(filePath.fsPath)
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

                let parser: xml2js.Parser = new xml2js.Parser();
                const contents: string = fs.readFileSync(getPathToChocolateyConfig()).toString();
                parser.parseString(contents, function(err: any, result: any): void {
                    if(err) {
                        console.log(err);
                        return;
                    }

                    let sourceQuickPickItems: Array<QuickPickItem> = new Array<QuickPickItem>();

                    result.chocolatey.sources[0].source.forEach((source  => {
                        sourceQuickPickItems.push({
                                label: source.$.id,
                                description: source.$.value
                            });console.log(source);
                    }));

                    if(sourceQuickPickItems.length === 0) {
                        // need to get user to specify source
                        window.showInputBox({
                            prompt: "Source URL"
                        }).then((specifiedSource) => {
                            window.showInputBox({
                                prompt: "API Key"
                            }).then((specifiedApiKey) => {
                                if(!specifiedSource || !specifiedApiKey) {
                                    return;
                                }

                                // tslint:disable-next-line:max-line-length
                                pushPackage(quickPickItems, nupkgSelection, nupkgSelection.label === "All nupkg files", specifiedSource, specifiedApiKey);
                            });
                        });
                    } else {
                        window.showQuickPick(sourceQuickPickItems, {
                            placeHolder: "Select configured source..."
                        }).then((sourceSelection) => {
                            if(!sourceSelection || !sourceSelection.description) {
                                return;
                            }

                            // tslint:disable-next-line:max-line-length
                            pushPackage(quickPickItems, nupkgSelection, nupkgSelection.label === "All nupkg files", sourceSelection.description, "");
                        });
                    }
                });
            });
        });
    }
}
