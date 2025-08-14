namespace termix.models;

public enum ClipboardMode
{
    Copy,
    Move
}

public record ClipboardItem(FileSystemItem Item, ClipboardMode Mode);