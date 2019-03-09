---
Order: 110
Title: ProjectUrl Is Empty
Description: ProjectUrl is missing or empty.
Category: Requirements
---

:::{.alert .alert-warning}
**Preliminary Notice**
This rule is not yet available in chocolatey-vscode.
It is a planned rule for 0.8.0.
:::

## Issue

In the nuspec, there is a `<projectUrl />` element. It was found to be missing or empty in the package.

## Recommended Solution

Please update the nuspec to include a `<projectUrl />` element that is non-empty. If your nuspec file is missing this field, you should run the `Chocolatey: new Chocolatey package` with the default template and look at the output from that (ensure you have the [latest version of Chocolatey](https://chocolatey.org/packages?q=id%3Achocolatey)).

## Reasoning

This is also known as the software site. Without this information, folks do not have anything to go on besides the package. On the community feed we want to ensure that folks have places to go for more information, the software site is a requirement.

## See also

- [Package validator rule](https://github.com/chocolatey/package-validator/wiki/ProjectUrl){target = _blank}
