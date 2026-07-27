namespace APW.Api.ViewModels;

// Contrato publico de la Api para Setting
public class SettingViewModel
{
    public int Id { get; set; }
    public int? SourceId { get; set; }
    public string KeyName { get; set; }
    public string KeyValue { get; set; }
}