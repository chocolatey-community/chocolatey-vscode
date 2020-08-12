# Chocolatey Visual Studio Code Extension [![VS Marketplace](https://vsmarketplacebadge.apphb.com/version-short/gep13.chocolatey-vscode.svg)](https://marketplace.visualstudio.com/items?itemName=gep13.chocolatey-vscode) [![Installs(short)](https://vsmarketplacebadge.apphb.com/installs-short/gep13.chocolatey-vscode.svg)](https://marketplace.visualstudio.com/items?itemName=gep13.chocolatey-vscode) [![Downloads(short)](https://vsmarketplacebadge.apphb.com/downloads-short/gep13.chocolatey-vscode.svg)](https://marketplace.visualstudio.com/items?itemName=gep13.chocolatey-vscode) [![Ratings(short)](https://vsmarketplacebadge.apphb.com/rating-short/gep13.chocolatey-vscode.svg)](https://marketplace.visualstudio.com/items?itemName=gep13.chocolatey-vscode)

[![All Contributors](https://img.shields.io/badge/all_contributors-7-orange.svg?style=flat-square)](#contributors)
[![Dependabot Status](https://api.dependabot.com/badges/status?host=github&repo=gep13/chocolatey-vscode)](https://dependabot.com)

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

## Context Menus

* `Chocolatey: Create new Chocolatey package` to create the default templated Chocolatey package in a directory of your choosing.

## Snippets

Snippets are provided for the following Chocolatey Helper Functions:

* Install-ChocolateyPackage
* Uninstall-ChocolateyPackage

To use them, simply open your powershell file, and then type `choco` followed by `ctrl-space`.  This will then show the available snippets...

![Available Chocolatey Snippets](https://raw.githubusercontent.com/chocolatey-community/chocolatey-vscode/master/images/Choco-Snippets.png)

and then simply arrow up/down to the one you want and press enter, or left mouse click.  From there the PowerShell code for the helper function will be generated, and the cursor will be placed ready for you to start filling in the content of function...

![Expanded Chocolatey Snippet](https://raw.githubusercontent.com/chocolatey-community/chocolatey-vscode/master/images/Expanded-Choco-Snippet.png)

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

You can find the documentation that is available for this project [here](https://chocolatey-community.github.io/chocolatey-vscode/).

## Thanks

The execution of the Chocolatey commands in this extension would not have been possible without the amazing work of the [Ember CLI VS Code extension](https://github.com/felixrieseberg/vsc-ember-cli), as this was used as the basis for creating this feature in this extension.  Huge thanks to [Felix Rieseberg](https://github.com/felixrieseberg).

## Contributing

If you would like to see any other snippet or features added for this VS Code Extension, feel free to raise an [issue](https://github.com/chocolatey-community/chocolatey-vscode/issues), and if possible, a follow up pull request.

You can also join in the Gitter Chat [![Join the chat at https://gitter.im/gep13-oss/community](https://badges.gitter.im/gep13-oss/community.svg)](https://gitter.im/gep13-oss/community?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

## Releases

To find out what was released in each version of this extension, check out the [releases](https://github.com/chocolatey-community/chocolatey-vscode/releases) page.

## Contributors

Thanks goes to these wonderful people ([emoji key](https://github.com/all-contributors/all-contributors#emoji-key)):

<!-- ALL-CONTRIBUTORS-LIST:START - Do not remove or modify this section -->
<!-- prettier-ignore -->
| [<img src="https://avatars3.githubusercontent.com/u/1474648?v=4" width="100px;" alt="Kim J. Nordmo"/><br /><sub><b>Kim J. Nordmo</b></sub>](https://github.com/AdmiringWorm)<br />[💻](https://github.com/gep13/chocolatey-vscode/commits?author=AdmiringWorm "Code") [📖](https://github.com/gep13/chocolatey-vscode/commits?author=AdmiringWorm "Documentation") [🎨](#design-AdmiringWorm "Design") [🤔](#ideas-AdmiringWorm "Ideas, Planning, & Feedback") [🚧](#maintenance-AdmiringWorm "Maintenance") [📦](#platform-AdmiringWorm "Packaging/porting to new platform") [👀](#review-AdmiringWorm "Reviewed Pull Requests") [🐛](https://github.com/gep13/chocolatey-vscode/issues?q=author%3AAdmiringWorm "Bug reports") | [<img src="https://avatars0.githubusercontent.com/u/834643?v=4" width="100px;" alt="Maurice Kevenaar"/><br /><sub><b>Maurice Kevenaar</b></sub>](https://github.com/mkevenaar)<br />[💻](https://github.com/gep13/chocolatey-vscode/commits?author=mkevenaar "Code") [📖](https://github.com/gep13/chocolatey-vscode/commits?author=mkevenaar "Documentation") [🎨](#design-mkevenaar "Design") [🤔](#ideas-mkevenaar "Ideas, Planning, & Feedback") [🚧](#maintenance-mkevenaar "Maintenance") [📦](#platform-mkevenaar "Packaging/porting to new platform") [👀](#review-mkevenaar "Reviewed Pull Requests") | [<img src="https://avatars1.githubusercontent.com/u/8674240?v=4" width="100px;" alt="Stephen Valdinger"/><br /><sub><b>Stephen Valdinger</b></sub>](http://chocolatey.org)<br />[💻](https://github.com/gep13/chocolatey-vscode/commits?author=steviecoaster "Code") [📖](https://github.com/gep13/chocolatey-vscode/commits?author=steviecoaster "Documentation") [🤔](#ideas-steviecoaster "Ideas, Planning, & Feedback") | [<img src="https://avatars1.githubusercontent.com/u/7863439?v=4" width="100px;" alt="Martin Björkström"/><br /><sub><b>Martin Björkström</b></sub>](https://twitter.com/mholo65)<br />[💻](https://github.com/gep13/chocolatey-vscode/commits?author=mholo65 "Code") [🤔](#ideas-mholo65 "Ideas, Planning, & Feedback") [👀](#review-mholo65 "Reviewed Pull Requests") | [<img src="https://avatars3.githubusercontent.com/u/1646284?v=4" width="100px;" alt="Adam Friedman"/><br /><sub><b>Adam Friedman</b></sub>](https://blog.tintoy.io/)<br />[🤔](#ideas-tintoy "Ideas, Planning, & Feedback") [👀](#review-tintoy "Reviewed Pull Requests") | [<img src="https://avatars0.githubusercontent.com/u/5354972?v=4" width="100px;" alt="Manfred Wallner"/><br /><sub><b>Manfred Wallner</b></sub>](https://www.mwallner.net)<br />[📖](https://github.com/gep13/chocolatey-vscode/commits?author=mwallner "Documentation") | [<img src="https://avatars3.githubusercontent.com/u/1271146?v=4" width="100px;" alt="Gary Ewan Park"/><br /><sub><b>Gary Ewan Park</b></sub>](http://www.gep13.co.uk/blog)<br />[💻](https://github.com/gep13/chocolatey-vscode/commits?author=gep13 "Code") [📖](https://github.com/gep13/chocolatey-vscode/commits?author=gep13 "Documentation") [🎨](#design-gep13 "Design") [🤔](#ideas-gep13 "Ideas, Planning, & Feedback") [🚧](#maintenance-gep13 "Maintenance") [📦](#platform-gep13 "Packaging/porting to new platform") [👀](#review-gep13 "Reviewed Pull Requests") [🐛](https://github.com/gep13/chocolatey-vscode/issues?q=author%3Agep13 "Bug reports") |
| :---: | :---: | :---: | :---: | :---: | :---: | :---: |
<!-- ALL-CONTRIBUTORS-LIST:END -->

This project follows the [all-contributors](https://github.com/all-contributors/all-contributors) specification. Contributions of any kind welcome!
