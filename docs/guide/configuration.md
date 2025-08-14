# Configuration

Termix is designed to work perfectly out of the box with sensible defaults. However, there are several ways to customize your experience and optimize Termix for your specific needs.

## Command Line Options

### Icon Control

Control whether Termix displays file type icons:

```bash
# Launch with icons (default)
termix

# Launch without icons (ASCII mode)
termix --no-icons
```

The `--no-icons` flag is useful when:
- Your terminal doesn't support Nerd Fonts
- You prefer a minimal ASCII-only interface
- Working over slow network connections
- Using terminals with poor Unicode support

## Environment Variables

### PATH Configuration

Ensure Termix is accessible from anywhere by verifying your PATH includes the .NET tools directory:

**Windows:**
```cmd
echo %PATH%
# Should include: %USERPROFILE%\.dotnet\tools
```

**macOS/Linux:**
```bash
echo $PATH
# Should include: ~/.dotnet/tools
```

If the path is missing, add it to your shell profile:

**Bash/Zsh:**
```bash
export PATH="$PATH:$HOME/.dotnet/tools"
```

**Fish:**
```fish
set -gx PATH $PATH $HOME/.dotnet/tools
```

## Terminal Configuration

### Font Setup

For the best experience, configure your terminal to use a Nerd Font:

#### Recommended Fonts
- **FiraCode Nerd Font**: Excellent for coding with ligatures
- **JetBrains Mono Nerd Font**: Clean, readable design
- **Cascadia Code Nerd Font**: Microsoft's modern programming font
- **Hack Nerd Font**: Optimized for source code

#### Terminal-Specific Setup

**Windows Terminal:**
```json
{
  "profiles": {
    "defaults": {
      "fontFace": "FiraCode Nerd Font",
      "fontSize": 12
    }
  }
}
```

**iTerm2 (macOS):**
1. Preferences → Profiles → Text
2. Change font to a Nerd Font variant
3. Ensure "Use ligatures" is checked for coding fonts

**VS Code Integrated Terminal:**
```json
{
  "terminal.integrated.fontFamily": "FiraCode Nerd Font"
}
```

### Terminal Behavior

#### Unicode Support
Ensure your terminal supports Unicode properly:

**Windows:**
- Use Windows Terminal or PowerShell 7+
- Enable UTF-8 support in terminal settings

**macOS:**
- Modern terminals support Unicode by default
- Verify locale is set to UTF-8: `locale | grep UTF-8`

**Linux:**
- Most modern terminals support Unicode
- Verify with: `echo $LANG` (should include UTF-8)

## Git Integration

### .gitignore Configuration

Termix automatically respects `.gitignore` files. To optimize the experience:

#### Project-Level .gitignore
Add common patterns to your project's `.gitignore`:

```gitignore
# Build outputs
bin/
obj/
dist/
build/

# Dependencies  
node_modules/
packages/

# IDE files
.vs/
.vscode/settings.json
*.swp
*.swo

# OS files
.DS_Store
Thumbs.db
```

#### Global .gitignore
Set up a global gitignore for system-wide patterns:

```bash
# Create global gitignore
touch ~/.gitignore_global

# Configure git to use it
git config --global core.excludesfile ~/.gitignore_global
```

Add system-wide patterns:
```gitignore
# OS generated files
.DS_Store
.DS_Store?
._*
.Spotlight-V100
.Trashes
ehthumbs.db
Thumbs.db

# Editor files
*~
*.swp
*.swo
.vscode/
```

## Performance Optimization

### Large Directory Performance

For projects with very large directory structures:

#### Ignore Patterns
Add performance-critical ignore patterns to `.gitignore`:

```gitignore
# Large dependency directories
node_modules/
.git/
__pycache__/
.pytest_cache/

# Build and cache directories
target/
.gradle/
.maven/
```

#### Search Optimization
- Use specific search terms rather than broad queries
- Clear search filters when done to improve navigation
- Avoid searching from filesystem root directories

### Memory Management

Termix automatically manages memory efficiently:
- Directory contents are cached temporarily
- Search results are managed to prevent memory leaks
- Preview generation is optimized for common file types

## Workflow Customization

### Shell Aliases

Create aliases for common Termix usage patterns:

**Bash/Zsh:**
```bash
# Quick launch aliases
alias t='termix'
alias ta='termix --no-icons'  # ASCII mode

# Project-specific launches  
alias twork='cd ~/work && termix'
alias tcode='cd ~/code && termix'
```

