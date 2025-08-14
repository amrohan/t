# Search & Filter

Termix's powerful search and filtering system allows you to find files instantly across your entire directory tree. This guide covers all search features, from basic filtering to advanced techniques.

## Overview

Termix search is designed to be:

- **Instant**: Real-time results as you type
- **Recursive**: Searches through all subdirectories
- **Smart**: Respects `.gitignore` and ignore patterns
- **Contextual**: Maintains search state for navigation

## Basic Search

### Starting a Search

Press `S` to enter search mode:

1. Press `S` to activate search
2. Start typing your search term
3. Results appear instantly as you type
4. Press `Esc` to finish searching and navigate results

### Search Example

<VideoPlayer src="/videos/search.mp4" />

```bash
# Search for all JavaScript files
S → .js → Esc

# Search for component files
S → component → Esc

# Search for configuration files
S → config → Esc
```

## Search Modes

Termix operates in different modes during search:

### Search Mode (`S`)

- **Active typing**: Characters are added to search query
- **Real-time filtering**: Results update as you type
- **Visual indicator**: Status bar shows "Search: query█"
- **Background processing**: Deep search runs in background

### Filtered Navigation Mode (after `Esc`)

- **Normal navigation**: Use arrow keys to move through results
- **Maintained filter**: Only matching files are shown
- **Status indication**: Shows current filter in status bar
- **Operation support**: All file operations work on filtered results

## Search Features

### Case-Insensitive Matching

Search is automatically case-insensitive:

```bash
# These searches are equivalent
S → README → Esc
S → readme → Esc
S → ReAdMe → Esc
```

### Partial Matching

Search matches partial filenames:

```bash
# Find all files containing "test"
S → test → Esc
# Matches: test.js, MyTest.cs, testing.md, etc.
```

### Recursive Search

Search automatically includes all subdirectories:

```bash
# Finds files in any subdirectory
S → controller → Esc
# Matches: src/controllers/UserController.cs, api/ProductController.js, etc.
```

## Advanced Search Techniques

### File Extension Search

Search by file extension to find specific file types:

```bash
# Find all C# files
S → .cs → Esc

# Find all image files
S → .png → Esc
S → .jpg → Esc
```

### Path-Based Search

Search can match directory names in file paths:

```bash
# Find files in src directories
S → src/ → Esc

# Find test-related files
S → /test/ → Esc
```

### Multiple Terms

While Termix doesn't support complex query syntax, you can search for multiple terms by using common substrings:

```bash
# Find user-related controller files
S → usercontroller → Esc

# Find configuration JSON files
S → config.json → Esc
```

## Smart Filtering

### Git Integration

Termix automatically respects `.gitignore` files:

- **Ignored files**: Files in `.gitignore` are excluded from search results
- **Ignored directories**: Entire directories can be ignored
- **Nested gitignore**: Supports `.gitignore` files in subdirectories
- **Global gitignore**: Respects user's global git ignore settings

### Default Ignore Patterns

Termix has built-in ignore patterns for common build and cache directories:

```
node_modules/     # Node.js dependencies
bin/              # Build outputs
obj/              # Build intermediates
.git/             # Git repository data
__pycache__/      # Python cache
.DS_Store         # macOS system files
Thumbs.db         # Windows thumbnails
```

### Ignore Service Benefits

This smart filtering means your searches return only relevant files:

- **Faster results**: Fewer files to process
- **Cleaner output**: No noise from build artifacts
- **Better focus**: See only source files and documents

## Search Performance

### Background Processing

Termix optimizes search performance:

1. **Immediate local results**: Shows matches from current directory instantly
2. **Background deep search**: Recursively searches subdirectories in background
3. **Progressive results**: Results improve as background search completes
4. **Search indicator**: Status shows when background search is active

### Caching Strategy

- **Search cache**: Results are cached for the current directory tree
- **Invalidation**: Cache is cleared when directory contents change
- **Memory efficient**: Cache size is managed automatically

### Large Directory Handling

For very large directory trees:

- **Responsive interface**: UI remains responsive during search
- **Cancellation**: Starting a new search cancels previous ones
- **Debouncing**: Rapid typing doesn't trigger excessive searches

## Navigation Within Search Results

### Moving Through Results

Once you've applied a filter (pressed `Esc` after searching):

