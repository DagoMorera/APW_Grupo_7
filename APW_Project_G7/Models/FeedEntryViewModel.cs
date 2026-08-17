namespace APW.Mvc.Models;

public class FeedEntryViewModel
{
    public int Id { get; set; }
    public int SourceId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Link { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string SourceName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public string RawJson { get; set; } = string.Empty;
}