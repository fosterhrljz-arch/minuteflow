param([switch]$Restore)
$ErrorActionPreference = 'Stop'
Set-Location $PSScriptRoot
$env:DOTNET_CLI_HOME = Join-Path $PSScriptRoot '.build'
$env:DOTNET_CLI_TELEMETRY_OPTOUT = '1'
$env:DOTNET_SKIP_FIRST_TIME_EXPERIENCE = '1'
$env:DOTNET_GENERATE_ASPNET_CERTIFICATE = 'false'
$env:MSBuildEnableWorkloadResolver = 'false'
$env:NUGET_PACKAGES = Join-Path $PSScriptRoot '.build\packages'
if ($Restore) {
    dotnet restore src/MinuteFlow.csproj --configfile NuGet.Config
    if ($LASTEXITCODE -ne 0) { throw 'Dependency restore failed.' }
}
dotnet publish src/MinuteFlow.csproj -c Release -o artifacts/MinuteFlow --no-restore -v minimal
if ($LASTEXITCODE -ne 0) { throw 'Build failed.' }
$model = Join-Path $PSScriptRoot 'models\ggml-base.bin'
if ((Get-FileHash $model -Algorithm SHA256).Hash -ne '60ed5bc3dd14eea856493d334349b405782ddcaf0028d4b5df4088345fba2efe') { throw 'Model checksum mismatch.' }
New-Item -ItemType Directory -Force -Path artifacts/MinuteFlow/models | Out-Null
Copy-Item -LiteralPath $model -Destination artifacts/MinuteFlow/models/ggml-base.bin
# App-local VC runtime; these Microsoft redistributable files avoid a separate installer.
foreach ($dll in @('msvcp140.dll','msvcp140_1.dll','msvcp140_2.dll','msvcp140_atomic_wait.dll','vcruntime140.dll','vcruntime140_1.dll','concrt140.dll','vcomp140.dll')) {
    Copy-Item -LiteralPath (Join-Path $env:WINDIR "System32\$dll") -Destination artifacts/MinuteFlow
}
foreach ($doc in @('README.md','PRIVACY.md','THIRD-PARTY-NOTICES.md')) {
    if (Test-Path $doc) { Copy-Item -LiteralPath $doc -Destination artifacts/MinuteFlow }
}
if (Test-Path licenses) { Copy-Item -LiteralPath licenses -Destination artifacts/MinuteFlow -Recurse -Force }
if (Test-Path docs) { Copy-Item -LiteralPath docs -Destination artifacts/MinuteFlow -Recurse -Force }
Write-Output 'Ready: artifacts/MinuteFlow/MinuteFlow.exe'
