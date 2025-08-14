# Troubleshooting

Having trouble with Termix? This guide covers common issues, solutions, and diagnostic steps to get you back on track quickly.

## Installation Issues

### "Command not found" Error

**Problem**: Running `termix` gives "command not found" error.

**Solutions**:

1. **Verify installation**:
   ```bash
   dotnet tool list --global
   ```
   Look for "termix" in the output.

2. **Check PATH**:
   ```bash
   # Windows
   echo %PATH%
   
   # macOS/Linux  
   echo $PATH
   ```
   Should include `.dotnet/tools` directory.

3. **Add to PATH manually**:
   ```bash
   # Bash/Zsh
   export PATH="$PATH:$HOME/.dotnet/tools"
   
   # Fish
   set -gx PATH $PATH $HOME/.dotnet/tools
   ```

4. **Reinstall**:
   ```bash
   dotnet tool uninstall --global termix
   dotnet tool install --global termix
   ```

### .NET SDK Issues

**Problem**: ".NET SDK not found" or version errors.

**Solutions**:

1. **Install .NET 9**:
   - Download from [dotnet.microsoft.com](https://dotnet.microsoft.com/download/dotnet/9.0)

2. **Verify version**:
   ```bash
   dotnet --version
   ```
   Should show 9.0 or higher.

3. **Check multiple versions**:
   ```bash
   dotnet --list-sdks
   ```

### Permission Errors

**Problem**: Permission denied during installation.

**Solutions**:

1. **Windows (run as Administrator)**:
   ```cmd
   dotnet tool install --global termix
   ```

2. **macOS/Linux (avoid sudo)**:
   ```bash
   # Don't use sudo - it can cause issues
   dotnet tool install --global termix
   ```

3. **Fix permissions**:
   ```bash
   # macOS/Linux
   sudo chown -R $(whoami) ~/.dotnet/
   ```

## Runtime Issues

### UI Rendering Problems

**Problem**: Flickering, corrupted display, or strange characters.

**Solutions**:

1. **Use ASCII mode**:
   ```bash
   termix --no-icons
   ```

2. **Check terminal compatibility**:
   - **Windows**: Use Windows Terminal or PowerShell 7+
   - **macOS**: Use iTerm2 or Terminal.app
   - **Linux**: Use modern terminals (GNOME Terminal, Konsole, Alacritty)

3. **Verify Unicode support**:
   ```bash
   echo "📁 🔍 ⚡️"
   ```

4. **Update terminal**:
   - Ensure you're using the latest version of your terminal

### Icons Not Displaying

**Problem**: Seeing squares, question marks, or weird characters instead of icons.

**Solutions**:

1. **Install Nerd Fonts**:
   - Download from [nerdfonts.com](https://www.nerdfonts.com/)
   - Configure terminal to use the font

2. **Use ASCII fallback**:
   ```bash
   termix --no-icons
   ```

3. **Test font support**:
   ```bash
   echo ""  # Should show a folder icon
   ```

### Performance Issues

**Problem**: Termix is slow or unresponsive.

**Solutions**:

1. **Check directory size**:
   ```bash
   find . -maxdepth 1 -type d -exec du -sh {} \; | sort -hr
   ```

2. **Improve .gitignore**:
   ```gitignore
   node_modules/
   .git/
   bin/
   obj/
   build/
   dist/
   __pycache__/
   ```

3. **Navigate to subdirectories**:
   - Don't search from filesystem root
   - Use specific directories for better performance

4. **Clear search filters**:
   - Press `Esc` to clear active filters
   - Restart Termix if needed

## Search and Filter Issues

### Search Not Working

**Problem**: Search doesn't find files you know exist.

**Solutions**:

1. **Check .gitignore**:
   - Files might be ignored by git patterns
   - Use `git check-ignore filename` to verify

2. **Verify file location**:
   - Ensure files are in current directory tree
   - Navigate closer to target directory

3. **Case sensitivity**:
   - Search is case-insensitive by default
   - Try partial matches

4. **Clear cache**:
   - Start a new Termix session to clear search cache

### Slow Search Results

**Problem**: Search takes a long time to return results.

**Solutions**:

1. **Use specific terms**:
   - Search for `.js` instead of just `j`
   - Use file extensions for faster filtering

2. **Navigate first**:
   - Go to a subdirectory before searching
   - Avoid searching from large root directories

3. **Check ignore patterns**:
   - Ensure large directories are properly ignored

## File Operation Issues

### Permission Denied

**Problem**: Cannot create, rename, or delete files.

**Solutions**:

1. **Check file permissions**:
   ```bash
   ls -la filename
   ```

2. **Verify directory permissions**:
   ```bash
   ls -la .
   ```

3. **Close other applications**:
   - File might be open in editor or other app
   - Close applications using the file

4. **Check file system**:
   - Some network drives have restrictions
   - Try operations on local drives first

### Files Not Appearing

**Problem**: Created files don't show up in the listing.

**Solutions**:

1. **Refresh view**:
   - Navigate to parent and back
   - Or restart Termix

2. **Check file creation**:
   ```bash
   ls -la | grep filename
   ```

3. **Verify location**:
   - Ensure you're in the expected directory
   - Check the current path in header

## Navigation Issues

### Lost in Directory Tree

**Problem**: Don't know where you are or how to get back.

**Solutions**:

1. **Check current path**:
   - Look at the path in the header
   - Use `pwd` in a separate terminal

2. **Go to known location**:
   - Use `H` repeatedly to go up
   - Navigate to familiar directories

3. **Start fresh**:
   - Quit (`Q`) and restart Termix
   - Start from a known directory

### Can't Enter Directory

**Problem**: Directory appears but can't be entered.

**Solutions**:

1. **Check permissions**:
   ```bash
   ls -la directory-name
   ```

2. **Verify it's a directory**:
   - Might be a special file type
   - Look for directory indicator

3. **Try different approach**:
   - Use system file manager to check
   - Navigate via command line separately

## Error Messages

### Common Error Interpretations

| Error Message | Meaning | Solution |
|---------------|---------|----------|
| **Access denied** | No permission | Check file/folder permissions |
| **File not found** | Path doesn't exist | Verify path and spelling |
| **Directory not empty** | Can't delete non-empty folder | Use recursive delete or empty first |
| **Disk full** | No space left | Free up disk space |
| **Path too long** | Exceeds system limits | Use shorter paths |

### Reading Stack Traces

If Termix crashes with a stack trace:

1. **Look for the main error**: Usually at the top
2. **Note the operation**: What were you doing when it crashed?
3. **Check the file path**: Was it a specific file or directory?
4. **Report the issue**: Include the stack trace in a bug report

## Getting Help

### Diagnostic Information

When reporting issues, include:

```bash
# System information
dotnet --version
dotnet tool list --global | grep termix

# Terminal information  
echo $TERM
echo $LANG

# Test Unicode
echo "📁 🔍 ⚡️"
```

### Bug Reports

When filing a bug report:

1. **Describe the problem**: What happened vs. what you expected
2. **Steps to reproduce**: Exact sequence that causes the issue
3. **System information**: OS, terminal, .NET version
4. **Screenshots**: If UI-related issues
5. **Error messages**: Include full error text

### Performance Reports

For performance issues:

1. **Directory size**: How many files/folders?
2. **Search terms**: What were you searching for?
3. **Time taken**: How long did the operation take?
4. **System specs**: CPU, RAM, storage type

## Prevention Tips

### Avoid Common Issues

1. **Keep .NET updated**: Regular updates fix bugs and improve performance
2. **Use supported terminals**: Modern terminals work best
3. **Maintain .gitignore**: Keep it updated to ignore large directories
4. **Regular restarts**: Restart Termix occasionally for long sessions
5. **Monitor disk space**: Ensure adequate free space

### Best Practices

1. **Start small**: Test Termix on smaller directories first
2. **Learn incrementally**: Master basic features before advanced ones
3. **Use alternatives**: Keep backup navigation methods available
4. **Stay updated**: Check for Termix updates regularly

## Recovery Strategies

### When Termix Becomes Unresponsive

1. **Try to quit**: Press `Q`
2. **Force quit**: `Ctrl+C` in terminal
3. **Kill process**: Find and kill Termix process
4. **Restart terminal**: Close and reopen terminal

### When File Operations Fail

1. **Check operation status**: Look at status bar messages
2. **Verify results**: Use `ls` to check file system state
3. **Undo if possible**: Some operations can be manually reversed
4. **Use system tools**: Fall back to `mv`, `cp`, `rm` commands

## Still Having Issues?

If this guide doesn't solve your problem:

1. **Check GitHub Issues**: [github.com/amrohan/termix/issues](https://github.com/amrohan/termix/issues)
2. **Search existing issues**: Your problem might already be reported
3. **Create new issue**: Include all diagnostic information
4. **Ask for help**: Use GitHub Discussions for questions

::: tip Quick Reset
When in doubt, quit Termix (`Q`) and restart. Many transient issues are resolved by a fresh session.
:::

::: warning Data Safety
Termix includes confirmation prompts for destructive operations. If you accidentally delete something, check your system's trash/recycle bin - many deletions can be recovered.
:::
