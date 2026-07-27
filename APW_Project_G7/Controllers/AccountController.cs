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

    public AccountController(IAuthService authService)
    {
        _authService = authService;
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