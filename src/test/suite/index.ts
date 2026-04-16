import * as path from "path";
import * as Mocha from "mocha";
import * as glob from "glob";

export function run(): Promise<void> {
    const mocha: Mocha = new Mocha({
        ui: "bdd",
        color: true,
        timeout: 20000
    });

    const testsRoot: string = path.resolve(__dirname, ".");

    return new Promise<void>((resolve, reject) => {
        glob("**/*.test.js", { cwd: testsRoot }, (err: Error | null, files: string[]) => {
            if (err) {
                return reject(err);
            }

            files.forEach((f: string) => mocha.addFile(path.resolve(testsRoot, f)));

            try {
                mocha.run((failures: number) => {
                    if (failures > 0) {
                        reject(new Error(`${failures} tests failed.`));
                    } else {
                        resolve();
                    }
                });
            } catch (runErr) {
                console.error(runErr);
                reject(runErr);
            }
        });
    });
}
