using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using APW.Mvc.Models;
using APW.Mvc.Service;

namespace APW.Mvc.Controllers;

// Solo Admin puede ver/modificar Configuracion (roles y secrets)
[Authorize(Roles = "Admin")]
public class SettingsController : Controller
{
    private readonly IUserService _userService;
    private readonly IRoleService _roleService;
    private readonly ISettingService _settingService;
    private readonly ISourceService _sourceService;

    public SettingsController(IUserService userService, IRoleService roleService, ISettingService settingService, ISourceService sourceService)
    {
        _userService = userService;
        _roleService = roleService;
        _settingService = settingService;
        _sourceService = sourceService;
    }

    // GET /Settings
    public async Task<IActionResult> Index()
    {
        var model = await BuildPageModelAsync();
        return View(model);
    }

    // POST /Settings/UpdateRole
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateRole(int userId, int roleId)
    {
        var user = await _userService.GetUserByIdAsync(userId);
        if (user is null) return NotFound();

        user.RoleId = roleId;
        await _userService.UpdateUserAsync(userId, user);

        TempData["Mensaje"] = $"Rol de '{user.Username}' actualizado correctamente";
        return RedirectToAction(nameof(Index));
    }

    // POST /Settings/CreateSecret
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateSecret(int? sourceId, string keyName, string keyValue)
    {
        var setting = new SettingViewModel
        {
            SourceId = sourceId,
            KeyName = keyName,
            KeyValue = keyValue
        };

        await _settingService.CreateSettingAsync(setting);
        TempData["Mensaje"] = "Secret guardado correctamente";
        return RedirectToAction(nameof(Index));
    }

    // POST /Settings/DeleteSecret
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteSecret(int id)
    {
        await _settingService.DeleteSettingAsync(id);
        TempData["Mensaje"] = "Secret eliminado correctamente";
        return RedirectToAction(nameof(Index));
    }

    // Arma el modelo combinado de la pagina, uniendo usuarios+roles y secrets+sources
    private async Task<SettingsPageViewModel> BuildPageModelAsync()
    {
        var users = await _userService.GetUsersAsync();
        var roles = await _roleService.GetRolesAsync();
        var secrets = await _settingService.GetSettingsAsync();
        var sources = await _sourceService.GetSourcesAsync();
        var sourceNames = sources.ToDictionary(s => s.Id, s => s.Name);

        return new SettingsPageViewModel
        {
            Users = users.Select(u => new UserRoleRowViewModel
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                RoleId = u.RoleId
            }).ToList(),
            Roles = roles.ToList(),
            Secrets = secrets.Select(s => new SecretRowViewModel
            {
                Id = s.Id,
                SourceId = s.SourceId,
                SourceName = s.SourceId.HasValue && sourceNames.TryGetValue(s.SourceId.Value, out var name) ? name : "Global",
                KeyName = s.KeyName,
                KeyValue = s.KeyValue
            }).ToList(),
            Sources = sources.ToList()
        };
    }
}