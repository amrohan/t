# FileManager

The FileManager class is the central controller of Termix, orchestrating all user interactions, state management, and UI coordination. It acts as the main application loop and coordinates between services, UI components, and user input.

## Overview

```csharp
public class FileManager
{
    public enum InputMode
    {
        Normal, Add, Rename, DeleteConfirm, Filter, FilteredNavigation, QuitConfirm
    }
    
    public InputMode CurrentMode { get; private set; } = InputMode.Normal;
    public bool IsViewFiltered => !string.IsNullOrEmpty(_inputText);
    
    public FileManager(bool useIcons);
    public void Run();
}
```

## Core Responsibilities

### 1. State Management
- **Current directory and navigation state**
- **File selection and view offset**
- **Input modes and user interaction state**
- **Clipboard contents and operation state**
- **Search/filter state and history**

### 2. Event Coordination
- **Keyboard input processing**
- **UI update orchestration**
- **Service operation coordination**
- **Progress tracking and reporting**

### 3. User Interface Control
- **Double-buffered rendering coordination**
- **Status message management**
- **Progress indication**
- **Mode-specific UI behavior**

## Input Mode State Machine

FileManager operates as a state machine with distinct modes:

```
Normal ←→ Filter ←→ FilteredNavigation
  ↕        ↕
 Add    Rename
  ↕        ↕
DeleteConfirm  QuitConfirm
```

### Mode Descriptions

| Mode | Purpose | Active Input | Exit Conditions |
|------|---------|--------------|----------------|
| **Normal** | Default navigation and operations | Navigation keys, operation keys | Mode change keys |
| **Add** | Creating new files/directories | Text input for name | Enter (create) or Esc (cancel) |
| **Rename** | Renaming existing items | Text input for new name | Enter (rename) or Esc (cancel) |
| **Filter** | Search input active | Text input for search query | Esc (apply filter) |
| **FilteredNavigation** | Navigating search results | Navigation keys | Esc (clear filter), B (back to results) |
| **DeleteConfirm** | Confirming deletion | y/n keys | y (delete), n/Esc (cancel) |
| **QuitConfirm** | Confirming quit during operations | y/n keys | y (force quit), n (continue) |

## Main Application Loop

### Run Method

The main application loop:

```csharp
public void Run()
{
    AnsiConsole.Clear();
    RefreshDirectory(setInitialSelection: true);

    while (!_shouldQuit)
    {
        if (_needsRedraw)
        {
            _needsRedraw = false;
            var footerContent = CreateFooterRenderable();
            var layout = _renderer.GetLayout(/* parameters */);
            _doubleBuffer.Render(layout);
        }

        while (!Console.KeyAvailable && !_needsRedraw && !_shouldQuit)
        {
            Thread.Sleep(50); // 20 FPS polling
        }

        if (Console.KeyAvailable)
        {
            _inputHandler.ProcessKey(Console.ReadKey(true));
        }
    }
}
```

**Key Features:**
- **Efficient polling**: 50ms sleep for ~20 FPS responsiveness
- **Conditional rendering**: Only redraws when needed
- **Input prioritization**: Keyboard input processed immediately
- **Clean exit**: Graceful shutdown handling

## State Management

### Directory State

```csharp
private string _currentPath = Directory.GetCurrentDirectory();
private List<FileSystemItem> _currentItems = [];
private List<FileSystemItem> _unfilteredItems = [];
private int _selectedIndex;
private int _viewOffset;
```

### Navigation State

```csharp
private readonly Stack<string> _navigationStack = new();
private (string Path, string Filter, List<FileSystemItem> Items, int SelectedIndex)? _savedFilterState;
```

### Operation State

```csharp
private ClipboardItem? _clipboard;
private bool _isOperationInProgress;
private string? _progressTaskDescription;
private double _progressValue;
private CancellationTokenSource? _operationCts;
```

### Input State

```csharp
private string _inputText = "";
private string _promptText = "";
private bool _isDeepSearchRunning;
private List<FileSystemItem>? _recursiveSearchCache;
```

## Key Methods

### Directory Management

#### RefreshDirectory
```csharp
private void RefreshDirectory(string? findAndSelect = null, bool preserveSelection = false, bool setInitialSelection = false)
```

Reloads current directory contents and manages selection:
- **findAndSelect**: Selects specific item by name after refresh
- **preserveSelection**: Maintains current selection index
- **setInitialSelection**: Sets selection to first item

#### NavigateToDirectory
```csharp
private void NavigateToDirectory(string path, string? findAndSelect = null)
```

Changes current directory with error handling and state management.

### File Operations

#### BeginAdd
```csharp
public void BeginAdd()
```

Initiates file/directory creation mode:
- Determines target directory (selected directory or current)
- Sets appropriate prompt text
- Switches to Add mode

#### BeginRename
```csharp
public void BeginRename()
```

Initiates rename mode:
- Pre-fills input with current name
- Validates rename capability
- Switches to Rename mode

#### BeginDelete
```csharp
public void BeginDelete()
```

Initiates deletion confirmation:
- Shows confirmation prompt
- Switches to DeleteConfirm mode

### Clipboard Operations

#### BeginCopy/BeginMove
```csharp
public void BeginCopy()
public void BeginMove()
```

Copies or cuts selected item to clipboard:
- Creates ClipboardItem with appropriate mode
- Updates status message
- Provides visual feedback

#### BeginPaste
```csharp
public void BeginPaste()
```

Initiates paste operation:
- Validates clipboard contents and destination
- Handles naming conflicts
- Starts async operation with progress tracking

### Search and Filter

#### BeginFilter
```csharp
public void BeginFilter()
```

