namespace termix.models;

public record FileSystemItem(
    string Path,
    string Name,
    bool IsDirectory,
    long Size,
    DateTime LastModified,
    bool IsParentDirectory = false)
{
    public string FormattedSize => IsDirectory ? "-" : FormatBytes(Size);
    public string FormattedDate => IsDirectory && !IsParentDirectory ? "-" : LastModified.ToString("yyyy-MM-dd HH:mm");

    private static string FormatBytes(long bytes)
    {
        if (bytes == 0) return "0 B";
        string[] suffixes = ["B", "KB", "MB", "GB", "TB", "PB"];
        var counter = 0;
        double number = bytes;
        while (Math.Round(number / 1024) >= 1 && counter < suffixes.Length - 1)
        {
            number /= 1024;
            counter++;
        }

        return $"{number:N1} {suffixes[counter]}";
    }
}