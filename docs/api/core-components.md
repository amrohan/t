# Core Components

This document details the core architectural components that form the foundation of Termix. These components work together to provide the application's functionality while maintaining clean separation of concerns.

## Component Overview

```
┌─────────────────────┐
│     Program.cs      │  Application Entry Point
└─────────────────────┘
           │
┌─────────────────────┐
│    FileManager      │  Central Controller
└─────────────────────┘
           │
    ┌──────┴──────┐
    │             │
┌───▼────┐   ┌────▼────┐
│Services│   │UI Layer │
└────────┘   └─────────┘
```

## Program.cs - Application Entry Point

### Responsibilities
- **Command-line argument processing**
- **Global exception handling**
- **Application initialization and cleanup**
- **Terminal environment setup**

### Implementation

```csharp
public static class Program
{
    public static Task Main(string[] args)
    {
        var useIcons = !args.Contains("--no-icons");
        try
        {
            AnsiConsole.Clear();
            var fileManager = new FileManager(useIcons);
            fileManager.Run();
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteException(ex, ExceptionFormats.ShortenEverything | ExceptionFormats.ShowLinks);
        }
        finally
        {
            AnsiConsole.Clear();
        }

        return Task.CompletedTask;
    }
}
```

### Key Features
- **Command-line options**: Processes `--no-icons` flag
- **Exception handling**: Global error catching with formatted output
- **Clean terminal**: Ensures terminal is cleared before and after execution
- **Simple entry**: Minimal startup logic, delegates to FileManager

## Models - Data Structures

### FileSystemItem

The core data model representing files and directories:

```csharp
public class FileSystemItem
{
    public string Path { get; init; }
    public string Name { get; init; }  
    public bool IsDirectory { get; init; }
    public long Size { get; init; }
    public DateTime LastWriteTime { get; init; }
    public bool IsParentDirectory { get; init; }

    public FileSystemItem(string path, string name, bool isDirectory, 
        long size, DateTime lastWriteTime, bool isParentDirectory = false)
    {
        Path = path;
        Name = name;
        IsDirectory = isDirectory;
        Size = size;
        LastWriteTime = lastWriteTime;
        IsParentDirectory = isParentDirectory;
    }
}
```

**Design Decisions:**
- **Immutable**: Uses `init` properties for thread safety
- **Complete metadata**: Includes all necessary file information
- **Display name**: Name may be truncated for UI purposes
- **Parent directory flag**: Special handling for ".." entries

### ClipboardItem

Represents items in the application clipboard:

```csharp
public class ClipboardItem
{
    public FileSystemItem Item { get; }
    public ClipboardMode Mode { get; }

    public ClipboardItem(FileSystemItem item, ClipboardMode mode)
    {
        Item = item;
        Mode = mode;
    }
}

public enum ClipboardMode
{
    Copy,
    Move
}
```

**Features:**
- **Mode tracking**: Distinguishes between copy and move operations
- **Item reference**: Maintains reference to the original file system item
- **State preservation**: Clipboard contents persist across navigation

## Input Processing

### InputHandler

Processes keyboard input and translates it to FileManager operations:

```csharp
public class InputHandler
{
    private readonly FileManager _fileManager;

    public InputHandler(FileManager fileManager)
    {
        _fileManager = fileManager;
    }

    public void ProcessKey(ConsoleKeyInfo keyInfo)
    {
        switch (_fileManager.CurrentMode)
        {
            case FileManager.InputMode.Normal:
                ProcessNormalModeKey(keyInfo);
                break;
            case FileManager.InputMode.Filter:
                ProcessFilterModeKey(keyInfo);
                break;
            // ... other modes
        }
    }
}
```

**Key Features:**
- **Mode-specific processing**: Different behavior for each input mode
- **Key translation**: Maps raw keyboard input to logical operations
- **Error isolation**: Input errors don't crash the application

### Key Processing Strategy

```csharp
private void ProcessNormalModeKey(ConsoleKeyInfo keyInfo)
{
    switch (keyInfo.Key)
    {
        case ConsoleKey.UpArrow:
        case ConsoleKey.K when keyInfo.Modifiers == 0:
            _fileManager.MoveSelection(-1);
            break;
            
        case ConsoleKey.DownArrow:
        case ConsoleKey.J when keyInfo.Modifiers == 0:
            _fileManager.MoveSelection(1);
            break;
            
        case ConsoleKey.Enter:
        case ConsoleKey.L when keyInfo.Modifiers == 0:
            _fileManager.OpenSelectedItem();
            break;
            
        // ... more key mappings
    }
}
```

## UI Components

### DoubleBufferedRenderer

Provides flicker-free terminal rendering:

```csharp
public class DoubleBufferedRenderer
{
    private string _lastRendered = "";

    public void Render(IRenderable content)
    {
        var currentContent = AnsiConsole.ToAnsi(content);
        
        if (currentContent != _lastRendered)
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(content);
            _lastRendered = currentContent;
        }
    }
}
```

**Benefits:**
- **Flicker reduction**: Only redraws when content changes
- **Performance**: Avoids unnecessary terminal operations
- **Smooth experience**: Maintains visual continuity

### FileManagerRenderer

Creates the visual layout for the file manager:

```csharp
public class FileManagerRenderer
{
    private readonly IconProvider _iconProvider;

    public IRenderable GetLayout(string currentPath, List<FileSystemItem> items, 
        int selectedIndex, IRenderable preview, int viewOffset, IRenderable footer)
    {
        var pathPanel = CreatePathPanel(currentPath);
        var contentLayout = CreateTwoPaneLayout(items, selectedIndex, preview, viewOffset);
        
        return new Rows(pathPanel, contentLayout, footer);
    }
}
```

