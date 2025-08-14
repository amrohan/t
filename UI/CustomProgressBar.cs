using Spectre.Console.Rendering;
using Spectre.Console;

namespace termix.UI;

public class CustomProgressBar : IRenderable
{
    public double Value { get; set; }
    public int? Width { get; set; }
    private Style CompletedStyle { get; set; } = new(Color.Green);
    private Style RemainingStyle { get; set; } = new(Color.Grey);

    public Measurement Measure(RenderOptions options, int maxWidth)
    {
        var width = Width ?? maxWidth;
        return new Measurement(width, width);
    }

    public IEnumerable<Segment> Render(RenderOptions options, int maxWidth)
    {
        var width = Width ?? maxWidth;
        var completedWidth = (int)Math.Clamp(width * (Value / 100.0), 0, width);
        var remainingWidth = width - completedWidth;

        var segments = new List<Segment>();

        if (completedWidth > 0)
        {
            segments.Add(new Segment(new string('█', completedWidth), CompletedStyle));
        }

        if (remainingWidth > 0)
        {
            segments.Add(new Segment(new string('░', remainingWidth), RemainingStyle));
        }

        if (width > 0 && segments.Count == 0)
        {
            segments.Add(new Segment(new string('░', width), RemainingStyle));
        }

        return segments;
    }
}