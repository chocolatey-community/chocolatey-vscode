import {window, commands} from "vscode";
import * as chocolateyCli from "./ChocolateyCliManager";
import * as chocolateyOps from "./ChocolateyOperation";

var chocolateyManager : chocolateyCli.ChocolateyCliManager;
var installed : boolean = false;

export function activate() {
    // Register Commands
    commands.registerCommand("chocolatey.new", 		 () => execute("new"));
}

function execute(cmd?: string, arg?: Array<any>) {
    if (!chocolateyManager) {
        chocolateyManager = new chocolateyCli.ChocolateyCliManager();
    }

    if (!installed) {
        installed = chocolateyOps.isChocolateyCliInstalled();
    }

    if (!cmd) {
        return;
    }

    // Ensure Chocolatey is installed
    if (!installed) {
        return window.showErrorMessage('Chocolatey is not installed');
    };

    let ecmd = chocolateyManager[cmd];
    if (typeof ecmd === 'function') {
        try {
            ecmd.apply(chocolateyManager, arg);
        } catch (e) {
            // Well, clearly we didn't call a function
            console.log(e);
        }
    }
}