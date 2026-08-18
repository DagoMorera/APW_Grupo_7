using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using APW.Mvc.Models;
using APW.Mvc.Service;

namespace APW.Mvc.Controllers;

// Solo el rol Admin puede importar items desde un archivo JSON externo
[Authorize(Roles = "Admin")]
public class ImportController : Controller
{
    private readonly ISourceService _sourceService;
    private readonly ISourceItemService _sourceItemService;

    public ImportController(ISourceService sourceService, ISourceItemService sourceItemService)
    {
        _sourceService = sourceService;
        _sourceItemService = sourceItemService;
    }

    // GET /Import
    public IActionResult Index()
    {
        return View();
    }

    // POST /Import
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(IFormFile jsonFile)
    {
        if (jsonFile is null || jsonFile.Length == 0)
        {
            ModelState.AddModelError(string.Empty, "Debes seleccionar un archivo JSON");
            return View();
        }

        ExportItemViewModel? export;
        string rawContent;
        using (var reader = new StreamReader(jsonFile.OpenReadStream()))
        {
            rawContent = await reader.ReadToEndAsync();
            try
            {
                export = System.Text.Json.JsonSerializer.Deserialize<ExportItemViewModel>(rawContent);
            }
            catch (System.Text.Json.JsonException)
            {
                ModelState.AddModelError(string.Empty, "El archivo no tiene un formato JSON valido");
                return View();
            }
        }

        if (export is null || string.IsNullOrWhiteSpace(export.SourceUrl))
        {
            ModelState.AddModelError(string.Empty, "El archivo no tiene la estructura esperada");
            return View();
        }

        // Busca si la Source ya existe (por Url), si no existe la crea
        var existingSources = await _sourceService.GetSourcesAsync();
        var matchingSource = existingSources.FirstOrDefault(s => s.Url == export.SourceUrl);

        int sourceId;
        if (matchingSource is not null)
        {
            sourceId = matchingSource.Id;
        }
        else
        {
            var newSource = new SourceViewModel
            {
                Url = export.SourceUrl,
                Name = export.SourceName,
                Description = export.SourceDescription,
                ComponentType = export.SourceComponentType,
                RequiresSecret = export.SourceRequiresSecret
            };
            await _sourceService.CreateSourceAsync(newSource);
            var refreshedSources = await _sourceService.GetSourcesAsync();
            sourceId = refreshedSources.First(s => s.Url == export.SourceUrl).Id;
        }

        // Crea el SourceItem con el contenido normalizado del item importado
        var parsedItem = new ParsedSourceItemViewModel
        {
            Title = export.Title,
            Description = export.Description,
            Link = export.Url,
            ImageUrl = export.ImageUrl,
            RawJson = rawContent
        };

        var sourceItem = new SourceItemViewModel
        {
            SourceId = sourceId,
            Json = System.Text.Json.JsonSerializer.Serialize(parsedItem)
        };

        await _sourceItemService.CreateSourceItemAsync(sourceItem);

        TempData["Mensaje"] = $"Item '{export.Title}' importado correctamente";
        return RedirectToAction(nameof(Index));
    }
}