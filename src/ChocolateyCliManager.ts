import { window, QuickPickItem, workspace } from "vscode";
import { ChocolateyOperation } from "./ChocolateyOperation";
import * as path from "path";
import * as xml2js from "xml2js";
import * as fs from "fs";
import { getPathToChocolateyConfig, getPathToChocolateyTemplates } from "./config";

export class ChocolateyCliManager {
    public new(uri: Uri | undefined): void {
        window.showInputBox({
            prompt: "Name for new Chocolatey Package?"
        }).then((result) => {
            if (!result || result === "") {
                return;
            }

            let availableTemplates : Array<QuickPickItem> = this._findPackageTemplates().map((filepath) => {
                return {
                    label: path.basename(filepath),

                };
            });

            if (availableTemplates.length > 0) {
                availableTemplates.unshift({label: "Default Template" });
                window.showQuickPick(availableTemplates, {
                    placeHolder: "Available templates"
                }).then(template => {
                    let chocoArguments: Array<string> = ["new", result];

                    if (template && template.label !== "Default Template") {
                        chocoArguments.push(`--template-name="'${template.label}'"`);
                    }

                    if (uri && this._isDirectory(uri.fsPath)) {
                        chocoArguments.push(`--output-directory="'${uri.fsPath}'"`)
                    }

                    let chocoProperties = readChocoProperties();
                    if (chocoProperties) {
                        for (let property of chocoProperties) {
                            chocoArguments.push(`"${property.key}=${property.value}"`);
                        }
                    }

                    let newOp: ChocolateyOperation = new ChocolateyOperation(chocoArguments);
                    newOp.run();
                });
            } else {
                let newOp: ChocolateyOperation = new ChocolateyOperation(["new", result]);
                newOp.run();
            }
        });

        function readChocoProperties() {
            const config = workspace.getConfiguration("chocolatey.new");

            let result = new Array<{key:string,value:string}>();
            if (config === undefined) { return result;}

            const properties = config.get("properties");
            if (properties === undefined) { return result; }

            for (const key in properties) {
                result.push({key: key, value: properties[key] });
            }

            return result;
        }
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

                if(!additionalArguments || additionalArguments === "") {
                    additionalArguments = "";
                }

                chocolateyArguments.push(additionalArguments);

                if(allPackages) {
                    packages.forEach((packageToPush) => {
                        if(packageToPush.label === "All nupkg files") {
                            return;
                        }

                        let cwd: string = packageToPush.description ? packageToPush.description : "";
                        chocolateyArguments.unshift(packageToPush.label);
                        chocolateyArguments.unshift("push");

                        // tslint:disable-next-line:max-line-length
                        let pushOp: ChocolateyOperation = new ChocolateyOperation(chocolateyArguments, { isOutputChannelVisible: true, currentWorkingDirectory: cwd });
                        pushOp.run();

                        // remove the first three arguments.  These will be replaced in next iteration
                        chocolateyArguments.splice(0, 3);
                    });
                } else {
                    let cwd: string = selectedNupkg.description ? selectedNupkg.description : "";
                    chocolateyArguments.unshift(selectedNupkg.label);
                    chocolateyArguments.unshift("push");

                    // tslint:disable-next-line:max-line-length
                    let pushOp: ChocolateyOperation = new ChocolateyOperation(chocolateyArguments, { isOutputChannelVisible: true, currentWorkingDirectory: cwd });
                    pushOp.run();
                }
            });
        }

        function getCustomSource(quickPickItems: Array<QuickPickItem>, nupkgSelection: QuickPickItem): void {
            // need to get user to specify source
            window.showInputBox({
                prompt: "Source to push package(s) to..."
            }).then((specifiedSource) => {
                window.showInputBox({
                    prompt: "API Key for Source (if required)..."
                }).then((specifiedApiKey) => {
                    if(!specifiedSource) {
                        return;
                    }

                    // tslint:disable-next-line:max-line-length
                    pushPackage(quickPickItems, nupkgSelection, nupkgSelection.label === "All nupkg files", specifiedSource, specifiedApiKey === undefined ? "" : specifiedApiKey);
                });
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

                    if(result.chocolatey.apiKeys[0].apiKeys) {
                        result.chocolatey.apiKeys[0].apiKeys.forEach((apiKey  => {
                            sourceQuickPickItems.push({
                                    label: apiKey.$.source,
                                });
                        }));
                    }

                    if(sourceQuickPickItems.length === 0) {
                        getCustomSource(quickPickItems, nupkgSelection);
                    } else {
                        if(sourceQuickPickItems.length > 0) {
                            sourceQuickPickItems.unshift({label: "Use custom source..."});
                        }

                        window.showQuickPick(sourceQuickPickItems, {
                            placeHolder: "Select configured source..."
                        }).then((sourceSelection) => {
                            if(!sourceSelection || !sourceSelection.label) {
                                return;
                            }

                            if(sourceSelection.label === "Use custom source...") {
                                getCustomSource(quickPickItems, nupkgSelection);
                            } else {
                                // tslint:disable-next-line:max-line-length
                                pushPackage(quickPickItems, nupkgSelection, nupkgSelection.label === "All nupkg files", sourceSelection.label, "");
                            }
                        });
                    }
                });
            });
        });
    }

    public installTemplates(): void {
        const config = workspace.getConfiguration("chocolatey").templatePackages;

        let chocoArguments: Array<string> = ["install"];

        config.names.forEach((name) => {
            chocoArguments.push(name);
        });

        chocoArguments.push(`--source="'${config.source}'"`);

        let installTemplatesOp: ChocolateyOperation = new ChocolateyOperation(chocoArguments);
        installTemplatesOp.run();
    }

    public apikey(): void {
        window.showInputBox({
            prompt: "API Key..."
        }).then((apiKey) => {
            if(!apiKey) {
                return;
            }

            window.showInputBox({
                prompt: "Source..."
            }).then((source) => {
                if(!source) {
                    return;
                }

                let chocolateyArguments: string[] = [];

                chocolateyArguments.push("-k=\"'" + apiKey + "'\"");
                chocolateyArguments.push("-s=\"'" + source + "'\"");

                let apiOp: ChocolateyOperation = new ChocolateyOperation(chocolateyArguments);
                apiOp.run()
            });
        });
    }

    private _findPackageTemplates(): string[] {
        let templateDir = getPathToChocolateyTemplates();

        if (!templateDir || !fs.existsSync(templateDir) || !this._isDirectory(templateDir)) {
            return [];
        }

        return fs.readdirSync(templateDir).map(name => path.join(templateDir, name)).filter(this._isDirectory);
    }

    private _isDirectory(path: string) {
        return fs.lstatSync(path).isDirectory();
    }
}
