// Bundle the extension for VS Code.  Keeps the VSIX small and improves
// activation time by inlining the runtime dependency tree (xml2js, etc.)
// into a single CommonJS file.  The `vscode` module is provided by the
// host at runtime and must remain external.
//
// Invocations:
//   node esbuild.config.js              (development: source maps, not minified)
//   node esbuild.config.js --production (VSIX build: minified, no source maps)
//   node esbuild.config.js --watch      (rebuild on save during development)

const esbuild = require("esbuild");

const production = process.argv.includes("--production");
const watch = process.argv.includes("--watch");

const buildOptions = {
    entryPoints: ["src/extension.ts"],
    bundle: true,
    format: "cjs",
    platform: "node",
    target: "node18",
    outfile: "out/extension.js",
    external: ["vscode"],
    minify: production,
    sourcemap: !production,
    logLevel: "info"
};

async function main() {
    if (watch) {
        const ctx = await esbuild.context(buildOptions);
        await ctx.watch();
    } else {
        await esbuild.build(buildOptions);
    }
}

main().catch((err) => {
    console.error(err);
    process.exit(1);
});
