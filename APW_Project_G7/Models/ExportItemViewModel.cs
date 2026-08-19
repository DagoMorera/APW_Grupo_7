namespace APW.Mvc.Models;

// Formato de intercambio para Download/Upload, interoperable con otras apps
public class ExportItemViewModel
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public string? Url { get; set; }
    public string? PublishedAt { get; set; }
    public string SourceName { get; set; } = string.Empty;
    public string SourceUrl { get; set; } = string.Empty;
    public string? SourceDescription { get; set; }
    public string SourceComponentType { get; set; } = string.Empty;
    public bool SourceRequiresSecret { get; set; }
}