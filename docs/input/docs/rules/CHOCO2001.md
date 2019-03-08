﻿---
Title: Nuspec Enhancements Missing
Description:
Category: Suggestions
---

:::{.alert .alert-warning}
**Preliminary Notice**
This rule is not yet available in chocolatey-vscode.
It is a planned rule for 0.8.0.
:::
## Issue

The nuspec is missing the recent enhancements that give consumers information related to the underlying software.

## Recommended Solution

Please update the nuspec to contain one or more of the following: 

  * `docsUrl` - points to the location of the wiki or docs of the software
  * `mailingListUrl` - points to the forum or email list group for the software
  * `bugTrackerUrl` - points to the location where issues and tickets can be accessed
  * `packageSourceUrl` - points to the location of your chocolatey package files in source (e.g. `https://github.com/chocolatey/chocolatey-coreteampackages`)
  * `projectSourceUrl` - points to the location of the underlying software source

**NOTE**: You must use `choco pack` with at least version [0.9.9.7](https://github.com/chocolatey/choco/blob/master/CHANGELOG.md#0997-june-20-2015) (see [#205](https://github.com/chocolatey/choco/issues/205)) to use these elements. Using NuGet or `nuget.exe` will error on this element.

**NOTE**: If your nuspec file is missing these fields, you should run `choco new testpkg` and look at the output from that (ensure you have the [latest version of Chocolatey](https://chocolatey.org/packages?q=id%3Achocolatey)).

## Reasoning
This provides folks with more information related to the software itself and gives folks a quick location reference to start from when attempting to find more information related to a project.

## See also

- [Package validator rule](https://github.com/chocolatey/package-validator/wiki/NuspecEnhancementsMissing){target = _blank}
