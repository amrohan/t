# ActionService

The ActionService is responsible for all file and directory operations in Termix. It provides static methods for creating, renaming, deleting, copying, and moving files and directories, with built-in error handling and progress reporting.

## Overview

```csharp
public abstract class ActionService
{
    // Synchronous operations
    public static ActionResponse Create(string basePath, string userInput);
    public static ActionResponse Rename(string basePath, string oldName, string newName);
    public static ActionResponse Delete(FileSystemItem item);
    
    // Asynchronous operations with progress
    public static Task<ActionResponse> CopyAsync(string sourcePath, string destinationPath, 
        IProgress<(long totalBytes, long completedBytes, string currentFile)> progress, 
        CancellationToken token, bool isMove = false);
    public static Task<ActionResponse> MoveAsync(string sourcePath, string destinationPath,
        IProgress<(long totalBytes, long completedBytes, string currentFile)> progress, 
        CancellationToken token);
    
    // Search operations
    public static Task<List<FileSystemItem>> GetDeepDirectoryContentsAsync(string basePath, 
        CancellationToken token);
}
```

## ActionResponse

All operations return an `ActionResponse` record that provides consistent feedback:

```csharp
public record ActionResponse(bool Success, string Message, object? Payload = null);
```

- **Success**: Whether the operation completed successfully
- **Message**: User-friendly message (formatted with markup for Spectre.Console)
- **Payload**: Optional additional data (e.g., created file name)

## File and Directory Creation

### Create Method

Creates new files or directories based on user input:

```csharp
public static ActionResponse Create(string basePath, string userInput)
```

**Parameters:**
- `basePath`: Directory where the item will be created
- `userInput`: Name/path specified by user

**Behavior:**
- **File Creation**: Creates file with specified name
- **Directory Creation**: If name ends with `/` or `\`
- **Auto Extension**: Adds `.txt` if no extension and not a directory
- **Nested Paths**: Creates parent directories as needed
- **Validation**: Checks for invalid characters and existing items

**Examples:**
```csharp
// Create a text file
var result = ActionService.Create("/home/user", "notes.txt");

// Create a directory
var result = ActionService.Create("/home/user", "projects/");

// Auto-extension
var result = ActionService.Create("/home/user", "script"); // Creates script.txt

// Nested creation
var result = ActionService.Create("/home/user", "docs/api/readme.md");
```

**Return Values:**
- **Success**: `ActionResponse(true, "[green]Created 'filename'[/]", "filename")`
- **Error**: `ActionResponse(false, "[red]Create failed: reason[/]")`

## File and Directory Renaming

### Rename Method

Renames files or directories:

```csharp
public static ActionResponse Rename(string basePath, string oldName, string newName)
```

**Parameters:**
- `basePath`: Directory containing the item
- `oldName`: Current name of the item
- `newName`: Desired new name

**Validation:**
- Checks for invalid filename characters
- Prevents duplicate names
- Handles both files and directories

**Examples:**
```csharp
// Rename a file
var result = ActionService.Rename("/project", "old-name.txt", "new-name.txt");

// Change file extension
var result = ActionService.Rename("/scripts", "script.txt", "script.sh");

// Rename directory
var result = ActionService.Rename("/home", "old-folder", "new-folder");
```

## File and Directory Deletion

### Delete Method

Deletes files or directories:

```csharp
public static ActionResponse Delete(FileSystemItem item)
```

**Parameters:**
- `item`: FileSystemItem to delete

**Behavior:**
- **Files**: Direct deletion
- **Directories**: Recursive deletion (all contents removed)
- **Safety**: No undo functionality - permanent deletion

**Examples:**
```csharp
var fileItem = new FileSystemItem("/home/user/old-file.txt", "old-file.txt", false, 1024, DateTime.Now);
var result = ActionService.Delete(fileItem);

var dirItem = new FileSystemItem("/home/user/old-dir", "old-dir", true, 0, DateTime.Now);
var result = ActionService.Delete(dirItem); // Deletes directory and all contents
```

## Asynchronous Copy Operations

### CopyAsync Method

Copies files or directories with progress reporting:

```csharp
public static async Task<ActionResponse> CopyAsync(
    string sourcePath, 
    string destinationPath,
    IProgress<(long totalBytes, long completedBytes, string currentFile)> progress, 
    CancellationToken token, 
    bool isMove = false)
```

**Parameters:**
- `sourcePath`: Source file or directory path
- `destinationPath`: Destination path
- `progress`: Progress reporting callback
- `token`: Cancellation token
- `isMove`: Internal flag indicating if this is part of a move operation

**Features:**
- **Progress Tracking**: Reports bytes copied and current file
- **Cancellation**: Supports cooperative cancellation
- **Directory Recursion**: Handles nested directory structures
- **Error Recovery**: Continues on individual file errors
- **Size Calculation**: Pre-calculates total operation size

**Progress Reporting:**
```csharp
var progress = new Progress<(long totalBytes, long completedBytes, string currentFile)>(
    value => Console.WriteLine($"Progress: {value.completedBytes}/{value.totalBytes} - {value.currentFile}"));

var result = await ActionService.CopyAsync(
    "/source/large-directory", 
    "/destination/large-directory",
    progress, 
    CancellationToken.None);
