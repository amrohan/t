# File Operations

Termix provides powerful file and directory operations with visual feedback and progress tracking. This guide covers all file management features, from basic operations to advanced techniques.

## Overview

All file operations in Termix are designed to be:

- **Safe**: Confirmation prompts for destructive operations
- **Informative**: Clear feedback and progress indicators
- **Efficient**: Optimized for both small files and large directories
- **Recoverable**: Clear error messages with suggested actions

## Creating Files and Directories

### Create New Items

Press `A` to create new files or directories:

1. Press `A` to enter creation mode
2. Type the desired name
3. Press `Enter` to create

### Creation Rules

| Input             | Result       | Description                      |
| ----------------- | ------------ | -------------------------------- |
| `filename.txt`    | File         | Creates a text file              |
| `script`          | `script.txt` | Auto-adds .txt extension         |
| `folder/`         | Directory    | Trailing slash creates directory |
| `path/to/file.md` | Nested file  | Creates directories as needed    |

### Smart Path Creation

Termix intelligently determines where to create new items:

- **Selected directory**: If a directory is selected, create inside it
- **Current directory**: Otherwise, create in the current directory
- **Nested paths**: Automatically creates parent directories when needed

### Examples

```bash
# Create a simple file
A → README.md → Enter

# Create a directory
A → src/ → Enter

# Create nested structure
A → docs/guide/setup.md → Enter
```

<VideoPlayer src="/videos/filecrud.mp4" />

## Renaming Files and Directories

### Rename Operation

Press `R` to rename the selected item:

1. Select the file or directory
2. Press `R` to enter rename mode
3. Edit the current name (pre-filled)
4. Press `Enter` to confirm or `Esc` to cancel

### Rename Features

- **Pre-filled name**: Current name is loaded for easy editing
- **Extension preservation**: Smart handling of file extensions
- **Validation**: Prevents invalid characters and duplicate names
- **Atomic operation**: Rename completes fully or not at all

### Rename Examples

```bash
# Rename a file
Select old-name.txt → R → new-name.txt → Enter

# Change file extension
Select script.txt → R → script.sh → Enter

# Rename directory
Select old-folder → R → new-folder → Enter
```

## Copy and Move Operations

Termix uses a clipboard-based system for copy and move operations:

### Copy Files (`C`)

1. Select the file or directory
2. Press `C` to copy to clipboard
3. Navigate to the destination
4. Press `P` to paste

### Move Files (`X`)

1. Select the file or directory
2. Press `X` to cut (move) to clipboard
3. Navigate to the destination
4. Press `P` to paste

### Clipboard Features

- **Visual feedback**: Status bar shows clipboard contents
- **Mode indicator**: Shows whether item is copied or cut
- **Smart pasting**: Handles naming conflicts automatically
- **Cross-drive support**: Efficiently handles moves across different drives

### Progress Tracking

For large files and directories, Termix shows detailed progress:

![Progress Image](/progress.png)

- **Progress bar**: Visual indication of completion
- **Current file**: Shows which file is being processed
- **Cancellation**: Press `Q` during operations to cancel

### Paste Behavior

When pasting, Termix handles various scenarios:

| Scenario                 | Behavior                                |
| ------------------------ | --------------------------------------- |
| **Same location (copy)** | Creates copy with different name        |
| **Same location (move)** | Shows "source and destination are same" |
| **Name conflict**        | Shows error, suggests resolution        |
| **Cross-drive move**     | Automatically converts to copy + delete |

## Delete Operations

### Delete Confirmation

Press `D` to delete the selected item:

1. Select the file or directory
2. Press `D` to start delete operation
3. Confirm with `y` or cancel with `n`

### Delete Features

- **Confirmation required**: All deletes require explicit confirmation
- **Recursive deletion**: Directories are deleted with all contents
- **Clear feedback**: Shows exactly what will be deleted
- **Safe operation**: Cannot accidentally delete without confirmation

### Delete Examples

```bash
# Delete a file
Select file.txt → D → y

# Delete a directory
Select folder → D → y  # Deletes folder and all contents

# Cancel deletion
Select important.txt → D → n  # Operation cancelled
```

