namespace APW.Api.ViewModels;

// Contrato publico de la Api para Source
public class SourceViewModel
{
    public int Id { get; set; }
    public string Url { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public string ComponentType { get; set; }
    public bool RequiresSecret { get; set; }
}