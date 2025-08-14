# API Overview

This section provides technical documentation for Termix's architecture, components, and APIs. While Termix is primarily an end-user application, understanding its structure can help with troubleshooting, contributing, or building similar applications.

## Architecture Overview

Termix follows a layered architecture with clear separation of concerns:

```
┌─────────────────────────────────────┐
│              Program.cs             │  Entry Point
├─────────────────────────────────────┤
│            FileManager              │  Main Controller
├─────────────────────────────────────┤
│    Services Layer                   │  Business Logic
│  ┌─────────┬─────────┬─────────────┐│
│  │ Action  │FileSystem│ Preview    ││
│  │Service  │ Service  │ Service    ││
│  └─────────┴─────────┴─────────────┘│
├─────────────────────────────────────┤
│         UI Layer                    │  Presentation
│  ┌─────────────┬──────────────────┐ │
│  │  Renderers  │  Input Handlers  │ │
│  └─────────────┴──────────────────┘ │
└─────────────────────────────────────┘
```

## Core Components

### Program.cs
The application entry point that:
- Initializes the FileManager
- Handles command-line arguments
- Manages global exception handling
- Sets up the terminal environment

### FileManager
The central controller that coordinates all functionality:
- **State Management**: Tracks current directory, selection, and mode
- **Event Coordination**: Handles input and updates UI
- **Operation Orchestration**: Coordinates file operations with UI feedback

### Services Layer
Business logic components that handle specific domains:

- **ActionService**: File and directory operations
- **FileSystemService**: Directory reading and file access
- **FilePreviewService**: Content preview generation
- **IgnoreService**: Git ignore pattern handling
- **IconProvider**: File type icon mapping

### UI Layer
Presentation components for terminal interface:

- **FileManagerRenderer**: Main UI layout and rendering
- **InputHandler**: Keyboard input processing
- **CustomProgressBar**: Progress visualization
- **DoubleBufferedRenderer**: Flicker-free display

## Data Models

### FileSystemItem
Core model representing files and directories:

```csharp
public class FileSystemItem
{
    public string Path { get; }
    public string Name { get; }
    public bool IsDirectory { get; }
    public long Size { get; }
    public DateTime LastWriteTime { get; }
    public bool IsParentDirectory { get; }
}
```

### ClipboardItem
Represents copied/cut items:

```csharp
public class ClipboardItem
{
    public FileSystemItem Item { get; }
    public ClipboardMode Mode { get; } // Copy or Move
}
```

### ActionResponse
Standard response format for operations:

```csharp
public record ActionResponse(
    bool Success, 
    string Message, 
    object? Payload = null
);
```

## Key Design Patterns

### Command Pattern
User inputs are processed as commands through the InputHandler, which translates keystrokes into specific operations.

### State Machine
FileManager operates as a state machine with distinct modes:
- **Normal**: Default navigation and file operations
- **Search**: Active text input for filtering
- **FilteredNavigation**: Moving through search results
- **Add/Rename**: Text input for file operations
- **DeleteConfirm**: Confirmation for destructive operations

### Observer Pattern
The UI updates reactively based on state changes, with the DoubleBufferedRenderer ensuring smooth visual updates.

### Strategy Pattern
Different file operations (copy, move, delete) implement similar interfaces but with different strategies for handling edge cases.

## Threading Model

Termix uses a hybrid threading approach:

### Main Thread
- **UI Rendering**: All display updates
- **Input Processing**: Keyboard event handling
- **State Management**: Application state changes

### Background Threads
- **Recursive Search**: Deep directory scanning
- **File Operations**: Copy, move operations with progress
- **Preview Generation**: Content analysis for large files

### Synchronization
- **CancellationTokens**: For cooperative cancellation
- **Progress Reporting**: Thread-safe progress updates
- **State Coordination**: Minimal locking with atomic operations

## Error Handling Strategy

### Graceful Degradation
- **Permission Errors**: Clear messages, continued operation
- **I/O Failures**: Specific error reporting, retry options
- **Performance Issues**: Automatic fallback to simpler operations

### User Feedback
- **Status Messages**: Real-time operation feedback
- **Progress Indicators**: Visual progress for long operations
- **Error Recovery**: Suggested actions for common issues

### Logging
Termix uses structured logging for:
- **Performance Monitoring**: Operation timing
- **Error Tracking**: Exception details and context
- **User Actions**: High-level operation auditing

## Performance Characteristics

### Memory Usage
- **Directory Caching**: Limited-size LRU cache
- **Search Results**: Managed result set sizes
- **Preview Content**: Lazy loading with size limits

### I/O Operations
- **Batch Operations**: Grouped file system calls
- **Async I/O**: Non-blocking file operations
- **Streaming**: Large file handling without loading entire content

### UI Responsiveness
- **Double Buffering**: Prevents screen flicker
- **Debounced Updates**: Reduces excessive redraws
- **Priority Queuing**: Important updates get priority

## Extension Points

While Termix doesn't currently support plugins, the architecture provides clear extension points:

### Custom File Handlers
New file types can be supported by extending:
- **IconProvider**: Add new file type icons
- **FilePreviewService**: Add new preview generators
- **FileSystemService**: Add new file system backends

### UI Customization
Visual aspects can be modified through:
- **FileManagerRenderer**: Layout and styling changes
- **Color Schemes**: Terminal color adaptation
- **Keybinding**: Input handler modifications

### New Operations
File operations can be extended by:
- **ActionService**: New file manipulation methods
- **InputHandler**: New keyboard shortcuts
- **FileManager modes**: New interaction modes

## API Stability

### Public Interfaces
Currently, Termix doesn't expose public APIs as it's designed as an end-user application. However, key interfaces are stable for internal use:

- **Service Interfaces**: Well-defined contracts
- **Data Models**: Stable structure and behavior
- **Configuration**: Command-line options

### Internal APIs
Internal APIs may change between versions:
- **UI Components**: Subject to refactoring
- **State Management**: May evolve with new features
- **Performance Optimizations**: Implementation details

## Technology Dependencies

### Core Framework
- **.NET 9**: Latest C# features and performance improvements
- **Spectre.Console**: Rich terminal UI framework

### Third-Party Libraries
- **DotNet.Glob**: Pattern matching for ignore rules
- **SixLabors.ImageSharp**: Image processing and preview

### Platform Integration
- **File System APIs**: Native OS file operations
- **Terminal Capabilities**: ANSI escape sequences and Unicode
- **Process Management**: External application launching

## Next Steps

Explore specific components:
- [ActionService](./action-service.md) - File operation APIs
- [FileSystemService](./filesystem-service.md) - File system abstraction
- [File Manager](./file-manager.md) - Core application logic
- [Core Components](./core-components.md) - Detailed architecture
- [Services](./services.md) - Business logic layer
- [UI Layer](./ui-layer.md) - Presentation layer

::: tip Contributing
Understanding this architecture is the first step to contributing to Termix. Each component has clear responsibilities and well-defined interfaces.
:::

::: info Documentation Note
This API documentation reflects the current internal structure of Termix v1.5.0. Future versions may evolve the architecture while maintaining backward compatibility for user workflows.
:::
