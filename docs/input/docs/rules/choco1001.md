---
Order: 10
Title: Description minimum character count
Description: Description text should be longer than 4 characters.
Category: Guidelines
---

:::{.alert .alert-warning}
**Preliminary Notice**
This rule is not yet available in chocolatey-vscode.
It is a planned rule for 0.8.0.
:::

## Issue

In the nuspec, the description should be long enough to describe the packaged software.

## Recommended Solution

Add addition text to the description to fully explain what the software is.

## Reasoning

The description explains the underlying software. Without sufficient description, folks may not know what they are installing.

## See also

- [Package validator rule](https://github.com/chocolatey/package-validator/wiki/DescriptionCharacterCountMinimum){target = _blank}
- [Maximum Characters Requirement](choco0003)
