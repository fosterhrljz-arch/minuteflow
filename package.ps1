$ErrorActionPreference = 'Stop'
Set-Location $PSScriptRoot
$folder = Join-Path $PSScriptRoot 'artifacts\MinuteFlow'
if (!(Test-Path (Join-Path $folder 'MinuteFlow.exe'))) { throw 'Build the application first.' }
foreach ($doc in @('README.md','PRIVACY.md','THIRD-PARTY-NOTICES.md')) {
    Copy-Item -LiteralPath $doc -Destination $folder
}
foreach ($dir in @('docs','licenses')) {
    Copy-Item -LiteralPath $dir -Destination $folder -Recurse -Force
}
$manifest = Get-ChildItem $folder -File -Recurse | Where-Object { $_.Name -ne 'SHA256SUMS.txt' } | Sort-Object FullName | ForEach-Object {
    $relative = $_.FullName.Substring($folder.Length + 1).Replace('\','/')
    '{0}  {1}' -f (Get-FileHash -LiteralPath $_.FullName -Algorithm SHA256).Hash.ToLowerInvariant(), $relative
}
$manifest | Set-Content -LiteralPath (Join-Path $folder 'SHA256SUMS.txt') -Encoding utf8
$zip = Join-Path $PSScriptRoot 'artifacts\MinuteFlow-Windows-x64.zip'
Compress-Archive -LiteralPath $folder -DestinationPath $zip -Force -CompressionLevel Optimal
$hash = (Get-FileHash -LiteralPath $zip -Algorithm SHA256).Hash.ToLowerInvariant()
"$hash  MinuteFlow-Windows-x64.zip" | Set-Content -LiteralPath (Join-Path $PSScriptRoot 'artifacts\MinuteFlow-Windows-x64.zip.sha256') -Encoding ascii
Write-Output "Package: $zip"
Write-Output "SHA-256: $hash"
