using System.Security.Claims;
using APW.Mvc.Models;
using APW.Mvc.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace APW.Mvc.Controllers;

// Explorar es visible para cualquiera, guardar esta protegido aparte
public class ExploreController : Controller
{
    private readonly ISourceService _sourceService;
    private readonly ISourceItemService _sourceItemService;
    private readonly ISubscriptionService _subscriptionService;

    public ExploreController(ISourceService sourceService, ISourceItemService sourceItemService, ISubscriptionService subscriptionService)
    {
        _sourceService = sourceService;
        _sourceItemService = sourceItemService;
        _subscriptionService = subscriptionService;
    }

    // GET /Explore
    public async Task<IActionResult> Index()
    {
        var sources = await _sourceService.GetSourcesAsync();
        ViewBag.SubscribedSourceIds = await GetSubscribedSourceIdsAsync();
        return View(sources);
    }

    // GET /Explore/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var source = await _sourceService.GetSourceByIdAsync(id);
        if (source is null) return NotFound();

        ViewBag.SourceId = source.Id;
        ViewBag.SourceName = source.Name;
        ViewBag.SubscribedSourceIds = await GetSubscribedSourceIdsAsync();

        var items = await _sourceService.GetParsedItemsAsync(id);
        return View(items);
    }

    // POST /Explore/Save
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Save(int sourceId, ParsedSourceItemViewModel item)
    {
        var sourceItem = new SourceItemViewModel
        {
            SourceId = sourceId,
            Json = System.Text.Json.JsonSerializer.Serialize(item)
        };

        await _sourceItemService.CreateSourceItemAsync(sourceItem);
        TempData["Mensaje"] = $"Item '{item.Title}' guardado correctamente";
        return RedirectToAction(nameof(Details), new { id = sourceId });
    }

    // Trae los ids de las Sources a las que el usuario actual esta suscrito
    private async Task<HashSet<int>> GetSubscribedSourceIdsAsync()
    {
        if (User.Identity is not { IsAuthenticated: true }) return new HashSet<int>();

        var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(claim, out var userId)) return new HashSet<int>();

        var ids = await _subscriptionService.GetSubscribedSourceIdsAsync(userId);
        return ids.ToHashSet();
    }
}