## Advanced Operations

### Batch Operations

While Termix doesn't support multi-select, you can efficiently handle multiple files:

1. **Use search**: Filter to show only target files
2. **Operate sequentially**: Work through filtered results
3. **Use clipboard**: Copy/move files one by one to same destination

### Cross-Drive Operations

Termix intelligently handles operations across different drives:

- **Same drive moves**: Use native filesystem move (instant)
- **Cross-drive moves**: Copy then delete original (with progress)
- **Automatic detection**: Termix determines the optimal strategy

### Large Directory Handling

For directories with many files:

- **Size calculation**: Pre-calculates total operation size
- **Progress tracking**: Shows overall and current file progress
- **Interruption handling**: Safe cancellation at any point
- **Error recovery**: Continues operation despite individual file errors

## Error Handling

### Common Error Scenarios

| Error                 | Cause                                | Resolution                                         |
| --------------------- | ------------------------------------ | -------------------------------------------------- |
| **Permission denied** | Insufficient file system permissions | Check file/directory permissions                   |
| **File in use**       | Another process has file locked      | Close other applications using the file            |
| **Disk full**         | Not enough space for operation       | Free up disk space or choose different location    |
| **Invalid name**      | Name contains illegal characters     | Use valid filename characters                      |
| **Path too long**     | Exceeds filesystem limits            | Use shorter names or shallower directory structure |

### Error Recovery

When errors occur, Termix provides:

- **Clear error messages**: Explains what went wrong
- **Suggested actions**: Tells you how to fix the issue
- **Operation state**: Shows what completed successfully
- **Safe rollback**: No partial operations left in inconsistent state

## Status and Feedback

### Status Bar Information

The status bar provides real-time information:

- **Operation mode**: Shows current operation (copy, move, etc.)
- **Clipboard contents**: Displays copied/cut items
- **Progress information**: Shows completion status
- **Error messages**: Displays problems and solutions

### Visual Indicators

Termix uses various visual cues:

- **Highlighted selection**: Shows which item will be operated on
- **Clipboard indicator**: Shows items ready for pasting
- **Progress bars**: Visual progress for long operations
- **Color coding**: Green for success, red for errors, yellow for warnings

## Tips and Best Practices

### Efficient File Management

1. **Use the clipboard effectively**: Copy/cut multiple items to same destination
2. **Combine with search**: Filter first, then operate on results
3. **Preview before operations**: Check file contents before moving/deleting
4. **Watch for conflicts**: Pay attention to naming conflict warnings

### Safety Practices

1. **Double-check deletions**: Verify you're deleting the right item
2. **Test operations**: Try with less important files first
3. **Understand clipboard state**: Know whether items are copied or cut
4. **Monitor progress**: Don't interrupt operations unnecessarily

### Performance Tips

1. **Same-drive moves**: Prefer moves over copy+delete when possible
2. **Batch similar operations**: Group related operations together
3. **Clean clipboard**: Clear clipboard when done to free memory
4. **Avoid deep nesting**: Very deep directory structures can be slower

## Keyboard Shortcuts Reference

| Key   | Operation | Description                                 |
| ----- | --------- | ------------------------------------------- |
| `A`   | Add       | Create new file or directory                |
| `R`   | Rename    | Rename selected item                        |
| `C`   | Copy      | Copy to clipboard                           |
| `X`   | Move/Cut  | Cut to clipboard                            |
| `P`   | Paste     | Paste from clipboard                        |
| `D`   | Delete    | Delete with confirmation                    |
| `Esc` | Cancel    | Cancel current operation or clear clipboard |

## Next Steps

Now that you understand file operations:

- Learn about [Search & Filter](./search-filter.md) to find files efficiently
- Master [Keyboard Shortcuts](./keyboard-shortcuts.md) for faster operations
- Explore [Tips & Tricks](./tips-tricks.md) for advanced workflows

::: tip Power User Technique
Combine search with file operations: Use `S` to filter files, perform operations on the filtered set, then use `B` to return to search results and continue working!
:::
