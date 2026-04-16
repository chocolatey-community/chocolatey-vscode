import { expect } from "chai";
import * as fs from "fs";
import * as os from "os";
import * as path from "path";
import * as sinon from "sinon";
import { RelativePattern, Uri, window, workspace, QuickPickItem } from "vscode";

import { ChocolateyCliManager } from "../../ChocolateyCliManager";
import { ChocolateyOperation } from "../../ChocolateyOperation";

/**
 * Tests that exercise the URI-branching behaviour added to pack() and
 * push() in support of GH-131 and GH-132.  Also locks in the
 * exception-safe behaviour of the _isFile / _isDirectory helpers.
 */

describe("ChocolateyCliManager helpers (safe-lstat)", () => {
    const tmpRoot = fs.mkdtempSync(path.join(os.tmpdir(), "choco-clitest-"));
    const realFile = path.join(tmpRoot, "exists.nuspec");
    const realDir = path.join(tmpRoot, "subdir");
    const missing = path.join(tmpRoot, "definitely-not-there");

    // Cast once to expose the private helpers to tests.  The helpers are
    // private by design but their behaviour needs regression coverage.
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    const mgr = new ChocolateyCliManager() as any;

    before(() => {
        fs.writeFileSync(realFile, "<package/>");
        fs.mkdirSync(realDir);
    });

    after(() => {
        // Clean up temp scratch folder.
        fs.rmSync(tmpRoot, { recursive: true, force: true });
    });

    it("_isFile returns true for an existing file", () => {
        expect(mgr._isFile(realFile)).to.equal(true);
    });

    it("_isFile returns false for an existing directory", () => {
        expect(mgr._isFile(realDir)).to.equal(false);
    });

    it("_isFile returns false for a missing path (no throw)", () => {
        expect(() => mgr._isFile(missing)).to.not.throw();
        expect(mgr._isFile(missing)).to.equal(false);
    });

    it("_isDirectory returns true for an existing directory", () => {
        expect(mgr._isDirectory(realDir)).to.equal(true);
    });

    it("_isDirectory returns false for an existing file", () => {
        expect(mgr._isDirectory(realFile)).to.equal(false);
    });

    it("_isDirectory returns false for a missing path (regression: used to throw)", () => {
        expect(() => mgr._isDirectory(missing)).to.not.throw();
        expect(mgr._isDirectory(missing)).to.equal(false);
    });
});

