using System.Windows;
    
public class DocumentData
{
    public string DocumentName { get; set; }
    public List<TextBlockData> TextBlocks { get; set; } = new List<TextBlockData>();
}

public class TextBlockData
{
    public string Text { get; set; }
    public int StartIndex { get; set; }
    public int Length { get; set; }
    
    public bool IsNewParagraph { get; set; }
    public TextFormatting Formatting { get; set; } = new TextFormatting();
}

public class TextFormatting
{
    public string FontFamily { get; set; } = "Segoe UI";
    public double FontSize { get; set; } = 12;
    public bool IsBold { get; set; }
    public bool IsItalic { get; set; }
    public bool IsUnderline { get; set; }
    public TextAlignment Alignment { get; set; } = TextAlignment.Left;
    public string FontColor { get; set; } = "#000000";
}