# FileSystemService

The FileSystemService provides low-level file system operations for Termix. It handles directory enumeration, file system item creation, and external application launching with cross-platform compatibility.

## Overview

```csharp
public abstract class FileSystemService
{
    public static List<FileSystemItem> GetDirectoryContents(string path);
    public static void OpenFile(string filePath);
}
```

## Core Functionality

### Directory Enumeration

The primary responsibility of FileSystemService is to read directory contents and convert them into FileSystemItem objects.

#### GetDirectoryContents Method

```csharp
public static List<FileSystemItem> GetDirectoryContents(string path)
```

**Parameters:**
- `path`: Absolute path to the directory to read

**Returns:**
- `List<FileSystemItem>`: Sorted list of files and directories

**Behavior:**
- **Parent Directory**: Always includes `..` entry (except at filesystem root)
- **Sorting**: Directories first, then files, both alphabetically
- **Error Handling**: Returns empty list on permission errors

### File System Item Structure

The method returns FileSystemItem objects with the following structure:

```csharp
public class FileSystemItem
{
    public string Path { get; }           // Full absolute path
    public string Name { get; }           // Display name (may be truncated)
    public bool IsDirectory { get; }      // True for directories
    public long Size { get; }             // File size in bytes (0 for directories)
    public DateTime LastWriteTime { get; } // Last modification time
    public bool IsParentDirectory { get; } // True for ".." entries
}
```

## Directory Reading Process

### 1. Parent Directory Handling

```csharp
if (directoryInfo.Parent != null)
    items.Add(new FileSystemItem(
        directoryInfo.Parent.FullName, 
        "..", 
        true, 
        0,
        directoryInfo.Parent.LastWriteTime, 
        true // IsParentDirectory flag
    ));
```

The parent directory entry (`..`) is always first in the list and provides:
- Quick navigation to parent
- Parent directory metadata
- Special handling in UI (different behavior)

### 2. Directory Enumeration

```csharp
var directories = directoryInfo.GetDirectories()
    .OrderBy(d => d.Name, StringComparer.OrdinalIgnoreCase)
    .Select(d => new FileSystemItem(d.FullName, d.Name, true, 0, d.LastWriteTime));
```

Directories are:
- Sorted alphabetically (case-insensitive)
- Listed before files
- Show size as 0 (directory size not calculated for performance)

### 3. File Enumeration

```csharp
var files = directoryInfo.GetFiles()
    .OrderBy(f => f.Name, StringComparer.OrdinalIgnoreCase)
    .Select(f => CreateFileSystemItem(f));
```

Files are:
- Sorted alphabetically (case-insensitive)
- Listed after directories
- Include actual file size and metadata

## File Name Truncation

For performance and display purposes, long filenames are truncated:

```csharp
if (name.Length <= 24)
    return new FileSystemItem(fullName, name, false, f.Length, f.LastWriteTime);

var ext = Path.GetExtension(name);
var extLen = ext.Length;
var baseLen = 24 - extLen - 2; // Space for ".." and extension

if (baseLen <= 0)
    name = ".." + ext;  // Very long extension
else
    name = string.Concat(name.AsSpan(0, baseLen), "..", ext);
```

**Truncation Rules:**
- Maximum display length: 24 characters
- Preserves file extension
- Uses `..` in the middle to indicate truncation
- Handles edge cases (very long extensions)

**Examples:**
- `very-long-filename.txt` → `very-long-filen..txt`
- `file.verylongextension` → `..verylongextension`

## External File Operations

### OpenFile Method

```csharp
public static void OpenFile(string filePath)
```

**Purpose:** Opens files in the system's default application

**Cross-Platform Implementation:**
```csharp
try
{
    var processStartInfo = new ProcessStartInfo(filePath) 
    { 
        UseShellExecute = true 
    };
    Process.Start(processStartInfo);
}
catch (Exception ex)
{
    // Error handling with user feedback
    AnsiConsole.MarkupLine($"[red]Error opening file: {ex.Message}[/]");
    Console.ReadKey(true);
}
```

**Platform Behavior:**
- **Windows**: Uses shell associations (e.g., `.txt` opens in Notepad)
- **macOS**: Uses `open` command equivalent
- **Linux**: Uses default application associations

**Supported File Types:**
- **Text files**: Open in default text editor
- **Images**: Open in default image viewer
- **Documents**: Open in associated applications
- **Executables**: Run (with appropriate permissions)
- **URLs**: Open in default browser

## Error Handling

### Directory Access Errors

When directory enumeration fails:

