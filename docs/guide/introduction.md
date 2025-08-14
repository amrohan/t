# Introduction

Welcome to Termix, a modern, high-performance file navigator designed to make command-line file management fast, intuitive, and visually enjoyable.

<VideoPlayer src="/videos/termix.mp4" />

## What is Termix?

Termix is a terminal-based file manager built with .NET 9 and [Spectre.Console](https://spectreconsole.net/). It provides a rich, interactive interface for navigating, searching, and managing files directly from your terminal.

## Why Choose Termix?

Traditional command-line file operations can be cumbersome and error-prone. Termix bridges the gap between the power of the command line and the convenience of modern file managers by offering:

### ⚡️ **Performance First**

- **Instant Search**: Real-time recursive filtering across entire directory trees
- **Memory Efficient**: Handles large directories without performance degradation
- **Responsive UI**: Double-buffered rendering ensures smooth, flicker-free updates

### 🎨 **Beautiful Interface**

- **Two-Pane Layout**: Clear separation between file list and preview
- **Syntax Highlighting**: Live preview of code files with proper syntax coloring
- **Icon Support**: Beautiful file type icons with Nerd Fonts integration
- **Progress Tracking**: Visual indicators for long-running operations

### 🔧 **Powerful Operations**

- **File Management**: Create, rename, move, copy, and delete files and directories
- **Smart Clipboard**: Copy/cut files and paste them anywhere in the file system
- **Bulk Operations**: Handle multiple files efficiently with progress tracking

### ⌨️ **Keyboard-Driven**

- **Vim-Style Navigation**: Use `J`/`K` alongside arrow keys
- **Intuitive Shortcuts**: Memorable keyboard shortcuts for all operations
- **Modal Interface**: Different modes for navigation, search, and file operations

## Key Features

- **Instant Recursive Filtering**: Start typing to filter files and directories in real-time
- **Intuitive File Operations**: Create, rename, move, copy, and delete with visual feedback
- **Smart Ignoring**: Respects `.gitignore` and filters out build directories automatically
- **Live Syntax Highlighting**: Preview various file types (`.cs`, `.js`, `.ts`, `.py`, and more)
- **Cross-Platform**: Works seamlessly on Windows, macOS, and Linux
- **Nerd Font Support**: Enhanced experience with icons, ASCII fallback for compatibility

## Built With Modern Technology

Termix leverages cutting-edge .NET technologies:

- **.NET 9**: Latest performance improvements and language features
- **Spectre.Console**: Rich terminal UI framework
- **DotNet.Glob**: Advanced pattern matching for ignore rules
- **SixLabors.ImageSharp**: Image handling and preview capabilities

## Getting Started

Ready to try Termix? Head over to the [Installation Guide](./installation.md) to get started, or jump straight to the [Quick Start](./quick-start.md) to see Termix in action.

::: tip Pro Tip
Termix works best with a terminal that supports [Nerd Fonts](https://www.nerdfonts.com/) for the full icon experience, but it gracefully falls back to ASCII characters if icons aren't available.
:::

## Community & Support

- **GitHub**: [amrohan/termix](https://github.com/amrohan/termix)
- **Issues**: Report bugs and request features on [GitHub Issues](https://github.com/amrohan/termix/issues)
- **Discussions**: Join the conversation in [GitHub Discussions](https://github.com/amrohan/termix/discussions)

## License

Termix is open-source software licensed under the [MIT License](https://github.com/amrohan/termix/blob/main/LICENSE.txt).
