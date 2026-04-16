import { expect } from "chai";
import * as path from "path";
import * as vscode from "vscode";
import {
    getWorkspaceRoot,
    getPathToChocolateyBin,
    getPathToChocolateyConfig,
    getPathToChocolateyTemplates
} from "../../config";

describe("config", () => {
    const ORIGINAL_ENV: string | undefined = process.env.ChocolateyInstall;
    const FAKE_CHOCO: string = path.join("C", "fake", "choco");

    afterEach(() => {
        if (ORIGINAL_ENV === undefined) {
            delete process.env.ChocolateyInstall;
        } else {
            process.env.ChocolateyInstall = ORIGINAL_ENV;
        }
    });

    describe("getWorkspaceRoot", () => {
        it("returns a non-empty path rooted in the open workspace", () => {
            // The test runner opens src/test/fixtures/workspace as the workspace root.
            const appPath: string = getWorkspaceRoot();
            expect(appPath).to.be.a("string");
            expect(appPath.length).to.be.greaterThan(0);
            // Should match whatever VS Code thinks the first workspace folder is.
            const folder = vscode.workspace.workspaceFolders?.[0];
            if (folder) {
                expect(path.normalize(appPath)).to.equal(path.normalize(path.join(folder.uri.fsPath, "./")));
            }
        });
    });

    describe("getPathToChocolateyBin", () => {
        it("returns an empty string when ChocolateyInstall is not set", () => {
            delete process.env.ChocolateyInstall;
            expect(getPathToChocolateyBin()).to.equal("");
        });

        it("joins ChocolateyInstall with bin/choco.exe when set", () => {
            process.env.ChocolateyInstall = FAKE_CHOCO;
            expect(getPathToChocolateyBin()).to.equal(path.join(FAKE_CHOCO, "bin/choco.exe"));
        });
    });

    describe("getPathToChocolateyConfig", () => {
        it("returns an empty string when ChocolateyInstall is not set", () => {
            delete process.env.ChocolateyInstall;
            expect(getPathToChocolateyConfig()).to.equal("");
        });

        it("joins ChocolateyInstall with config/chocolatey.config when set", () => {
            process.env.ChocolateyInstall = FAKE_CHOCO;
            expect(getPathToChocolateyConfig()).to.equal(path.join(FAKE_CHOCO, "config/chocolatey.config"));
        });
    });

    describe("getPathToChocolateyTemplates", () => {
        it("returns an empty string when ChocolateyInstall is not set", () => {
            delete process.env.ChocolateyInstall;
            expect(getPathToChocolateyTemplates()).to.equal("");
        });

        it("joins ChocolateyInstall with templates when set", () => {
            process.env.ChocolateyInstall = FAKE_CHOCO;
            expect(getPathToChocolateyTemplates()).to.equal(path.join(FAKE_CHOCO, "templates"));
        });
    });
});
