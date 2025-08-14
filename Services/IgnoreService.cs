using DotNet.Globbing;

namespace termix.Services;

public class IgnoreService
{
    private readonly List<Glob> _ignoreGlobs;
    private readonly string _rootPath;

    public IgnoreService(string basePath)
    {
        _rootPath = basePath;
        var patterns = LoadIgnorePatterns(basePath);
        _ignoreGlobs = patterns.Select(p => Glob.Parse(p, GlobOptions.Default)).ToList();
    }

    private static List<string> LoadIgnorePatterns(string startPath)
    {
        var patterns = new List<string>
        {
            "**/bin/**", "**/obj/**", "**/node_modules/**",
            "**/.git/**", "**/.vs/**", "**/.vscode/**"
        };

        var currentPath = new DirectoryInfo(startPath);
        while (currentPath != null)
        {
            var gitignoreFile = Path.Combine(currentPath.FullName, ".gitignore");
            if (File.Exists(gitignoreFile))
            {
                var lines = File.ReadAllLines(gitignoreFile)
                    .Select(line => line.Trim())
                    .Where(line => !string.IsNullOrEmpty(line) && !line.StartsWith('#'));

                patterns.AddRange(lines);
            }

            currentPath = currentPath.Parent;
        }

        return patterns;
    }

    public bool IsIgnored(string fullPath)
    {
        var relativePath = Path.GetRelativePath(_rootPath, fullPath).Replace(Path.DirectorySeparatorChar, '/');
        return _ignoreGlobs.Any(glob => glob.IsMatch(relativePath));
    }
}