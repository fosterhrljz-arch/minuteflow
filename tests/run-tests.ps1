param([switch]$Speech)
$ErrorActionPreference = 'Stop'
$project = Split-Path $PSScriptRoot -Parent
$exe = Join-Path $project 'artifacts\MinuteFlow\MinuteFlow.exe'
$out = Join-Path $PSScriptRoot 'output'
$arguments = @('--self-test', ('"' + $out + '"'))
if ($Speech) { $arguments += '--speech' }
$p = Start-Process -FilePath $exe -ArgumentList $arguments -WindowStyle Hidden -PassThru
$p.WaitForExit()
$result = Get-Content (Join-Path $out 'results.json') -Raw | ConvertFrom-Json
$result.checks | Select-Object name,passed | Format-Table -AutoSize
if ($p.ExitCode -ne 0 -or $result.failures -gt 0) { throw "Tests failed: $($result.failures)" }
Write-Output "Passed $($result.checks.Count) checks."
