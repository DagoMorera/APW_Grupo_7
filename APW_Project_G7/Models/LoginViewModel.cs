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

// Datos del formulario de registro
public class RegisterViewModel
{
    [Required(ErrorMessage = "El usuario es requerido")]
    public string Username { get; set; }

    [Required(ErrorMessage = "El correo es requerido")]
    [EmailAddress(ErrorMessage = "Correo invalido")]
    public string Email { get; set; }

    [Required(ErrorMessage = "La password es requerida")]
    [MinLength(6, ErrorMessage = "La password debe tener al menos 6 caracteres")]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Required(ErrorMessage = "Debes confirmar la password")]
    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "Las passwords no coinciden")]
    public string ConfirmPassword { get; set; }
}