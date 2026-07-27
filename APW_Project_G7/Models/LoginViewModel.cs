using System.ComponentModel.DataAnnotations;

namespace APW.Mvc.Models;

// Datos del formulario de login
public class LoginViewModel
{
    [Required(ErrorMessage = "El usuario es requerido")]
    public string Username { get; set; }

    [Required(ErrorMessage = "La password es requerida")]
    [DataType(DataType.Password)]
    public string Password { get; set; }
}

// Datos del usuario autenticado, devueltos por la Api
public class LoginResultViewModel
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string RoleName { get; set; }
}