Initiates search mode:
- Clears previous search state
- Switches to Filter mode
- Prepares for recursive search

#### UpdateFilter
```csharp
public void UpdateFilter(string newFilterText)
```

Updates search results in real-time:
- Debounces rapid input changes
- Triggers background recursive search if needed
- Applies filtering to current results

#### ApplyFilter/ClearFilter
```csharp
private void ApplyFilter()
public void ClearFilter()
```

Manages filter application and removal.

## UI Integration

### Rendering Coordination

The FileManager coordinates with the UI layer:

```csharp
private IRenderable CreateFooterRenderable()
{
    if (_isOperationInProgress)
    {
        // Show progress bar and status
    }
    else if (_statusMessage != null)
    {
        // Show status message
    }
    else if (_clipboard != null)
    {
        // Show clipboard contents
    }
    else
    {
        // Show mode-appropriate shortcuts
    }
}
```

### Double Buffering

Uses DoubleBufferedRenderer for flicker-free updates:
- Only renders when `_needsRedraw` is true
- Efficient for terminal applications
- Smooth user experience

## Progress Reporting

### Async Operation Progress

```csharp
private void BeginPaste()
{
    var progress = new Progress<(long totalBytes, long completedBytes, string currentFile)>(value =>
    {
        _progressTaskDescription = value.currentFile;
        _progressValue = value.totalBytes > 0 ? (double)value.completedBytes / value.totalBytes * 100 : 0;
        SetNeedsRedraw();
    });

    Task.Run(async () =>
    {
        var result = await ActionService.CopyAsync(source, dest, progress, token);
        // Handle completion
    });
}
```

**Features:**
- **Real-time progress**: Updates UI during long operations
- **File-level detail**: Shows current file being processed
- **Percentage calculation**: Provides completion percentage
- **Cancellation support**: User can interrupt operations

## Error Handling

### Graceful Error Management

```csharp
private void NavigateToDirectory(string path, string? findAndSelect = null)
{
    try
    {
        _currentPath = Path.GetFullPath(path);
        RefreshDirectory(findAndSelect, setInitialSelection: true);
    }
    catch (Exception ex)
    {
        _statusMessage = $"[red]Navigation failed: {ex.Message.EscapeMarkup()}[/]";
        SetNeedsRedraw();
    }
}
```

**Error Handling Strategy:**
- **Non-fatal errors**: Show status messages, continue operation
- **User feedback**: Clear, actionable error messages
- **State preservation**: Maintain valid state even after errors
- **Graceful degradation**: Application remains functional

## Performance Optimization

### Efficient State Updates

```csharp
private void SetNeedsRedraw()
{
    _needsRedraw = true;
}
```

Only triggers redraws when necessary, reducing CPU usage.

### Debounced Operations

Search input is debounced to prevent excessive operations:
```csharp
_debounceCts.Cancel();
_debounceCts = new CancellationTokenSource();
var token = _debounceCts.Token;
```

### Background Processing

Long-running operations like recursive search run in background:
```csharp
ActionService.GetDeepDirectoryContentsAsync(_currentPath, token).ContinueWith(task =>
{
    if (task.IsCompletedSuccessfully)
    {
        _recursiveSearchCache = task.Result;
        ApplyFilter();
    }
    _isDeepSearchRunning = false;
    SetNeedsRedraw();
}, token);
```

## Integration Points

### Service Coordination

FileManager coordinates multiple services:
- **ActionService**: File operations
- **FileSystemService**: Directory reading
- **FilePreviewService**: Content preview
- **IgnoreService**: Implicit through ActionService

### InputHandler Integration

```csharp
private readonly InputHandler _inputHandler;

public FileManager(bool useIcons)
{
    _inputHandler = new InputHandler(this);
}
```

InputHandler processes keyboard input and calls FileManager methods.

### Renderer Integration

```csharp
private readonly FileManagerRenderer _renderer;

var layout = _renderer.GetLayout(_currentPath, _currentItems, _selectedIndex, 
    _currentPreview, _viewOffset, footerContent);
```

FileManagerRenderer creates the visual layout based on current state.

## Testing Considerations

### State Testing

Key state transitions should be tested:
- Mode changes
- Directory navigation
- Selection management
- Filter state

### Integration Testing

Test FileManager with real services:
- File system operations
- UI rendering
- Input processing
- Error scenarios

### Mock Integration

Mock services for isolated testing:
```csharp
var mockActionService = new Mock<ActionService>();
var mockFileSystemService = new Mock<FileSystemService>();
```

## Thread Safety

### Single-Threaded UI

FileManager runs on the main thread:
- UI updates are not thread-safe
- Background operations use Progress&lt;T&gt; for thread-safe updates
- CancellationTokens coordinate async operations

### Background Operation Coordination

```csharp
Task.Run(async () => {
    // Background work
}).ContinueWith(t => {
    // UI updates on main thread
}, CancellationToken.None);
```

## Extension Points

### Custom Modes

New input modes can be added by:
1. Adding to InputMode enum
2. Implementing mode logic
3. Adding input handling
4. Updating UI rendering

### Custom Operations

New file operations can be integrated by:
1. Adding methods to FileManager
2. Coordinating with ActionService
3. Adding input bindings
4. Updating status messages

::: tip Architecture Insight
FileManager is intentionally monolithic to maintain tight control over application state and ensure consistent behavior. While this might seem contrary to separation of concerns, it provides the reliability needed for a file manager application.
:::

::: warning State Complexity
FileManager manages complex state interactions. When modifying the code, be careful to maintain state consistency, especially during mode transitions and async operations.
:::
