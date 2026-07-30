using APW.Mvc.Models;
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

    public ExploreController(ISourceService sourceService, ISourceItemService sourceItemService)
    {
        _sourceService = sourceService;
        _sourceItemService = sourceItemService;
    }

    // GET /Explore - catalogo de fuentes disponibles
    public async Task<IActionResult> Index()
    {
        var sources = await _sourceService.GetSourcesAsync();
        return View(sources);
    }

    // GET /Explore/Details/5 - items en vivo de una fuente especifica
    public async Task<IActionResult> Details(int id)
    {
        var source = await _sourceService.GetSourceByIdAsync(id);
        if (source is null) return NotFound();

        ViewBag.SourceId = source.Id;
        ViewBag.SourceName = source.Name;

        var items = await _sourceService.GetParsedItemsAsync(id);
        return View(items);
    }

    // POST /Explore/Save - guarda un item elegido, solo Admin
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
}