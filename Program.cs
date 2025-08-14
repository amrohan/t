using Spectre.Console;

namespace termix;

public static class Program
{
    public static Task Main(string[] args)
    {
        var useIcons = !args.Contains("--no-icons");
        try
        {
            AnsiConsole.Clear();
            var fileManager = new FileManager(useIcons);
            fileManager.Run();
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteException(ex, ExceptionFormats.ShortenEverything | ExceptionFormats.ShowLinks);
        }
        finally
        {
            AnsiConsole.Clear();
        }

        return Task.CompletedTask;
    }
}