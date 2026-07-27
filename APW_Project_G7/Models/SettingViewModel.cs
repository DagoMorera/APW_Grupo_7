namespace APW.Mvc.Models;

// ViewModel de Setting para las vistas de Mvc
public class SettingViewModel
{
    public int Id { get; set; }
    public int? SourceId { get; set; }
    public string KeyName { get; set; }
    public string KeyValue { get; set; }
}