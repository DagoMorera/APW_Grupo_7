using System.Text.Json;
using APW.Mvc.Models;

namespace APW.Mvc.Service;

public interface IFeedEntryProvider
{
    Task<List<FeedEntryViewModel>> GetEntriesAsync(string baseUrl);
}

// Trae los SourceItem guardados, los parsea, deduplica y ordena.
public class FeedEntryProvider : IFeedEntryProvider
{
    private const int MaxItems = 50;

    private readonly ISourceItemService _sourceItemService;
    private readonly ISourceService _sourceService;

    public FeedEntryProvider(ISourceItemService sourceItemService, ISourceService sourceService)
    {
        _sourceItemService = sourceItemService;
        _sourceService = sourceService;
    }

    public async Task<List<FeedEntryViewModel>> GetEntriesAsync(string baseUrl)
    {
        var savedItems = await _sourceItemService.GetSourceItemsAsync();
        var sources = await _sourceService.GetSourcesAsync();
        var sourceNames = sources.ToDictionary(s => s.Id, s => s.Name);

        var parsed = savedItems
            .Select(item => new
            {
                item.Id,
                item.CreatedAt,
                item.SourceId,
                Parsed = JsonSerializer.Deserialize<ParsedSourceItemViewModel>(item.Json)
            })
            .Where(x => x.Parsed is not null)
            .ToList();

        // Deduplica
        var deduplicated = parsed
            .GroupBy(x => string.IsNullOrWhiteSpace(x.Parsed!.Link)
                ? $"title:{x.Parsed.Title}"
                : $"link:{x.Parsed.Link}")
            .Select(group => group.OrderByDescending(x => x.CreatedAt).First());

        return deduplicated
            .OrderByDescending(x => x.CreatedAt)
            .Take(MaxItems)
            .Select(x => new FeedEntryViewModel
            {
                Id = x.Id,
                SourceId = x.SourceId,
                Title = string.IsNullOrWhiteSpace(x.Parsed!.Title) ? "Sin titulo" : x.Parsed.Title,
                Description = x.Parsed.Description ?? string.Empty,
                Link = string.IsNullOrWhiteSpace(x.Parsed.Link) ? $"{baseUrl}/Home/DownloadItem/{x.Id}" : x.Parsed.Link,
                ImageUrl = x.Parsed.ImageUrl,
                SourceName = sourceNames.TryGetValue(x.SourceId, out var name) ? name : "Desconocida",
                CreatedAt = x.CreatedAt,
                RawJson = PrettyPrintJson(x.Parsed.RawJson)
            })
            .ToList();
    }

    private static string PrettyPrintJson(string json)
    {
        if (string.IsNullOrWhiteSpace(json)) return string.Empty;

        try
        {
            using var document = JsonDocument.Parse(json);
            return JsonSerializer.Serialize(document, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (JsonException)
        {
            return json;
        }
    }
}