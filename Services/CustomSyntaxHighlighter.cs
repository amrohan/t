using System.Text;
using System.Text.RegularExpressions;
using Spectre.Console;
using termix.models;

namespace termix.Services;

public class CustomSyntaxHighlighter
{
    private readonly Dictionary<string, LanguageTheme> _themes = new();

    public CustomSyntaxHighlighter()
    {
        var styleControl = new Style(Color.Plum1);
        var styleKeyword = new Style(Color.SkyBlue2);
        var styleType = new Style(Color.Teal);
        var styleString = new Style(Color.Gold3);
        var styleComment = new Style(Color.Grey58);
        var styleNumber = new Style(Color.Magenta2);
        var styleFunction = new Style(Color.LightGreen);
        var styleBuiltin = new Style(Color.Turquoise2);

        _themes[".cs"] = new LanguageTheme
        {
            TokenStyles = new Dictionary<string, Style>
            {
                { "string", styleString },
                { "comment", styleComment },
                { "number", styleNumber },
                { "keyword", styleKeyword },
                { "control", styleControl },
                { "type", styleType },
                { "function", styleFunction }
            },
            TokenizerRegex = BuildRegex(
                keywords:
                ["public", "private", "protected", "class", "void", "namespace", "using", "var", "get", "set"],
                controls: ["if", "else", "for", "foreach", "while", "return", "new"],
                types: ["string", "int", "bool", "double", "float", "long", "decimal"],
                stringPatterns: [@"(\"".*?\"")"],
                commentPatterns: [@"(//.*)"],
                functionPattern: @"(\w+)\s*\("
            )
        };

        _themes[".py"] = new LanguageTheme
        {
            TokenStyles = new Dictionary<string, Style>
            {
                { "string", styleString },
                { "comment", styleComment },
                { "number", styleNumber },
                { "keyword", styleKeyword },
                { "control", styleControl },
                { "builtin", styleBuiltin }
            },
            TokenizerRegex = BuildRegex(
                keywords: ["def", "class", "import", "from", "as", "and", "or", "not", "in", "is"],
                controls: ["if", "else", "elif", "for", "while", "return", "try", "except", "with"],
                builtins: ["True", "False", "None", "self"],
                stringPatterns: [@"(\"".*?\"")", @"(\'.*?\')"],
                commentPatterns: [@"(#.*)"]
            )
        };

        _themes[".js"] = new LanguageTheme
        {
            TokenStyles = new Dictionary<string, Style>
            {
                { "string", styleString },
                { "comment", styleComment },
                { "number", styleNumber },
                { "keyword", styleKeyword },
                { "control", styleControl },
                { "function", styleFunction },
                { "builtin", styleBuiltin }
            },
            TokenizerRegex = BuildRegex(
                keywords: ["const", "let", "var", "function", "class", "import", "from", "export", "async", "await"],
                controls: ["if", "else", "for", "while", "return", "new", "switch", "case"],
                builtins: ["true", "false", "null", "undefined", "this"],
                stringPatterns: [@"(\"".*?\"")", @"(\'.*?\')", @"(\`.*?\`)"],
                commentPatterns: [@"(//.*)", @"(/\*[\s\S]*?\*/)"],
                functionPattern: @"(\w+)\s*\("
            )
        };

        _themes[".ts"] = new LanguageTheme
        {
            TokenStyles = _themes[".js"].TokenStyles,
            TokenizerRegex = BuildRegex(
                keywords:
                [
                    "const", "let", "var", "function", "class", "import", "from", "export", "async", "await", "public",
                    "private", "protected", "readonly", "interface", "type", "enum", "implements", "extends"
                ],
                controls: ["if", "else", "for", "while", "return", "new", "switch", "case"],
                types: ["string", "number", "boolean", "any", "void", "unknown", "never"],
                builtins: ["true", "false", "null", "undefined", "this"],
                stringPatterns: [@"(\"".*?\"")", @"(\'.*?\')", @"(\`.*?\`)"],
                commentPatterns: [@"(//.*)", @"(/\*[\s\S]*?\*/)"],
                functionPattern: @"(\w+)\s*\("
            )
        };
    }

    private static Regex BuildRegex(
        IEnumerable<string>? stringPatterns = null,
        IEnumerable<string>? commentPatterns = null,
        IEnumerable<string>? keywords = null,
        IEnumerable<string>? controls = null,
        IEnumerable<string>? types = null,
        IEnumerable<string>? builtins = null,
        string? functionPattern = null)
    {
        var pattern = new StringBuilder();

        if (commentPatterns != null)
            AppendPattern(pattern, string.Join('|', commentPatterns.Select(p => $"(?<comment>{p})")));
        if (stringPatterns != null)
            AppendPattern(pattern, string.Join('|', stringPatterns.Select(p => $"(?<string>{p})")));
        if (functionPattern != null) AppendPattern(pattern, $"(?<function>{functionPattern})");
        if (keywords != null) AppendPattern(pattern, $@"\b(?<keyword>({string.Join('|', keywords)}))\b");
        if (controls != null) AppendPattern(pattern, $@"\b(?<control>({string.Join('|', controls)}))\b");
        if (types != null) AppendPattern(pattern, $@"\b(?<type>({string.Join('|', types)}))\b");
        if (builtins != null) AppendPattern(pattern, $@"\b(?<builtin>({string.Join('|', builtins)}))\b");

        AppendPattern(pattern, @"\b(?<number>\d+[\.\d_]*)\b");
        AppendPattern(pattern, @"\b(?<type>[A-Z][a-zA-Z0-9_]*)\b");

        return new Regex(pattern.ToString(), RegexOptions.Compiled | RegexOptions.ExplicitCapture);
    }

    private static void AppendPattern(StringBuilder sb, string pattern)
    {
        if (sb.Length > 0) sb.Append('|');
        sb.Append(pattern);
    }

    public Markup Highlight(string code, string languageExtension)
    {
        var langKey = languageExtension.ToLowerInvariant();
        if (string.IsNullOrEmpty(langKey) || !_themes.TryGetValue(langKey, out var theme))
            return new Markup(code.EscapeMarkup());

        var sb = new StringBuilder();

        foreach (var line in code.Split('\n'))
        {
            var lastIndex = 0;
            var matches = theme.TokenizerRegex.Matches(line);

            foreach (Match match in matches)
            {
                if (match.Index > lastIndex)
                    sb.Append(line.Substring(lastIndex, match.Index - lastIndex).EscapeMarkup());

                var groupName = theme.TokenizerRegex.GetGroupNames()
                    .Skip(1)
                    .FirstOrDefault(g => match.Groups[g].Success);

                if (groupName != null && theme.TokenStyles.TryGetValue(groupName, out var style))
                {
                    var valueToStyle = match.Value;
                    var hasTrailingParenthesis = groupName == "function" && valueToStyle.EndsWith('(');

                    if (hasTrailingParenthesis)
                        valueToStyle = valueToStyle[..^1];

                    sb.Append($"[{style.ToMarkup()}]{valueToStyle.EscapeMarkup()}[/]");

                    if (hasTrailingParenthesis)
                        sb.Append('(');
                }
                else
                {
                    sb.Append(match.Value.EscapeMarkup());
                }

                lastIndex = match.Index + match.Length;
            }

            if (lastIndex < line.Length) sb.Append(line[lastIndex..].EscapeMarkup());

            sb.AppendLine();
        }

        return new Markup(sb.ToString());
    }
}