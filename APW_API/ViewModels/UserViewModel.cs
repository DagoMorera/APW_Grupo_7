namespace APW.Api.ViewModels;

// Contrato publico de la Api para User, nunca expone el PasswordHash ya guardado
public class UserViewModel
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string? Password { get; set; } // solo se usa al crear el usuario, no se devuelve
    public int RoleId { get; set; }
    public bool IsActive { get; set; }
}