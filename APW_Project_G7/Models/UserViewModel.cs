namespace APW.Mvc.Models;

// ViewModel de User para las vistas de Mvc
public class UserViewModel
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string? Password { get; set; } // solo se usa al crear el usuario
    public int RoleId { get; set; }
    public bool IsActive { get; set; }
}