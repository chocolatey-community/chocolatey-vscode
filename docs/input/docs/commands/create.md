---
Order: 10
Title: Create new Chocolatey package
Description: Usage instructions for the create new package command
---

Open a folder in VSCode. Failure to do so will cause the extension to throw an error.

Press `Ctrl+Shift+P` to bring up the Command Pallete. Type `Chocolatey: Create new Chocolatey Package`. The extension will ask you to give the Package a name. This should match closely with the name of the application you are pacakging. Version in the name is not necessary, and should be avoided as this is handled in the nuspec file with the `<version></version>` tag.

The extension then creates a new folder with the name you typed into the extension's prompt and generates template installation, modification, and uninstallation scripts, as well as the nuspec file and some other miscellaneous files.

The generated files are well-documented inside, and have all the information required inside of them to build a successful package.
