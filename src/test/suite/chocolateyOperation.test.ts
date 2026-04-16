import { expect } from "chai";
import * as path from "path";
import { ChocolateyOperation, isChocolateyCliInstalled } from "../../ChocolateyOperation";

describe("ChocolateyOperation", () => {
    const ORIGINAL_ENV: string | undefined = process.env.ChocolateyInstall;

    afterEach(() => {
        if (ORIGINAL_ENV === undefined) {
            delete process.env.ChocolateyInstall;
        } else {
            process.env.ChocolateyInstall = ORIGINAL_ENV;
        }
    });

    describe("constructor", () => {
        it("wraps a single-string command into an array", () => {
            const op: ChocolateyOperation = new ChocolateyOperation("pack");
            expect(op.cmd).to.deep.equal(["pack"]);
        });

        it("accepts an array of command arguments unchanged", () => {
            const args: string[] = ["push", "foo.nupkg", "--source=\"'x'\""];
            const op: ChocolateyOperation = new ChocolateyOperation(args);
            expect(op.cmd).to.deep.equal(args);
        });

        it("starts with empty stdout and stderr buffers", () => {
            const op: ChocolateyOperation = new ChocolateyOperation(["new", "foo"]);
            expect(op.getStdout()).to.deep.equal([]);
            expect(op.getStderr()).to.deep.equal([]);
        });
    });

    describe("lifecycle hooks (no output channel created yet)", () => {
        it("showOutputChannel is safe to call before run()", () => {
            const op: ChocolateyOperation = new ChocolateyOperation("pack");
            expect(() => op.showOutputChannel()).to.not.throw();
        });

        it("hideOutputChannel is safe to call before run()", () => {
            const op: ChocolateyOperation = new ChocolateyOperation("pack");
            expect(() => op.hideOutputChannel()).to.not.throw();
        });

        it("kill is safe to call before run()", () => {
            const op: ChocolateyOperation = new ChocolateyOperation("pack");
            expect(() => op.kill()).to.not.throw();
        });

        it("dispose is safe to call before run()", () => {
            const op: ChocolateyOperation = new ChocolateyOperation("pack");
            expect(() => op.dispose()).to.not.throw();
        });
    });
});

describe("isChocolateyCliInstalled", () => {
    const ORIGINAL_ENV: string | undefined = process.env.ChocolateyInstall;

    afterEach(() => {
        if (ORIGINAL_ENV === undefined) {
            delete process.env.ChocolateyInstall;
        } else {
            process.env.ChocolateyInstall = ORIGINAL_ENV;
        }
    });

    it("returns false when ChocolateyInstall points to a non-existent location", () => {
        process.env.ChocolateyInstall = path.join("C", "does", "not", "exist", "anywhere");
        // The function spawns a binary at that path; since it does not exist,
        // execSync throws, the catch returns false.
        expect(isChocolateyCliInstalled()).to.equal(false);
    });

    it("returns false when ChocolateyInstall is unset (bin path becomes empty)", () => {
        delete process.env.ChocolateyInstall;
        expect(isChocolateyCliInstalled()).to.equal(false);
    });
});
