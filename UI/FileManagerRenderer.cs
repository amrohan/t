using Spectre.Console;
using Spectre.Console.Rendering;
using termix.models;
using termix.Services;

namespace termix.UI;

public class FileManagerRenderer(IconProvider iconProvider)
{
    public Layout GetLayout(string currentPath, List<FileSystemItem> items, int selectedIndex,
        IRenderable previewContent, int viewOffset, IRenderable footerRenderable)
    {
        var header = CreateHeader(currentPath);
        var body = CreateBody(items, selectedIndex, previewContent, viewOffset);
        
        var footer = new Panel(Align.Center(footerRenderable)) { Border = BoxBorder.None };

        return new Layout("Root")
            .SplitRows(
                new Layout("Header").Update(header).Size(3),
                new Layout("Body").Update(body),
                new Layout("Footer").Update(footer).Size(3) 
            );
    }

    private static Panel CreateHeader(string currentPath)
    {
        var displayPath = currentPath.Length > 80 ? "..." + currentPath[^77..] : currentPath;
        var headerContent = new Markup($"[bold cyan3]\uE5FF {displayPath.EscapeMarkup()}[/]");
        return new Panel(headerContent) { Border = BoxBorder.Rounded, BorderStyle = new Style(Color.Cyan1) };
    }

    private Layout CreateBody(List<FileSystemItem> items, int selectedIndex, IRenderable previewContent, int viewOffset)
    {
        var fileTable = CreateFileTable(items, selectedIndex, viewOffset);
        return new Layout("Body").SplitColumns(
            new Layout("FileList").Update(fileTable).Ratio(3),
            new Layout("Preview").Update(previewContent).Ratio(3)
        );
    }

    private Table CreateFileTable(List<FileSystemItem> items, int selectedIndex, int viewOffset)
    {
        var table = new Table().Expand().Border(TableBorder.None);
        table.AddColumn("Name");
        table.AddColumn(new TableColumn("Size").RightAligned());
        table.AddColumn(new TableColumn("Modified").RightAligned());
        table.AddColumn(new TableColumn("").Width(1));

        var pageSize = Console.WindowHeight - 12;
        pageSize = Math.Max(5, pageSize);
        var visibleItems = items.Skip(viewOffset).Take(pageSize).ToList();

        for (var i = 0; i < visibleItems.Count; i++)
        {
            var item = visibleItems[i];
            var originalIndex = i + viewOffset;
            var isSelected = originalIndex == selectedIndex;
            var style = isSelected ? new Style(background: Color.DodgerBlue1) : Style.Plain;
            var name = CreateNameMarkup(item);
            var scrollChar = GetScrollbarChar(i, items.Count, pageSize, viewOffset, visibleItems.Count);

            table.AddRow(
                new Markup(name, style),
                new Markup(item.FormattedSize, style),
                new Markup(item.FormattedDate, style),
                new Markup(scrollChar, style)
            );
        }

        return table;
    }

    private static string GetScrollbarChar(int currentIndex, int totalItems, int pageSize, int viewOffset,
        int visibleCount)
    {
        if (totalItems <= pageSize) return " ";

        if (currentIndex == 0 && viewOffset > 0) return "⬆";
        if (currentIndex == visibleCount - 1 && viewOffset + pageSize < totalItems) return "⬇";

        var thumbStart = (int)((double)viewOffset / totalItems * visibleCount);
        var thumbEnd = (int)((double)(viewOffset + pageSize) / totalItems * visibleCount);
        if (currentIndex >= thumbStart && currentIndex <= thumbEnd) return "█";

        return "║";
    }

    private string CreateNameMarkup(FileSystemItem item)
    {
        var icon = iconProvider.GetIcon(item);
        var name = item.Name.EscapeMarkup();
        var nameStyle = item.IsDirectory ? "bold" : "";
        return $"{icon}  [{nameStyle}]{name}[/]";
    }

    public static void ShowError(string message)
    {
        AnsiConsole.Clear();
        AnsiConsole.MarkupLine($"[bold red]Error:[/] [red]{message.EscapeMarkup()}[/]");
        AnsiConsole.MarkupLine("[grey]Press any key to continue...[/]");
        Console.ReadKey(true);
    }
}
