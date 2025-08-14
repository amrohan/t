using Spectre.Console;
using termix.models;

namespace termix.Services;

public record ActionResponse(bool Success, string Message, object? Payload = null);

public abstract class ActionService
{
    public static Task<List<FileSystemItem>> GetDeepDirectoryContentsAsync(string basePath, CancellationToken token)
    {
        return Task.Run(() =>
        {
            var results = new List<FileSystemItem>();
            var ignoreService = new IgnoreService(basePath);
            RecursiveSearch(new DirectoryInfo(basePath), ignoreService, basePath, results, token);
            return results;
        }, token);
    }

    private static void RecursiveSearch(DirectoryInfo currentDir, IgnoreService ignoreService, string rootPath,
        List<FileSystemItem> results, CancellationToken token)
    {
        if (token.IsCancellationRequested || ignoreService.IsIgnored(currentDir.FullName)) return;

        foreach (var entry in currentDir.EnumerateFileSystemInfos())
        {
            if (token.IsCancellationRequested) return;

            switch (entry)
            {
                case DirectoryInfo subDir:
                    RecursiveSearch(subDir, ignoreService, rootPath, results, token);
                    break;
                case FileInfo file when ignoreService.IsIgnored(file.FullName):
                    continue;
                case FileInfo file:
                    results.Add(new FileSystemItem(
                        file.FullName,
                        Path.GetRelativePath(rootPath, file.FullName),
                        false,
                        file.Length,
                        file.LastWriteTime
                    ));
                    break;
            }
        }
    }