**PowerShell:**
```powershell
# PowerShell profile functions
function t { termix }
function ta { termix --no-icons }
function twork { Set-Location ~/work; termix }
```

**Fish:**
```fish
# Fish shell aliases
alias t 'termix'
alias ta 'termix --no-icons'
alias twork 'cd ~/work; and termix'
```

### Project Integration

#### NPM Scripts
Add Termix to your project's package.json:

```json
{
  "scripts": {
    "browse": "termix",
    "files": "termix --no-icons"
  }
}
```

#### Makefile Integration
```makefile
.PHONY: browse
browse:
	@termix

.PHONY: files  
files:
	@termix --no-icons
```

## Color and Theme Configuration

### Terminal Color Schemes

Termix adapts to your terminal's color scheme. For optimal appearance:

#### Dark Themes
- **Dracula**: Excellent contrast with Termix's color choices
- **Nord**: Clean, modern appearance
- **One Dark**: Popular dark theme with good readability

#### Light Themes  
- **Solarized Light**: Well-balanced light theme
- **GitHub Light**: Clean, familiar appearance
- **Atom One Light**: Bright, clear display

### Syntax Highlighting

Termix's preview pane uses built-in syntax highlighting that adapts to:
- Your terminal's color capabilities
- Light vs dark theme detection
- Available color palette

## Advanced Configuration

### Development Environment Integration

#### VS Code Integration
Create a VS Code task to launch Termix:

```json
{
  "version": "2.0.0",
  "tasks": [
    {
      "label": "Open Termix",
      "type": "shell",
      "command": "termix",
      "group": "build",
      "presentation": {
        "echo": true,
        "reveal": "always",
        "focus": false,
        "panel": "new"
      }
    }
  ]
}
```

#### JetBrains IDE Integration
Add Termix as an external tool:

1. File → Settings → Tools → External Tools
2. Add new tool:
   - **Name**: Termix
   - **Program**: termix
   - **Working directory**: $ProjectFileDir$

## Troubleshooting Configuration

### Common Issues

**Icons not displaying:**
- Install a Nerd Font in your terminal
- Use `--no-icons` flag as fallback
- Verify Unicode support in terminal

**Slow performance:**
- Check .gitignore patterns
- Avoid searching from filesystem root
- Close other resource-intensive applications

**PATH issues:**
- Verify .NET tools directory is in PATH
- Restart terminal after PATH changes
- Use full path as temporary workaround: `~/.dotnet/tools/termix`

### Diagnostic Commands

Check your configuration:

```bash
# Verify Termix installation
dotnet tool list --global | grep termix

# Check .NET version
dotnet --version

# Verify PATH includes .NET tools
echo $PATH | grep -o '[^:]*dotnet[^:]*'

# Test Unicode support
echo "📁 🔍 ⚡️"
```

## Default Behavior

### File Type Recognition

Termix automatically recognizes these file types for preview and icons:

**Code Files:**
- `.cs`, `.fs`, `.vb` (C#, F#, VB.NET)
- `.js`, `.ts`, `.jsx`, `.tsx` (JavaScript/TypeScript)
- `.py`, `.pyx` (Python)
- `.java`, `.kt` (Java, Kotlin)
- `.go`, `.rs` (Go, Rust)
- `.c`, `.cpp`, `.h`, `.hpp` (C/C++)

**Configuration Files:**
- `.json`, `.yaml`, `.yml`
- `.toml`, `.ini`, `.conf`
- `.xml`, `.config`

**Documentation:**
- `.md`, `.txt`, `.rst`
- `.pdf` (basic info display)

### Ignore Patterns

Built-in ignore patterns (always active):
```
.git/
node_modules/
bin/
obj/
.DS_Store
Thumbs.db
__pycache__/
.pytest_cache/
target/
.gradle/
```

## Next Steps

With your configuration optimized:

- Explore [Tips & Tricks](./tips-tricks.md) for advanced workflows
- Check [Troubleshooting](./troubleshooting.md) if issues persist
- Review the [API Reference](/api/overview.md) for technical details

::: tip Configuration Philosophy
Termix follows the principle of "sensible defaults with easy customization." Most users never need to configure anything, but power users have options when needed.
:::

::: warning Configuration Persistence
Currently, Termix doesn't support persistent configuration files. All customization is done through command-line options, environment variables, and external tool integration. A configuration file system is planned for future releases.
:::
