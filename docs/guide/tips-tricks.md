# Tips & Tricks

Become a Termix power user with these tips and tricks. This guide covers advanced workflows, efficiency hacks, and hidden features to help you get the most out of Termix.

## Advanced Workflows

### Search → Operate → Return Cycle
This is the ultimate power user workflow:

1. **Search (`S`)**: Find a set of files (e.g., all `.tsx` components)
2. **Navigate**: Move through the filtered results
3. **Operate**: Perform an action (e.g., copy a file with `C`)
4. **Navigate Away**: Go to a different directory to paste
5. **Return (`B`)**: Press `B` to instantly return to your search results

This cycle is incredibly efficient for refactoring, organizing files, or working with distributed code.

### Quick Project Scaffolding

Create a new project structure in seconds:

```bash
# Create project root
A → my-new-project/ → Enter

# Navigate into it
Enter

# Create source and test directories
A → src/ → Enter
A → tests/ → Enter

# Create initial files
A → src/index.js → Enter 
A → README.md → Enter
```

### Efficient Refactoring

When renaming a component and its related files:

1. **Search (`S`)**: Find all files related to the component (e.g., `OldComponent`)
2. **Rename (`R`)**: Rename each file (`NewComponent.tsx`, `NewComponent.test.tsx`)
3. **Open**: Use `Enter` to open files in your default editor for content changes
4. **Return (`B`)**: Use `B` to come back to your search results after editing

## Efficiency Hacks

### Chaining Commands
Combine Termix commands for faster operations:

- **Create and Enter**: `A → new-folder/ → Enter → Enter`
- **Jump and Open**: `End → Enter` (open last file)
- **Vim-style Dive**: `K K L L` (down, down, enter, enter)

### Text Input Tricks

When creating files with `A`:

- **Auto-extension**: Type `file` to get `file.txt`
- **Directory creation**: End with `/` to create a directory
- **Nested paths**: `path/to/file` creates directories automatically

### Clipboard Management

- **Quick Move**: `X → navigate → P`
- **Quick Copy**: `C → navigate → P`
- **Clear Clipboard**: `Esc` when in normal mode with an active clipboard

## Hidden Features

### Parent Directory Preview

Select the `..` entry to see information about the parent directory, including:
- Full path
- Last modified time
- Total size (if available)

### Cross-Drive Move Magic

When you move a file across different drives (e.g., from `/` to a mounted drive), Termix automatically performs a copy-then-delete operation, complete with progress tracking.

### Smart File Naming

Termix handles long filenames gracefully by truncating them with `..` in the middle, preserving the file extension.

## Customization and Integration

### Shell Aliases for Speed
Create shell aliases for lightning-fast access:

```bash
# zsh/bash in ~/.zshrc or ~/.bashrc
alias t='termix'
alias td='cd ~/Downloads && termix'

# Fish in ~/.config/fish/config.fish
alias t termix
alias td 'cd ~/Downloads; and termix'
```

### Integrate with Other Tools

Termix opens files in your system's default application. You can leverage this for powerful integrations:

- **Image Editing**: Select an image and press `Enter` to open in your default image editor
- **Code Editing**: Select a code file to open in your favorite IDE/editor
- **Document Viewing**: Open PDFs, documents, and more

### Scripting with Termix
While Termix is interactive, you can use it in scripts:

```bash
# Open Termix in a specific directory
(cd /path/to/project && termix)

# Use Termix as part of a larger script
echo "Organizing project files..."
termix
echo "File organization complete."
```

## Performance Tips

### Optimize for Large Repositories

If Termix feels slow in a very large repository:

1. **Check your `.gitignore`**: Ensure `node_modules`, `build`, and other large directories are ignored.
2. **Search specifically**: Broad searches in large repos can be slow. Use specific queries.
3. **Navigate first**: Navigate to a subdirectory before searching to limit the scope.

### Memory Usage

Termix is memory-efficient, but you can further optimize:

- **Clear filters**: Press `Esc` to clear search filters when done
- **Clear clipboard**: `Esc` also clears the clipboard
- **Restart session**: For very long sessions, a quick restart can clear all caches

## User Interface Tricks

### Reading the Status Bar
The status bar is your guide to what's happening:

- **Normal Mode**: Shows essential shortcuts
- **Search Mode**: Shows your current query
- **Clipboard Active**: Shows what you've copied/cut
- **Progress Indicator**: Shows progress for long operations

### Interpreting Icons
- `📁`: Directory
- `📄`: Text file
- `💻`: Code file
- `🖼️`: Image file
- `⚙️`: Config file
- `..`: Parent directory

(Icons may vary based on Nerd Font support)

### Understanding Colors
- **Yellow**: Input prompts, warnings
- **Green**: Success messages
- **Red**: Error messages
- **Cyan**: Keyboard shortcuts
- **Fuchsia**: Status messages

## Advanced Scenarios

### Working with Symlinks
- **Navigation**: Termix follows symlinks to directories
- **Operations**: File operations act on the symlink itself, not the target
- **Preview**: Preview pane shows information about the symlink, not the target file

### Handling Special Permissions
- **Permission errors**: Termix displays a clear error message in the status bar
- **Read-only directories**: Navigation is allowed, but operations will fail gracefully
- **Sudo not required**: Termix operates with user-level permissions

### Cross-Platform Workflows
Because Termix is cross-platform, you can use the same workflows on Windows, macOS, and Linux, making it great for teams with diverse development environments.

## Troubleshooting Tips

### UI Glitches or Flickering
- **Cause**: Often due to terminal emulator limitations.
- **Solution**: 
  - Use a modern terminal like Windows Terminal, iTerm2, or Alacritty.
  - Ensure your terminal has good Unicode and color support.
  - Try launching with `termix --no-icons`.

### Slow Search Performance
- **Cause**: Large, un-ignored directories.
- **Solution**: 
  - Add directories like `node_modules` to your `.gitignore`.
  - Navigate to subdirectories before searching.

## Final Power User Advice

> The most effective way to use Termix is to let it handle file *navigation* and *organization*, while letting your other tools (IDE, editor, etc.) handle file *content*.

Use Termix to find, move, and organize. Use your editor to code. This combination creates a seamless and powerful development experience.

## Next Steps
- Review [Configuration](./configuration.md) for customization options
- Check the [API Reference](/api/overview.md) for technical details
- Visit [Troubleshooting](./troubleshooting.md) for help with issues

::: tip The Ultimate Combo
Combine `S` (Search) with `B` (Back to results). It's the most powerful navigation pattern in Termix. Master it, and you'll fly through your filesystem.
:::
