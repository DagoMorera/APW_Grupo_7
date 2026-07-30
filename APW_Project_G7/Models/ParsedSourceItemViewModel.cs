namespace APW.Mvc.Models;

// ViewModel de un item ya parseado (JSON/XML/HTML normalizado), para mostrar en las vistas
public class ParsedSourceItemViewModel
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Link { get; set; }
    public string? ImageUrl { get; set; }
    public string RawJson { get; set; } = string.Empty;
}