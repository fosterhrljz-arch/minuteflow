using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MinuteFlow;
public class HelpWindow : Window
{
    public HelpWindow()
    {
        Title = "Privacy & help — MinuteFlow"; Width = 650; Height = 670;
        WindowStartupLocation = WindowStartupLocation.CenterOwner; Background = Brushes.White;
        var p = new StackPanel { Margin = new Thickness(30) };
        p.Children.Add(new TextBlock { Text = "Private by design.", FontSize = 28, FontWeight = FontWeights.SemiBold });
        p.Children.Add(new TextBlock
        {
            Margin = new Thickness(0, 18, 0, 20), TextWrapping = TextWrapping.Wrap, FontSize = 14, LineHeight = 23,
            Text = "GET STARTED\n1. Import one recording, import notes, or paste text in the source pane.\n2. For audio, wait for the local transcript and check names and dates.\n3. Select Generate Minutes. Review and edit the result.\n4. Copy or export as Markdown or plain text. Export the source separately if needed.\n\nLOCAL PROCESSING\nWhisper base multilingual is bundled for English and Chinese speech recognition. The minutes organizer uses explicit keyword and field rules, not a generative AI model. Summary excerpts are the first five non-question statements. Ambiguous or missed facts remain in Source Reference for your review.\n\nNO INVENTED DETAILS\nNames are extracted only from explicit ownership wording; deadlines retain source wording. Relative dates are not resolved. There is no speaker identification. Recognition can make mistakes, including words not actually spoken: review the transcript against your recording.\n\nYOUR DATA\nNo accounts, analytics, network calls, downloads, or automatic content saving. Decoded audio and text stay in process memory; no sensitive temporary files are created. Closing discards the workspace. Exported files and copied text remain under your control. Clipboard cloud/history flags are disabled, but other apps can read copied text.\n\nChoose a local folder outside OneDrive or other sync services for exports. MinuteFlow cannot control operating-system paging, crash dumps, backups, third-party clipboard tools, or folder synchronization. RAM is not a secure deletion guarantee.\n\nSUPPORTED INPUT\nMP3, WAV, and AAC M4A; mono/stereo; up to 500 MB and 2 hours. Text: UTF-8 or UTF-16 TXT/MD, up to 1 MB and 200,000 characters. Protected audio is not supported. Windows 11 x64 with AVX2-class CPU; Windows N needs its media components.\n\nMODEL STATUS\n" + (Transcriber.ModelPresent ? "Bundled Whisper model is present. No downloads are needed." : "Speech model is missing or incomplete. Re-extract the entire distribution, including models and runtimes.")
        });
        var close = new Button { Content = "Back to workspace", HorizontalAlignment = HorizontalAlignment.Left };
        close.Click += (_, _) => Close(); p.Children.Add(close);
        Content = new ScrollViewer { Content = p, VerticalScrollBarVisibility = ScrollBarVisibility.Auto };
    }
}
