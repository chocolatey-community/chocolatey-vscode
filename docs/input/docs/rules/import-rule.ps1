<#
.SYNOPSIS
  Adds a new rule by importing an existing rule from the package validator.

.DESCRIPTION
  Clones the wiki part of the package validator, and imports the rule
  that matches the specified chocoRuleName into the rules directory.

.PARAMETER chocoRuleName
  The name of the rule as specified in the package validator repository.

.PARAMETER ruleType
  The kind of rule to import.

.PARAMETER Cleanup
  Remove the cloned package validator wiki after importing the wanted rule.

.EXAMPLE
  .\import-rule.ps1 -chocoRuleName CopyrightCharacterCountMinimum
#>

param(
  [Parameter(Mandatory = $true)]
  [ValidateNotNullOrEmpty()]
  [string]$chocoRuleName,
  [ValidateSet('Requirements', 'Guidelines', 'Suggestions', 'Notes')]
  [string]$ruleType = 'Requirements',
  [switch]$Cleanup
)

function import-rule() {
  param(
    [string]$chocoRuleName,
    [string]$ruleType,
    [switch]$Cleanup
  )

  $wikiCloneLink = "https://github.com/chocolatey/package-validator.wiki.git"
  $validatorLink = "https://github.com/chocolatey/package-validator/wiki/$chocoRuleName"
  $cloneTo = "$env:TEMP\wikiClone\package-validator.wiki"

  if (!(Test-Path "$cloneTo")) {
    . git clone "$wikiCloneLink" "$cloneTo"
  }
  else {
    pushd $cloneTo
    . git pull
    popd
  }

  $ruleMarkdown = gci -Recurse "$cloneTo\$chocoRuleName*" | select -First 1

  if ($ruleMarkdown) {

    $markdownContent = Get-Content -Encoding UTF8 $ruleMarkdown

    # This will be the title of the new rule
    $title = ($markdownContent | ? { $_ -match "^\s*\#" } | select -First 1).Trim('#', ' ')

    if ($title -eq 'Stub') {
      Write-Warning "$chocoRuleName is a Stub. Creating empty stub $ruleType rule..."
      $title = $chocoRuleName
    }
  } else {
    Write-Warning "No package validator rule matching $chocoRuleName* was found."
    Write-Warning "Creating empty $ruleType rule..."
    $markdownContent = ""
    $title = $chocoRuleName
  }

  $sb = New-Object System.Text.StringBuilder;
  $sb.AppendLine('---') | Out-Null
  $sb.AppendLine("Title: $title") | Out-Null
  $sb.AppendLine("Description:") | Out-Null
  $sb.AppendLine("Category: $ruleType") | Out-Null
  $sb.AppendLine("---") | Out-Null
  $sb.AppendLine('') | Out-Null
  $sb.AppendLine(":::{.alert .alert-warning}") | Out-Null
  $sb.AppendLine("**Preliminary Notice**") | Out-Null
  $sb.AppendLine("This rule is not yet available in chocolatey-vscode.") | Out-Null
  $sb.AppendLine("It is a planned rule for 0.8.0.") | Out-Null
  $sb.AppendLine(":::") | Out-Null
  $markdownContent | select -Skip 1 | % {
    $sb.AppendLine($_)
  } | Out-Null

  $sb.AppendLine("") | Out-Null
  $sb.AppendLine("## See also") | Out-Null
  $sb.AppendLine("") | Out-Null
  $sb.AppendLine("- [Package validator rule]($validatorLink){target = _blank}") | Out-Null

  $namePrefix = "CHOCO"
  if ($ruleType -eq 'Requirements') {
    $namePrefix += "0"
  }
  elseif ($ruleType -eq "Guidelines") {
    $namePrefix += "1"
  }
  elseif ($ruleType -eq "Suggestions") {
    $namePrefix += "2"
  }
  else {
    $namePrefix += "3"
  }

  $prevRule = gci "$PSScriptRoot\$namePrefix*.md" | % { $_.BaseName -replace "^$namePrefix", "" } | select -Last 1

  if ($prevRule) {
    $prevRuleNum = [int]::Parse($prevRule)
  }
  else {
    $prevRuleNum = 0
  }
  $prevRuleNum++

  $newRuleNum = $prevRuleNum.ToString("d3")

  $newRuleName = "$namePrefix$newRuleNum.md"

  $sb.ToString().TrimEnd() | Out-File -Encoding utf8 -FilePath "$PSScriptRoot\$newRuleName"

  if ($Cleanup) {
    rm -Recurse -Force $cloneTo
  }
}

import-rule -chocoRuleName $chocoRuleName `
  -ruleType $ruleType `
  -Cleanup:$Cleanup