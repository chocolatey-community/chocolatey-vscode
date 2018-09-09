import {window, commands} from "vscode";
import * as chocolateyCli from "./ChocolateyCliManager";
import * as chocolateyOps from "./ChocolateyOperation";

var chocolateyManager : chocolateyCli.ChocolateyCliManager;
var installed : boolean = false;

export function activate(): void {
    // register Commands
    commands.registerCommand("chocolatey.new", 		 () => execute("new"));
}

function execute(cmd?: string | undefined, arg?: any[] | undefined): Thenable<string | undefined> | undefined {
    if (!chocolateyManager) {
        chocolateyManager = new chocolateyCli.ChocolateyCliManager();
    }

    if (!installed) {
        installed = chocolateyOps.isChocolateyCliInstalled();
    }

    if (!cmd) {
        return;
    }

    // ensure Chocolatey is installed
    if (!installed) {
        return window.showErrorMessage("Chocolatey is not installed");
    }

    let ecmd: any = chocolateyManager[cmd];
    if (typeof ecmd === "function") {
        try {
            ecmd.apply(chocolateyManager, arg);
            return;
        } catch (e) {
            // well, clearly we didn't call a function
            console.log(e);
            return;
        }
    }
}