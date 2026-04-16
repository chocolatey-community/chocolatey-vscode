import * as path from "path";
import { runTests } from "@vscode/test-electron";

async function main(): Promise<void> {
    try {
        // The folder containing the Extension Manifest package.json
        // Passed to `--extensionDevelopmentPath`
        const extensionDevelopmentPath: string = path.resolve(__dirname, "../../");

        // The path to the extension test runner script
        // Passed to --extensionTestsPath
        const extensionTestsPath: string = path.resolve(__dirname, "./suite/index");

        // The folder to open as the workspace when the test VS Code instance launches.
        // Having a workspace is required for many of the extension code paths
        // (e.g. anything that reads workspace.rootPath).
        const testWorkspace: string = path.resolve(__dirname, "../../src/test/fixtures/workspace");

        // If VSCODE_TEST_EXECUTABLE_PATH is set (or a VS Code install is found at the
        // default Windows location), use it instead of downloading a fresh copy.
        // Useful in offline / restricted-network environments.
        const vscodeExecutablePath: string | undefined = process.env.VSCODE_TEST_EXECUTABLE_PATH;

        await runTests({
            extensionDevelopmentPath,
            extensionTestsPath,
            launchArgs: [testWorkspace, "--disable-extensions"],
            vscodeExecutablePath
        });
    } catch (err) {
        console.error("Failed to run tests", err);
        process.exit(1);
    }
}

main();