    public static ActionResponse Create(string basePath, string userInput)
    {
        if (string.IsNullOrWhiteSpace(userInput)) return new ActionResponse(false, "Input was empty.");
        if (userInput.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
            return new ActionResponse(false, "[red]Invalid characters in path.[/]");

        var finalPath = userInput;
        var isDir = finalPath.EndsWith(Path.DirectorySeparatorChar) ||
                    finalPath.EndsWith(Path.AltDirectorySeparatorChar);
        if (!isDir && !Path.HasExtension(finalPath)) finalPath += ".txt";

        var newPath = Path.Combine(basePath, finalPath);
        if (File.Exists(newPath) || Directory.Exists(newPath))
            return new ActionResponse(false, $"[red]'{finalPath.EscapeMarkup()}' already exists.[/]");

        try
        {
            if (isDir)
            {
                Directory.CreateDirectory(newPath);
            }
            else
            {
                Directory.CreateDirectory(Path.GetDirectoryName(newPath)!);
                File.Create(newPath).Close();
            }

            var createdName =
                Path.GetFileName(newPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
            return new ActionResponse(true, $"[green]Created '{createdName.EscapeMarkup()}'[/]", createdName);
        }
        catch (Exception ex)
        {
            return new ActionResponse(false, $"[red]Create failed: {ex.Message.EscapeMarkup()}[/]");
        }
    }

    public static ActionResponse Rename(string basePath, string oldName, string newName)
    {
        if (string.IsNullOrWhiteSpace(newName) || newName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            return new ActionResponse(false, "[red]Invalid name.[/]");

        var oldPath = Path.Combine(basePath, oldName);
        var newPath = Path.Combine(basePath, newName);

        if (File.Exists(newPath) || Directory.Exists(newPath))
            return new ActionResponse(false, $"[red]'{newName.EscapeMarkup()}' already exists.[/]");

        try
        {
            Directory.Move(oldPath, newPath);
            return new ActionResponse(true, $"[green]Renamed to '{newName.EscapeMarkup()}'[/]", newName);
        }
        catch (Exception ex)
        {
            return new ActionResponse(false, $"[red]Rename failed: {ex.Message.EscapeMarkup()}[/]");
        }
    }

    public static ActionResponse Delete(FileSystemItem item)
    {
        try
        {
            if (item.IsDirectory)
                Directory.Delete(item.Path, true);
            else
                File.Delete(item.Path);
            return new ActionResponse(true, $"[green]Deleted '{item.Name.EscapeMarkup()}'[/]");
        }
        catch (Exception ex)
        {
            return new ActionResponse(false, $"[red]Delete failed: {ex.Message.EscapeMarkup()}[/]");
        }
    }
    
    public static async Task<ActionResponse> MoveAsync(string sourcePath, string destinationPath,
        IProgress<(long totalBytes, long completedBytes, string currentFile)> progress, CancellationToken token)
    {
        try
        {
            var sourceDrive = Path.GetPathRoot(Path.GetFullPath(sourcePath));
            var destDrive = Path.GetPathRoot(Path.GetFullPath(destinationPath));

            if (sourceDrive == destDrive)
            {
                progress.Report((1, 0, $"Moving [green]{Path.GetFileName(sourcePath).EscapeMarkup()}[/]"));
                Directory.Move(sourcePath, destinationPath);
                progress.Report((1, 1, $"Moved [green]{Path.GetFileName(sourcePath).EscapeMarkup()}[/]"));
                return new ActionResponse(true, $"[green]Moved '{Path.GetFileName(sourcePath).EscapeMarkup()}'[/]");
            }

            var copyResponse = await CopyAsync(sourcePath, destinationPath, progress, token, true);
            if (copyResponse.Success)
            {
                progress.Report((1, 1,
                    $"Cleaning up original for [green]{Path.GetFileName(sourcePath).EscapeMarkup()}[/]"));
                if (File.GetAttributes(sourcePath).HasFlag(FileAttributes.Directory))
                    Directory.Delete(sourcePath, true);
                else
                    File.Delete(sourcePath);

                return new ActionResponse(true,
                    $"[green]Moved '{Path.GetFileName(sourcePath).EscapeMarkup()}' across drives[/]");
            }

            return new ActionResponse(false,
                $"[red]Move (copy phase) failed: {copyResponse.Message.EscapeMarkup()}[/]");
        }
        catch (Exception ex)
        {
            return new ActionResponse(false, $"[red]Move failed: {ex.Message.EscapeMarkup()}[/]");
        }
    }

    public static async Task<ActionResponse> CopyAsync(string sourcePath, string destinationPath,
        IProgress<(long totalBytes, long completedBytes, string currentFile)> progress, CancellationToken token,
        bool isMove = false)
    {
        var operationName = isMove ? "Moving" : "Copying";
        try
        {
            if (File.GetAttributes(sourcePath).HasFlag(FileAttributes.Directory))
            {
                var sourceDir = new DirectoryInfo(sourcePath);
                var totalSize = GetDirectorySize(sourceDir);
                long completedBytes = 0;

                progress.Report((totalSize, 0, $"{operationName} directory [green]{sourceDir.Name.EscapeMarkup()}[/]"));
                await CopyDirectoryRecursive(sourceDir, new DirectoryInfo(destinationPath), token, (chunkCopied) =>
                {
                    completedBytes += chunkCopied;
                    progress.Report((totalSize, completedBytes, "")); 
                });
            }
            else
            {
                var sourceFile = new FileInfo(sourcePath);
                long completedBytes = 0;
                progress.Report((sourceFile.Length, 0,
                    $"{operationName} file [green]{sourceFile.Name.EscapeMarkup()}[/]"));
                await CopyFileAsync(sourceFile, destinationPath, token,
                    (chunkCopied) =>
                    {
                        completedBytes += chunkCopied;
                        progress.Report((sourceFile.Length, completedBytes, ""));
                    });
            }

            return new ActionResponse(true,
                $"[green]Successfully {operationName.ToLower()}ed '{Path.GetFileName(sourcePath).EscapeMarkup()}'[/]");
        }
        catch (OperationCanceledException)
        {
            if (Directory.Exists(destinationPath)) Directory.Delete(destinationPath, true);
            if (File.Exists(destinationPath)) File.Delete(destinationPath);
            return new ActionResponse(false, $"[yellow]{operationName} was cancelled.[/]");
        }
        catch (Exception ex)
        {
            return new ActionResponse(false, $"[red]{operationName} failed: {ex.Message.EscapeMarkup()}[/]");
        }
    }

    private static long GetDirectorySize(DirectoryInfo dirInfo)
    {
        long size = 0;
        try
        {
            size += dirInfo.GetFiles().Sum(fi => fi.Length);
            size += dirInfo.GetDirectories().Sum(di => GetDirectorySize(di));
        }
        catch (UnauthorizedAccessException)
        {
            /* Skip inaccessible directories */
        }

        return size;
    }

    private static async Task CopyDirectoryRecursive(DirectoryInfo source, DirectoryInfo target,
        CancellationToken token, Action<long> onBytesCopied)
    {
        token.ThrowIfCancellationRequested();
        Directory.CreateDirectory(target.FullName);

        foreach (var fi in source.GetFiles())
        {
            token.ThrowIfCancellationRequested();
            var targetFilePath = Path.Combine(target.FullName, fi.Name);
            await CopyFileAsync(fi, targetFilePath, token, onBytesCopied);
        }

        foreach (var diSourceSubDir in source.GetDirectories())
        {
            token.ThrowIfCancellationRequested();
            var nextTargetSubDir = new DirectoryInfo(Path.Combine(target.FullName, diSourceSubDir.Name));
            await CopyDirectoryRecursive(diSourceSubDir, nextTargetSubDir, token, onBytesCopied);
        }
    }

    private static async Task CopyFileAsync(FileInfo sourceFile, string destFile, CancellationToken token,
        Action<long> onProgress)
    {
        const int bufferSize = 81920; 

        await using var sourceStream = new FileStream(sourceFile.FullName, FileMode.Open, FileAccess.Read,
            FileShare.Read, bufferSize, true);
        await using var destinationStream =
            new FileStream(destFile, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize, true);

        var buffer = new byte[bufferSize];
        int bytesRead;
        while ((bytesRead = await sourceStream.ReadAsync(buffer, token)) > 0)
        {
            token.ThrowIfCancellationRequested();
            await destinationStream.WriteAsync(buffer.AsMemory(0, bytesRead), token);
            onProgress(bytesRead);
        }
    }
}
