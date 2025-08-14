using System.Text.RegularExpressions;
using Spectre.Console;

namespace termix.models;

public partial record LanguageTheme
{
    public Dictionary<string, Style> TokenStyles { get; init; } = new();
    public Regex TokenizerRegex { get; init; } = MyRegex();

    [GeneratedRegex("")]
    private static partial Regex MyRegex();
}