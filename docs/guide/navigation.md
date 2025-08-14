# Navigation

Master Termix's navigation system to move through your file system quickly and efficiently. This guide covers all navigation features, from basic movement to advanced techniques.

## Interface Overview

Termix uses a clean two-pane layout:

![navigation icons](/navigation.png)

- **Left pane**: File and directory listing with icons
- **Right pane**: Preview of the currently selected item
- **Header**: Current directory path
- **Footer**: Status information and keyboard shortcuts

## Basic Movement

### Vertical Navigation

| Key    | Action          | Description                            |
| ------ | --------------- | -------------------------------------- |
| `↑`    | Move up         | Select previous item in list           |
| `↓`    | Move down       | Select next item in list               |
| `J`    | Move up (Vim)   | Vim-style upward movement              |
| `K`    | Move down (Vim) | Vim-style downward movement            |
| `Home` | Jump to top     | Select first item in current directory |
| `End`  | Jump to bottom  | Select last item in current directory  |

### Directory Traversal

| Key         | Action     | Description                           |
| ----------- | ---------- | ------------------------------------- |
| `Enter`     | Open/Enter | Open file or enter selected directory |
| `L`         | Open/Enter | Alternative key for opening           |
| `Backspace` | Go up      | Move to parent directory              |
| `H`         | Go up      | Vim-style parent directory navigation |

## Advanced Navigation Features

### Navigation Stack

Termix maintains a navigation history as you move through directories. This allows for intelligent "breadcrumb" navigation:

- When you enter a directory, Termix remembers which item was selected
- Returning to the parent directory automatically selects the folder you came from
- This creates a natural flow for deep directory exploration

### Viewport Management

Termix automatically manages the viewport to keep the selected item visible:

- **Auto-scrolling**: The view automatically scrolls to keep selection in sight
- **Page-based movement**: Large directories are paginated for performance
- **Smart positioning**: New selections are positioned optimally on screen

### Parent Directory Navigation

The `..` entry at the top of each directory listing provides quick access to the parent directory:

- Always available (except at filesystem root)
- Maintains context of where you came from
- Shows parent directory information in preview pane

## Preview Pane Navigation

### File Content Scrolling

When previewing large files, you can scroll through the content:

| Key       | Action       | Description                                  |
| --------- | ------------ | -------------------------------------------- |
| `Alt + ↑` | Scroll up    | Move preview content up                      |
| `Alt + ↓` | Scroll down  | Move preview content down                    |
| `Alt + ←` | Scroll left  | Move preview content left (for wide content) |
| `Alt + →` | Scroll right | Move preview content right                   |

### Preview Reset

- Preview scrolling resets automatically when you select a new file
- Horizontal and vertical offsets are cleared for each new selection

## Navigation Modes

Termix operates in different navigation modes depending on your current activity:

### Normal Mode

- Default navigation mode
- All standard movement keys available
- Preview pane updates with selection changes
- File operations accessible

### Filtered Navigation Mode

- Active when navigating search results
- Special handling for returning to search context
- Use `B` to return to original search results
- Different status bar indicators

### Search Mode

- Navigation limited while typing search query
- Real-time filtering affects available items
- `Esc` transitions to filtered navigation

## Directory Handling

### Large Directories

Termix efficiently handles directories with many files:

- **Lazy loading**: Only visible items are fully processed
- **Pagination**: Large lists are broken into manageable pages
- **Performance**: Maintains responsiveness even with thousands of files

### Empty Directories

- Empty directories show appropriate messaging
- Navigation still works (can go up to parent)
- Preview pane indicates directory status

### Inaccessible Directories

When encountering permission issues:

- Clear error messages in status bar
- Graceful fallback to accessible parent directory
- No crashes or undefined behavior

## Navigation Tips and Tricks

### Efficient Directory Traversal

1. **Use Vim-style keys**: `J`/`K` for movement, `H`/`L` for traversal
2. **Jump to extremes**: `Home`/`End` for quick positioning
3. **Combine with search**: Use search to find, then navigate normally

### Preview-First Exploration

1. **Preview before opening**: Check file contents before committing to open
2. **Scroll through large files**: Use `Alt + ↑/↓` to scan content
3. **Identify file types**: Use previews to understand file structure

### Context Awareness

1. **Watch the path**: Header shows your current location
2. **Use the stack**: Termix remembers where you came from
3. **Status feedback**: Footer provides navigation context

## Keyboard Navigation Patterns

### The "Vim Way"

If you're familiar with Vim, these patterns will feel natural:

```
h, j, k, l    # Left, down, up, right
H, L          # High (parent), Low (enter)
gg, G         # Go to top, Go to bottom (Home/End)
```

### The "Arrow Way"

For those who prefer arrow keys:

```
↑↓←→          # Standard directional movement
Backspace     # Go back/up
Enter         # Go forward/enter
```

## Navigation State

Termix tracks several aspects of your navigation state:

- **Current directory**: Where you are now
- **Selected item**: Which file/folder is highlighted
- **View offset**: What portion of the list is visible
- **Navigation history**: Where you came from
- **Search context**: Active filters and search state

This state information ensures a consistent and predictable navigation experience.

## Performance Considerations

### Efficient Navigation

- Directory contents are cached for quick re-entry
- Preview generation is optimized for common file types
- Navigation operations are prioritized for responsiveness

### Memory Usage

- Large directories don't consume excessive memory
- Preview content is managed efficiently
- Caching strategies balance speed and resource usage

## Next Steps

Now that you understand navigation:

- Learn about [File Operations](./file-operations.md) to manipulate files efficiently
- Explore [Search & Filter](./search-filter.md) for advanced file finding
- Check out [Keyboard Shortcuts](./keyboard-shortcuts.md) for a complete reference

::: tip Pro Navigation
The most efficient Termix users combine navigation with search. Use `S` to narrow down to relevant files, then navigate normally through the results!
:::
