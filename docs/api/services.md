# Services

The Services layer in Termix provides the business logic and domain-specific functionality. This layer sits between the UI components and the core application logic, handling specialized tasks like file operations, preview generation, and ignore pattern management.

## Service Architecture

```
┌─────────────────────────────────────────────────────┐
│                 Services Layer                      │
├─────────────┬─────────────┬─────────────┬────────────┤
│ActionService│FileSystemSer│FilePreview  │IgnoreService│
│             │vice         │Service      │            │
└─────────────┴─────────────┴─────────────┴────────────┘
```

## IgnoreService

Manages file and directory ignore patterns based on `.gitignore` files and built-in rules.

### Overview

```csharp
public class IgnoreService
{
    public IgnoreService(string basePath);
    public bool IsIgnored(string path);
}
```

### Functionality

The IgnoreService reads `.gitignore` files and applies ignore patterns to determine which files and directories should be excluded from listings and searches.

#### Pattern Sources

1. **Local .gitignore**: Project-specific ignore patterns
2. **Global .gitignore**: User's global git ignore file
3. **Built-in patterns**: Common directories like `node_modules`, `bin`, `obj`

#### Built-in Ignore Patterns

```csharp
private static readonly string[] DefaultIgnorePatterns = {
    "node_modules/",
    ".git/",
    "bin/",
    "obj/", 
    ".vs/",
    ".vscode/",
    "__pycache__/",
    ".pytest_cache/",
    "target/",
    ".gradle/",
    ".DS_Store",
    "Thumbs.db"
};
```

#### Pattern Matching

Uses DotNet.Glob for efficient pattern matching:
- **Glob patterns**: Supports `*`, `?`, `**` wildcards
- **Directory patterns**: Handles trailing slashes correctly
- **Negation**: Supports `!` patterns for exceptions
- **Case sensitivity**: Respects file system case rules

### Usage Example

```csharp
var ignoreService = new IgnoreService("/path/to/project");

// Check individual files
bool shouldIgnore = ignoreService.IsIgnored("/path/to/project/node_modules/package.json");

// Filter file collections
var visibleFiles = allFiles.Where(f => !ignoreService.IsIgnored(f.Path)).ToList();
```

## FilePreviewService

Generates content previews for files in the preview pane.

### Overview

```csharp
public class FilePreviewService
{
    public IRenderable GetPreview(string? filePath, int verticalOffset, int horizontalOffset);
}
```

### Supported File Types

#### Text Files
- **Source code**: `.cs`, `.js`, `.ts`, `.py`, `.go`, `.rs`, etc.
- **Configuration**: `.json`, `.yaml`, `.xml`, `.ini`, `.toml`
- **Documentation**: `.md`, `.txt`, `.rst`

#### Binary Files  
- **Images**: Basic metadata and dimensions (via ImageSharp)
- **Archives**: File count and basic info
- **Executables**: File size and basic metadata

#### Special Handling
- **Large files**: Truncated with "..." indicator
- **Binary files**: Hex preview or metadata only
- **Encrypted files**: Shows as binary data

### Preview Features

#### Syntax Highlighting
```csharp
public class CustomSyntaxHighlighter
{
    public IRenderable Highlight(string content, string fileExtension)
    {
        var language = GetLanguageFromExtension(fileExtension);
        return ApplySyntaxHighlighting(content, language);
    }
}
```

**Supported Languages:**
- **C#**: Full syntax highlighting with keywords, strings, comments
- **JavaScript/TypeScript**: ES6+ syntax support
- **Python**: Python 3 syntax
- **JSON**: Key-value highlighting
- **Markdown**: Basic formatting

#### Scrolling Support
- **Vertical scrolling**: Navigate through long files
- **Horizontal scrolling**: Handle wide content
- **Offset tracking**: Maintains scroll position
- **Performance**: Only renders visible portion

### Performance Optimization

```csharp
public IRenderable GetPreview(string filePath, int verticalOffset, int horizontalOffset)
{
    // Check file size first
    var fileInfo = new FileInfo(filePath);
    if (fileInfo.Length > MaxPreviewSize)
    {
        return CreateLargeFilePreview(fileInfo);
    }

    // Read only needed portion for large text files
    var lines = ReadFileLines(filePath, verticalOffset, PreviewHeight);
    return CreateTextPreview(lines, horizontalOffset);
}
```

