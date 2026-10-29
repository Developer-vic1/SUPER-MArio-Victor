$ErrorActionPreference = 'Stop'
$projectPath = Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
$localCli = Join-Path $projectPath 'Logs/Tools/unity.exe'
$unityCli = if (Test-Path -LiteralPath $localCli) { $localCli } else { (Get-Command unity.exe -ErrorAction Stop).Source }
& $unityCli mcp --project-path $projectPath
exit $LASTEXITCODE
