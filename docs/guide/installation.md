# Installation

This guide will walk you through installing Termix on your system. Termix is distributed as a .NET Global Tool, making installation quick and easy across all supported platforms.

## Prerequisites

Before installing Termix, ensure you have:

- **.NET 9 SDK** or later installed on your system
- A terminal application (Command Prompt, PowerShell, Terminal.app, etc.)
- Optional: A [Nerd Font](https://www.nerdfonts.com/) for enhanced icon support

### Installing .NET 9

If you don't have .NET 9 installed, download it from the [official .NET website](https://dotnet.microsoft.com/download/dotnet/9.0).

To verify your .NET installation:

```bash
dotnet --version
```

You should see version 9.0 or higher.

## Option 1: Install from NuGet (Recommended)

The easiest way to install Termix is as a .NET Global Tool from NuGet:

```bash
dotnet tool install --global termix
```

This command will:
- Download the latest version of Termix from NuGet
- Install it globally on your system
- Add the `termix` command to your PATH

### Launching Termix

Once installed, you can launch Termix from any directory:

```bash
termix
```

## Option 2: Install from Source

If you want to build Termix from source or contribute to development:

### 1. Clone the Repository

```bash
git clone https://github.com/amrohan/termix.git
cd termix
```

### 2. Build and Pack

```bash
dotnet pack
```

### 3. Install Locally

```bash
dotnet tool install --global --add-source ./nupkg termix
```

## Updating Termix

To update to the latest version:

```bash
dotnet tool update --global termix
```

## Uninstalling Termix

To remove Termix from your system:

```bash
dotnet tool uninstall --global termix
```

## Platform-Specific Notes

### Windows

Termix works great on Windows with:
- **Windows Terminal** (recommended)
- **PowerShell** 
- **Command Prompt**

For the best experience, enable Unicode support in your terminal.

### macOS

On macOS, Termix works with:
- **Terminal.app** (built-in)
- **iTerm2** (recommended for enhanced features)
- **Other terminal emulators**

### Linux

Termix is compatible with most Linux terminal emulators:
- **GNOME Terminal**
- **Konsole**
- **Alacritty**
- **Kitty**
- And many more

## Font Setup (Optional)

For the best visual experience with file type icons, install a [Nerd Font](https://www.nerdfonts.com/):

### Recommended Fonts
- **FiraCode Nerd Font**
- **JetBrains Mono Nerd Font** 
- **Cascadia Code Nerd Font**

### Installation Steps
1. Download your preferred Nerd Font from [nerdfonts.com](https://www.nerdfonts.com/)
2. Install the font on your system
3. Configure your terminal to use the installed font

::: tip Icon Fallback
Don't worry if you can't install Nerd Fonts! Termix automatically falls back to ASCII characters, ensuring full functionality regardless of your font setup.
:::

## Command Line Options

Termix supports several command line options:

```bash
# Launch normally with icons
termix

# Launch without icons (ASCII mode)
termix --no-icons
```

## Troubleshooting

### "Command not found" Error

If you get a "command not found" error after installation:

1. **Check if .NET tools are in PATH**: The .NET global tools directory needs to be in your system PATH
2. **Restart your terminal**: Sometimes PATH changes require a terminal restart
3. **Verify installation**: Run `dotnet tool list --global` to see if Termix is listed

### .NET SDK Not Found

If you get a .NET SDK error:

1. **Install .NET 9**: Download from [dotnet.microsoft.com](https://dotnet.microsoft.com/)
2. **Verify installation**: Run `dotnet --version`
3. **Check PATH**: Ensure the `dotnet` command is available

### Permission Errors

On some systems, you might need elevated permissions:

```bash
# Linux/macOS
sudo dotnet tool install --global termix

# Windows (run as Administrator)
dotnet tool install --global termix
```

## Next Steps

Now that Termix is installed, check out the [Quick Start Guide](./quick-start.md) to learn the basics, or dive into the [Navigation Guide](./navigation.md) to master file navigation.

::: info Version Information
Current stable version: **1.5.0**  
Minimum .NET version: **9.0**  
Supported platforms: **Windows, macOS, Linux**
:::