**Optimizations:**
- **Size limits**: Prevents loading huge files into memory
- **Lazy loading**: Only reads visible content
- **Caching**: Recently accessed files cached briefly
- **Async loading**: Large files processed in background

## Icon Provider Service

Maps file types to appropriate visual icons.

### Overview

```csharp
public class IconProvider
{
    public IconProvider(bool useIcons);
    public string GetIcon(FileSystemItem item);
    public string GetFileTypeDescription(FileSystemItem item);
}
```

### Icon Categories

#### File Type Icons (Nerd Fonts)
```csharp
private readonly Dictionary<string, string> _iconMap = new()
{
    // Programming languages
    { ".cs", "" },      // C# icon
    { ".js", "" },      // JavaScript icon  
    { ".ts", "" },      // TypeScript icon
    { ".py", "" },      // Python icon
    { ".go", "" },      // Go icon
    { ".rs", "" },      // Rust icon
    
    // Web technologies
    { ".html", "" },    // HTML icon
    { ".css", "" },     // CSS icon
    { ".scss", "" },    // SCSS icon
    
    // Configuration files
    { ".json", "" },    // JSON icon
    { ".yaml", "" },    // YAML icon
    { ".xml", "" },     // XML icon
    
    // Documentation
    { ".md", "" },      // Markdown icon
    { ".txt", "" },     // Text icon
};
```

#### ASCII Fallbacks
When icons aren't supported:
```csharp
private string GetAsciiIcon(FileSystemItem item)
{
    if (item.IsDirectory) return "[DIR]";
    
    var extension = Path.GetExtension(item.Name).ToLower();
    return extension switch
    {
        ".exe" => "[EXE]",
        ".dll" => "[DLL]", 
        ".zip" => "[ZIP]",
        _ => "[FILE]"
    };
}
```

#### Special Cases
- **Parent directory**: Always shows `..` or folder icon
- **Hidden files**: Dimmed or special styling
- **Executable files**: Special executable indicators
- **Symbolic links**: Link indicators where supported

## Service Integration Patterns

### Dependency Chain

Services often work together:

```csharp
// ActionService uses IgnoreService for recursive operations
public static async Task<List<FileSystemItem>> GetDeepDirectoryContentsAsync(
    string basePath, CancellationToken token)
{
    var results = new List<FileSystemItem>();
    var ignoreService = new IgnoreService(basePath);
    
    RecursiveSearch(currentDir, ignoreService, basePath, results, token);
    return results;
}

// FileManager coordinates all services
private void RefreshDirectory()
{
    // FileSystemService reads directory
    var items = FileSystemService.GetDirectoryContents(_currentPath);
    
    // IgnoreService filters implicitly through ActionService
    _currentItems = items; // Already filtered
    
    // FilePreviewService generates preview
    UpdatePreview();
}
```

### Service Lifecycle

Services are designed to be stateless or have minimal state:

```csharp
// FilePreviewService - stateless, can be reused
private readonly FilePreviewService _filePreviewService = new();

// IgnoreService - stateful for performance (caches patterns)  
private IgnoreService? _currentIgnoreService;

private IgnoreService GetIgnoreService(string basePath)
{
    if (_currentIgnoreService?.BasePath != basePath)
    {
        _currentIgnoreService = new IgnoreService(basePath);
    }
    return _currentIgnoreService;
}
```

## Error Handling in Services

### Graceful Degradation

Services handle errors gracefully:

```csharp
public IRenderable GetPreview(string filePath, int verticalOffset, int horizontalOffset)
{
    try
    {
        return GeneratePreview(filePath, verticalOffset, horizontalOffset);
    }
    catch (UnauthorizedAccessException)
    {
        return new Text("[red]Permission denied[/]");
    }
    catch (FileNotFoundException)
    {
        return new Text("[yellow]File not found[/]");
    }
    catch (Exception ex)
    {
        return new Text($"[red]Preview error: {ex.Message}[/]");
    }
}
```

