---
Title: LicenseUrl Is Empty When Requiring License Acceptance
Description:
Category: Requirements
---

:::{.alert .alert-warning}
**Preliminary Notice**
This rule is not yet available in chocolatey-vscode.
It is a planned rule for 0.8.0.
:::

## Issue

In the nuspec, there is a `<licenseUrl>` element. It was found to be missing or empty in the package.

## Recommended Solution

Please update the nuspec to include a `<licenseUrl />` element that is non-empty. If your nuspec file is missing this field, you should use the `Chocolatey: Create new Chocolatey package` command with the default template.

## Reasoning

If you want to require license acceptance, you must include the license url.

## See also

- [Package validator rule](https://github.com/chocolatey/package-validator/wiki/LicenseUrlMissingWhenLicenseAcceptanceTrue){target = _blank}
