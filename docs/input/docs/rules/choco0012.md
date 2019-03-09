---
Order: 120
Title: Tags Found With Commas
Description: Tags have been incorrectly separated with commas
Category: Requirements
---

:::{.alert .alert-warning}
**Preliminary Notice**
This rule is not yet available in chocolatey-vscode.
It is a planned rule for 0.8.0.
:::

## Issue

In the nuspec in the `<tags />` element, the verifier found that the tags were separated with commas. They should only be separated with spaces.

## Recommended Solution

Please remove the commas from the `<tags />` element.

## Reasoning

We could just fix this on Chocolatey.org, but tags are used in other places besides just Chocolatey.org, and they are expected to be space separated.

## See also

- [Package validator rule](https://github.com/chocolatey/package-validator/wiki/TagsAreSpaceSeparated){target = _blank}
