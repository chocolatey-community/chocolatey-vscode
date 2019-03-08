---
Title: Copyright Character Count Minimum
Description:
Category: Requirements
---

:::{.alert .alert-warning}
**Preliminary Notice**
This rule is not yet available in chocolatey-vscode.
It is a planned rule for 0.8.0.
:::

## Issue

In the nuspec, there is a `<copyright>` field. Usually a copyright contains year (or range of years) and a company. Since a year takes up four characters by itself, the validator has detected that you are not using the copyright field correctly.

## Recommended Solution

Please update the copyright field so that it is using at least 4 characters.

## Reasoning

Usually the year alone takes up four characters, so properly setting a copyright would include years and name of company.

## See also

- [Package validator rule](https://github.com/chocolatey/package-validator/wiki/CopyrightCharacterCountMinimum){target = _blank}
