$ErrorActionPreference = 'Stop'
$out = Join-Path $PSScriptRoot 'output'
New-Item -ItemType Directory -Force -Path $out | Out-Null
Add-Type -AssemblyName System.Speech
$speech = New-Object System.Speech.Synthesis.SpeechSynthesizer
try {
    $speech.SelectVoice('Microsoft Zira Desktop')
    $speech.Rate = -1
    $speech.SetOutputToWaveFile((Join-Path $out 'en.wav'))
    $speech.Speak('Today we reviewed the project report. We decided to begin the internal pilot. Alice will send the report by Friday. The budget is still an open question.')
} finally { $speech.Dispose() }
# Read an installed Windows Chinese voice directly; no registry modification or download.
$voice = New-Object -ComObject SAPI.SpVoice
$token = New-Object -ComObject SAPI.SpObjectToken
$token.SetId('HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Speech_OneCore\Voices\Tokens\MSTTS_V110_zhCN_HuihuiM')
$voice.Voice = $token
$stream = New-Object -ComObject SAPI.SpFileStream
try {
    $stream.Open((Join-Path $out 'zh.wav'), 3, $false)
    $voice.AudioOutputStream = $stream
    $voice.Rate = -1
    $voice.Speak('今天我们讨论了项目报告。我们决定启动内部测试。张伟负责在星期五之前提交报告。预算问题还需要进一步确认。') | Out-Null
} finally { $stream.Close() }
Get-ChildItem $out -Filter '*.wav' | Select-Object Name,Length
