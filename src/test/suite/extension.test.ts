import { expect } from "chai";
import * as vscode from "vscode";

describe("extension activation", () => {
    const EXTENSION_ID: string = "gep13.chocolatey-vscode";

    const EXPECTED_COMMANDS: string[] = [
        "chocolatey.new",
        "chocolatey.pack",
        "chocolatey.delete",
        "chocolatey.push",
        "chocolatey.installTemplates",
        "chocolatey.apikey",
        "chocolatey.open"
    ];

    before(async () => {
        const extension: vscode.Extension<any> | undefined = vscode.extensions.getExtension(EXTENSION_ID);
        expect(extension, `extension ${EXTENSION_ID} should be present`).to.not.be.undefined;
        if (extension && !extension.isActive) {
            await extension.activate();
        }
    });

    it("is loaded and active", () => {
        const extension: vscode.Extension<any> | undefined = vscode.extensions.getExtension(EXTENSION_ID);
        expect(extension).to.not.be.undefined;
        expect(extension!.isActive).to.equal(true);
    });

    it("registers every command declared in package.json", async () => {
        const registered: string[] = await vscode.commands.getCommands(true);
        for (const cmd of EXPECTED_COMMANDS) {
            expect(registered, `command '${cmd}' should be registered`).to.include(cmd);
        }
    });
});