```

## Asynchronous Move Operations

### MoveAsync Method

Moves files or directories with progress reporting:

```csharp
public static async Task<ActionResponse> MoveAsync(
    string sourcePath, 
    string destinationPath,
    IProgress<(long totalBytes, long completedBytes, string currentFile)> progress, 
    CancellationToken token)
```

**Smart Move Logic:**
1. **Same Drive**: Uses native filesystem move (instant)
2. **Cross Drive**: Performs copy + delete with progress tracking
3. **Automatic Detection**: Determines optimal strategy

**Examples:**
```csharp
// Same drive - instant move
var result = await ActionService.MoveAsync(
    "/home/user/file.txt", 
    "/home/projects/file.txt", 
    progress, token);

// Cross drive - copy with progress, then delete
var result = await ActionService.MoveAsync(
    "C:\\temp\\file.txt", 
    "D:\\backup\\file.txt", 
    progress, token);
```

## Search Operations

### GetDeepDirectoryContentsAsync Method

Recursively searches directory trees for files:

```csharp
public static Task<List<FileSystemItem>> GetDeepDirectoryContentsAsync(
    string basePath, 
    CancellationToken token)
```

**Features:**
- **Recursive Search**: Scans all subdirectories
- **Ignore Integration**: Respects .gitignore patterns
- **Cancellation**: Can be interrupted
- **Performance**: Optimized for large directory trees

**Usage:**
```csharp
var token = new CancellationTokenSource().Token;
var allFiles = await ActionService.GetDeepDirectoryContentsAsync("/project", token);

// Filter results
var jsFiles = allFiles.Where(f => f.Name.EndsWith(".js")).ToList();
```

## Error Handling

The ActionService provides comprehensive error handling:

### Common Error Scenarios

| Error Type | Cause | ActionResponse |
|------------|-------|----------------|
| **Permission Denied** | Insufficient file system permissions | `ActionResponse(false, "[red]Permission denied[/]")` |
| **File Not Found** | Source file doesn't exist | `ActionResponse(false, "[red]File not found[/]")` |
| **Disk Full** | Insufficient storage space | `ActionResponse(false, "[red]Disk full[/]")` |
| **Path Too Long** | Exceeds filesystem path limits | `ActionResponse(false, "[red]Path too long[/]")` |
| **Invalid Characters** | Illegal characters in filename | `ActionResponse(false, "[red]Invalid characters[/]")` |

### Exception Handling Strategy

1. **Catch and Convert**: Exceptions are caught and converted to ActionResponse
2. **User-Friendly Messages**: Technical errors are translated to readable messages
3. **Markup Formatting**: Messages use Spectre.Console markup for styling
4. **Context Preservation**: Important error details are preserved

### Cancellation Handling

For async operations:
```csharp
try 
{
    var result = await ActionService.CopyAsync(source, dest, progress, token);
}
catch (OperationCanceledException)
{
    // Operation was cancelled by user
    // Cleanup is handled automatically
}
```

## Progress Reporting

### Progress Data Structure

```csharp
public struct ProgressData
{
    public long TotalBytes { get; }
    public long CompletedBytes { get; }
    public string CurrentFile { get; }
}
```

### Implementation Example

```csharp
public class ProgressReporter : IProgress<(long total, long completed, string current)>
{
    public void Report((long total, long completed, string current) value)
    {
        var percentage = value.total > 0 ? (value.completed * 100.0 / value.total) : 0;
        Console.WriteLine($"[{percentage:F1}%] {value.current}");
    }
}

var progress = new ProgressReporter();
await ActionService.CopyAsync(source, dest, progress, token);
```

## Performance Considerations

### File I/O Optimization
- **Buffer Size**: Uses optimal buffer sizes for file copying (81,920 bytes)
- **Async I/O**: Non-blocking file operations
- **Streaming**: Processes large files without loading entirely into memory

### Memory Management
- **Disposal**: Proper cleanup of file streams and resources
- **Cancellation**: Early termination to free resources
- **Progress Throttling**: Limits progress update frequency

### Directory Operations
- **Batch Processing**: Groups related operations
- **Error Isolation**: Individual file failures don't stop entire operation
- **Path Optimization**: Efficient path manipulation and validation

## Integration with FileManager

The ActionService integrates tightly with the FileManager:

```csharp
// FileManager orchestrates operations with UI feedback
private async void ExecuteFileOperation()
{
    _isOperationInProgress = true;
    _progressValue = 0;
    
    var progress = new Progress<(long, long, string)>(UpdateProgress);
    
    try 
    {
        var result = await ActionService.CopyAsync(source, dest, progress, _cancellationToken);
        _statusMessage = result.Message;
        
        if (result.Success) 
        {
            RefreshDirectory();
        }
    }
    finally 
    {
        _isOperationInProgress = false;
    }
}
```

## Testing Considerations

When testing ActionService methods:

1. **Temporary Directories**: Use test-specific temporary directories
2. **Cleanup**: Ensure test artifacts are removed
3. **Permission Testing**: Test various permission scenarios
4. **Cancellation**: Verify cancellation token behavior
5. **Progress Reporting**: Mock progress interfaces for testing

::: tip Performance Tips
For best performance with ActionService:
- Use cancellation tokens to allow user interruption
- Implement progress reporting for operations that may take time
- Handle errors gracefully and provide actionable feedback
- Consider using async operations for better UI responsiveness
:::

::: warning Data Safety
ActionService operations are irreversible. The Delete operation permanently removes files and directories. Always implement appropriate confirmation mechanisms in the UI layer.
:::
