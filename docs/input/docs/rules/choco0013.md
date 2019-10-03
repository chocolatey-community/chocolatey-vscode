---
Order: 130
Title: Tags Are Missing
Description:
Category: Requirements
---

:::{.alert .alert-warning}
**Preliminary Notice**
This rule is not yet available in chocolatey-vscode.
It is a planned rule for 0.8.0.
:::

## Issue

In the nuspec, there is a `<tags />` element. It was found to be missing or empty in the package.

## Recommended Solution

Please update the nuspec to include a `<tags />` element that is non-empty. If your nuspec file is missing this field, you should run the `Chocolatey: Create new Chocolatey package` with the default template and look at the output from that (ensure you have the [latest version of Chocolatey](https://chocolatey.org/packages?q=id%3Achocolatey)).

## Reasoning

Tags aid in categorization of packages and underlying software. Tags should be relevant to the software that is being installed. It will give folks more options when searching for particular software on the site and may turn up in results it may have not otherwise came up in.

## See also

- [Package validator rule](https://github.com/chocolatey/package-validator/wiki/TagsNotEmpty){target = _blank}
