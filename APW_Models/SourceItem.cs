namespace APW.Models;

// Item obtenido de una Source, guardado como JSON
public class SourceItem
{
    public int Id { get; set; }
    public int SourceId { get; set; }
    public string Json { get; set; }
    public DateTime CreatedAt { get; set; }

    // Fuente a la que pertenece este item
    public Source Source { get; set; }
}