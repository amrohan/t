using termix.models;

namespace termix.Services;

public class IconProvider(bool useIcons)
{
    private const string DefaultFileIcon = "\uF15B";
    private const string FolderIcon = "\uE5FF";
    private const string ParentFolderIcon = "\uF062";

    private const string DefaultFileFallback = "  ";

    private const string FolderFallback = "[[DIR]]";

    private const string ParentFolderFallback = "[[..]]";

    private readonly Dictionary<string, string> _extensionIcons = new()
    {
        { ".cs", "\uE73A" }, { ".js", "\uE74E" }, { ".json", "\uE60B" },
        { ".html", "\uE736" }, { ".css", "\uE749" }, { ".py", "\uE73C" },
        { ".java", "\uE738" }, { ".cpp", "\uE61D" }, { ".c", "\uE61E" },
        { ".md", "\uF48A" }, { ".txt", "\uF15C" }, { ".pdf", "\uF1C1" },
        { ".doc", "\uF1C2" }, { ".docx", "\uF1C2" }, { ".xls", "\uF1C3" },
        { ".xlsx", "\uF1C3" }, { ".ppt", "\uF1C4" }, { ".pptx", "\uF1C4" },
        { ".jpg", "\uF1C5" }, { ".jpeg", "\uF1C5" }, { ".png", "\uF1C5" },
        { ".gif", "\uF1C5" }, { ".bmp", "\uF1C5" }, { ".mp3", "\uF1C7" },
        { ".wav", "\uF1C7" }, { ".mp4", "\uF1C8" }, { ".avi", "\uF1C8" },
        { ".mkv", "\uF1C8" }, { ".zip", "\uF1C6" }, { ".rar", "\uF1C6" },
        { ".7z", "\uF1C6" }, { ".exe", "\uE70F" }, { ".msi", "\uE70F" },
        { ".dll", "\uF81B" }, { ".git", "\uE702" }, { ".gitignore", "\uE702" },
        { ".yml", "\uF481" }, { ".yaml", " \uF481" }
    };

    public string GetIcon(FileSystemItem item)
    {
        if (!useIcons)
        {
            if (item.IsParentDirectory) return ParentFolderFallback;
            return item.IsDirectory ? FolderFallback : DefaultFileFallback;
        }

        if (item.IsParentDirectory) return $"[yellow]{ParentFolderIcon}[/]";
        if (item.IsDirectory) return $"[cyan]{FolderIcon}[/]";

        var extension = Path.GetExtension(item.Name).ToLowerInvariant();
        return _extensionIcons.GetValueOrDefault(extension, DefaultFileIcon);
    }
}