import { workspace } from "vscode";
import * as path from "path";

export function getFullAppPath(): string {
    if (workspace.rootPath) {
        return path.join(workspace.rootPath, "./");
    }

    return "";
}

export function getPathToChocolateyBin(): string {
    let chocolateyInstallEnvironmentVariable = process.env.ChocolateyInstall || process.env["ChocolateyInstall"];

    return path.join(chocolateyInstallEnvironmentVariable, "bin/choco.exe");
}