| Key               | Action   | Description                   |
| ----------------- | -------- | ----------------------------- |
| `↑` / `↓`         | Navigate | Move through filtered results |
| `J` / `K`         | Navigate | Vim-style movement            |
| `Enter` / `L`     | Open     | Open selected file            |
| `H` / `Backspace` | Up       | Navigate to parent directory  |

### Maintaining Search Context

When navigating from search results:

1. **Enter directory**: Search context is temporarily saved
2. **Use `B`**: Returns to original search results
3. **Clear filter**: Use `Esc` to clear and show all files
4. **New search**: Press `S` to start a new search

### Search State Management

Termix tracks several aspects of search state:

- **Active query**: Current search terms
- **Result set**: Files matching the query
- **Navigation position**: Where you are in the results
- **Original context**: How to return to search results

## Search Examples and Use Cases

### Code Development

```bash
# Find all TypeScript component files
S → .tsx → Esc

# Find specific component
S → Button → Esc

# Find test files
S → .test. → Esc
S → .spec. → Esc
```

### Configuration Management

```bash
# Find configuration files
S → config → Esc

# Find environment files
S → .env → Esc

# Find package files
S → package.json → Esc
```

### Documentation

```bash
# Find README files
S → README → Esc

# Find markdown documentation
S → .md → Esc

# Find specific docs
S → api → Esc
```

### Project Structure

```bash
# Explore source directories
S → src/ → Esc

# Find build scripts
S → build → Esc
S → script → Esc
```

## Search Tips and Tricks

### Efficient Searching

1. **Start broad, then narrow**: Begin with general terms, then add specificity
2. **Use extensions**: File extensions are very effective filters
3. **Combine with navigation**: Search to find, navigate to explore
4. **Use partial matches**: Don't type full filenames

### Search Patterns

1. **By file type**: `.js`, `.py`, `.md`, `.json`
2. **By purpose**: `test`, `config`, `util`, `helper`
3. **By feature**: `auth`, `user`, `payment`, `api`
4. **By location**: `src/`, `lib/`, `docs/`

### Workflow Integration

1. **Search → Navigate → Operate**: Find files, explore context, perform operations
2. **Search → Open → Return**: Use `B` to return to search results after opening files
3. **Iterative refinement**: Refine search terms based on initial results

## Clearing and Managing Filters

### Clear Active Filter

To remove an active filter and show all files:

1. **Press `Esc`**: If in normal mode, clears active filter
2. **Status confirmation**: Status bar confirms filter is cleared
3. **Full directory**: Shows complete directory contents again

### Search History

While Termix doesn't persist search history between sessions:

- **Current session**: Last search query is remembered
- **Quick re-search**: Easy to repeat recent searches
- **Context switching**: Can switch between filtered and unfiltered views

## Keyboard Shortcuts Reference

| Key            | Mode     | Action                                    |
| -------------- | -------- | ----------------------------------------- |
| `S`            | Normal   | Start search                              |
| `Esc`          | Search   | Apply filter and enter navigation mode    |
| `Esc`          | Filtered | Clear filter and show all files           |
| `B`            | Any      | Return to search results (when available) |
| Character keys | Search   | Add to search query                       |
| `Backspace`    | Search   | Remove from search query                  |

## Performance Tips

### Optimize Search Performance

1. **Use specific terms**: More specific searches are faster
2. **Leverage ignore patterns**: Let Termix skip irrelevant directories
3. **Don't over-search**: Find what you need, then navigate normally
4. **Clear when done**: Clear filters to improve general navigation

### Best Practices

1. **Search before navigate**: Use search to find the right area, then navigate normally
2. **Combine techniques**: Use search with other Termix features
3. **Understand your project**: Know your directory structure to search effectively
4. **Practice patterns**: Develop consistent search patterns for your workflow

## Next Steps

Now that you understand search and filtering:

- Check out [Keyboard Shortcuts](./keyboard-shortcuts.md) for a complete reference
- Learn [Tips & Tricks](./tips-tricks.md) for advanced workflows
- Explore [Troubleshooting](./troubleshooting.md) if you encounter issues

::: tip Pro Search Technique
The most powerful search workflow: Use `S` to find files, `Esc` to navigate results, perform operations, then `B` to return to results and continue. This creates an efficient find-operate-repeat cycle!
:::
