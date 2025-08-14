# UI Layer

The UI Layer is responsible for all visual presentation in Termix. Built on top of Spectre.Console, it provides a rich terminal-based interface with double-buffered rendering, progress indicators, and responsive layout management.

## Architecture Overview

```
┌─────────────────────────────────────────────────────┐
│                    UI Layer                         │
├─────────────┬─────────────┬─────────────┬────────────┤
│FileManager  │DoubleBuffer │CustomProgress│InputHandler│
│Renderer     │Renderer     │Bar          │            │
└─────────────┴─────────────┴─────────────┴────────────┘
```

## FileManagerRenderer

The main UI component responsible for creating the visual layout of the file manager interface.

### Overview

```csharp
public class FileManagerRenderer
{
    private readonly IconProvider _iconProvider;

    public FileManagerRenderer(IconProvider iconProvider);
    public IRenderable GetLayout(string currentPath, List<FileSystemItem> items, 
        int selectedIndex, IRenderable preview, int viewOffset, IRenderable footer);
}
```

### Layout Structure

The FileManagerRenderer creates a three-section layout:

```
┌─────────────────────────────────────────────────────┐
│                  Header Panel                       │ ← Path display
├─────────────────┬───────────────────────────────────┤
│                 │                                   │
│   File List     │        Preview Pane               │ ← Main content
│   (Left Pane)   │        (Right Pane)               │
│                 │                                   │
├─────────────────┴───────────────────────────────────┤
│                  Footer Panel                       │ ← Status/shortcuts
└─────────────────────────────────────────────────────┘
```

### Header Panel

```csharp
private Panel CreatePathPanel(string currentPath)
{
    var pathText = new Text(currentPath)
    {
        Style = new Style(Color.Cyan1)
    };
    
    return new Panel(pathText)
    {
        Border = BoxBorder.Rounded,
        BorderStyle = new Style(Color.Blue),
        Padding = new Padding(1, 0)
    };
}
```

**Features:**
- **Current path display**: Shows full directory path
- **Styled borders**: Rounded border with blue color
- **Responsive**: Adjusts to terminal width
- **Path truncation**: Handles very long paths gracefully

### File List Panel (Left Pane)

```csharp
private Panel CreateFileListPanel(List<FileSystemItem> items, int selectedIndex, int viewOffset)
{
    var table = new Table()
        .Border(TableBorder.None)
        .HideHeaders()
        .AddColumn(new TableColumn("Icon").Width(3))
        .AddColumn(new TableColumn("Name").Width(25))
        .AddColumn(new TableColumn("Size").Width(10))
        .AddColumn(new TableColumn("Date").Width(12));

    var visibleItems = GetVisibleItems(items, viewOffset);
    
    for (int i = 0; i < visibleItems.Count; i++)
    {
        var item = visibleItems[i];
        var isSelected = (i + viewOffset) == selectedIndex;
        
        AddFileRow(table, item, isSelected);
    }
    
    return new Panel(table);
}
```

**Columns:**
1. **Icon**: File type icon (3 characters wide)
2. **Name**: File/directory name (25 characters, truncated if needed)
3. **Size**: File size in human-readable format (10 characters)
4. **Date**: Last modified date (12 characters)

**Selection Highlighting:**
- **Selected item**: Highlighted with different background color
- **Different styles**: Directories vs files have different styling
- **Parent directory**: Special styling for `..` entries

### Preview Panel (Right Pane)

```csharp
private Panel CreatePreviewPanel(IRenderable preview)
{
    return new Panel(preview)
    {
        Border = BoxBorder.Rounded,
        BorderStyle = new Style(Color.Grey),
        Header = new PanelHeader("Preview"),
        Padding = new Padding(1, 0)
    };
}
```

**Content Types:**
- **Text files**: Syntax-highlighted content
- **Images**: Metadata and basic information
- **Directories**: Directory information or empty
- **Binary files**: Hex preview or metadata
- **Error states**: Error messages when files can't be previewed

### Footer Panel

The footer adapts based on current application state:

```csharp
private IRenderable CreateFooterContent(/* state parameters */)
{
    return currentMode switch
    {
        InputMode.Normal => CreateNormalModeFooter(),
        InputMode.Filter => CreateSearchModeFooter(),
        InputMode.Add => CreateAddModeFooter(),
        InputMode.Rename => CreateRenameModeFooter(),
        _ => CreateDefaultFooter()
    };
}
```

