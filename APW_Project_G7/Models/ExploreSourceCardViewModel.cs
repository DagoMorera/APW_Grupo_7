namespace APW.Mvc.Models;

// Tarjeta de Source para Explorar, con una imagen de preview del primer item en vivo
public class ExploreSourceCardViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string ComponentType { get; set; } = string.Empty;
    public string? PreviewImageUrl { get; set; }
}