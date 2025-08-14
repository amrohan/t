# Quick Start

Get up and running with Termix in minutes! This guide covers the essentials to help you become productive with Termix right away.

## First Launch

After [installing Termix](./installation.md), launch it from your terminal:

```bash
termix
```

You'll see Termix's two-pane interface:
- **Left pane**: File and directory listing
- **Right pane**: File preview (shows content of selected file)
- **Bottom**: Status bar with keyboard shortcuts and current mode

## Basic Navigation

### Moving Around

| Key | Action |
|-----|--------|
| `↑` / `↓` | Move selection up/down |
| `J` / `K` | Vim-style movement (up/down) |
| `Home` / `End` | Jump to first/last item |

### Opening Files and Directories

| Key | Action |
|-----|--------|
| `Enter` or `L` | Open selected file or enter directory |
| `Backspace` or `H` | Go to parent directory |

Try navigating through your file system using these keys. Notice how the right pane updates to show a preview of the selected file!

## Essential File Operations

### Creating Files and Folders

Press `A` to create a new file or folder:

1. Press `A` 
2. Type the name (add `/` at the end for folders)
3. Press `Enter`

Examples:
- `README.md` - Creates a file
- `new-folder/` - Creates a directory
- `script` - Creates `script.txt` (auto-adds .txt extension)

### Renaming

1. Select the file/folder you want to rename
2. Press `R`
3. Edit the name
4. Press `Enter`

### Copying and Moving

**Copy a file:**
1. Select the file
2. Press `C` (copy to clipboard)
3. Navigate to destination
4. Press `P` (paste)

**Move a file:**
1. Select the file  
2. Press `X` (cut to clipboard)
3. Navigate to destination
4. Press `P` (paste)

### Deleting

1. Select the file/folder
2. Press `D`
3. Confirm with `y` or cancel with `n`

## Search and Filter

One of Termix's most powerful features is real-time search:

1. Press `S` to enter search mode
2. Start typing to filter files instantly
3. Press `Esc` to apply the filter and navigate results
4. Press `Esc` again to clear the filter

The search is **recursive**, meaning it searches through all subdirectories automatically!

### Search Tips

- Search is **case-insensitive**
- Matches partial file names
- Works across your entire directory tree
- Results update in real-time as you type

## Preview Pane

The right pane shows previews of your files:

- **Text files**: Content with syntax highlighting
- **Images**: Terminal-friendly image preview
- **Directories**: Shows as directory (no preview)

### Scrolling Previews

For large files, you can scroll the preview:

| Key | Action |
|-----|--------|
| `Alt + ↑/↓` | Scroll vertically |
| `Alt + ←/→` | Scroll horizontally |

## Working with the Clipboard

Termix has a smart clipboard system:

1. **Copy** (`C`) or **Cut** (`X`) a file
2. The bottom status bar shows what's in your clipboard
3. Navigate anywhere and **Paste** (`P`)
4. Clear clipboard anytime with `Esc`

The clipboard remembers whether you copied or cut, so pasting will either copy or move the file accordingly.

## Quick Reference

Here are the essential shortcuts you'll use daily:

### Navigation
- `↑↓` or `JK` - Move selection
- `Enter` or `L` - Open/Enter
- `Backspace` or `H` - Go up
- `Q` - Quit

### File Operations  
- `A` - Add (create file/folder)
- `R` - Rename
- `D` - Delete
- `C` - Copy
- `X` - Move/Cut
- `P` - Paste

### Search
- `S` - Start search
- `Esc` - Apply filter / Clear filter
- `B` - Return to search results (when navigating from filtered results)

## Common Workflows

### Organizing Files

1. **Create a new project folder:**
   - Press `A`, type `my-project/`, press `Enter`
   
2. **Move files into it:**
   - Select file, press `X` (cut)
   - Enter the folder, press `P` (paste)

### Finding Files

1. **Search for a specific file:**
   - Press `S`, type part of the filename
   - Press `Esc` to navigate results
   - Use arrow keys to select, `Enter` to open

2. **Return to search results:**
   - After opening a file from search results, press `B` to go back

### Code Exploration  

1. **Navigate to a code project:**
   - Use normal navigation to find your project
   
2. **Search for specific files:**
   - Press `S`, type `.cs` to find C# files
   - Or type `controller` to find controller files
   
3. **Preview files:**
   - Select files to see syntax-highlighted previews
   - Use `Alt + ↑/↓` to scroll long files

## Tips for Success

::: tip Smart Ignoring
Termix automatically respects `.gitignore` files and ignores common build directories like `node_modules`, `bin`, and `obj`. This keeps your search results clean and relevant.
:::

::: tip Vim Users
If you're comfortable with Vim, you'll feel right at home with `J`/`K` for movement and `H`/`L` for navigation.
:::

::: tip Large Directories
Termix handles large directories efficiently. The recursive search works even with thousands of files, updating results in real-time.
:::

## Next Steps

Now that you know the basics:

- Learn more about [Navigation](./navigation.md) techniques
- Master [File Operations](./file-operations.md) 
- Explore [Search & Filter](./search-filter.md) in depth
- Memorize [Keyboard Shortcuts](./keyboard-shortcuts.md)

Ready to become a Termix power user? Let's dive deeper into each feature!
