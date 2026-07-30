namespace APW.Mvc.Models;

// Item del feed principal, combina el contenido parseado con el nombre de su fuente
public class FeedItemViewModel
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Link { get; set; }
    public string? ImageUrl { get; set; }
    public string SourceName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}