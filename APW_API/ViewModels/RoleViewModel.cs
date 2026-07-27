namespace APW.Api.ViewModels;

// Contrato publico de la Api para Role, no expone el Model de EF directamente
public class RoleViewModel
{
    public int Id { get; set; }
    public string Name { get; set; }
}