**Footer Types:**
- **Normal mode**: Shows keyboard shortcuts
- **Search mode**: Shows search query and progress
- **Input modes**: Shows current input and instructions
- **Progress mode**: Shows operation progress
- **Error states**: Shows error messages

## DoubleBufferedRenderer

Provides flicker-free rendering for smooth terminal updates.

### Overview

```csharp
public class DoubleBufferedRenderer
{
    private string _lastRendered = "";

    public void Render(IRenderable content);
}
```

### Rendering Strategy

```csharp
public void Render(IRenderable content)
{
    // Convert renderable to ANSI string
    var currentContent = AnsiConsole.ToAnsi(content);
    
    // Only update if content has changed
    if (currentContent != _lastRendered)
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(content);
        _lastRendered = currentContent;
    }
}
```

**Benefits:**
- **Flicker elimination**: Only redraws when content changes
- **Performance**: Reduces unnecessary terminal operations
- **Smooth experience**: Maintains visual continuity during navigation
- **Efficient**: Minimal CPU usage for static displays

### Optimization Features

- **String comparison**: Fast detection of content changes
- **ANSI caching**: Avoids re-rendering identical content
- **Clear then write**: Ensures clean display on updates
- **Memory efficient**: Single string storage for last render

## CustomProgressBar

Provides visual feedback for long-running operations.

### Overview

```csharp
public class CustomProgressBar : IRenderable
{
    public double Value { get; set; }
    public int Width { get; set; } = 40;
    public string Style { get; set; } = "green";
    public string BackgroundStyle { get; set; } = "grey";
}
```

### Visual Design

```csharp
public Measurement Measure(RenderOptions options, int maxWidth)
{
    return new Measurement(Width, Width);
}

public IEnumerable<Segment> Render(RenderOptions options, int maxWidth)
{
    var filledWidth = (int)(Value / 100.0 * Width);
    var emptyWidth = Width - filledWidth;
    
    // Filled portion
    yield return new Segment(new string('█', filledWidth), new Style(foreground: Color.Parse(Style)));
    
    // Empty portion  
    yield return new Segment(new string('░', emptyWidth), new Style(foreground: Color.Parse(BackgroundStyle)));
}
```

**Features:**
- **Configurable width**: Adapts to available space
- **Color customization**: Different colors for fill and background
- **Unicode blocks**: Uses block characters for smooth appearance
- **Percentage-based**: 0-100% value range

### Usage Patterns

```csharp
// File operation progress
var progressBar = new CustomProgressBar 
{ 
    Value = completionPercentage, 
    Width = 30,
    Style = "green"
};

// Error state progress
var errorProgress = new CustomProgressBar
{
    Value = 0,
    Style = "red"
};
```

## Input Handling Integration

### InputHandler UI Coordination

```csharp
public class InputHandler
{
    public void ProcessKey(ConsoleKeyInfo keyInfo)
    {
        // Process input based on current UI mode
        switch (_fileManager.CurrentMode)
        {
            case InputMode.Normal:
                HandleNormalInput(keyInfo);
                break;
            case InputMode.Filter:
                HandleSearchInput(keyInfo);
                break;
            // ... other modes
        }
        
        // Trigger UI update if needed
        if (inputChangedState)
        {
            _fileManager.SetNeedsRedraw();
        }
    }
}
```

### Mode-Specific UI Behavior

Each input mode affects UI presentation:

```csharp
// Normal mode - full shortcuts displayed
"[cyan]↑↓[/] Move | [cyan]Enter[/] Open | [cyan]S[/] Search | [cyan]Q[/] Quit"

// Search mode - search-specific UI
"Search: [yellow]{query}[/][grey]█[/] | [cyan]Esc[/] Apply Filter"

// Add mode - creation UI  
"Create: [yellow]{filename}[/][grey]█[/] | [cyan]Enter[/] Confirm | [cyan]Esc[/] Cancel"
```

## Responsive Design

### Terminal Size Adaptation

```csharp
public IRenderable GetLayout(/* parameters */)
{
    var terminalWidth = Console.WindowWidth;
    var terminalHeight = Console.WindowHeight;
    
    // Adjust layout based on terminal size
    var fileListWidth = Math.Max(30, terminalWidth / 2 - 2);
    var previewWidth = terminalWidth - fileListWidth - 4; // Account for borders
    
    return CreateResponsiveLayout(fileListWidth, previewWidth);
}
```

