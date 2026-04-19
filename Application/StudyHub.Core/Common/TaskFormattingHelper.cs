using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace Application.Helpers;

public static partial class TaskFormattingHelper
{
    public static string GenerateTaskCodePrefix(string? subjectName)
    {
        var normalized = NormalizeSubjectForCode(subjectName);
        var words = SplitWords(normalized);
        if (words.Count == 0)
        {
            return "UNKN";
        }

        if (words.Count == 1)
        {
            return TakeLetters(words[0], 4);
        }

        if (words.Count == 2)
        {
            return (TakeLetters(words[0], 2) + TakeLetters(words[1], 2)).ToUpperInvariant();
        }

        if (words.Count == 3)
        {
            return (TakeLetters(words[0], 2) + TakeLetters(words[1], 1) + TakeLetters(words[2], 1)).ToUpperInvariant();
        }

        var sb = new StringBuilder();
        foreach (var word in words.Take(4))
        {
            sb.Append(TakeLetters(word, 1));
        }

        return sb.ToString().ToUpperInvariant();
    }

    public static string ShortenTextWithEllipsis(string? text, int maxLength)
    {
        if (maxLength <= 0)
        {
            return string.Empty;
        }

        var value = text?.Trim() ?? string.Empty;
        if (value.Length <= maxLength)
        {
            return value;
        }

        if (maxLength <= 3)
        {
            return new string('.', maxLength);
        }

        return value[..(maxLength - 3)] + "...";
    }

    public static string PrepareSummary(string? rawSummary, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(rawSummary))
        {
            return string.Empty;
        }

        var normalized = rawSummary.Replace("\r\n", "\n", StringComparison.Ordinal)
            .Replace("\r", "\n", StringComparison.Ordinal);

        normalized = BrRegex().Replace(normalized, "\n");
        normalized = BlockBoundaryRegex().Replace(normalized, "\n");
        normalized = HtmlTagRegex().Replace(normalized, string.Empty);
        normalized = WebUtility.HtmlDecode(normalized);
        normalized = NonBreakingSpaceRegex().Replace(normalized, " ");
        normalized = normalized.TrimStart('\n', ' ', '\t');
        normalized = MultiNewLineRegex().Replace(normalized, "\n\n\n");

        if (normalized.Length > maxLength)
        {
            normalized = normalized[..maxLength];
        }

        return normalized.Trim();
    }

    private static string NormalizeSubjectForCode(string? subjectName)
    {
        var normalized = (subjectName ?? string.Empty).Trim();
        if (string.IsNullOrEmpty(normalized))
        {
            return "Unknown Subject";
        }

        normalized = ElectiveMarkerRegex().Replace(normalized, " ");
        normalized = NonLetterSeparatorRegex().Replace(normalized, " ");
        normalized = MultiSpaceRegex().Replace(normalized, " ").Trim();

        if (string.IsNullOrEmpty(normalized))
        {
            return "Unknown Subject";
        }

        return normalized;
    }

    private static List<string> SplitWords(string value)
    {
        return WordRegex()
            .Matches(value)
            .Select(match => match.Value)
            .Where(word => !string.IsNullOrWhiteSpace(word))
            .ToList();
    }

    private static string TakeLetters(string word, int count)
    {
        if (count <= 0)
        {
            return string.Empty;
        }

        var letters = word.Where(char.IsLetter).Take(count).ToArray();
        return new string(letters).ToUpperInvariant();
    }

    [GeneratedRegex(@"\(\s*ДВ\d+\s*\)", RegexOptions.IgnoreCase)]
    private static partial Regex ElectiveMarkerRegex();

    [GeneratedRegex(@"[^\p{L}\s]+", RegexOptions.IgnoreCase)]
    private static partial Regex NonLetterSeparatorRegex();

    [GeneratedRegex(@"\s{2,}", RegexOptions.IgnoreCase)]
    private static partial Regex MultiSpaceRegex();

    [GeneratedRegex(@"[\p{L}]+")]
    private static partial Regex WordRegex();

    [GeneratedRegex(@"<br\s*/?>", RegexOptions.IgnoreCase)]
    private static partial Regex BrRegex();

    [GeneratedRegex(@"</?(div|p|li|ul|ol)>", RegexOptions.IgnoreCase)]
    private static partial Regex BlockBoundaryRegex();

    [GeneratedRegex(@"<[^>]+>", RegexOptions.IgnoreCase)]
    private static partial Regex HtmlTagRegex();

    [GeneratedRegex(@"\n{4,}", RegexOptions.IgnoreCase)]
    private static partial Regex MultiNewLineRegex();

    [GeneratedRegex(@"\u00A0", RegexOptions.IgnoreCase)]
    private static partial Regex NonBreakingSpaceRegex();
}