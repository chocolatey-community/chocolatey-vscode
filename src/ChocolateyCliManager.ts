import { window } from "vscode";

import { ChocolateyOperation } from "./ChocolateyOperation";

export class ChocolateyCliManager {
    constructor() {

    }

    public new() {
        window.showInputBox({
            prompt: "Name for new Chocolatey Package?"
        }).then((result) => {
            if (!result || result === "") {
                return;
            };

            let newOp = new ChocolateyOperation(["new", result]);
            newOp.run();
        });
    }
}