describe("ChocolateyCliManager.pack URI routing (GH-131)", () => {
    let sandbox: sinon.SinonSandbox;
    let runStub: sinon.SinonStub;
    let showQuickPickStub: sinon.SinonStub;
    let showInputBoxStub: sinon.SinonStub;
    let findFilesStub: sinon.SinonStub;
    let tmpRoot: string;

    beforeEach(() => {
        sandbox = sinon.createSandbox();
        // Stub ChocolateyOperation.run so we don't actually spawn choco.exe.
        // Keeping run as the stub target lets us inspect the this-value to
        // observe the ChocolateyOperation instance's cmd and cwd.
        runStub = sandbox.stub(ChocolateyOperation.prototype, "run").resolves({
            code: 0,
            stdout: [],
            stderr: []
        });

        showQuickPickStub = sandbox.stub(window, "showQuickPick");
        showInputBoxStub = sandbox.stub(window, "showInputBox").resolves("");
        findFilesStub = sandbox.stub(workspace, "findFiles");

        tmpRoot = fs.mkdtempSync(path.join(os.tmpdir(), "choco-pack-test-"));
    });

    afterEach(() => {
        sandbox.restore();
        fs.rmSync(tmpRoot, { recursive: true, force: true });
    });

    it("with a .nuspec URI, packs that file directly and skips showQuickPick", async () => {
        const nuspec = path.join(tmpRoot, "foo.nuspec");
        fs.writeFileSync(nuspec, "<package/>");
        showInputBoxStub.resolves("--extra-args");

        const mgr = new ChocolateyCliManager();
        await mgr.pack(Uri.file(nuspec));

        expect(showQuickPickStub.called, "showQuickPick must not be called when a specific .nuspec is supplied").to.equal(false);
        expect(findFilesStub.called, "findFiles must not be called when a specific .nuspec is supplied").to.equal(false);
        expect(runStub.calledOnce, "exactly one pack operation should run").to.equal(true);

        const op = runStub.thisValues[0] as ChocolateyOperation;
        expect(op.cmd).to.deep.equal(["pack", "foo.nuspec", "--extra-args"]);
    });

    it("with a folder URI, scopes findFiles to that folder via RelativePattern", async () => {
        const folder = path.join(tmpRoot, "packages");
        fs.mkdirSync(folder);
        const nuspec = path.join(folder, "bar.nuspec");
        fs.writeFileSync(nuspec, "<package/>");

        findFilesStub.resolves([Uri.file(nuspec)]);
        showQuickPickStub.resolves({ label: "bar.nuspec", description: folder } as QuickPickItem);
        showInputBoxStub.resolves("");

        const mgr = new ChocolateyCliManager();
        await mgr.pack(Uri.file(folder));

        expect(findFilesStub.calledOnce).to.equal(true);
        const firstArg = findFilesStub.firstCall.args[0];
        expect(firstArg, "findFiles should receive a RelativePattern when scoped to a folder").to.be.instanceOf(RelativePattern);
        const pattern = firstArg as RelativePattern;
        // Normalise for Windows: Uri.file() can lower-case the drive letter
        // while fs.mkdtempSync returns an upper-case one.  We only care that
        // the two paths refer to the same location.
        expect(path.normalize(pattern.baseUri.fsPath).toLowerCase()).to.equal(path.normalize(folder).toLowerCase());
        expect(pattern.pattern).to.equal("**/*.nuspec");

        expect(showQuickPickStub.calledOnce, "quick-pick shows even for a single match, so users can still cancel").to.equal(true);
        expect(runStub.calledOnce).to.equal(true);
    });

    it("with no URI, uses the workspace-wide glob string", async () => {
        findFilesStub.resolves([Uri.file(path.join(tmpRoot, "a.nuspec"))]);
        showQuickPickStub.resolves({ label: "a.nuspec", description: tmpRoot } as QuickPickItem);
        showInputBoxStub.resolves("");

        const mgr = new ChocolateyCliManager();
        await mgr.pack();

        expect(findFilesStub.calledOnce).to.equal(true);
        expect(findFilesStub.firstCall.args[0]).to.equal("**/*.nuspec");
    });

    it("with a missing URI (file has been deleted), falls back to quick-pick flow", async () => {
        const gonePath = path.join(tmpRoot, "phantom.nuspec");
        findFilesStub.resolves([]);

        const mgr = new ChocolateyCliManager();
        const showErrorStub = sandbox.stub(window, "showErrorMessage").resolves(undefined);
        await mgr.pack(Uri.file(gonePath));

        // The URI doesn't resolve to an existing file or directory, so the
        // code falls through to the generic findFiles path.  With an empty
        // result, we show the "no nuspec files" error and exit.
        expect(findFilesStub.calledOnce).to.equal(true);
        expect(showErrorStub.calledOnce).to.equal(true);
        expect(runStub.called).to.equal(false);
    });
});

