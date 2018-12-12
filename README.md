# Chocolatey Visual Studio Code Extension

This extension brings support for [Chocolatey](https://chocolatey.org/) to Visual Studio Code.

## Table of Contents

1. [What is Chocolatey?](#what-is-chocolatey)
1. [Commands](#commands)
1. [Snippets](#snippets)
1. [Dependencies](#dependencies)
1. [Resources](#resources)
1. [Installation](#installation)
1. [Documentation](#documentation)
1. [Thanks](#thanks)
1. [Contributing](#contributing)
1. [Releases](#releases)

## What is Chocolatey?

Chocolatey is a Package Manager for Windows, which allows the automation of all your software needs.

For more information about [Chocolatey](https://chocolatey.org/), please see the Chocolatey Website or the Chocolatey [source code repository](https://github.com/chocolatey/choco).


## Commands

The Chocolatey Visual Studio Code Extension provides the following commands:

* `Chocolatey: Create new Chocolatey package` to create the default templated Chocolatey package at the root of current workspace.
* `Chocolatey: Pack Chocolatey package(s)` to search current workspace for nuspec files and package them
* `Chocolatey: Delete Chocolatey package(s)` to search current workspace for nupkg files and delete them
* `Chocolatey: Push Chocolatey package(s)` to search current workspace for nupkg files and push them
* `Chocolatey: Install Template package(s)` to install a list of template packages from a specified source

## Snippets

Snippets are provided for the following Chocolatey Helper Functions:

* Install-ChocolateyPackage
* Uninstall-ChocolateyPackage

To use them, simply open your powershell file, and then type `choco` followed by `ctrl-space`.  This will then show the available snippets...

![Available Chocolatey Snippets](https://raw.githubusercontent.com/gep13/chocolatey-vscode/master/images/Choco-Snippets.png)

and then simply arrow up/down to the one you want and press enter, or left mouse click.  From there the PowerShell code for the helper function will be generated, and the cursor will be placed ready for you to start filling in the content of function...

![Expanded Chocolatey Snippet](https://raw.githubusercontent.com/gep13/chocolatey-vscode/master/images/Expanded-Choco-Snippet.png)

## Dependencies

The extension takes a dependency on the following extensions:

* [PowerShell](https://marketplace.visualstudio.com/items?itemName=ms-vscode.PowerShell) - since Chocolatey Packaging Scripts are written in PowerShell, this extension helps with the creation/maintenance of those.
* [Zip File Explorer](https://marketplace.visualstudio.com/items?itemName=slevesque.vscode-zipexplorer) - to enable the ability to "view" the contents of the generated `nupkg` files, which are simply fancy zip files.
  * **NOTE** This extension will attempt to add the necessary file association to your User Settings, so that `nupkg` files are treated in the same way as `zip` files.

## Resources

Short YouTube videos of each of the releases of this extension can be found in this [playlist](https://www.youtube.com/playlist?list=PL84yg23i9GBhIhNG4LaeXNHwxZYJaSqgj).

## Installation

You can either install this extension in the normal way, or you can choose to install using Chocolatey:

```
choco install chocolatey-vscode
```

## Documentation

You can find the documentation that is available for this project [here](https://gep13.github.io/chocolatey-vscode/).

## Thanks

The execution of the Chocolatey commands in this extension would not have been possible without the amazing work of the [Ember CLI VS Code extension](https://github.com/felixrieseberg/vsc-ember-cli), as this was used as the basis for creating this feature in this extension.  Huge thanks to [Felix Rieseberg](https://github.com/felixrieseberg).

## Contributing

If you would like to see any other snippet or features added for this VS Code Extension, feel free to raise an [issue](https://github.com/gep13/chocolatey-vscode/issues), and if possible, a follow up pull request.

You can also join in the Gitter Chat [![Join the chat at https://gitter.im/gep13/chocolatey-vscode](https://badges.gitter.im/gep13/chocolatey-vscode.svg)](https://gitter.im/gep13/chocolatey-vscode?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

## Releases

To find out what was released in each version of this extension, check out the [releases](https://github.com/gep13/chocolatey-vscode/releases) page.
