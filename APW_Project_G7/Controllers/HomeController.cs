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

    public HomeController(ILogger<HomeController> logger, ISourceItemService sourceItemService, ISourceService sourceService)
    {
        _logger = logger;
        _sourceItemService = sourceItemService;
        _sourceService = sourceService;
    }

    // Feed principal: items ya guardados en la BD, mezclados de todas las fuentes
    public async Task<IActionResult> Index()
    {
        var savedItems = await _sourceItemService.GetSourceItemsAsync();
        var sources = await _sourceService.GetSourcesAsync();
        var sourceNames = sources.ToDictionary(s => s.Id, s => s.Name);

        var feed = savedItems
            .Select(item => new { item.Id, item.CreatedAt, item.SourceId, Parsed = System.Text.Json.JsonSerializer.Deserialize<ParsedSourceItemViewModel>(item.Json) })
            .Where(x => x.Parsed is not null)
            .Select(x => new FeedItemViewModel
            {
                Id = x.Id,
                Title = x.Parsed!.Title,
                Description = x.Parsed.Description,
                Link = x.Parsed.Link,
                ImageUrl = x.Parsed.ImageUrl,
                SourceName = sourceNames.TryGetValue(x.SourceId, out var name) ? name : "Desconocida",
                CreatedAt = x.CreatedAt
            })
            .OrderByDescending(f => f.CreatedAt)
            .ToList();

        return View(feed);
    }
    // Descarga un item guardado en formato JSON, interoperable con otras apps
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
            Source = new ExportSourceViewModel
            {
                Url = source.Url,
                Name = source.Name,
                Description = source.Description,
                ComponentType = source.ComponentType,
                RequiresSecret = source.RequiresSecret
            },
            Item = new ExportContentViewModel
            {
                Title = parsed.Title,
                Description = parsed.Description,
                Link = parsed.Link,
                ImageUrl = parsed.ImageUrl,
                RawJson = parsed.RawJson
            }
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