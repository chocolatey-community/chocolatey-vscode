import { window } from "vscode";

import { ChocolateyOperation } from "./ChocolateyOperation";

export class ChocolateyCliManager {
    public new(): void {
        window.showInputBox({
            prompt: "Name for new Chocolatey Package?"
        }).then((result) => {
            if (!result || result === "") {
                return;
            }

            let newOp: ChocolateyOperation = new ChocolateyOperation(["new", result]);
            newOp.run();
        });
    }
}