$ErrorActionPreference = 'Stop'
$projectPath = Split-Path $PSScriptRoot -Parent
$localCli = Join-Path $projectPath 'Logs/Tools/unity.exe'
$cli = if (Test-Path -LiteralPath $localCli) { $localCli } else { (Get-Command unity.exe -ErrorAction Stop).Source }
& $cli command editor invoke_menu --path 'Mario Victor/2. Verificar y compilar Windows 64-bit' --project-path $projectPath --timeout 900 --format json
if ($LASTEXITCODE -ne 0) { throw 'No se completó la compilación. Abre Unity, sal de Play y revisa la consola.' }
