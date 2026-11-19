# MinuteFlow verification report

Date: 11 November 2026 (America/Denver).

## Environment

- Windows x64, build 26200, release 25H2; Intel Core Ultra 5 125H.
- Self-contained .NET / Windows Desktop 10.0.5 application, built with SDK 10.0.201.
- Whisper.net + CPU runtime 1.8.1, bundled multilingual base model; NAudio 2.2.1.
- Model SHA-256 verified during build against `60ed5bc3dd14eea856493d334349b405782ddcaf0028d4b5df4088345fba2efe`.

## Offline test method

The packaged executable was launched and tested inside the execution environment's outbound-network-denied Windows sandbox. A TCP probe to 1.1.1.1:443 in that same sandbox policy failed with Windows SocketException **10013 (access denied)**. Tests then ran without elevated network permissions. Dependencies/model had been bundled before those launches.

This validates operation with outbound networking unavailable under that sandbox policy. The host's Wi-Fi adapter was not disabled, and the test is not a packet-capture audit or a fresh Windows virtual-machine certification. The live desktop UI was also launched from the same restricted execution environment. No source recording or notes were uploaded.

## Automated checks — 34 passed, 0 failed

The suite exercised the delivered executable through its explicit diagnostic entry point, including:

- Desktop window construction.
- English decision, action, and question organization.
- Explicit owner/deadline fields and supported natural-language assignments.
- Missing details remain unspecified; pronouns do not become invented people.
- Negated, tentative, and conditional claims excluded from positive decisions/actions.
- Simplified and Traditional Chinese extraction, including actual speech-recognition wording.
- Every source statement retained in the source reference snapshot.
- Empty/oversize notes, UNC paths, corrupt audio, unsupported export extensions, invalid text encodings, and silent recordings rejected.
- Markdown and UTF-16 Chinese notes import.
- Markdown export round trip and plain-text export.
- Cancellation token handling.
- English and Chinese recognition for **WAV, MP3, and AAC M4A**: all six combinations passed.
- All six audio combinations processed through transcription, minutes extraction, and local Markdown export.

Speech fixtures were generated with installed Windows Zira/Huihui voices using fictional project-report content. MP3/AAC fixtures were encoded locally using Windows Media Foundation. This checks functionality and recognizable content, not accuracy across real-world accents, noisy rooms, speaker overlap, or long meetings. Raw machine-readable evidence is in `test-results.json`.

## Desktop checks

The actual WPF app was inspected using Windows Computer Use. Launch layout, example import, generated source/minutes, enabled copy/export controls, and Copy completion were observed. The native export picker opened correctly. Local file writing and export content were verified by the automated end-to-end suite; the automation helper could not reliably target the native picker filename control, so a complete picker-driven save is not claimed.

A contrast issue on dark buttons was corrected and the final launch layout re-inspected. The included screenshots show the actual native application with fictional example content, not a design mockup.

## Privacy review

The native CPU engine's PE import tables were audited recursively. The audit identified `VCOMP140.DLL` as an additional dependency; it was then bundled alongside the other app-local VC runtime files. The final dependency check passes without relying on a separately installed VC redistributable. This supplements, but does not replace, clean-machine validation.

Application source review found no network client, account, telemetry, updater, or model-download calls. Normal application content writes occur only in the explicit export method. Audio decoding uses RAM, not temporary files. Explicit diagnostic tests write synthetic fixtures/results only when invoked with `--self-test`; normal launch never runs them.

This is a source-level review and blocked-network functional test, not a formal security audit. OS paging, crash dumps, external clipboard software, file synchronization, and user-chosen exports are documented privacy boundaries.

## Remaining validation limits

- No clean-machine VM, Windows 10/ARM64, Windows N, older CPU, accessibility certification, or two-hour endurance benchmark was performed.
- Cancellation was exercised with a pre-cancelled token; cancellation latency during a long native decode was not benchmarked.
- The application is unsigned. No installer/signing certificate or update service is included.
- Rule-based summaries intentionally use the first five non-question excerpts. They are not semantic or comprehensive summaries, and classification requires human review.

## Reproduce

1. Extract the full distribution to a normal local folder.
2. Disconnect internet access before opening `MinuteFlow.exe`.
3. Try the fictional example; generate, edit, copy, and export both formats.
4. Import an English or Chinese WAV/MP3/M4A; verify a transcript appears without a download or login.
5. Edit the transcript and regenerate; verify the stale-source notice clears.
6. Close and reopen; verify that no meeting history reappears.

Success: all operations work locally and only files explicitly exported remain.
