import { window, QuickPickItem, RelativePattern, workspace, Uri } from "vscode";
import { ChocolateyOperation } from "./ChocolateyOperation";
import * as path from "path";
import * as xml2js from "xml2js";
import * as fs from "fs";
import { getPathToChocolateyConfig, getPathToChocolateyTemplates } from "./config";

const ALL_NUSPEC_LABEL = "All nuspec files";
const ALL_NUPKG_LABEL = "All nupkg files";
const DEFAULT_TEMPLATE_LABEL = "Default Template";
const CUSTOM_SOURCE_LABEL = "Use custom source...";

export class ChocolateyCliManager {
    public async new(uri: Uri | undefined): Promise<void> {
        const result = await window.showInputBox({
            prompt: "Name for new Chocolatey Package?"
        });
        if (!result) {
            return;
        }

        const availableTemplates: Array<QuickPickItem> = this._findPackageTemplates().map((filepath) => {
            return { label: path.basename(filepath) };
        });

        const chocoArguments: Array<string> = ["new", result];

        if (availableTemplates.length > 0) {
            availableTemplates.unshift({ label: DEFAULT_TEMPLATE_LABEL });
            const template = await window.showQuickPick(availableTemplates, {
                placeHolder: "Available templates"
            });
            if (template && template.label !== DEFAULT_TEMPLATE_LABEL) {
                chocoArguments.push(`--template-name="'${template.label}'"`);
            }
        }

        if (uri && this._isDirectory(uri.fsPath)) {
            chocoArguments.push(`--output-directory="'${uri.fsPath}'"`);
        }

        for (const { key, value } of readChocoProperties()) {
            chocoArguments.push(`"${key}=${value}"`);
        }

        new ChocolateyOperation(chocoArguments).run();

        function readChocoProperties(): Array<{ key: string, value: string }> {
            const config = workspace.getConfiguration("chocolatey.commands.new");
            const result: Array<{ key: string, value: string }> = [];
            const properties = config?.get<Record<string, string> | undefined>("properties");
            if (!properties) {
                return result;
            }
            for (const key in properties) {
                result.push({ key, value: properties[key] });
            }
            return result;
        }
    }

    public async pack(uri?: Uri): Promise<void> {
        let quickPickItems: Array<QuickPickItem>;
        let nuspecSelection: QuickPickItem | undefined;

        // Context menu on a specific .nuspec file: pack it directly, no
        // quick-pick.  Matches the user intent from Issue GH-131.
        if (uri && this._isFile(uri.fsPath) && uri.fsPath.toLowerCase().endsWith(".nuspec")) {
            nuspecSelection = this._toQuickPickItem(uri.fsPath);
            quickPickItems = [nuspecSelection];
        } else {
            // Either no URI (command palette) or a folder URI (context menu
            // on a folder).  In both cases we still show a quick-pick, but
            // the search is scoped to the folder when one was supplied.
            const nuspecFiles = await this._findFilesByExtension("nuspec", uri);
            if (nuspecFiles.length === 0) {
                window.showErrorMessage(this._noFilesMessage("nuspec", uri));
                return;
            }

            quickPickItems = nuspecFiles.map((filePath) => this._toQuickPickItem(filePath.fsPath));

            if (quickPickItems.length > 1) {
                quickPickItems.unshift({ label: ALL_NUSPEC_LABEL });
            }

            nuspecSelection = await window.showQuickPick(quickPickItems, {
                placeHolder: "Available nuspec files..."
            });
            if (!nuspecSelection) {
                return;
            }
        }

        const additionalArguments = (await window.showInputBox({
            prompt: "Additional command arguments?"
        })) ?? "";

        if (nuspecSelection.label === ALL_NUSPEC_LABEL) {
            for (const item of quickPickItems) {
                if (item.label === ALL_NUSPEC_LABEL) {
                    continue;
                }
                this._runPack(item, additionalArguments);
            }
        } else {
            this._runPack(nuspecSelection, additionalArguments);
        }
    }

    public async push(uri?: Uri): Promise<void> {
        let quickPickItems: Array<QuickPickItem>;
        let nupkgSelection: QuickPickItem | undefined;

        // Context menu on a specific .nupkg file: push it directly, no
        // quick-pick.  Matches the user intent from Issue GH-132.
        if (uri && this._isFile(uri.fsPath) && uri.fsPath.toLowerCase().endsWith(".nupkg")) {
            nupkgSelection = this._toQuickPickItem(uri.fsPath);
            quickPickItems = [nupkgSelection];
        } else {
            // Either no URI (command palette) or a folder URI (context menu
            // on a folder).  In both cases we still show a quick-pick, but
            // the search is scoped to the folder when one was supplied.
            const nupkgFiles = await this._findFilesByExtension("nupkg", uri);
            if (nupkgFiles.length === 0) {
                window.showErrorMessage(this._noFilesMessage("nupkg", uri));
                return;
            }

            quickPickItems = nupkgFiles.map((filePath) => this._toQuickPickItem(filePath.fsPath));

            if (quickPickItems.length > 1) {
                quickPickItems.unshift({ label: ALL_NUPKG_LABEL });
            }

            nupkgSelection = await window.showQuickPick(quickPickItems, {
                placeHolder: "Available nupkg files..."
            });
            if (!nupkgSelection) {
                return;
            }
        }

        const configuredSources = await this._readConfiguredSources();
        let source: string;
        let apiKey: string;

        if (configuredSources.length === 0) {
            const custom = await this._promptForCustomSource();
            if (!custom) {
                return;
            }
            source = custom.source;
            apiKey = custom.apiKey;
        } else {
            const sourceItems: Array<QuickPickItem> = [
                { label: CUSTOM_SOURCE_LABEL },
                ...configuredSources.map((s) => ({ label: s }))
            ];
            const sourceSelection = await window.showQuickPick(sourceItems, {
                placeHolder: "Select configured source..."
            });
            if (!sourceSelection?.label) {
                return;
            }

            if (sourceSelection.label === CUSTOM_SOURCE_LABEL) {
                const custom = await this._promptForCustomSource();
                if (!custom) {
                    return;
                }
                source = custom.source;
                apiKey = custom.apiKey;
            } else {
                source = sourceSelection.label;
                apiKey = "";
            }
        }

        const additionalArguments = (await window.showInputBox({
            prompt: "Additional command arguments?"
        })) ?? "";

        const allPackages = nupkgSelection.label === ALL_NUPKG_LABEL;
        if (allPackages) {
            for (const item of quickPickItems) {
                if (item.label === ALL_NUPKG_LABEL) {
                    continue;
                }
                this._runPush(item, source, apiKey, additionalArguments);
            }
        } else {
            this._runPush(nupkgSelection, source, apiKey, additionalArguments);
        }
    }

