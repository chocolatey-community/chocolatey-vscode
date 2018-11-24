import { OutputChannel, window, workspace } from "vscode";
import * as cp from "child_process";
import * as os from "os";

import { capitalizeFirstLetter } from "./helpers";
import { getFullAppPath, getPathToChocolateyBin } from "./config";

export interface IChocolateyOperationResult {
    code: Number;
    stdout: Array<string>;
    stderr: Array<string>;
}

export class ChocolateyOperation {
    private _spawn = cp.spawn;
    private _oc!: OutputChannel;
    private _process!: cp.ChildProcess;
    private _isOutputChannelVisible: boolean;
    private _currentWorkingDirectory: string;
    private _stdout: Array<string> = [];
    private _stderr: Array<string> = [];

    public cmd: Array<string>;
    public created: boolean;

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

    public run(): Promise<{}> {
        return new Promise((resolve, reject) => {
            if (!workspace || !workspace.rootPath) {
                return reject();
            }

            let lastOut: string = "";
            let chocolateyPath: string = getPathToChocolateyBin();

            this._oc = window.createOutputChannel(`Chocolatey: ${capitalizeFirstLetter(this.cmd[0])}`);

            if (os.platform() === "win32") {
                let joinedArgs: string[] = this.cmd;
                joinedArgs.unshift(chocolateyPath);

                this._process = this._spawn("powershell.exe", joinedArgs, {
                    cwd: this._currentWorkingDirectory ? this._currentWorkingDirectory : getFullAppPath(),
                    stdio: ["ignore", "pipe", "pipe"]
                });
            } else {
                this._process = this._spawn(chocolateyPath, this.cmd, {
                    cwd: this._currentWorkingDirectory ? this._currentWorkingDirectory : getFullAppPath()
                });
            }

            this._oc.append("Building...");

            if (this._isOutputChannelVisible) {
                this._oc.show();
            }

            this._process.stdout.on("data", (data) => {
                let out: string = data.toString();

                if (lastOut && out && (lastOut + "." === out)
                    || (lastOut.slice(0, lastOut.length - 1)) === out
                    || (lastOut.slice(0, lastOut.length - 2)) === out
                    || (lastOut.slice(0, lastOut.length - 3)) === out) {
                    lastOut = out;
                    return this._oc.append(".");
                }

                this._oc.appendLine(out);
                this._stdout.push(out);
                lastOut = out;
            });

            this._process.stderr.on("data", (data) => {
				let out: string = data.toString();
                this._oc.appendLine(out);
                this._stderr.push(out);
            });

            this._process.on("close", (code) => {
                this._oc.appendLine(`Chocolatey ${this.cmd[0]} process exited with code ${code}`);

                resolve(<IChocolateyOperationResult>{
                    code: code,
                    stderr: this._stderr,
                    stdout: this._stdout
                });
            });
        });
    }

    constructor (cmd: string | Array<string>, options: { isOutputChannelVisible: boolean; currentWorkingDirectory: string } = { isOutputChannelVisible: true, currentWorkingDirectory: getFullAppPath() }) {
        this._isOutputChannelVisible = options.isOutputChannelVisible;
        this.cmd = (Array.isArray(cmd)) ? cmd : [cmd];
        this._currentWorkingDirectory = options.currentWorkingDirectory;
        this.created = true;
    }

    dispose(): void {
        if (this._oc) {
            this._oc.dispose();
        }
        if (this._process) {
            this._process.kill();
        }
    }
}

export function isChocolateyCliInstalled(): boolean {
    let chocolateyBin: string = getPathToChocolateyBin();

    try {
        let exec: Buffer = cp.execSync(`${chocolateyBin} -v`, {
            cwd: getFullAppPath()
        });

        console.log("Chocolatey is apparently installed");
        console.log(exec.toString());

        return true;
    } catch (e) {
        return false;
    }
}
