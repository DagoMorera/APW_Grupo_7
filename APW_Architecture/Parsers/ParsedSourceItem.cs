namespace APW.Architecture.Parsers;

// Modelo generico de un item parseado, sin importar de que formato vino (JSON, XML, HTML)
public class ParsedSourceItem
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Link { get; set; }
    public string? ImageUrl { get; set; }

    // El objeto original completo, en formato JSON, para no perder informacion extra
    public string RawJson { get; set; } = string.Empty;
}