    public installTemplates(): void {
        const config = workspace.getConfiguration("chocolatey").get<{ names: string[]; source: string }>("templates");
        if (!config) {
            return;
        }

        const chocoArguments: Array<string> = ["install", ...config.names];
        chocoArguments.push(`--source="'${config.source}'"`);

        new ChocolateyOperation(chocoArguments).run();
    }

    public async apikey(): Promise<void> {
        const apiKey = await window.showInputBox({ prompt: "API Key..." });
        if (!apiKey) {
            return;
        }

        const source = await window.showInputBox({ prompt: "Source..." });
        if (!source) {
            return;
        }

        const chocolateyArguments: string[] = [
            `-k="'${apiKey}'"`,
            `-s="'${source}'"`
        ];

        new ChocolateyOperation(chocolateyArguments).run();
    }

    private _runPack(item: QuickPickItem, additionalArguments: string): void {
        const cwd = item.description ?? "";
        new ChocolateyOperation(
            ["pack", item.label, additionalArguments],
            { isOutputChannelVisible: true, currentWorkingDirectory: cwd }
        ).run();
    }

    private _runPush(item: QuickPickItem, source: string, apiKey: string, additionalArguments: string): void {
        const cwd = item.description ?? "";
        const chocolateyArguments: string[] = ["push", item.label];
        if (source) {
            chocolateyArguments.push(`--source="'${source}'"`);
        }
        if (apiKey) {
            chocolateyArguments.push(`--api-key="'${apiKey}'"`);
        }
        if (additionalArguments) {
            chocolateyArguments.push(additionalArguments);
        }
        new ChocolateyOperation(
            chocolateyArguments,
            { isOutputChannelVisible: true, currentWorkingDirectory: cwd }
        ).run();
    }

    private async _promptForCustomSource(): Promise<{ source: string, apiKey: string } | undefined> {
        const source = await window.showInputBox({ prompt: "Source to push package(s) to..." });
        if (!source) {
            return undefined;
        }
        const apiKey = (await window.showInputBox({ prompt: "API Key for Source (if required)..." })) ?? "";
        return { source, apiKey };
    }

    private async _readConfiguredSources(): Promise<string[]> {
        const configPath = getPathToChocolateyConfig();
        if (!configPath || !fs.existsSync(configPath)) {
            return [];
        }
        const contents = fs.readFileSync(configPath).toString();
        try {
            const parsed: any = await xml2js.parseStringPromise(contents);
            const apiKeyEntries = parsed?.chocolatey?.apiKeys?.[0]?.apiKeys;
            if (!apiKeyEntries) {
                return [];
            }
            return apiKeyEntries.map((entry: any) => entry.$.source as string);
        } catch (err) {
            console.error("Failed to parse chocolatey.config:", err);
            return [];
        }
    }

    private _findPackageTemplates(): string[] {
        const templateDir = getPathToChocolateyTemplates();

        if (!templateDir || !fs.existsSync(templateDir) || !this._isDirectory(templateDir)) {
            return [];
        }

        return fs.readdirSync(templateDir)
            .map((name) => path.join(templateDir, name))
            .filter((p) => this._isDirectory(p));
    }

    private _isDirectory(p: string): boolean {
        try {
            return fs.lstatSync(p).isDirectory();
        } catch {
            return false;
        }
    }

    private _isFile(p: string): boolean {
        try {
            return fs.lstatSync(p).isFile();
        } catch {
            return false;
        }
    }

    private _toQuickPickItem(fsPath: string): QuickPickItem {
        return {
            label: path.basename(fsPath),
            description: path.dirname(fsPath)
        };
    }

    /**
     * Finds *.{extension} files in the workspace, optionally scoped to
     * the folder represented by `uri`.  When `uri` points at a directory
     * the glob is applied relative to that directory; otherwise the search
     * covers the whole workspace.
     */
    private async _findFilesByExtension(extension: "nuspec" | "nupkg", uri?: Uri): Promise<Uri[]> {
        const glob = `**/*.${extension}`;
        if (uri && this._isDirectory(uri.fsPath)) {
            return Array.from(await workspace.findFiles(new RelativePattern(uri.fsPath, glob)));
        }
        return Array.from(await workspace.findFiles(glob));
    }

    private _noFilesMessage(extension: "nuspec" | "nupkg", uri?: Uri): string {
        const scope = uri ? `'${uri.fsPath}'` : "the current workspace";
        return `There are no ${extension} files in ${scope}.`;
    }
}
