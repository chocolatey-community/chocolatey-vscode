# Chocolatey Visual Studio Code Documentation

## Requirements

This extension expects Chocolatey to be installed.
All other VSCode extension dependencies will be installed alongside this extension

## Getting Started

### Available Commands

- Chocolatey: Install Template package(s)
- Chocolatey: Create new Chocolatey Package
- Chocolatey: Pack Chocolatey package(s)
- Chocolatey: Push Chocolatey packages(s)
- Chocolatey: Delete Chocolatey package(s)

### Installing Templates

Press `Ctrl+Shift+P` to bring up the Command Pallete. Type `Chocolatey: Install Template packages(s)`*

* _VSCode must be ran as Administrator for access to C:\ProgramData\chocolatey\lib_

This will pull down all the templates for creating different Chocolatey package types.

### Creating a new Chocolatey Package

Open a folder in VSCode. Failure to do so will cause the extension to throw an error.

Press `Ctrl+Shift+P` to bring up the Command Pallete. Type `Chocolatey: Create new Chocolatey Package`. The extension will ask you to give the Package a name. This should match closely with the name of the application you are pacakging. Version in the name is not necessary, and should be avoided as this is handled in the nuspec file with the `<version></version>` tag.

The extension then creates a new folder with the name you typed into the extension's prompt and generates template installation, modification, and uninstallation scripts, as well as the nuspec file and some other miscellaneous files.

The generated files are well-documented inside, and have all the information required inside of them to build a successful package.

### Packing a Chocolatey Package

Once you have your package built to your liking and the nuspec file properly filled in (again review the comments in the nuspec file for more information) it is time to Pack the package, which bundles everything into the needed nupkg.

Press `Ctrl+Shift+P` to bring up the Command Pallete. Type `Chocolatey: Pack Chocolatey Package(s)`. The extension will ask you for any additional arguments you require for the pack operation. Documentation on those additional arguments and their usage can be found [here]('https://github.com/chocolatey/choco/wiki/CommandsPack').

The package operation will bundle everything, and you should see a new file created in the folder named _packagename.nupkg_.

### Pushing a Chocolatey Package

Pushing a package to the Community Repository requires the use of an API Key. You can head over to the [Community Repository]('https://chocolatey.org/packages') and create an account. Once you have your account, you will see the API listed in your Profile.

*Add your API key to your local chocolatey installation*

Run the following command to add your api key for the community repository to your Chocolatey installation:

`choco apikey -k <your key here> -s https://push.chocolatey.org/`

Note that we have covered the Community Repository here. For other sources, information for pushing to them can be found [here]('https://github.com/chocolatey/choco/wiki/CommandsPush').

When you are ready to push a package press `Ctrl+Shift+P` to bring up the Command Pallete and type `Chocolatey: Push Package(s)`. This will add your new package to the repository.

### Deleting a Package

Sometimes, you no longer need a package, or an older version of a package. `choco delete` makes it easy to remove those unnecessary packages. The extension has this capability built in

Press `Ctrl+Shift+P` to bring up the Command Pallete and type `Chocolatey: Delete Package`. Select the Chocolatey package nupkg to remove and it will be deleted from disk.
