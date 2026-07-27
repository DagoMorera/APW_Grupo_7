namespace APW.Models;

// Rol del sistema, ej. Admin o User
public class Role
{
    public int Id { get; set; }
    public string Name { get; set; }

    // Usuarios que tienen este rol
    public ICollection<User> Users { get; set; }
}