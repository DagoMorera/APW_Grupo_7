namespace APW.Mvc.Models;

// Formato de intercambio para Download/Upload, interoperable con otras apps
public class ExportItemViewModel
{
    public ExportSourceViewModel Source { get; set; } = new();
    public ExportContentViewModel Item { get; set; } = new();
}

// Datos de la fuente de origen, para que la app receptora pueda recrearla si no la tiene
public class ExportSourceViewModel
{
    public string Url { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string ComponentType { get; set; } = string.Empty;
    public bool RequiresSecret { get; set; }
}

// Contenido normalizado del item
public class ExportContentViewModel
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Link { get; set; }
    public string? ImageUrl { get; set; }
    public string RawJson { get; set; } = string.Empty;
}