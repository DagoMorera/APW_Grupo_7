namespace APW.Api.ViewModels;

// Datos que se reciben al hacer login
public class LoginRequestViewModel
{
    public string Username { get; set; }
    public string Password { get; set; }
}

// Datos que se devuelven si el login es correcto
public class LoginResponseViewModel
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string RoleName { get; set; }
}