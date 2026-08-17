using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using APW.Mvc.Service;

namespace APW.Mvc.Controllers;

// Suscribirse/desuscribirse de una Source, como el boton de un canal de YouTube.
// Cualquier usuario logueado puede suscribirse
[Authorize]
public class SubscriptionController : Controller
{
    private readonly ISubscriptionService _subscriptionService;

    public SubscriptionController(ISubscriptionService subscriptionService)
    {
        _subscriptionService = subscriptionService;
    }

    // POST /Subscription/Toggle
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Toggle(int sourceId, string? returnUrl)
    {
        var userId = GetUserId();
        if (userId > 0)
        {
            await _subscriptionService.ToggleSubscriptionAsync(userId, sourceId);
        }

        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);

        return RedirectToAction("Index", "Explore");
    }

    private int GetUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(claim, out var id) ? id : 0;
    }
}