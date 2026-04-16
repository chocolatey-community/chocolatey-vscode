import * as path from "path";
import { runTests } from "@vscode/test-electron";

// Pin the VS Code version used by the integration test host.  Two reasons:
//   1. Reproducibility: every CI run exercises the same VS Code build, so a
//      flaky test is the extension's fault rather than a host-side regression.
//   2. Reliability: passing an explicit version lets @vscode/test-electron
//      download directly from https://update.code.visualstudio.com/<VERSION>/...
//      instead of first calling /api/releases/stable?released=true to discover
//      the newest build.  That API endpoint is routinely rate-limited from
//      GitHub Actions IP ranges and intermittently returns HTML instead of
//      JSON, which manifests as "Failed to parse response ... as JSON".
// Must be >= the engine floor declared in package.json.  Bump occasionally.
const VSCODE_TEST_VERSION: string = "1.96.4";

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
        // (e.g. anything that reads workspace.workspaceFolders).
        const testWorkspace: string = path.resolve(__dirname, "../../src/test/fixtures/workspace");

        // If VSCODE_TEST_EXECUTABLE_PATH is set, use that VS Code install
        // instead of downloading a fresh copy.  Useful in offline /
        // restricted-network environments.  When it is set, VSCODE_TEST_VERSION
        // is ignored by @vscode/test-electron.
        const vscodeExecutablePath: string | undefined = process.env.VSCODE_TEST_EXECUTABLE_PATH;

        await runTests({
            extensionDevelopmentPath,
            extensionTestsPath,
            launchArgs: [testWorkspace, "--disable-extensions"],
            vscodeExecutablePath,
            version: vscodeExecutablePath ? undefined : VSCODE_TEST_VERSION
        });
    } catch (err) {
        console.error("Failed to run tests", err);
        process.exit(1);
    }
}

main();