**Layout Structure:**
```
┌─────────────────────────────────────┐
│          Path Panel                 │  Current directory path
├─────────────────────────────────────┤
│ File List    │    Preview Panel     │  Two-pane layout
│              │                      │
│              │                      │
├─────────────────────────────────────┤
│          Footer Panel               │  Status and shortcuts
└─────────────────────────────────────┘
```

### IconProvider

Maps file types to appropriate icons:

```csharp
public class IconProvider
{
    private readonly bool _useIcons;
    private readonly Dictionary<string, string> _iconMap;

    public IconProvider(bool useIcons)
    {
        _useIcons = useIcons;
        _iconMap = BuildIconMap();
    }

    public string GetIcon(FileSystemItem item)
    {
        if (!_useIcons) return GetAsciiIcon(item);
        
        if (item.IsDirectory) return "📁";
        
        var extension = Path.GetExtension(item.Name).ToLower();
        return _iconMap.TryGetValue(extension, out var icon) ? icon : "📄";
    }
}
```

**Icon Categories:**
- **Directories**: Folder icons
- **Code files**: Language-specific icons
- **Documents**: Document type icons
- **Images**: Image format icons
- **Configuration**: Settings/config icons

## Service Integration

### Service Coordination

Core components coordinate with the services layer:

```csharp
// FileManager coordinates services
private void RefreshDirectory()
{
    // FileSystemService reads directory
    _unfilteredItems = FileSystemService.GetDirectoryContents(_currentPath);
    
    // IgnoreService filters results (implicit)
    _currentItems = ApplyIgnoreFilters(_unfilteredItems);
    
    // Update UI
    UpdatePreview();
    SetNeedsRedraw();
}

private void ExecuteFileOperation()
{
    // ActionService performs operation
    var result = await ActionService.CopyAsync(source, dest, progress, token);
    
    // Update UI based on result
    _statusMessage = result.Message;
    if (result.Success) RefreshDirectory();
}
```

## Component Communication

### Event Flow

```
User Input → InputHandler → FileManager → Services → UI Update
    ↑                                                      ↓
    └──────────── Rendered Display ←────────────────────────┘
```

### Data Flow

1. **Input Processing**: InputHandler receives keystrokes
2. **Command Translation**: Keys mapped to FileManager operations
3. **Service Coordination**: FileManager calls appropriate services
4. **State Update**: Application state modified based on results
5. **UI Refresh**: Renderer updates display with new state

## Error Handling Architecture

### Error Boundaries

Each component handles errors at its level:

```csharp
// Program.cs - Global handler
catch (Exception ex)
{
    AnsiConsole.WriteException(ex);
}

// FileManager - Operation handler  
catch (Exception ex)
{
    _statusMessage = $"[red]Operation failed: {ex.Message}[/]";
    SetNeedsRedraw();
}

// Services - Detailed handler
catch (FileNotFoundException ex)
{
    return new ActionResponse(false, $"[red]File not found: {ex.FileName}[/]");
}
```

**Error Handling Principles:**
- **Graceful degradation**: Application continues running
- **User feedback**: Clear, actionable error messages
- **State preservation**: Errors don't corrupt application state
- **Logging context**: Preserve error details for debugging

## Performance Architecture

### Efficient Updates

Components are designed for minimal work:

```csharp
// Only redraw when necessary
private void SetNeedsRedraw()
{
    _needsRedraw = true;
}

// Debounced operations
_debounceCts.Cancel();
_debounceCts = new CancellationTokenSource();

// Background processing
Task.Run(async () => {
    var result = await LongRunningOperation();
    // Update UI on main thread
}, token);
```

### Memory Management

- **Minimal object creation**: Reuse objects where possible
- **Efficient collections**: Use appropriate data structures
- **Proper disposal**: Clean up resources promptly
- **Garbage collection**: Minimize GC pressure

## Testing Architecture

### Component Testing Strategy

```csharp
// Unit test individual components
[Test]
public void FileSystemItem_Constructor_SetsProperties()
{
    var item = new FileSystemItem("/path", "name", true, 1024, DateTime.Now);
    Assert.That(item.Path, Is.EqualTo("/path"));
    Assert.That(item.IsDirectory, Is.True);
}

// Integration test component interactions
[Test] 
public void FileManager_Navigation_UpdatesState()
{
    var fileManager = new FileManager(false);
    fileManager.NavigateToDirectory("/test");
    Assert.That(fileManager.CurrentPath, Is.EqualTo("/test"));
}
```

### Mock Strategies

Components are designed for testability:
- **Dependency injection**: Services injected into components
- **Interface abstraction**: Mock implementations for testing
- **State verification**: Internal state can be inspected
- **Behavior testing**: Operations produce expected results

## Extension Points

### Adding New Components

1. **Define interfaces**: Create clear contracts
2. **Implement component**: Follow existing patterns
3. **Integrate with FileManager**: Add coordination logic
4. **Update UI**: Modify renderer if needed
5. **Add input handling**: Map keys to new operations

### Component Guidelines

- **Single responsibility**: Each component has one clear purpose
- **Clear interfaces**: Well-defined public API
- **Error handling**: Robust error management
- **Performance conscious**: Efficient implementation
- **Testable design**: Easy to unit test

::: tip Component Design
Termix components follow the principle of "high cohesion, loose coupling." Each component has a clear, focused responsibility but can work independently of others.
:::

::: warning State Management
While components are loosely coupled, state management is centralized in FileManager. Be careful when modifying state-related code to maintain consistency.
:::
