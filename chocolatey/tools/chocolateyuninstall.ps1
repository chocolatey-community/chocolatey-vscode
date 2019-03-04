$ErrorActionPreference = 'Stop'

$toolsDir = "$(Split-Path -parent $MyInvocation.MyCommand.Definition)"

Uninstall-VsCodeExtension "gep13.chocolatey-vscode"
