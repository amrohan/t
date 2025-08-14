# Keyboard Shortcuts

Master all of Termix's keyboard shortcuts for maximum productivity. This comprehensive reference covers every key binding organized by function and context.

## Quick Reference Card

### Essential Shortcuts
| Key | Action | Key | Action |
|-----|--------|-----|--------|
| `↑↓` / `JK` | Move selection | `Enter` / `L` | Open/Enter |
| `Backspace` / `H` | Go up | `S` | Search |
| `A` | Add | `R` | Rename |
| `C` | Copy | `X` | Move/Cut |
| `P` | Paste | `D` | Delete |
| `Q` | Quit | `Esc` | Cancel/Clear |

## Navigation Shortcuts

### Basic Movement
| Key | Action | Description |
|-----|--------|-------------|
| `↑` | Move up | Select previous item |
| `↓` | Move down | Select next item |
| `J` | Move up (Vim) | Vim-style up movement |
| `K` | Move down (Vim) | Vim-style down movement |
| `Home` | Jump to top | Select first item in list |
| `End` | Jump to bottom | Select last item in list |

### Directory Navigation
| Key | Action | Description |
|-----|--------|-------------|
| `Enter` | Open/Enter | Open file or enter directory |
| `L` | Open/Enter | Alternative key for opening |
| `Backspace` | Go up | Move to parent directory |
| `H` | Go up (Vim) | Vim-style parent navigation |

### Preview Pane Control
| Key | Action | Description |
|-----|--------|-------------|
| `Alt + ↑` | Scroll up | Scroll preview content up |
| `Alt + ↓` | Scroll down | Scroll preview content down |
| `Alt + ←` | Scroll left | Scroll preview content left |
| `Alt + →` | Scroll right | Scroll preview content right |

## File Operations

### Creation and Modification
| Key | Action | Description |
|-----|--------|-------------|
| `A` | Add | Create new file or directory |
| `R` | Rename | Rename selected item |
| `D` | Delete | Delete with confirmation |

### Clipboard Operations
| Key | Action | Description |
|-----|--------|-------------|
| `C` | Copy | Copy item to clipboard |
| `X` | Move/Cut | Cut item to clipboard |
| `P` | Paste | Paste from clipboard |

## Search and Filter

### Search Control
| Key | Mode | Action | Description |
|-----|------|--------|-------------|
| `S` | Normal | Start search | Enter search mode |
| `Esc` | Search | Apply filter | Finish search, navigate results |
| `Esc` | Filtered | Clear filter | Remove filter, show all files |
| `B` | Any | Back to results | Return to search results |

### Search Input
| Key | Mode | Action |
|-----|------|--------|
| Character keys | Search | Add to query |
| `Backspace` | Search | Remove from query |
| `Enter` | Search | (Same as Esc) Apply filter |

## Application Control

### Session Management
| Key | Action | Description |
|-----|--------|-------------|
| `Q` | Quit | Exit Termix |
| `Esc` | Cancel | Cancel current operation |

### Special Contexts
| Key | Context | Action |
|-----|---------|--------|
| `y` | Delete confirm | Confirm deletion |
| `n` | Delete confirm | Cancel deletion |
| `y` | Quit confirm | Force quit with operations |
| `n` | Quit confirm | Cancel quit |

## Mode-Specific Shortcuts

### Normal Mode
*Default mode for navigation and file operations*

| Category | Keys | Actions |
|----------|------|---------|
| **Movement** | `↑↓JK`, `Home/End` | Navigate through files |
| **Navigation** | `Enter/L`, `Backspace/H` | Open/close directories |
| **Operations** | `ARCXPD` | File operations |
| **Search** | `S` | Start search |
| **Preview** | `Alt + arrows` | Scroll preview |
| **Exit** | `Q` | Quit application |

### Search Mode
*Active when typing search queries*

| Key | Action | Notes |
|-----|--------|-------|
| Character keys | Add to search | Real-time filtering |
| `Backspace` | Remove character | Update results |
| `Esc` | Apply filter | Switch to filtered navigation |
| `Enter` | Apply filter | Same as Esc |

### Filtered Navigation Mode  
*Active after applying a search filter*

| Key | Action | Notes |
|-----|--------|-------|
| Navigation keys | Move through results | Only filtered items shown |
| File operations | Work on filtered items | All operations available |
| `B` | Back to search | Return to original results |
| `Esc` | Clear filter | Show all files |
| `S` | New search | Start fresh search |

### Add Mode
*Active when creating files/directories*

| Key | Action |
|-----|--------|
| Character keys | Type name |
| `Backspace` | Remove character |
| `Enter` | Create item |
| `Esc` | Cancel creation |

