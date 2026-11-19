using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace MinuteFlow;

public record SourceStatement(int Id, string Text);
public record ActionEntry(SourceStatement Source, string Owner, string Deadline);
public record MinutesResult(string Markdown, int Decisions, int Actions, int Questions);

/// <summary>Conservative extractive organization, never a generative language model.</summary>
public static class MinutesEngine
{
    public const int MaxCharacters = 200_000;
    static bool Has(string value, string pattern) => Regex.IsMatch(value, pattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant, TimeSpan.FromSeconds(1));
    public static List<SourceStatement> Split(string text)
    {
        if (text.Length > MaxCharacters) throw new InvalidDataException("Notes are too long. Use up to 200,000 characters per meeting.");
        // Preserve every source character other than surrounding whitespace. References are statement numbers.
        return Regex.Split(text.Replace("\r\n", "\n"), @"(?<=[.!?。！？])\s+|(?<=[。！？])|\n+")
            .Select(s => s.Trim()).Where(s => s.Length > 0).Select((s, i) => new SourceStatement(i + 1, s)).ToList();
    }

    public static MinutesResult Generate(string text, string title)
    {
        var source = Split(text);
        if (source.Count == 0) throw new InvalidDataException("Paste notes or transcribe a recording first.");
        var questions = source.Where(s => Has(s.Text, @"[?？]|\b(open question|unresolved|to be confirmed|tbd|not decided|no decision|undecided|pending decision)\b|待[确確][认認]|待[解決决]+|未[决決]定|[问問][题題][：:]|需要.*[确確][认認]")).ToList();
        var decisions = source.Where(s => !questions.Contains(s) && Has(s.Text, @"\bdecision\s*:|\b(decided|agreed|approved|resolved)\b|[决決]定|[决決][议議][：:]|一致同意|已批准")
            && !Has(s.Text, @"\b(not|never|didn't|haven't|hasn't|may|might|could|would|propos\w*|suggest\w*|if|whether)\b|尚未|[没沒]有|不|建[议議]|如果|是否|可能")).ToList();
        var actionSources = source.Where(s => !questions.Contains(s) && Has(s.Text, @"\b(action(?: item)?|todo)\s*:|\b(will|must|assigned to|responsible for)\b|待[办辦][：:]|行[动動][项項][：:]|[负負][责責]|[将將](?:于|於|在|会|會)?")
            && !Has(s.Text, @"\b(will not|won't|not assigned|might|could|would|if|whether)\b|不[负負][责責]|不[会會]|如果|是否|可能")).ToList();
        var actions = actionSources.Select(s => new ActionEntry(s, Owner(s.Text), Deadline(s.Text))).ToList();
        var summary = source.Where(s => !questions.Contains(s)).Take(5).ToList();
        if (summary.Count == 0) summary = source.Take(3).ToList();
        var b = new StringBuilder();
        b.AppendLine("# " + (string.IsNullOrWhiteSpace(title) ? "Meeting minutes" : OneLine(title)));
        b.AppendLine().AppendLine("> Rule-based extraction • Source wording preserved • Review before sharing.");
        b.AppendLine("> Missing details are not inferred. [S#] refers to the source statement index below.");
        Section(b, "Meeting Summary", summary, "No summary statements identified.");
        b.AppendLine("_Summary contains the first five non-question statements; it is not a comprehensive AI summary._").AppendLine();
        Section(b, "Key Decisions", decisions, "No explicit decisions identified.");
        b.AppendLine("## Action Items").AppendLine();
        if (actions.Count == 0) b.AppendLine("No explicit action items identified.").AppendLine();
        foreach (var a in actions) b.AppendLine($"- [ ] {a.Source.Text} [S{a.Source.Id}]").AppendLine($"  - Responsible person: {a.Owner}").AppendLine($"  - Deadline: {a.Deadline}").AppendLine();
        b.AppendLine("## Responsible Persons & Deadlines").AppendLine();
        if (actions.Count == 0) b.AppendLine("Not specified.").AppendLine();
        foreach (var a in actions) b.AppendLine($"- [S{a.Source.Id}] Responsible person: {a.Owner} | Deadline: {a.Deadline}");
        b.AppendLine();
        Section(b, "Open Questions", questions, "No explicit open questions identified.");
        b.AppendLine("## Source Reference").AppendLine();
        b.AppendLine("Source snapshot at generation time. Later transcript edits require regeneration.").AppendLine();
        foreach (var s in source) b.AppendLine($"[S{s.Id}] {s.Text}").AppendLine();
        return new(b.ToString(), decisions.Count, actions.Count, questions.Count);
    }

    static void Section(StringBuilder b, string title, List<SourceStatement> items, string empty)
    {
        b.AppendLine("## " + title).AppendLine();
        if (items.Count == 0) b.AppendLine(empty);
        foreach (var s in items) b.AppendLine($"- {s.Text} [S{s.Id}]");
        b.AppendLine();
    }
    static string OneLine(string s) => s.Replace('\n', ' ').Replace('\r', ' ').Trim();
    static string Owner(string s)
    {
        var m = Regex.Match(s, @"(?:\bOwner|\bResponsible person|[负負][责責]人)\s*[:：]\s*(?<v>[^;；|\r\n]+)", RegexOptions.IgnoreCase);
        if (m.Success) return m.Groups["v"].Value.Trim().TrimEnd('.');
        m = Regex.Match(s, @"^(?:(?:Action(?: item)?|TODO)\s*:\s*)?(?<v>[A-Z][a-z]+(?:[ -][A-Z][a-z]+){0,2})\s+(?:will|must|is responsible for)\b");
        if (m.Success && !Has(m.Groups["v"].Value, @"^(I|We|He|She|They|It|This|That|The|Someone)$")) return m.Groups["v"].Value;
        m = Regex.Match(s, @"^(?:(?:待[办辦]|行[动動][项項])[：:]\s*)?(?<v>[\p{IsCJKUnifiedIdeographs}]{2,4})(?=[负負][责責])");
        if (m.Success && !Has(m.Groups["v"].Value, @"^([我他她][们們]|有人)$")) return m.Groups["v"].Value;
        return "Not specified (check source wording)";
    }
    static string Deadline(string s)
    {
        var m = Regex.Match(s, @"(?:\bDeadline|\bDue|截止日期|截止[时時][间間])\s*[:：]\s*(?<v>[^;；|\r\n]+)", RegexOptions.IgnoreCase);
        if (m.Success) return m.Groups["v"].Value.Trim().TrimEnd('.');
        m = Regex.Match(s, @"\bby\s+(?<v>(?:(?:next|this)\s+)?(?:Monday|Tuesday|Wednesday|Thursday|Friday|Saturday|Sunday|tomorrow|today|end of (?:day|week|month))|\d{4}-\d{2}-\d{2})(?=[\s.;；,，]|$)", RegexOptions.IgnoreCase);
        if (m.Success) return m.Groups["v"].Value;
        m = Regex.Match(s, @"(?<v>(?:下|本)?(?:周|週|星期)[一二三四五六日天]|明天|今天)(?:之)?前");
        if (m.Success) return m.Groups["v"].Value;
        return "Not specified (check source wording)";
    }
    public static string PlainText(string markdown) => Regex.Replace(Regex.Replace(markdown, @"(?m)^#{1,6}\s+", ""), @"(?m)^>\s?", "").Replace("- [ ] ", "- ");
}
