using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using APW.Mvc.Models;
using APW.Mvc.Service;

namespace APW_Project_G7.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ISourceItemService _sourceItemService;
    private readonly ISourceService _sourceService;
    private readonly IFeedEntryProvider _feedEntryProvider;

    public HomeController(
        ILogger<HomeController> logger,
        ISourceItemService sourceItemService,
        ISourceService sourceService,
        IFeedEntryProvider feedEntryProvider)
    {
        _logger = logger;
        _sourceItemService = sourceItemService;
        _sourceService = sourceService;
        _feedEntryProvider = feedEntryProvider;
    }

    // Home es el feed global
    public async Task<IActionResult> Index()
    {
        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        var entries = await _feedEntryProvider.GetEntriesAsync(baseUrl);
        return View(entries);
    }

    // Descarga un item guardado en formato JSON, interoperable con otras apps.
    public async Task<IActionResult> DownloadItem(int id)
    {
        var item = await _sourceItemService.GetSourceItemByIdAsync(id);
        if (item is null) return NotFound();

        var source = await _sourceService.GetSourceByIdAsync(item.SourceId);
        if (source is null) return NotFound();

        var parsed = System.Text.Json.JsonSerializer.Deserialize<ParsedSourceItemViewModel>(item.Json);
        if (parsed is null) return NotFound();

        var export = new ExportItemViewModel
        {
            Title = parsed.Title,
            Description = parsed.Description,
            ImageUrl = parsed.ImageUrl,
            Url = parsed.Link,
            PublishedAt = item.CreatedAt, // TODO: reemplazar por la fecha real de publicacion si en algun momento se captura
            SourceName = source.Name,
            SourceUrl = source.Url,
            SourceDescription = source.Description,
            SourceComponentType = source.ComponentType,
            SourceRequiresSecret = source.RequiresSecret
        };

        var json = System.Text.Json.JsonSerializer.Serialize(export, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
        var bytes = System.Text.Encoding.UTF8.GetBytes(json);
        return File(bytes, "application/json", $"apw-item-{id}.json");
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}