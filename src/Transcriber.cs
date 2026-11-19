using System.IO;
using System.Text;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using Whisper.net;
using Whisper.net.LibraryLoader;

namespace MinuteFlow;
public static class Transcriber
{
    public static string ModelPath => Path.Combine(AppContext.BaseDirectory, "models", "ggml-base.bin");
    public static bool ModelPresent => File.Exists(ModelPath) && new FileInfo(ModelPath).Length == 147951465;
    public static async Task<string> Run(string path, string language, IProgress<string>? progress, CancellationToken token)
    {
        path = LocalFiles.RequireLocal(path);
        if (!new[] { ".wav", ".mp3", ".m4a" }.Contains(Path.GetExtension(path).ToLowerInvariant()))
            throw new InvalidDataException("Choose an MP3, WAV, or M4A recording.");
        if (!ModelPresent) throw new InvalidDataException("The bundled speech model is missing or incomplete. Extract the complete MinuteFlow package again.");
        if (new FileInfo(path).Length > 500_000_000) throw new InvalidDataException("Audio files must be smaller than 500 MB. Split this recording first.");
        RuntimeOptions.RuntimeLibraryOrder = [RuntimeLibrary.Cpu];
        return await Task.Run(async () =>
        {
            token.ThrowIfCancellationRequested();
            progress?.Report("Reading audio locally…");
            using var reader = new MediaFoundationReader(path);
            if (reader.TotalTime > TimeSpan.FromHours(2)) throw new InvalidDataException("Recordings can be up to 2 hours long. Split this recording first.");
            ISampleProvider samples = reader.ToSampleProvider();
            if (samples.WaveFormat.Channels == 2) samples = new StereoToMonoSampleProvider(samples);
            if (samples.WaveFormat.Channels != 1) throw new InvalidDataException("This recording has more than two channels. Export it as mono or stereo first.");
            samples = new WdlResamplingSampleProvider(samples, 16000);
            // Keep decoded audio only in memory. No conversion files or transcript logs.
            using var memory = new MemoryStream();
            var block = new float[16000];
            float[]? audio = null;
            try
            {
                int count; double energy = 0;
                while ((count = samples.Read(block, 0, block.Length)) > 0)
                {
                    token.ThrowIfCancellationRequested();
                    if (memory.Length + count * 4 > 16000L * 4 * 7200) throw new InvalidDataException("Recordings can be up to 2 hours long.");
                    for (int i = 0; i < count; i++) energy += block[i] * block[i];
                    memory.Write(System.Runtime.InteropServices.MemoryMarshal.AsBytes(block.AsSpan(0, count)));
                }
                if (memory.Length < 16000 * 4 / 2 || energy / (memory.Length / 4) < 0.0000001)
                    throw new InvalidDataException("No audible speech was detected. Check the recording and try again.");
                audio = new float[memory.Length / 4];
                Buffer.BlockCopy(memory.GetBuffer(), 0, audio, 0, (int)memory.Length);
                progress?.Report("Loading bundled Whisper model…");
                using var factory = WhisperFactory.FromPath(ModelPath);
                await using var processor = factory.CreateBuilder()
                    .WithLanguage(language).WithThreads(Math.Clamp(Environment.ProcessorCount / 2, 1, 8))
                    .WithNoContext().WithTemperature(0).WithNoSpeechThreshold(0.6f)
                    .WithProgressHandler(p => progress?.Report($"Transcribing on this device · {p}%"))
                    .Build();
                var b = new StringBuilder();
                await foreach (var part in processor.ProcessAsync(audio, token))
                {
                    token.ThrowIfCancellationRequested();
                    var value = part.Text.Trim();
                    if (value.Length > 0) b.AppendLine(value);
                    if (b.Length > MinutesEngine.MaxCharacters) throw new InvalidDataException("The transcript exceeds the workspace limit. Split this recording first.");
                }
                token.ThrowIfCancellationRequested();
                if (b.Length == 0) throw new InvalidDataException("No speech was recognized. Try selecting the recording language explicitly.");
                return b.ToString().Trim();
            }
            finally
            {
                Array.Clear(block);
                if (audio != null) Array.Clear(audio);
                if (memory.TryGetBuffer(out var bytes)) Array.Clear(bytes.Array!);
            }
        }, token);
    }
}
