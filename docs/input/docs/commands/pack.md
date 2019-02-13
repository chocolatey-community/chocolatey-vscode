---
Order: 20
Title: Pack Chocolatey package(s)
Description: Usage instructions for the package Chocolatey package(s) command
---

Once you have your package built to your liking and the nuspec file properly filled in (again review the comments in the nuspec file for more information) it is time to Pack the package, which bundles everything into the needed nupkg.

Press `Ctrl+Shift+P` to bring up the Command Pallete. Type `Chocolatey: Pack Chocolatey Package(s)`. The extension will ask you for any additional arguments you require for the pack operation. Documentation on those additional arguments and their usage can be found [here]('https://github.com/chocolatey/choco/wiki/CommandsPack').

The package operation will bundle everything, and you should see a new file created in the folder named _packagename.nupkg_.
