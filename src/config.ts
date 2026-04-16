import { workspace } from "vscode";
import * as path from "path";

/**
 * Returns the absolute path to the first workspace folder, or an empty
 * string when no folder is open.  Previously named `getFullAppPath`,
 * which used the deprecated `workspace.rootPath` API.
 */
export function getWorkspaceRoot(): string {
    const folder = workspace.workspaceFolders?.[0];
    if (!folder) {
        return "";
    }
    return path.join(folder.uri.fsPath, "./");
}

export function getPathToChocolateyConfig(): string {
    const chocolateyInstall = process.env.ChocolateyInstall;
    if (!chocolateyInstall) {
        return "";
    }
    return path.join(chocolateyInstall, "config/chocolatey.config");
}

export function getPathToChocolateyBin(): string {
    const chocolateyInstall = process.env.ChocolateyInstall;
    if (!chocolateyInstall) {
        return "";
    }
    return path.join(chocolateyInstall, "bin/choco.exe");
}

export function getPathToChocolateyTemplates(): string {
    const chocolateyInstall = process.env.ChocolateyInstall;
    if (!chocolateyInstall) {
        console.error("Chocolatey installation path could not be found.");
        return "";
    }
    return path.join(chocolateyInstall, "templates");
}