### Service Error Patterns

1. **Fallback values**: Return safe defaults instead of throwing
2. **User-friendly messages**: Convert technical errors to readable text
3. **Logging**: Record errors for debugging without crashing
4. **State preservation**: Don't corrupt application state on errors

## Performance Characteristics

### Memory Usage

Services are designed for efficiency:

- **IgnoreService**: Caches compiled patterns, minimal memory overhead
- **FilePreviewService**: Stateless, no persistent cache
- **IconProvider**: Small lookup table, negligible memory usage

### I/O Optimization

- **Lazy loading**: Read files only when needed
- **Streaming**: Large files processed in chunks
- **Caching**: Frequently accessed data cached appropriately
- **Async patterns**: Non-blocking operations where beneficial

### CPU Efficiency

- **Pattern compilation**: Glob patterns compiled once, reused many times
- **Syntax highlighting**: Efficient parsing algorithms
- **Icon lookup**: O(1) hash table lookups

## Testing Services

### Unit Testing Approach

```csharp
[Test]
public void IgnoreService_NodeModules_ShouldBeIgnored()
{
    // Arrange
    var ignoreService = new IgnoreService("/test/project");
    
    // Act
    var result = ignoreService.IsIgnored("/test/project/node_modules/package.json");
    
    // Assert
    Assert.That(result, Is.True);
}

[Test]
public void FilePreviewService_TextFile_GeneratesPreview()
{
    // Arrange
    var previewService = new FilePreviewService();
    var testFile = CreateTestTextFile("Hello World");
    
    // Act
    var preview = previewService.GetPreview(testFile, 0, 0);
    
    // Assert
    Assert.That(preview, Is.Not.Null);
    // Additional preview content verification
}
```

### Integration Testing

Services should be tested with real file systems:

```csharp
[Test]
public void IgnoreService_WithRealGitIgnore_FiltersCorrectly()
{
    // Create temporary directory with .gitignore
    var tempDir = CreateTempDirectoryWithGitIgnore();
    
    try
    {
        var ignoreService = new IgnoreService(tempDir);
        var shouldIgnore = ignoreService.IsIgnored(Path.Combine(tempDir, "ignored.txt"));
        
        Assert.That(shouldIgnore, Is.True);
    }
    finally
    {
        Directory.Delete(tempDir, true);
    }
}
```

## Service Extension Points

### Adding New Services

1. **Define interface**: Create service contract
2. **Implement service**: Follow existing patterns
3. **Integrate with FileManager**: Add coordination logic
4. **Add error handling**: Graceful failure modes
5. **Write tests**: Unit and integration tests

### Custom Preview Providers

Extend FilePreviewService for new file types:

```csharp
public interface IFilePreviewProvider
{
    bool CanPreview(string filePath);
    IRenderable GeneratePreview(string filePath, int verticalOffset, int horizontalOffset);
}

// Custom provider for PDF files
public class PdfPreviewProvider : IFilePreviewProvider
{
    public bool CanPreview(string filePath) => 
        Path.GetExtension(filePath).Equals(".pdf", StringComparison.OrdinalIgnoreCase);
    
    public IRenderable GeneratePreview(string filePath, int verticalOffset, int horizontalOffset)
    {
        // PDF-specific preview logic
        return new Text("PDF Document Preview");
    }
}
```

## Future Enhancements

### Planned Service Features

- **Plugin architecture**: Loadable service modules
- **Configuration service**: User preferences and settings
- **Cache service**: Intelligent caching of expensive operations
- **Network service**: Remote file system support
- **Thumbnail service**: Image thumbnails in preview pane

### Performance Improvements

- **Background processing**: Move more operations to background threads
- **Smart caching**: More sophisticated cache management
- **Batch operations**: Group related operations for efficiency
- **Memory pooling**: Reuse objects to reduce GC pressure

::: tip Service Design Principles
Termix services follow these principles: stateless where possible, graceful error handling, efficient resource usage, and clear separation of concerns.
:::

::: info Extension Opportunity
The services layer is the most extensible part of Termix. New file types, preview modes, and ignore patterns can be added by extending existing services or creating new ones.
:::
