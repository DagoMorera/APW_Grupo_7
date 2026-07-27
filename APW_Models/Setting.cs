namespace APW.Models;

// Configuracion o secret, global o ligado a una Source especifica
public class Setting
{
    public int Id { get; set; }
    public int? SourceId { get; set; } // null = setting global de la app
    public string KeyName { get; set; }
    public string KeyValue { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? ModifiedBy { get; set; }

    // Fuente asociada, si aplica
    public Source? Source { get; set; }
}