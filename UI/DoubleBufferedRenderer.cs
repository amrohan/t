using Spectre.Console;
using Spectre.Console.Rendering;
using System.Text;

namespace termix.UI;

public class DoubleBufferedRenderer
{
    private readonly IAnsiConsole _console;
    private readonly StringWriter _writer;

    public DoubleBufferedRenderer()
    {
        _writer = new StringWriter();

        _console = AnsiConsole.Create(new AnsiConsoleSettings
        {
            ColorSystem = ColorSystemSupport.TrueColor,
            Out = new AnsiConsoleOutput(_writer)
        });
    }

    public void Render(IRenderable layout)
    {
        _writer.GetStringBuilder().Clear();
        _console.Write(layout);
        var output = _writer.ToString();
        Console.SetCursorPosition(0, 0);

        if (Console.OutputEncoding.CodePage != Encoding.UTF8.CodePage)
        {
            Console.OutputEncoding = new UTF8Encoding(false);
        }

        Console.Write(output);
    }
}