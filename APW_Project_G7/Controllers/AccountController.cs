using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using APW.Mvc.Models;
using APW.Mvc.Service;

namespace APW.Mvc.Controllers;

public class AccountController : Controller
{
    private readonly IAuthService _authService;
    private readonly IUserService _userService;
    private readonly IRoleService _roleService;

    public AccountController(IAuthService authService, IUserService userService, IRoleService roleService)
    {
        _authService = authService;
        _userService = userService;
        _roleService = roleService;
    }

    // GET /Account/Login
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    // POST /Account/Login
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var result = await _authService.LoginAsync(model.Username, model.Password);
        if (result is null)
        {
            ModelState.AddModelError(string.Empty, "Usuario o password incorrectos");
            return View(model);
        }

        // Claims que identifican al usuario logueado, incluyendo su rol
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, result.Id.ToString()),
            new(ClaimTypes.Name, result.Username),
            new(ClaimTypes.Email, result.Email),
            new(ClaimTypes.Role, result.RoleName)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

        return RedirectToAction("Index", "Home");
    }
    // GET /Account/Register
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    // POST /Account/Register
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        // Verifica que el username y el correo no esten ya en uso
        var existingUsers = await _userService.GetUsersAsync();
        if (existingUsers.Any(u => u.Username == model.Username))
        {
            ModelState.AddModelError(nameof(model.Username), "Ese usuario ya existe");
            return View(model);
        }
        if (existingUsers.Any(u => u.Email == model.Email))
        {
            ModelState.AddModelError(nameof(model.Email), "Ese correo ya esta registrado");
            return View(model);
        }

        // Busca el rol "User" para asignarlo por defecto
        var roles = await _roleService.GetRolesAsync();
        var defaultRole = roles.FirstOrDefault(r => r.Name == "User");
        if (defaultRole is null)
        {
            ModelState.AddModelError(string.Empty, "No se encontro el rol por defecto, contacta al administrador");
            return View(model);
        }

        var newUser = new UserViewModel
        {
            Username = model.Username,
            Email = model.Email,
            Password = model.Password,
            RoleId = defaultRole.Id,
            IsActive = true
        };

        await _userService.CreateUserAsync(newUser);

        TempData["Mensaje"] = "Cuenta creada correctamente, ya puedes iniciar sesion";
        return RedirectToAction(nameof(Login));
    }


    // POST /Account/Logout
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    // GET /Account/AccessDenied
    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }

}