function Install-VsCodeExtension() {
  param(
    [Parameter(Mandatory = $true)]
    [string]$packageName,
    [Parameter(Mandatory = $true)]
    [string]$extensionPath
  )
  Write-Debug "Trying to locate Visual Studio Code executable..."
  $code = Get-AppInstallLocation "Microsoft Visual Studio Code" | ? { Test-Path "$_\bin\code.cmd" } | select -first 1 | % { "$_\bin\code.cmd" }

  if (!$code) {
    Write-Error "Visual Studio Code install directory was not found..."
    throw "Visual Studio Code install directory was not found."
  }

  Write-Host "Installing $packageName for Visual Studio Code..."
  Start-ChocolateyProcess -exeToRun $code -statements "--install-extension",$extensionPath
}