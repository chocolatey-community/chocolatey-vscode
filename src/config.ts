import { workspace } from "vscode";
import * as path from "path";

export function getFullAppPath(): string {
    if (workspace.rootPath) {
        return path.join(workspace.rootPath, "./");
    }

    return "";
}

export function getPathToChocolateyConfig(): string {
    let chocolateyInstallEnvironmentVariable: string | undefined = process.env.ChocolateyInstall;

    if(chocolateyInstallEnvironmentVariable === undefined) {
        // todo: this is really an error condition, and something should be done
        return "";
    }

    return path.join(chocolateyInstallEnvironmentVariable, "config/chocolatey.config");
}

export function getPathToChocolateyBin(): string {
    let chocolateyInstallEnvironmentVariable: string | undefined = process.env.ChocolateyInstall;

    if(chocolateyInstallEnvironmentVariable === undefined) {
        // todo: this is really an error condition, and something should be done
        return "";
    }

    return path.join(chocolateyInstallEnvironmentVariable, "bin/choco.exe");
}