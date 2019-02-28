---
Title: Description Is Empty
Description: Description element can not be empty.
Category: Requirements
---

:::{.alert .alert-warning}
**Preliminary Notice**
This rule is not yet available in chocolatey-vscode.
It is a planned rule for 0.8.0.
:::

## Issue

In the nuspec, there is a `<description />` element. It was found to be missing or empty in the package.

## Recommended Solution

Please update the nuspec to include a `<description />` element that is non-empty.
If your nuspec file is missing this field, you should run `Chocolatey: Create new Chocolatey package` and look at the output from that (ensure you have the [latest version of Chocolatey](https://chocolatey.org/packages?q=id%3Achocolatey) and [latest version of chocolatey-vscode](https://chocolatey.org/packages/chocolatey-vscode)).

## Reasoning

The description explains the underlying software. Without any information, folks may not know what they are installing.

## See Also

- [Package validator rule](https://github.com/chocolatey/package-validator/wiki/DescriptionNotEmpty){target = _blank}
- [Maximum Characters Requirement](choco0003)
- [Minimum Characters Guideline](choco1001)
