# MinuteFlow privacy design

## Data flow

Local recording → Windows audio decoder → 16 kHz mono samples in RAM → bundled Whisper → editable transcript in RAM → deterministic extraction → editable minutes in RAM → optional Copy/Export.

Local text file or pasted text → editable source in RAM → deterministic extraction → editable minutes in RAM → optional Copy/Export.

There is no network step, account, remote model, server, analytics, automatic update, or application telemetry. Models and runtime libraries ship in the portable distribution. Missing dependencies produce an error instead of triggering a download.

## Storage and retention

- Normal usage does not write recordings, transcripts, minutes, meeting titles, or file paths to application storage/logs.
- There is no saved meeting history or recovery file. The app does not use temporary audio files.
- Audio sample buffers are cleared in a `finally` block on success, failure, or cancellation. Native model state is disposed. Managed strings and native allocations cannot be guaranteed to be securely wiped.
- Source files are opened for reading. User originals are never modified or deleted.
- Export is the only content file write in normal use. An explicit export can replace a file after the Windows Save dialog confirms it.
- Copy writes minutes to the Windows clipboard on request, with `CanIncludeInClipboardHistory` and `CanUploadToCloudClipboard` set to zero. Third-party clipboard tools and manual keyboard copying are outside this control.
- File dialogs use `AddToRecent=false`; Windows or other software may still maintain OS-level file activity records.
- Closing or New meeting discards the workspace. New imports clear editor undo histories. Pending operations are cancelled before closing.

## Filesystem boundaries

The application rejects UNC/device paths, mapped network drives, and reparse paths (including many cloud placeholders and junctions). File pickers are Windows components; navigating remote locations within a picker can involve the OS. Select local paths. Ordinary local folders can still be monitored or synchronized by other software, which MinuteFlow cannot universally detect.

## Limits of an offline application

The app makes no network requests. This does not disable Windows networking or prevent unrelated applications, Windows Update, security scanning, cloud-drive synchronization, backups, memory paging, crash dumps, screenshots, or clipboard monitoring. Use an appropriately managed offline computer when those system-level properties matter. No claim of secure deletion or protection from a compromised operating system is made.

## Tests and development

The explicitly invoked `--self-test` diagnostic command writes synthetic fixtures and test results to the supplied test folder. It is never invoked at normal launch. These files are test artifacts, not user meeting history. Build scripts download public dependencies only when the developer requests restore. Public dependencies were downloaded during construction of this distribution; no user meeting content was involved.
