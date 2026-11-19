# Third-party notices

This distribution includes:

| Component | Version / model | Origin | License |
|---|---|---|---|
| Whisper.net and CPU runtime | 1.8.1 | https://github.com/sandrohanea/whisper.net | MIT; see licenses/Whisper.net-LICENSE.txt |
| whisper.cpp / ggml native engine | Runtime supplied by Whisper.net.Runtime 1.8.1 | https://github.com/ggml-org/whisper.cpp | MIT; see licenses/whisper.cpp-LICENSE.txt |
| Whisper model | Multilingual base, ggml-base.bin | https://huggingface.co/ggerganov/whisper.cpp | MIT; see licenses/Whisper-model-LICENSE.txt |
| NAudio | 2.2.1 | https://github.com/naudio/NAudio | MIT; see licenses/NAudio-LICENSE.txt |
| .NET / Windows Desktop runtime | 10.0.5 | https://github.com/dotnet/runtime and https://github.com/dotnet/wpf | MIT and bundled third-party notices; see licenses/dotnet-LICENSE.txt and runtime notices |
| Microsoft Visual C++ runtime | App-local redistributable DLLs | Microsoft | Microsoft Visual Studio runtime redistribution terms; see licenses/Visual-C-Runtime-LICENSE.txt |

Windows Media Foundation and installed codecs are operating-system components. MinuteFlow includes the managed NAudio integration, not a separate codec installer. Windows N requires the Media Feature Pack.

Model integrity: SHA-256 `60ed5bc3dd14eea856493d334349b405782ddcaf0028d4b5df4088345fba2efe`; size 147,951,465 bytes. The model is loaded locally and never downloaded at application runtime.

All third-party trademarks belong to their respective owners. MinuteFlow is not affiliated with their maintainers.
