namespace APW.Mvc.Models;
// Contrato publico de la Api para SourceItem
public class SourceItemViewModel
{
    public int Id { get; set; }
    public int SourceId { get; set; }
    public string Json { get; set; }
    public DateTime CreatedAt { get; set; }
}