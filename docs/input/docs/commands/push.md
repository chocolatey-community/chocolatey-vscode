---
Order: 40
Title: Push Chocolatey package(s)
Description: Usage instructions for the push Chocolatey package(s) command
---

Pushing a package to the Community Repository requires the use of an API Key. You can head over to the [Community Repository]('https://chocolatey.org/packages') and create an account. Once you have your account, you will see the API listed in your Profile.

*Add your API key to your local chocolatey installation*

Run the following command to add your api key for the community repository to your Chocolatey installation:

`choco apikey -k <your key here> -s https://push.chocolatey.org/`

Note that we have covered the Community Repository here. For other sources, information for pushing to them can be found [here]('https://github.com/chocolatey/choco/wiki/CommandsPush').

When you are ready to push a package press `Ctrl+Shift+P` to bring up the Command Pallete and type `Chocolatey: Push Package(s)`. This will add your new package to the repository.
