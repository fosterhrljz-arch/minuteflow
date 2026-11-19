# MinuteFlow — Download and Independent Review

**[DOWNLOAD THE WINDOWS APPLICATION ZIP — 212 MB](https://agent-git.com/@fosterhrljz/minuteflow/preview/artifacts/releases/MinuteFlow-Windows-x64.zip?ref=c49645fc6297e41ca2b59752e6352e3a54919e58)**

Open the link and choose **Download**. No sign-in required. AgentGit groups ZIPs under the collapsed **Supporting files** section; the complete application is **211,891,009 bytes**, even if the list shows its small LFS pointer.

**Private Meetings. Local Intelligence.**

This release provides the actual portable Windows application, including its executable, speech model, and runtime dependencies. Application code and the original tested ZIP are unchanged.

## Download the application

- [MinuteFlow for Windows x64 — complete portable ZIP](https://agent-git.com/@fosterhrljz/minuteflow/preview/artifacts/releases/MinuteFlow-Windows-x64.zip?ref=c49645fc6297e41ca2b59752e6352e3a54919e58)
- [SHA-256 checksum](releases/MinuteFlow-Windows-x64.zip.sha256)
- [Optional fictional English and Chinese audio samples](releases/MinuteFlow-Reviewer-Samples.zip)
- [Product overview, architecture, limitations, and actual screenshots](MinuteFlow-Product-Overview.md)

The application ZIP is **211,891,009 bytes** (approximately 202.07 MiB). Open the application link above and choose **Download** in AgentGit's file view. This large ZIP cannot be previewed inline; the Download action serves the full application. A Git LFS pointer is only a small text reference and is not the application; use the file's download control to obtain the ZIP.

Expected SHA-256:

```text
104830cbf900f40cdeaf7ba0ec6472532f1592b18616855a867610426adad280
```

The full ZIP contains 504 files and one directory entry, including `MinuteFlow/MinuteFlow.exe`, `models/ggml-base.bin`, the self-contained .NET/Windows Desktop runtime, Whisper CPU libraries, app-local native dependencies, licenses, a per-file `SHA256SUMS.txt`, and the original verification report and machine-readable test results. Its total uncompressed file size is 333,515,891 bytes.

Downloading the distribution requires internet access. Running the downloaded application, including its first launch and audio transcription, does not. Publishing software on AgentGit does not add any network connection or upload behavior to MinuteFlow.

## Requirements

Use Windows 11 x64 with an AVX2/FMA-capable CPU and standard Windows media components. Recommended: 8 GB RAM and at least 1 GB free storage. No GPU, separately installed .NET runtime, API key, account, command prompt, or server setup is required to use the application.

Windows N editions need their media components installed beforehand. Windows 10, ARM64, and older CPUs have not been validated. This build is unsigned; use your organization's normal application approval process if Windows or company policy blocks execution. Do not disable security software to run it.

## Review without a terminal

Use fictional notes and the optional synthetic audio samples for this evaluation. Export only to an ordinary local folder outside cloud-sync locations.

1. **Download and extract.** Save the complete application ZIP, choose Windows **Extract All**, and open its `MinuteFlow` folder. Keep all accompanying files together. Extract the optional samples ZIP separately if testing audio.
   **Success:** `MinuteFlow.exe` and its `models` and runtime files are present. Do not run the executable from inside the ZIP preview.
2. **Disconnect before first launch.** Turn off Wi-Fi and disconnect wired internet using the usual Windows controls, then double-click `MinuteFlow.exe`.
   **Success:** the two-pane workspace opens without a sign-in or download prompt, with **100% Offline** and **No Cloud Uploads** displayed.
3. **Generate from notes.** Choose **Try an example**, then **Generate Minutes**.
   **Success:** editable minutes contain Meeting Summary, Key Decisions, Action Items, Responsible Persons & Deadlines, Open Questions, and numbered source references.
4. **Check missing information.** In a new meeting, enter `Action: Review the report.` and generate minutes.
   **Success:** missing ownership and deadline information remains **Not specified (check source wording)**. The app does not add a person or date.
5. **Test local speech recognition.** Select **English**, import `en.wav` from the samples, and wait. Repeat with **Chinese** and `zh.wav`; the samples also include MP3 and AAC M4A versions of each recording.
   **Success:** a transcript appears without internet access, and **Generate Minutes** produces editable output. The English recording discusses a project report, an internal pilot, Alice's report, and an unresolved budget; the Chinese recording discusses the corresponding report, pilot, Zhang Wei's responsibility, and unresolved budget. Exact wording can vary.
6. **Edit and regenerate.** Correct a transcript phrase, then regenerate; also edit the minutes directly.
   **Success:** a source-change notice appears after source edits and clears after regeneration. Replacing edited minutes asks for confirmation.
7. **Copy and export.** Copy minutes, export Markdown and TXT, and use **Export source**. Open the exported files locally.
   **Success:** the files contain the edited result in the selected format, and the application reports completion.
8. **Check retention.** Close the application and reopen while still disconnected. Respond to any discard confirmation only after exporting what you want to keep.
   **Success:** a new empty workspace appears; prior meeting content is not automatically restored. Explicit exports remain where you saved them.
9. **Check an error case.** Use **Import notes** on an empty TXT file.
   **Success:** the application displays an understandable error instead of fabricating minutes or silently uploading the file.

Restore your normal network connection after the review. These steps are an independent functional check, not a packet-capture security audit.

## Optional checksum verification

Normal application use does not require commands. Reviewers who want to verify the download can open PowerShell in the download folder and run this read-only command:

```powershell
Get-FileHash -LiteralPath '.\MinuteFlow-Windows-x64.zip' -Algorithm SHA256
```

**Success:** the reported hash exactly matches the value above. A mismatch means the file is not the verified release; download it again before evaluating it. The extracted `SHA256SUMS.txt` provides hashes for individual packaged files.

## Evidence and limits

- [Original verification report](verification/TEST-REPORT.md): environment, blocked-network method, native dependency audit, desktop checks, and limitations.
- [Original 34-check machine-readable results](verification/test-results.json): zero failures. Both original files are also inside the application ZIP.
- [Downloaded-release test results](verification/public-release-test-results.json): the executable retrieved from the public release passed all 34 checks again on 17 November 2026 under the outbound-network restriction.
- [Publication and retrieval evidence](releases/MinuteFlow-Publication-Evidence.md): the published revision, immutable artifact link, byte count, checksum, and retrieval checks.

The existing tests ran in an outbound-network-denied Windows sandbox, with a TCP probe rejected by Windows error 10013. The host's physical network adapters were not disabled. This is not clean-machine certification, a real-world speech-accuracy benchmark, a two-hour endurance test, or a formal security audit. New independent review results should be reported separately from that original evidence.

Minutes use deterministic extraction rules, including up to five initial non-question excerpts for the summary. They are not generative AI summaries. Whisper transcription can misrecognize or hallucinate speech; people must review names, dates, wording, and extracted classifications. Speaker identification is not implemented.

The application does not automatically save meeting content or create temporary conversion files. Windows paging, crash dumps, screenshots, other applications, clipboard tools, backups, and user-chosen exports remain outside its control. See the product overview and the packaged `PRIVACY.md` for the complete boundaries.

The ZIP is preserved byte for byte from the tested build. Its packaged README predates publication and ends with a historical statement that it had not yet been uploaded. This guide and the publication evidence describe the current distribution status; that old statement is not a download restriction.
