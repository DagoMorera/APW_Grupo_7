namespace APW.Mvc.Models;

// Datos combinados para la pagina de Configuracion
public class SettingsPageViewModel
{
    public List<UserRoleRowViewModel> Users { get; set; } = new();
    public List<RoleViewModel> Roles { get; set; } = new();
    public List<SecretRowViewModel> Secrets { get; set; } = new();
    public List<SourceViewModel> Sources { get; set; } = new();
}

// Fila de usuario con su rol actual, para la seccion de asignacion de roles
public class UserRoleRowViewModel
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int RoleId { get; set; }
}

// Fila de secret, mostrando a que Source pertenece
public class SecretRowViewModel
{
    public int Id { get; set; }
    public int? SourceId { get; set; }
    public string SourceName { get; set; } = "Global";
    public string KeyName { get; set; } = string.Empty;
    public string KeyValue { get; set; } = string.Empty;
}