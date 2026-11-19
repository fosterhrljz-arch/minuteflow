# MinuteFlow — Publication and Retrieval Evidence

Release publication date: **17 November 2026**.

**Latest delivery check:** [18 November 2026 anonymous download and discoverability verification](MinuteFlow-Delivery-Verification.md). The complete 211,891,009-byte ZIP was downloaded again and all integrity checks passed. The historical release and test evidence below is preserved.

## Published runnable application

**[Open the Windows x64 application download](https://agent-git.com/@fosterhrljz/minuteflow/s/agit-7a04d4a3b8363baca0467018736d23b1b77f68d3?ref=0d57feb36afa137a131729ef855458c3c29c2bcd&tab=files&file=artifacts/releases/MinuteFlow-Windows-x64.zip&sharer=fosterhrljz)**, then choose **Download**. The ZIP is too large for inline preview. Extract all files and double-click `MinuteFlow.exe` inside the extracted `MinuteFlow` folder.

| Release property | Verified value |
|---|---|
| Repository | `fosterhrljz/minuteflow` on `https://agent-git.com` |
| Visibility | Public; public publication explicitly approved by the owner |
| Session branch | `minuteflow-development` |
| Published revision containing the application | `0d57feb36afa137a131729ef855458c3c29c2bcd` |
| File path | `artifacts/releases/MinuteFlow-Windows-x64.zip` |
| Download size | 211,891,009 bytes |
| SHA-256 / Git LFS object ID | `104830cbf900f40cdeaf7ba0ec6472532f1592b18616855a867610426adad280` |
| Package contents | 504 files and one directory entry; 333,515,891 uncompressed bytes |

The application, its source, and the tested ZIP were not rebuilt or modified for publication. The release includes `MinuteFlow.exe`, the multilingual Whisper base model, .NET/Windows Desktop runtime, speech libraries, native dependencies, component licenses, a per-file checksum manifest, and the original test evidence.

## Manual publication acknowledgement

The release was staged explicitly with AgentGit's `file add` workflow, with the application ZIP tracked in Git LFS, followed by `file commit` and an explicitly authorized `push`. No automatic upload workflow was enabled. Only this existing session's release files and documentation were selected.

AgentGit 0.2.53 returned exit code **0**, `ok: true`, and these publication acknowledgements:

```text
Publication destination: fosterhrljz/minuteflow on https://agent-git.com (public, existing repository) as fosterhrljz
heads batch 0: refs/heads/minuteflow-development updated
LFS publication: complete; 0 objects not attempted
Published reviewed sessions to fosterhrljz/minuteflow.
```

A subsequent `agit fetch` confirmed the published branch at the revision above. The public file API independently reported the same revision, binary file size, and LFS SHA-256. See [public file metadata](public-file-metadata.json).

## Retrieval checks

**Complete public download verified at 13:03:33 UTC on 17 November 2026.** All 211,891,009 bytes were retrieved through AgentGit's public Download flow without an account, Authorization header, existing browser session, or local AgentGit cache. A fresh anonymous client used only the download-access cookies issued by the public site. The initial HTTP 200 transfer was resumed with HTTP 206 byte ranges and assembled in order. The resulting SHA-256 exactly matches the original release. See [machine-readable download verification](public-download-verification.json).

The downloaded ZIP also passed CRC validation and all 503 packaged per-file checksum checks before extraction. The application executable and bundled model are present in the retrieved distribution.

## Downloaded application execution

The executable extracted from the public download was then run through its explicit diagnostic entry point with the English and Chinese WAV fixtures obtained from the public sample ZIP. **All 34 checks passed, zero failed, and the process exited with code 0.** The workflow included local transcription for both languages in WAV, MP3, and AAC M4A, minutes extraction, exports, missing-information handling, input errors, and desktop window construction. See [downloaded-release test results](../verification/public-release-test-results.json).

Immediately before that run, a TCP connection attempt to `1.1.1.1:443` was rejected with Windows error **10013** under the same default execution sandbox. The application tests ran without elevated network permissions. See [network-block verification](network-block-verification.json). The host adapters were not physically disabled; this remains a blocked-network test on the existing Windows machine, not clean-machine certification. Diagnostic mode deliberately writes fictional fixtures and results; normal application launch does not run those tests.

## Supporting files

The optional sample ZIP was downloaded anonymously and verified as **1,007,619 bytes**, SHA-256 `da69db686c49f83bdf909e64dbf41079e7bf0899632c5fa8381245cea9469ee5`. It contains six fictional English/Chinese audio files: WAV, MP3, and AAC M4A for each language, plus English instructions.

Both application screenshots returned HTTP 200 and matched the intended JPEG bytes exactly. The separately published test-results JSON and checksum file matched their local contents after Git's ordinary LF/CRLF text normalization. Binary ZIP contents are unaffected by text normalization. See [supporting-file verification](supporting-file-verification.json).

The local release passed ZIP CRC validation and checks for all **503 files** listed in its packaged `SHA256SUMS.txt`; the manifest itself is the remaining packaged file. The original report and results remain available in [the verification folder](../verification/TEST-REPORT.md) and inside the ZIP.

## Scope and limitations

The original evidence records 34 passing checks under an outbound-network-denied Windows sandbox, and the downloaded release passed the same 34 checks again on 17 November. These results verify distribution accessibility, integrity, and the exercised workflows; they are not clean-machine certification or a formal security audit. Speech recognition and rule-based extraction still require human review.

The full ZIP is available through AgentGit's **Download** control. Its raw preview endpoint returns HTTP 413 because the artifact is large. Download speed depends on the hosting service and connection; the verification transfer required resumable byte-range requests. The pinned file-view link above is persistent and does not embed a temporary download cookie or expiring access URL.

Publication puts the application and fictional review materials online. It does not add networking, telemetry, accounts, content saving, or cloud processing to MinuteFlow. The [reviewer guide](../MinuteFlow-Reviewer-Guide.md) explains how to disconnect before first launch and independently exercise the workflow.
