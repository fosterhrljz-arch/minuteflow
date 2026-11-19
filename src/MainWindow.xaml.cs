using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;

namespace MinuteFlow;
public partial class MainWindow : Window
{
    CancellationTokenSource? operation;
    bool modified, generating, busy, closeAfterCancel;
    string generatedSource = "", generatedTitle = "";
    public const string Example = "Product planning meeting — 12 November 2026\nThe team reviewed the onboarding pilot and customer feedback.\nDecision: The team approved a two-week internal pilot.\nAction: Prepare the pilot checklist; Owner: Alex Chen; Deadline: 20 November 2026.\nAction: Review the accessibility findings; Owner: Maya Patel; Deadline: 18 November 2026.\nOpen question: Who will own customer training?\nThe budget was discussed, but no decision was made.";

    public MainWindow()
    {
        InitializeComponent();
        if (!Transcriber.ModelPresent) ModelLabel.Text = "Speech model missing — see Privacy & help";
    }
    void Source_Changed(object sender, TextChangedEventArgs e)
    {
        if (SourcePlaceholder == null) return;
        SourcePlaceholder.Visibility = SourceBox.Text.Length == 0 ? Visibility.Visible : Visibility.Collapsed;
        SourceCount.Text = $"{SourceBox.Text.Length:N0} / 200,000 characters";
        GenerateButton.IsEnabled = !busy && !string.IsNullOrWhiteSpace(SourceBox.Text);
        modified = true; UpdateStale();
    }
    void Title_Changed(object sender, TextChangedEventArgs e) { if (ReviewLabel != null) { modified = true; UpdateStale(); } }
    void UpdateStale()
    {
        if (generating || MinutesBox.Text.Length == 0) return;
        ReviewLabel.Text = SourceBox.Text != generatedSource || MeetingTitle.Text != generatedTitle
            ? "Source changed. Regenerate to update minutes; existing edits will be replaced."
            : "Review extracted statements and speech recognition before sharing.";
    }
    void Minutes_Changed(object sender, TextChangedEventArgs e)
    {
        if (MinutesPlaceholder == null) return;
        var any = !string.IsNullOrWhiteSpace(MinutesBox.Text);
        MinutesPlaceholder.Visibility = any ? Visibility.Collapsed : Visibility.Visible;
        CopyButton.IsEnabled = ExportButton.IsEnabled = any && !busy;
        modified = true;
    }
    bool ConfirmReplace() => (SourceBox.Text.Length == 0 && MinutesBox.Text.Length == 0) || MessageBox.Show(this,
        "Replace the current workspace? Unexported notes and minutes will be lost.", "Start a new source", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
    void SetSource(string text, string label, string title)
    {
        generating = true;
        MinutesBox.Clear(); generatedSource = generatedTitle = "";
        SourceBox.Text = text; MeetingTitle.Text = title; SourceLabel.Text = label;
        ResultCount.Text = "Ready to generate";
        ReviewLabel.Text = "Source wording preserved. Missing details stay unspecified.";
        generating = false; modified = true;
        SourceBox.IsUndoEnabled = MinutesBox.IsUndoEnabled = false;
        SourceBox.IsUndoEnabled = MinutesBox.IsUndoEnabled = true;
    }
    OpenFileDialog Picker(string filter) => new() { Filter = filter, CheckFileExists = true, Multiselect = false, AddToRecent = false };
    async void Audio_Click(object sender, RoutedEventArgs e)
    {
        var d = Picker("Meeting recordings|*.mp3;*.wav;*.m4a");
        if (d.ShowDialog(this) == true) await ImportAudio(d.FileName);
    }
    void Notes_Click(object sender, RoutedEventArgs e)
    {
        var d = Picker("Text notes|*.txt;*.md");
        if (d.ShowDialog(this) == true) ImportNotes(d.FileName);
    }
    void ImportNotes(string path)
    {
        try
        {
            var text = LocalFiles.ReadNotes(path);
            if (!ConfirmReplace()) return;
            SetSource(text, Path.GetFileName(path), Path.GetFileNameWithoutExtension(path));
            StatusLabel.Text = "Notes imported locally. Review the source, then generate minutes.";
        }
        catch (Exception ex) { ShowError(ex); }
    }
    async Task ImportAudio(string path)
    {
        if (!ConfirmReplace()) return;
        operation = new(); SetBusy(true);
        try
        {
            var lang = LanguageBox.SelectedIndex switch { 1 => "en", 2 => "zh", _ => "auto" };
            var text = await Transcriber.Run(path, lang, new Progress<string>(s => StatusLabel.Text = s), operation.Token);
            SetSource(text, "Transcript · " + Path.GetFileName(path), Path.GetFileNameWithoutExtension(path));
            StatusLabel.Text = "Transcription complete. Check names, dates, and uncertain speech before generating minutes.";
        }
        catch (OperationCanceledException) { StatusLabel.Text = "Transcription cancelled. Your previous workspace is unchanged."; }
        catch (Exception ex) { ShowError(ex); }
        finally
        {
            operation.Dispose(); operation = null; SetBusy(false);
            if (closeAfterCancel) { modified = false; Close(); }
        }
    }
    void SetBusy(bool value)
    {
        busy = value;
        AudioButton.IsEnabled = NotesButton.IsEnabled = SampleButton.IsEnabled = NewButton.IsEnabled = LanguageBox.IsEnabled = !value;
        SourceBox.IsReadOnly = MinutesBox.IsReadOnly = value; MeetingTitle.IsReadOnly = value;
        GenerateButton.IsEnabled = !value && !string.IsNullOrWhiteSpace(SourceBox.Text);
        CopyButton.IsEnabled = ExportButton.IsEnabled = !value && !string.IsNullOrWhiteSpace(MinutesBox.Text);
        CancelButton.Visibility = Progress.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
        CancelButton.IsEnabled = true;
    }
    async void Generate_Click(object sender, RoutedEventArgs e)
    {
        if (MinutesBox.Text.Length > 0 && MessageBox.Show(this, "Regenerate minutes from the current source? This replaces all edits in the minutes pane.", "Regenerate minutes", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes) return;
        var text = SourceBox.Text; var title = MeetingTitle.Text;
        SetBusy(true); CancelButton.Visibility = Visibility.Collapsed;
        StatusLabel.Text = "Organizing source statements locally…";
        try
        {
            var result = await Task.Run(() => MinutesEngine.Generate(text, title));
            generating = true; MinutesBox.Text = result.Markdown;
            generatedSource = text; generatedTitle = title; generating = false;
            ResultCount.Text = $"{result.Decisions} decisions · {result.Actions} actions · {result.Questions} questions";
            ReviewLabel.Text = "Review extracted statements and speech recognition before sharing.";
            StatusLabel.Text = "Minutes ready. Edit either pane, then copy or export when you choose.";
        }
        catch (Exception ex) { ShowError(ex); }
        finally { generating = false; SetBusy(false); }
    }
    void Cancel_Click(object sender, RoutedEventArgs e) { operation?.Cancel(); CancelButton.IsEnabled = false; StatusLabel.Text = "Cancelling local transcription…"; }
    void Sample_Click(object sender, RoutedEventArgs e)
    {
        if (!ConfirmReplace()) return;
        SetSource(Example, "Example notes · fictional meeting", "Onboarding pilot review");
        StatusLabel.Text = "Example loaded. Select Generate Minutes to try the complete workflow.";
    }
    void New_Click(object sender, RoutedEventArgs e)
    {
        if (!ConfirmReplace()) return;
        SetSource("", "Your notes or original transcript", "Untitled meeting");
        modified = false;
        StatusLabel.Text = "New workspace. Nothing from the previous meeting was saved automatically.";
    }
    void Copy_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            // Disable Windows clipboard history/cloud clipboard for this explicitly copied payload.
            var data = new DataObject(); data.SetText(MinutesBox.Text, TextDataFormat.UnicodeText);
            data.SetData("CanIncludeInClipboardHistory", new MemoryStream(BitConverter.GetBytes(0)));
            data.SetData("CanUploadToCloudClipboard", new MemoryStream(BitConverter.GetBytes(0)));
            Clipboard.SetDataObject(data, true);
            StatusLabel.Text = "Minutes copied. Other local apps can read the clipboard; history and cloud flags are disabled.";
        }
        catch (ExternalException) { StatusLabel.Text = "The clipboard is busy. Try Copy again, or export a file."; }
    }
    void Export_Click(object sender, RoutedEventArgs e) => ExportText(MinutesBox.Text, "minutes");
    void ExportSource_Click(object sender, RoutedEventArgs e) => ExportText(SourceBox.Text, "transcript");
    void ExportText(string text, string kind)
    {
        if (busy) return;
        if (string.IsNullOrWhiteSpace(text)) { StatusLabel.Text = "There is no text to export yet."; return; }
        var d = new SaveFileDialog { Filter = "Markdown (*.md)|*.md|Plain text (*.txt)|*.txt", FileName = "meeting-" + kind + ".md", DefaultExt = ".md", AddExtension = true, AddToRecent = false, OverwritePrompt = true };
        if (d.ShowDialog(this) != true) return;
        try { LocalFiles.Export(d.FileName, text); StatusLabel.Text = $"Exported {kind}. This file remains on disk until you delete it."; }
        catch (Exception ex) { ShowError(ex); }
    }
    async void OnDrop(object sender, DragEventArgs e)
    {
        e.Handled = true;
        if (busy || !e.Data.GetDataPresent(DataFormats.FileDrop)) return;
        var files = (string[])e.Data.GetData(DataFormats.FileDrop);
        if (files.Length != 1) { StatusLabel.Text = "Drop one recording or notes file at a time."; return; }
        var ext = Path.GetExtension(files[0]).ToLowerInvariant();
        if (ext is ".txt" or ".md") ImportNotes(files[0]);
        else if (ext is ".wav" or ".mp3" or ".m4a") await ImportAudio(files[0]);
        else StatusLabel.Text = "Unsupported file. Choose MP3, WAV, M4A, TXT, or MD.";
    }
    void ShowError(Exception ex)
    {
        string message = ex switch
        {
            InvalidDataException => ex.Message,
            DecoderFallbackException => "Unsupported text encoding. Save the notes as UTF-8 or UTF-16 and try again.",
            UnauthorizedAccessException => "Access denied. Choose a readable local file or a writable export folder.",
            FileNotFoundException => "A required file is missing. Check the source file and extract the complete app package.",
            DllNotFoundException => "The bundled speech runtime could not load. Extract the complete package onto a local drive.",
            OutOfMemoryException => "Not enough free memory. Close other apps or use a shorter recording.",
            _ => "This file or operation could not be processed. For audio, try a standard mono or stereo WAV, MP3, or AAC M4A file. Windows N requires the Media Feature Pack."
        };
        StatusLabel.Text = message;
        MessageBox.Show(this, message, "MinuteFlow", MessageBoxButton.OK, MessageBoxImage.Information);
    }
    void OnClosing(object? sender, CancelEventArgs e)
    {
        if (busy)
        {
            e.Cancel = true;
            if (operation != null && MessageBox.Show(this, "Cancel transcription and close? Unexported work will be discarded.", "Close MinuteFlow", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            { closeAfterCancel = true; operation.Cancel(); }
            return;
        }
        if (modified && (SourceBox.Text.Length > 0 || MinutesBox.Text.Length > 0) && MessageBox.Show(this,
            "Close this workspace? Nothing is saved automatically. Export any source text and minutes you want to keep first.", "Close MinuteFlow", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes) e.Cancel = true;
    }
    void Privacy_Click(object sender, RoutedEventArgs e) => new HelpWindow { Owner = this }.ShowDialog();
}