```csharp
try
{
    var directories = directoryInfo.GetDirectories();
    var files = directoryInfo.GetFiles();
    // Process items...
}
catch (UnauthorizedAccessException)
{
    return new List<FileSystemItem>(); // Empty list
}
catch (DirectoryNotFoundException)
{
    return new List<FileSystemItem>(); // Empty list
}
```

**Common Scenarios:**
- **Permission denied**: Returns empty list, allows navigation to continue
- **Directory not found**: Graceful fallback
- **Network timeouts**: Handled transparently

### File Opening Errors

When file opening fails:
- **Permission errors**: Clear error message displayed
- **Missing applications**: System error dialog
- **File corruption**: Application-specific handling

## Performance Characteristics

### Directory Reading Performance

**Optimizations:**
- **Lazy evaluation**: Uses LINQ deferred execution
- **Single enumeration**: Files and directories read once
- **Memory efficient**: Doesn't load file contents
- **Cached metadata**: OS provides cached file information

**Scalability:**
- **Large directories**: Handles thousands of files efficiently
- **Deep hierarchies**: No recursion in basic enumeration
- **Network drives**: Respects network timeout settings

### Memory Usage

**FileSystemItem Memory Footprint:**
- **Path strings**: Shared when possible
- **Metadata**: Minimal overhead
- **No file content**: Only metadata stored
- **Garbage collection**: Efficient disposal

## Integration with Other Services

### IgnoreService Integration

While FileSystemService doesn't directly integrate with IgnoreService, the results are filtered elsewhere:

```csharp
// In calling code
var allItems = FileSystemService.GetDirectoryContents(path);
var filteredItems = allItems.Where(item => !ignoreService.IsIgnored(item.Path));
```

### ActionService Coordination

FileSystemService provides the data that ActionService operates on:

```csharp
// FileSystemService reads directory contents
var items = FileSystemService.GetDirectoryContents(currentPath);

// ActionService operates on specific items
var selectedItem = items[selectedIndex];
var result = ActionService.Delete(selectedItem);

// FileSystemService re-reads to reflect changes
var updatedItems = FileSystemService.GetDirectoryContents(currentPath);
```

## Cross-Platform Considerations

### Path Handling

**Windows:**
- Uses backslash (`\`) as separator
- Handles drive letters (`C:\`)
- Supports UNC paths (`\\server\share`)

**Unix/Linux/macOS:**
- Uses forward slash (`/`) as separator
- Case-sensitive file systems supported
- Symbolic link handling

### File System Features

**Windows-Specific:**
- NTFS security descriptors
- File attributes (hidden, system)
- Long path support (>260 characters)

**Unix-Specific:**
- File permissions (rwx)
- Symbolic and hard links
- Case sensitivity variations

## Testing Strategies

### Unit Testing

```csharp
[Test]
public void GetDirectoryContents_ReturnsExpectedItems()
{
    // Arrange
    var testDir = CreateTestDirectory();
    CreateTestFiles(testDir);
    
    // Act
    var items = FileSystemService.GetDirectoryContents(testDir);
    
    // Assert
    Assert.That(items.Count, Is.GreaterThan(0));
    Assert.That(items.First().IsParentDirectory, Is.True);
    
    // Cleanup
    DeleteTestDirectory(testDir);
}
```

### Integration Testing

Testing with real file system:
- **Permission scenarios**: Read-only directories
- **Special files**: Symlinks, devices
- **Large directories**: Performance testing
- **Network drives**: Timeout behavior

## Best Practices

### When Using FileSystemService

1. **Error Handling**: Always handle potential exceptions
2. **Path Validation**: Ensure paths are absolute and valid
3. **Performance**: Don't call repeatedly for the same directory
4. **Memory**: Dispose of large lists when done

### Performance Tips

1. **Caching**: Cache results for frequently accessed directories
2. **Async Operations**: Use Task.Run for large directories
3. **Filtering**: Apply filters after reading to avoid re-enumeration
4. **Pagination**: Consider limiting results for very large directories

## Future Enhancements

### Planned Features

- **Asynchronous enumeration**: For better UI responsiveness
- **Watch functionality**: File system change notifications
- **Metadata caching**: Improved performance for repeated access
- **Extended attributes**: Support for file tags and extended metadata

### Extensibility Points

- **Custom item types**: Support for special file system objects
- **Metadata providers**: Pluggable metadata enrichment
- **File system abstraction**: Support for virtual file systems
- **Performance monitoring**: Built-in performance metrics

::: tip Usage Recommendation
Use FileSystemService for basic directory enumeration and file opening. For complex file operations, prefer ActionService which provides better error handling and progress reporting.
:::

::: warning Performance Note
FileSystemService operations are synchronous and can block on slow file systems. Consider using Task.Run for large directories or network drives to maintain UI responsiveness.
:::
