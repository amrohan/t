<div align="center">

# ⚡️ Termix 

**A modern, high-performance file navigator for your terminal**

[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](./LICENSE.txt)
[![NuGet Version](https://img.shields.io/nuget/v/Termix)](https://www.nuget.org/packages/Termix/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Termix)](https://www.nuget.org/packages/Termix/)
[![.NET](https://img.shields.io/badge/.NET-9.0-purple)](https://dotnet.microsoft.com/)
[![GitHub release](https://img.shields.io/github/release/amrohan/termix.svg)](https://GitHub.com/amrohan/termix/releases/)
[![GitHub issues](https://img.shields.io/github/issues/amrohan/termix.svg)](https://GitHub.com/amrohan/termix/issues/)
[![GitHub stars](https://img.shields.io/github/stars/amrohan/termix.svg?style=social&label=Star)](https://GitHub.com/amrohan/termix/stargazers/)

Built with .NET 9 and Spectre.Console, Termix delivers a fluid, visually rich, flicker-free interface for navigating, searching, and managing files—all from your terminal.

[Documentation](https://termix.pages.dev)

</div>

---

##  Demo


https://github.com/user-attachments/assets/c7b47493-ed6b-4b29-b334-f11f65d2dd18



> **Watch the full walkthrough** → [Termix on Vimeo](https://vimeo.com/1105824424) *(1 minute)*

##  Features

-  **Instant Recursive Filtering**: Start typing to filter files and directories in real-time
-  **Intuitive File Operations**: Create, rename, move, copy, and delete files and directories with visual progress tracking
-  **Smart Ignoring**: Respects `.gitignore` and automatically filters out common build directories
-  **Flicker-Free UI**: Double-buffered interface ensures smooth rendering in a two-pane layout
-  **Live Syntax Highlighting**: Preview various file types with syntax highlighting (`.cs`, `.js`, `.ts`, `.py`, and more)
-  **Vim-Style Navigation**: Use `J`/`K` alongside arrow keys for efficient movement
-  **Nerd Font Support**: Enhanced experience with icon support, with ASCII fallback for maximum compatibility
-  **Cross-Platform**: Works seamlessly on Windows, macOS, and Linux

##  Installation

### Prerequisites

- **.NET 9 SDK** or later installed on your system

### Option 1: Install as a .NET Global Tool (All Platforms)

```bash
dotnet tool install --global termix
```

Launch the application by running:

```bash
termix
```

### Option 2: Install from Source

```bash
# Clone the repository
git clone https://github.com/amrohan/termix.git
cd termix

# Build and install locally
dotnet pack
dotnet tool install --global --add-source ./nupkg termix
```

### Updating Termix

```bash
dotnet tool update --global termix
```

### Uninstalling Termix

```bash
dotnet tool uninstall --global termix
```

##  Keyboard Shortcuts

### Navigation

| Keys              | Action                       |
|:------------------|:-----------------------------|
| ↑ / ↓             | Move selection up/down       |
| `J` / `K`         | Vim-style movement           |
| `Enter` / `L`     | Open file or enter directory |
| `Backspace` / `H` | Go to parent directory       |
| `Home` / `End`    | Jump to first or last entry  |
| `Q`               | Quit Termix                  |

### File Operations

| Keys                  | Action                                              |
|:----------------------|:----------------------------------------------------|
| `S`                   | Enter search mode (filters recursively as you type) |
| `Esc` (during search) | Apply the filter and navigate the results           |
| `Esc` (after search)  | Clear the filter and show all items                 |
| `B`                   | Go back to the search results when navigating       |
| `A`                   | Create a new file or folder                         |
| `R`                   | Rename the selected file or folder                  |
| `D`                   | Delete the selected item (with confirmation)        |
| `X`                   | Move the selected file or folder                    |
| `C`                   | Copy the selected file or folder                    |
| `P`                   | Paste the file or folder                            |

### Preview Pane

| Keys          | Action              |
|:--------------|:--------------------|
| `Alt + ↑ / ↓` | Scroll vertically   |
| `Alt + ← / →` | Scroll horizontally |

##  Documentation
You can find the detail [documentation here](https://termix.pages.dev)

### Configuration

Termix works out of the box with sensible defaults. Future versions will support custom configuration options.

### Supported File Types

Termix provides rich previews and syntax highlighting for many common file types:

- **Code**: `.cs`, `.js`, `.ts`, `.py`  and more
- **Images**: Preview support for `.png`, `.jpg`, `.jpeg`, (terminal-compatible rendering)
- **Configuration**: `.yaml`, `.yml`, `.toml`, `.ini`, `.conf`, `.config`

##  Project Status

Termix is actively maintained and continuously improved. Check the [GitHub releases page](https://github.com/amrohan/termix/releases) for the latest updates and features.

##  Contributing

Contributions are always welcome! Whether it's bug reports, feature requests, or code contributions, please feel free to get involved.

1. **Fork** the repository on GitHub
2. **Clone** the forked repository to your machine
3. **Create a branch** for your feature or bug fix
4. **Make your changes** and commit them with descriptive messages
5. **Push** your changes back to your fork
6. Submit a **Pull Request** to the main repository

Please visit the [Issues page](https://github.com/amrohan/termix/issues) to report bugs or suggest features.

### Development Setup

If you'd like to contribute to the development of Termix:

1. Ensure you have the **.NET 9 SDK** installed
2. Clone the repository and open it in your preferred editor (Visual Studio, VS Code, Rider)
3. Run `dotnet restore` to install dependencies
4. Run `dotnet build` to build the project
5. Run `dotnet run` to test your changes locally

### Key Components

- `Services/ActionService.cs`: Logic for creating, renaming, deleting, and searching
- `Services/IgnoreService.cs`: Handles `.gitignore` and default ignore rules
- `Services/IconProvider.cs`: Manages file type icon mappings
- `Services/CustomSyntaxHighlighter.cs`: Add new language themes here
- `UI/FileManagerRenderer.cs`: Contains the logic for styling and rendering panes
- `UI/CustomProgressBar.cs`: Progress visualization for long-running operations

### Coding Guidelines

- Follow existing code style and patterns
- Use meaningful variable and method names
- Write tests for new features when applicable
- Update documentation to reflect your changes
- Branch from `main` and use descriptive commit messages

##  Acknowledgements

- This project is made possible by the fantastic [Spectre.Console](https://spectreconsole.net/) library
- Glob-style ignore matching is handled by [DotNet.Glob](https://github.com/dazinator/DotNet.Glob)
- Image handling is provided by [SixLabors.ImageSharp](https://github.com/SixLabors/ImageSharp)
- Icons are powered by the [Nerd Fonts](https://www.nerdfonts.com/) project
- Inspired by the simplicity and elegance of .NET Global Tools

##  License

Termix is licensed under the [MIT License](./LICENSE.txt).
