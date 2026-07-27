using static System.Runtime.InteropServices.JavaScript.JSType;

namespace APW.Models;

// Fuente externa (API/feed) agregada por el Admin
public class Source
{
    public int Id { get; set; }
    public string Url { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public string ComponentType { get; set; } // 'widget','api','feed'
    public bool RequiresSecret { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? ModifiedBy { get; set; }

    // Items obtenidos de esta fuente
    public ICollection<SourceItem> SourceItems { get; set; }

    // Settings/secrets asociados a esta fuente
    public ICollection<Setting> Settings { get; set; }
}