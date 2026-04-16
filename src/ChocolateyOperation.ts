import { OutputChannel, window } from "vscode";
import * as cp from "child_process";
import * as os from "os";

import { capitalizeFirstLetter } from "./helpers";
import { getWorkspaceRoot, getPathToChocolateyBin } from "./config";

export interface IChocolateyOperationResult {
    code: number | null;
    stdout: Array<string>;
    stderr: Array<string>;
}

export class ChocolateyOperation {
    private _oc: OutputChannel | undefined;
    private _process: cp.ChildProcess | undefined;
    private _isOutputChannelVisible: boolean;
    private _currentWorkingDirectory: string;
    private _stdout: Array<string> = [];
    private _stderr: Array<string> = [];

    public cmd: Array<string>;

    constructor(
        cmd: string | Array<string>,
        options: { isOutputChannelVisible: boolean; currentWorkingDirectory: string } = {
            isOutputChannelVisible: true,
            currentWorkingDirectory: getWorkspaceRoot()
        }
    ) {
        this.cmd = Array.isArray(cmd) ? cmd : [cmd];
        this._isOutputChannelVisible = options.isOutputChannelVisible;
        this._currentWorkingDirectory = options.currentWorkingDirectory;
    }

    public getStdout(): string[] {
        return this._stdout;
    }

    public getStderr(): string[] {
        return this._stderr;
    }

    public showOutputChannel(): void {
        if (this._oc) {
            this._oc.show();
            this._isOutputChannelVisible = true;
        }
    }

    public hideOutputChannel(): void {
        if (this._oc) {
            this._oc.dispose();
            this._oc.hide();
            this._isOutputChannelVisible = false;
        }
    }

    public kill(): void {
        if (this._process) {
            this._process.kill();
        }
    }

    public run(): Promise<IChocolateyOperationResult> {
        return new Promise<IChocolateyOperationResult>((resolve, reject) => {
            if (!getWorkspaceRoot()) {
                return reject(new Error("No workspace folder is open."));
            }

            const chocolateyPath = getPathToChocolateyBin();
            const cwd = this._currentWorkingDirectory || getWorkspaceRoot();

            this._oc = window.createOutputChannel(`Chocolatey: ${capitalizeFirstLetter(this.cmd[0])}`);

            if (os.platform() === "win32") {
                this._process = cp.spawn("powershell.exe", [chocolateyPath, ...this.cmd], {
                    cwd,
                    stdio: ["ignore", "pipe", "pipe"]
                });
            } else {
                this._process = cp.spawn(chocolateyPath, this.cmd, { cwd });
            }

            this._oc.appendLine("Building...");

            if (this._isOutputChannelVisible) {
                this._oc.show();
            }

            this._process.stdout?.on("data", (data: Buffer | string) => {
                const out = data.toString();
                this._oc?.appendLine(out);
                this._stdout.push(out);
            });

            this._process.stderr?.on("data", (data: Buffer | string) => {
                const out = data.toString();
                this._oc?.appendLine(out);
                this._stderr.push(out);
            });

            this._process.on("close", (code: number | null) => {
                this._oc?.appendLine(`Chocolatey ${this.cmd[0]} process exited with code ${code}`);
                resolve({
                    code,
                    stderr: this._stderr,
                    stdout: this._stdout
                });
            });
        });
    }

    public dispose(): void {
        if (this._oc) {
            this._oc.dispose();
        }
        if (this._process) {
            this._process.kill();
        }
    }
}

export function isChocolateyCliInstalled(): boolean {
    const chocolateyBin = getPathToChocolateyBin();
    if (!chocolateyBin) {
        return false;
    }

    try {
        cp.execSync(`${chocolateyBin} -v`, { cwd: getWorkspaceRoot() });
        return true;
    } catch {
        return false;
    }
}
