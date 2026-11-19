using System.IO;
using System.Text;
using System.Text.Json;
using NAudio.Wave;

namespace MinuteFlow;
// Explicit diagnostic entry point; never runs on normal launch. Synthetic data only.
public static class SelfTest
{
    public static async Task<int> Run(string outputDirectory, bool speech)
    {
        Directory.CreateDirectory(outputDirectory);
        var checks = new List<object>(); int failures = 0;
        void Check(string name, bool ok, string detail = "") { checks.Add(new { name, passed = ok, detail }); if (!ok) failures++; }
        void Reject(string name, Action a) { try { a(); Check(name, false); } catch { Check(name, true); } }
        try
        {
            var ui = new MainWindow();
            Check("Desktop window constructs", ui.Title.Contains("MinuteFlow"));
            var sample = MinutesEngine.Generate(MainWindow.Example, "Pilot review");
            Check("English decision/action/question classification", sample.Decisions == 1 && sample.Actions == 2 && sample.Questions == 2);
            Check("Explicit owner and deadline", sample.Markdown.Contains("Responsible person: Alex Chen") && sample.Markdown.Contains("Deadline: 20 November 2026"));
            Check("Negative decision not promoted", !sample.Markdown.Split("## Key Decisions")[1].Split("## Action Items")[0].Contains("budget"));
            var missing = MinutesEngine.Generate("Action: Review the report.", "");
            Check("Missing owner and deadline stay unspecified", missing.Markdown.Contains("Responsible person: Not specified") && missing.Markdown.Contains("Deadline: Not specified"));
            var natural = MinutesEngine.Generate("Alice will send the report by Friday.\nWe will review it.", "Test");
            Check("Explicit natural-language owner and relative deadline preserved", natural.Markdown.Contains("Responsible person: Alice") && natural.Markdown.Contains("Deadline: Friday") && natural.Markdown.Contains("Responsible person: Not specified"));
            var zh = MinutesEngine.Generate("项目会议。\n决定：启动内部测试。\n行动项：检查报告；负责人：张伟；截止日期：下周五。\n问题：预算是否确定？", "会议");
            Check("Chinese rules", zh.Decisions == 1 && zh.Actions == 1 && zh.Questions == 1 && zh.Markdown.Contains("Responsible person: 张伟") && zh.Markdown.Contains("Deadline: 下周五"));
            var traditional = MinutesEngine.Generate("我們決定啟動內部測試。\n張偉負責在星期五之前提交報告。\n預算問題還需要進一步確認。", "Test");
            Check("Traditional Chinese speech wording", traditional.Decisions == 1 && traditional.Actions == 1 && traditional.Questions == 1 && traditional.Markdown.Contains("Responsible person: 張偉") && traditional.Markdown.Contains("Deadline: 星期五"));
            var unsure = MinutesEngine.Generate("We might approve this.\nNo decision was made.\nIf Alice will send it, we can start.\nWe have not agreed.\nThey won't prepare it.", "Test");
            Check("Tentative, conditional, and negated claims excluded", unsure.Decisions == 0 && unsure.Actions == 0);
            Check("Every original statement retained", MinutesEngine.Split(MainWindow.Example).All(s => sample.Markdown.Contains($"[S{s.Id}] {s.Text}")));
            Reject("Empty notes rejected", () => MinutesEngine.Generate(" \n", ""));
            Reject("Oversize notes rejected", () => MinutesEngine.Generate(new string('x', 200001), ""));
            Reject("Network paths rejected", () => LocalFiles.RequireLocal(@"\\localhost\share\notes.txt"));
            var note = Path.Combine(outputDirectory, "synthetic-notes.md");
            File.WriteAllText(note, MainWindow.Example, new UTF8Encoding(false));
            Check("Markdown import", LocalFiles.ReadNotes(note) == MainWindow.Example);
            var unicode = Path.Combine(outputDirectory, "synthetic-chinese.txt");
            File.WriteAllText(unicode, "决定：启动内部测试。", Encoding.Unicode);
            Check("UTF-16 Chinese import", LocalFiles.ReadNotes(unicode) == "决定：启动内部测试。");
            var invalid = Path.Combine(outputDirectory, "invalid.txt"); File.WriteAllBytes(invalid, [0xff, 0xff, 0xff]);
            Reject("Invalid text encoding rejected", () => LocalFiles.ReadNotes(invalid));
            var exported = Path.Combine(outputDirectory, "synthetic-minutes.md"); LocalFiles.Export(exported, sample.Markdown);
            Check("Markdown export round trip", File.ReadAllText(exported) == sample.Markdown);
            var plain = Path.Combine(outputDirectory, "synthetic-minutes.txt"); LocalFiles.Export(plain, sample.Markdown);
            Check("Plain text export", !File.ReadAllText(plain).Contains("## "));
            Reject("Unsupported export rejected", () => LocalFiles.Export(Path.Combine(outputDirectory, "test.html"), sample.Markdown));
            var bad = Path.Combine(outputDirectory, "corrupt.wav"); File.WriteAllText(bad, "not audio");
            try { await Transcriber.Run(bad, "en", null, default); Check("Corrupt audio rejected", false); } catch { Check("Corrupt audio rejected", true); }
            var silence = Path.Combine(outputDirectory, "silence.wav");
            using (var writer = new WaveFileWriter(silence, new WaveFormat(16000, 16, 1))) writer.Write(new byte[64000], 0, 64000);
            try { await Transcriber.Run(silence, "en", null, default); Check("Silence rejected", false); } catch (InvalidDataException) { Check("Silence rejected", true); }
            using var cancelled = new CancellationTokenSource(); cancelled.Cancel();
            try { await Transcriber.Run(silence, "en", null, cancelled.Token); Check("Cancellation", false); } catch (OperationCanceledException) { Check("Cancellation", true); }
            if (speech)
            {
                foreach (var language in new[] { "en", "zh" })
                {
                    var wav = Path.Combine(outputDirectory, language + ".wav");
                    foreach (var ext in new[] { ".wav", ".mp3", ".m4a" })
                    {
                        var audio = Path.ChangeExtension(wav, ext);
                        if (ext != ".wav")
                        {
                            using var reader = new MediaFoundationReader(wav);
                            if (ext == ".mp3") MediaFoundationEncoder.EncodeToMp3(reader, audio);
                            else MediaFoundationEncoder.EncodeToAac(reader, audio);
                        }
                        var text = await Transcriber.Run(audio, language, null, default);
                        var plausible = language == "en" ? text.Contains("report", StringComparison.OrdinalIgnoreCase) : text.Contains("报告") || text.Contains("報告");
                        Check($"{language} {ext} local transcription", plausible, text);
                        var result = MinutesEngine.Generate(text, "Speech workflow test");
                        var outPath = Path.Combine(outputDirectory, language + ext + ".minutes.md");
                        LocalFiles.Export(outPath, result.Markdown);
                        Check($"{language} {ext} transcription to exported minutes", File.ReadAllText(outPath).Contains(text.Split('\n')[0]));
                    }
                }
            }
        }
        catch (Exception ex) { Check("Unexpected test exception", false, ex.ToString()); }
        await File.WriteAllTextAsync(Path.Combine(outputDirectory, "results.json"), JsonSerializer.Serialize(new { failures, checks }, new JsonSerializerOptions { WriteIndented = true }));
        return failures == 0 ? 0 : 1;
    }
}
