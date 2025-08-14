using System.Diagnostics;
using Spectre.Console;
using termix.models;

namespace termix.Services;

public abstract class FileSystemService
{
    public static List<FileSystemItem> GetDirectoryContents(string path)
    {
        var items = new List<FileSystemItem>();
        var directoryInfo = new DirectoryInfo(path);

        if (directoryInfo.Parent != null)
            items.Add(new FileSystemItem(
                directoryInfo.Parent.FullName, "..", true, 0,
                directoryInfo.Parent.LastWriteTime, true
            ));

        var directories = directoryInfo.GetDirectories()
            .OrderBy(d => d.Name, StringComparer.OrdinalIgnoreCase)
            .Select(d => new FileSystemItem(d.FullName, d.Name, true, 0, d.LastWriteTime));
        items.AddRange(directories);

        var files = directoryInfo.GetFiles()
            .OrderBy(f => f.Name, StringComparer.OrdinalIgnoreCase)
            .Select(f =>
            {
                var fullName = f.FullName;
                var name = f.Name;
                if (name.Length <= 24)
                    return new FileSystemItem(fullName, name, false, f.Length, f.LastWriteTime);
                var ext = Path.GetExtension(name);
                var extLen = ext.Length;
                var baseLen = 24 - extLen - 2;

                if (baseLen <= 0)
                    name = ".." + ext;
                else
                    name = string.Concat(name.AsSpan(0, baseLen), "..", ext);
                return new FileSystemItem(fullName, name, false, f.Length, f.LastWriteTime);
            });

        items.AddRange(files);

        return items;
    }

    public static void OpenFile(string filePath)
    {
        try
        {
            var processStartInfo = new ProcessStartInfo(filePath) { UseShellExecute = true };
            Process.Start(processStartInfo);
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error opening file: {ex.Message}[/]");
            Console.ReadKey(true);
        }
    }
}