describe("ChocolateyCliManager.push URI routing (GH-132)", () => {
    let sandbox: sinon.SinonSandbox;
    let runStub: sinon.SinonStub;
    let showQuickPickStub: sinon.SinonStub;
    let showInputBoxStub: sinon.SinonStub;
    let findFilesStub: sinon.SinonStub;
    let tmpRoot: string;

    beforeEach(() => {
        sandbox = sinon.createSandbox();
        runStub = sandbox.stub(ChocolateyOperation.prototype, "run").resolves({
            code: 0,
            stdout: [],
            stderr: []
        });

        showQuickPickStub = sandbox.stub(window, "showQuickPick");
        showInputBoxStub = sandbox.stub(window, "showInputBox").resolves("");
        findFilesStub = sandbox.stub(workspace, "findFiles");

        tmpRoot = fs.mkdtempSync(path.join(os.tmpdir(), "choco-push-test-"));
    });

    afterEach(() => {
        sandbox.restore();
        fs.rmSync(tmpRoot, { recursive: true, force: true });
    });

    it("with a .nupkg URI, pushes that file directly and skips findFiles", async () => {
        const nupkg = path.join(tmpRoot, "foo.1.0.0.nupkg");
        fs.writeFileSync(nupkg, "");
        // push() flow after the URI branch: source quick-pick, optional api-key
        // input, then additionalArguments input.  Stub the source pick and
        // custom source prompt.
        showQuickPickStub.onFirstCall().resolves({ label: "Use custom source..." } as QuickPickItem);
        showInputBoxStub.onFirstCall().resolves("https://example.org/api/v2");
        showInputBoxStub.onSecondCall().resolves("secret-api-key");
        showInputBoxStub.onThirdCall().resolves("");

        const mgr = new ChocolateyCliManager();
        await mgr.push(Uri.file(nupkg));

        expect(findFilesStub.called, "findFiles must not be called when a specific .nupkg is supplied").to.equal(false);
        expect(runStub.calledOnce).to.equal(true);

        const op = runStub.thisValues[0] as ChocolateyOperation;
        expect(op.cmd[0]).to.equal("push");
        expect(op.cmd[1]).to.equal("foo.1.0.0.nupkg");
        expect(op.cmd.some((a) => a.includes("https://example.org/api/v2"))).to.equal(true);
        expect(op.cmd.some((a) => a.includes("secret-api-key"))).to.equal(true);
    });

    it("with a folder URI, scopes findFiles to that folder via RelativePattern", async () => {
        const folder = path.join(tmpRoot, "out");
        fs.mkdirSync(folder);
        const nupkg = path.join(folder, "baz.1.0.0.nupkg");
        fs.writeFileSync(nupkg, "");

        findFilesStub.resolves([Uri.file(nupkg)]);
        showQuickPickStub.onFirstCall().resolves({ label: "baz.1.0.0.nupkg", description: folder } as QuickPickItem);
        showQuickPickStub.onSecondCall().resolves({ label: "Use custom source..." } as QuickPickItem);
        showInputBoxStub.onFirstCall().resolves("https://example.org/");
        showInputBoxStub.onSecondCall().resolves("");
        showInputBoxStub.onThirdCall().resolves("");

        const mgr = new ChocolateyCliManager();
        await mgr.push(Uri.file(folder));

        expect(findFilesStub.calledOnce).to.equal(true);
        const firstArg = findFilesStub.firstCall.args[0];
        expect(firstArg).to.be.instanceOf(RelativePattern);
        const pattern = firstArg as RelativePattern;
        // Normalise for Windows: Uri.file() can lower-case the drive letter
        // while fs.mkdtempSync returns an upper-case one.  We only care that
        // the two paths refer to the same location.
        expect(path.normalize(pattern.baseUri.fsPath).toLowerCase()).to.equal(path.normalize(folder).toLowerCase());
        expect(pattern.pattern).to.equal("**/*.nupkg");

        expect(runStub.calledOnce).to.equal(true);
    });

    it("with no URI, uses the workspace-wide glob string", async () => {
        findFilesStub.resolves([Uri.file(path.join(tmpRoot, "solo.1.0.0.nupkg"))]);
        showQuickPickStub.onFirstCall().resolves({ label: "solo.1.0.0.nupkg", description: tmpRoot } as QuickPickItem);
        showQuickPickStub.onSecondCall().resolves({ label: "Use custom source..." } as QuickPickItem);
        showInputBoxStub.onFirstCall().resolves("https://example.org/");
        showInputBoxStub.onSecondCall().resolves("");
        showInputBoxStub.onThirdCall().resolves("");

        const mgr = new ChocolateyCliManager();
        await mgr.push();

        expect(findFilesStub.calledOnce).to.equal(true);
        expect(findFilesStub.firstCall.args[0]).to.equal("**/*.nupkg");
    });
});
