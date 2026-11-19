using System.IO;
using System.Text;

namespace MinuteFlow;
public static class LocalFiles
{
    public static string RequireLocal(string path)
    {
        var full = Path.GetFullPath(path);
        if (full.StartsWith(@"\\") || new DriveInfo(Path.GetPathRoot(full)!).DriveType == DriveType.Network)
            throw new InvalidDataException("Choose a file on a local drive. Network locations are not supported.");
        // Reparse paths can silently redirect local-looking paths to remote shares.
        for (var p = File.Exists(full) ? full : Path.GetDirectoryName(full); p != null; p = Path.GetDirectoryName(p))
            if ((File.GetAttributes(p) & FileAttributes.ReparsePoint) != 0)
                throw new InvalidDataException("Choose a regular local folder. Linked folders and cloud placeholders are not supported.");
        return full;
    }
    public static string ReadNotes(string path)
    {
        path = RequireLocal(path);
        if (!new[] { ".txt", ".md" }.Contains(Path.GetExtension(path).ToLowerInvariant()))
            throw new InvalidDataException("Choose a TXT or Markdown file.");
        if (new FileInfo(path).Length > 1_000_000) throw new InvalidDataException("Text files must be smaller than 1 MB.");
        using var reader = new StreamReader(path, new UTF8Encoding(false, true), true);
        var text = reader.ReadToEnd();
        if (text.Length > MinutesEngine.MaxCharacters) throw new InvalidDataException("Notes must contain at most 200,000 characters.");
        if (text.Contains('\0')) throw new InvalidDataException("This file does not contain supported text. Save it as UTF-8 TXT or Markdown.");
        if (string.IsNullOrWhiteSpace(text)) throw new InvalidDataException("This notes file is empty.");
        return text;
    }
    public static void Export(string path, string text)
    {
        path = RequireLocal(path);
        var ext = Path.GetExtension(path).ToLowerInvariant();
        if (ext != ".txt" && ext != ".md") throw new InvalidDataException("Export as .txt or .md.");
        File.WriteAllText(path, ext == ".txt" ? MinutesEngine.PlainText(text) : text, new UTF8Encoding(false));
    }
}
