# MinuteFlow — Public Download and Discoverability Verification

Verified on **18 November 2026**. Scope: public release delivery and integrity, not a new application functionality or security audit.

**[DOWNLOAD MINUTEFLOW FOR WINDOWS — 212 MB](https://agent-git.com/@fosterhrljz/minuteflow/preview/artifacts/releases/MinuteFlow-Windows-x64.zip?ref=c49645fc6297e41ca2b59752e6352e3a54919e58)** — open the file and choose **Download**. No account or sign-in is required.

## Finding and correction

The application was present in the published revision `c49645fc6297e41ca2b59752e6352e3a54919e58`. AgentGit's anonymous output listing classified the ZIP as `supporting` / `other`, with `lfs_missing: false`. The Deliverables section listed documents and screenshots. Supporting files are collapsed by default, and this listing reported the 134-byte LFS pointer size rather than the actual binary size. The separate public file metadata correctly reported **211,891,009 bytes**. The project automatically featured a screenshot and had no deliverables README.

The delivery fix adds `artifacts/00-DOWNLOAD-WINDOWS-APP-212-MB.md`, an explicitly named download entry in the Deliverables list, and `artifacts/README.md`, which AgentGit recognizes as the README accompanying the outputs. Both lead with the full application download, its size, and the Download-button instructions. The product overview and reviewer guide also lead with the download link. Existing release paths, screenshots, evidence, and application bytes are preserved. No application source or functionality is changed.

## Fresh anonymous retrieval

| Check | Result |
|---|---|
| Public file metadata | PASS — HTTP 200, expected size and LFS object ID |
| Account or authentication required | None — no Authorization header, account, browser session, or AgentGit credential/cache |
| Complete download | PASS — 211,891,009 bytes retrieved over HTTPS |
| Download transport | AgentGit's public Download flow; eight HTTP 206 byte ranges, all Content-Range headers checked |
| ZIP integrity | PASS — CRC check for every archive entry |
| Per-file integrity | PASS — all 503 entries in `SHA256SUMS.txt` verified |
| Runnable payload | `MinuteFlow/MinuteFlow.exe` present |
| Local transcription model | `MinuteFlow/models/ggml-base.bin` present |
| Archive contents | 504 files plus one directory entry |
| Verification completed | 2026-11-18 08:01:23 UTC |

SHA-256:

```text
104830cbf900f40cdeaf7ba0ec6472532f1592b18616855a867610426adad280
```

See [machine-readable fresh download evidence](anonymous-download-verification-20261118.json), [anonymous output listing before the fix](outputs-before-delivery-fix.json), and [public binary metadata](delivery-release-metadata.json).

The verification client started with empty cookie jars and downloaded every application byte anew. AgentGit issued scoped anonymous download-access cookies; these are not account authentication. Temporary download URLs and cookies are deliberately omitted from the evidence. The permanent public file link above remains usable after temporary download leases expire.

## Review boundaries

AgentGit's large-file preview endpoint is not the binary download endpoint. A missing inline preview does not mean the release is absent. Use **Download** in the file view, then extract the archive. The file-list pointer size is not the application size.

The browser automation connection timed out during this review, so the current page's rendered layout was not visually certified. The diagnosis uses the public project/output APIs and the site's published display logic; the complete download was independently exercised over anonymous HTTP. Publication checks are reported separately after the manual push.

The existing [publication evidence](MinuteFlow-Publication-Evidence.md) records the earlier downloaded executable's 34 passing checks under an outbound-network-denied Windows sandbox. Those are historical application checks, not new results from this delivery verification. See the [reviewer guide](../MinuteFlow-Reviewer-Guide.md) for independently testing the application offline.

Only delivery documentation and verification evidence are selected for the manual AgentGit commit and push. Automatic session uploads remain disabled.