### Rename Mode
*Active when renaming items*

| Key | Action |
|-----|--------|
| Character keys | Edit name |
| `Backspace` | Remove character |
| `Enter` | Confirm rename |
| `Esc` | Cancel rename |

### Delete Confirm Mode
*Active when confirming deletions*

| Key | Action |
|-----|--------|
| `y` | Confirm delete |
| `n` | Cancel delete |
| `Esc` | Cancel delete |

## Advanced Key Combinations

### Vim-Style Navigation
For Vim users, these combinations feel natural:

```
h j k l    →    H J K L
← ↓ ↑ →    →    Parent Down Up Enter
```

### Power User Workflows

**Quick File Creation:**
```
A → filename.ext → Enter
```

**Search and Operate:**
```
S → query → Esc → navigate → operate → B
```

**Clipboard Workflow:**
```
C → navigate → P  (copy)
X → navigate → P  (move)
```

## Context-Sensitive Behavior

### Esc Key Behavior
The `Esc` key behaves differently based on context:

| Context | Action |
|---------|--------|
| Normal mode with filter | Clear active filter |
| Normal mode with clipboard | Clear clipboard |
| Search mode | Apply filter and enter navigation |
| Add/Rename mode | Cancel operation |
| Delete confirm | Cancel deletion |

### Backspace Key Behavior
The `Backspace` key has dual purposes:

| Context | Action |
|---------|--------|
| Normal navigation | Go to parent directory |
| Text input mode | Remove last character |

## Custom Key Patterns

### Directory Traversal Patterns
```
L L L    # Dive deep into directories
H H H    # Go up multiple levels  
End L    # Go to last item and enter
Home L   # Go to first item and enter
```

### Search Patterns
```
S .js Esc          # Find JavaScript files
S test Esc         # Find test files
S config Esc       # Find config files
S src/ Esc         # Find files in src directories
```

### File Management Patterns
```
C → navigate → P   # Copy workflow
X → navigate → P   # Move workflow
A folder/ Enter    # Create directory
R newname Enter    # Quick rename
```

## Accessibility and Alternatives

### Alternative Key Bindings
Most actions have alternative keys for different preferences:

| Primary | Alternative | Action |
|---------|-------------|--------|
| `↑↓` | `JK` | Vertical movement |
| `Enter` | `L` | Open/Enter |
| `Backspace` | `H` | Go up |

### No Mouse Required
Termix is entirely keyboard-driven:
- **No mouse dependency**: All features accessible via keyboard
- **Fast navigation**: Keyboard is faster than mouse for file operations  
- **Terminal native**: Works in any terminal environment

## Memory Aids

### Mnemonic Devices
| Key | Mnemonic | Action |
|-----|----------|--------|
| `A` | **A**dd | Create new item |
| `R` | **R**ename | Rename item |
| `C` | **C**opy | Copy to clipboard |
| `X` | Cut (e**X**tract) | Move to clipboard |
| `P` | **P**aste | Paste from clipboard |
| `D` | **D**elete | Delete item |
| `S` | **S**earch | Start search |
| `Q` | **Q**uit | Exit application |

### Vim Connections
| Vim | Termix | Action |
|-----|--------|--------|
| `j` | `J` | Move down |
| `k` | `K` | Move up |
| `h` | `H` | Go left/up |
| `l` | `L` | Go right/down |

## Troubleshooting Key Issues

### Keys Not Working
If shortcuts aren't responding:

1. **Check terminal focus**: Ensure terminal window has focus
2. **Verify key support**: Some terminals may not support all key combinations
3. **Check for conflicts**: Other applications might intercept keys
4. **Try alternatives**: Most actions have alternative key bindings

### Special Key Combinations
Some terminals handle special keys differently:

- **Alt combinations**: May not work in all terminals
- **Function keys**: Not used by Termix to avoid conflicts
- **Ctrl combinations**: Avoided to prevent terminal conflicts

## Next Steps

Now that you know all the shortcuts:

- Practice with [Tips & Tricks](./tips-tricks.md) to build efficient workflows
- Check [Configuration](./configuration.md) for customization options
- Visit [Troubleshooting](./troubleshooting.md) if you encounter issues

::: tip Muscle Memory Development
Focus on learning one category of shortcuts at a time. Start with navigation (`↑↓JK`, `Enter`, `Backspace`), then add file operations (`ARCXPD`), and finally master search (`S`, `Esc`, `B`).
:::

::: warning Terminal Compatibility
While Termix works in most terminals, some key combinations (especially Alt+arrow keys) may not work in all environments. All essential functions have alternative key bindings that work everywhere.
:::