**Responsive Features:**
- **Dynamic sizing**: Adapts to terminal dimensions
- **Minimum constraints**: Ensures usability at small sizes
- **Proportional scaling**: Maintains aspect ratios
- **Content wrapping**: Handles overflow gracefully

### Content Adaptation

- **Text truncation**: Long filenames abbreviated with `..`
- **Column sizing**: Table columns adjust to available space
- **Scroll indicators**: Shows when content extends beyond viewport
- **Overflow handling**: Graceful degradation when space is limited

## Color and Styling

### Color Scheme

```csharp
public static class Colors
{
    public static readonly Color Primary = Color.Cyan1;
    public static readonly Color Secondary = Color.Blue;
    public static readonly Color Success = Color.Green;
    public static readonly Color Warning = Color.Yellow;
    public static readonly Color Error = Color.Red;
    public static readonly Color Muted = Color.Grey;
}
```

### Style Consistency

- **Selection highlighting**: Consistent across all modes
- **Status colors**: Green for success, red for errors, yellow for warnings
- **Hierarchy**: Different intensities for different information levels
- **Accessibility**: High contrast for readability

### Theme Support

While not currently configurable, the UI is designed to adapt to:
- **Terminal color capabilities**: Degrades gracefully on limited terminals
- **Light/dark themes**: Colors chosen to work in both contexts
- **Monochrome support**: Functional with styling disabled

## Animation and Transitions

### Smooth Transitions

While terminals don't support true animations, Termix provides smooth experiences through:

- **Progressive loading**: Content appears as it's loaded
- **State transitions**: Smooth mode changes
- **Progress indication**: Visual feedback for operations
- **Cursor positioning**: Maintains context during navigation

### Performance Considerations

- **Minimal redraws**: Only updates changed content
- **Efficient rendering**: Uses Spectre.Console optimizations
- **Memory conscious**: Doesn't hold unnecessary render state
- **Battery friendly**: Reduces CPU usage on laptops

## Testing the UI Layer

### Visual Testing Approach

```csharp
[Test]
public void FileManagerRenderer_CreateLayout_ReturnsValidRenderable()
{
    // Arrange
    var renderer = new FileManagerRenderer(new IconProvider(false));
    var items = CreateTestItems();
    var preview = new Text("Test preview");
    var footer = new Text("Test footer");
    
    // Act
    var layout = renderer.GetLayout("/test/path", items, 0, preview, 0, footer);
    
    // Assert
    Assert.That(layout, Is.Not.Null);
    Assert.That(layout, Is.InstanceOf<IRenderable>());
}
```

### Integration Testing

```csharp
[Test]
public void DoubleBufferedRenderer_SameContent_DoesNotRedraw()
{
    // Arrange
    var renderer = new DoubleBufferedRenderer();
    var content = new Text("Test content");
    
    // Act - render twice
    renderer.Render(content);
    var consoleClearCountBefore = GetConsoleClearCount();
    renderer.Render(content); // Same content
    var consoleClearCountAfter = GetConsoleClearCount();
    
    // Assert - should not clear console again
    Assert.That(consoleClearCountAfter, Is.EqualTo(consoleClearCountBefore));
}
```

## Future UI Enhancements

### Planned Features

- **Customizable themes**: User-configurable color schemes
- **Layout options**: Different layout arrangements
- **Font size scaling**: Better support for different terminal sizes
- **Accessibility**: Screen reader support and high contrast modes

### Advanced UI Features

- **Split panes**: Multiple file views
- **Tabs**: Multiple directories open simultaneously  
- **Status dashboard**: More detailed system information
- **Custom widgets**: Extensible UI components

## Best Practices

### UI Development Guidelines

1. **Consistency**: Use established patterns and styles
2. **Responsiveness**: Test at different terminal sizes
3. **Performance**: Minimize unnecessary redraws
4. **Accessibility**: Ensure functionality without colors
5. **Error handling**: Graceful degradation for display issues

### Performance Tips

- **Batch updates**: Combine multiple UI changes
- **Lazy rendering**: Only render visible content
- **Cache frequently used renderables**: Avoid recreating static content
- **Profile rendering**: Use tools to identify performance bottlenecks

::: tip UI Design Philosophy
Termix UI follows terminal conventions while providing modern visual feedback. The goal is familiarity for terminal users with enhanced usability through visual cues.
:::

::: warning Terminal Compatibility
Always test UI changes across different terminal emulators and operating systems. What works in one terminal may not work in another.
:::
