namespace APW.Models;

// Usuario del sistema, cada uno tiene un solo rol asignado
public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public int RoleId { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? ModifiedBy { get; set; }

    // Rol asignado a este usuario
    public Role Role { get; set; }
}