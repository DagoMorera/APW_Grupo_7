using System.Text;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc;
using APW.Mvc.Models;
using APW.Mvc.Service;

namespace APW.Mvc.Controllers;

// Feed RSS/Atom publico del contenido guardado
public class FeedController : Controller
{
    private readonly ISourceItemService _sourceItemService;
    private readonly ISourceService _sourceService;

    public FeedController(ISourceItemService sourceItemService, ISourceService sourceService)
    {
        _sourceItemService = sourceItemService;
        _sourceService = sourceService;
    }

    // GET /Feed/Rss
    public async Task<IActionResult> Rss()
    {
        var entries = await BuildFeedEntriesAsync();
        var siteUrl = $"{Request.Scheme}://{Request.Host}";

        var channel = new XElement("channel",
            new XElement("title", "APW - Contenido guardado"),
            new XElement("link", siteUrl),
            new XElement("description", "Feed publico con el contenido guardado en APW"),
            new XElement("language", "es"),
            new XElement("lastBuildDate", DateTime.UtcNow.ToString("R")),
            entries.Select(e => new XElement("item",
                new XElement("title", e.Title ?? "Sin titulo"),
                new XElement("link", e.Link),
                new XElement("guid", new XAttribute("isPermaLink", "true"), e.PermaLink),
                new XElement("pubDate", e.CreatedAt.ToUniversalTime().ToString("R")),
                new XElement("description", e.Description ?? string.Empty),
                new XElement("source", new XAttribute("url", siteUrl), e.SourceName)
            ))
        );

        var rss = new XDocument(
            new XDeclaration("1.0", "utf-8", null),
            new XElement("rss", new XAttribute("version", "2.0"), channel)
        );

        return Content(rss.Declaration!.ToString() + Environment.NewLine + rss.ToString(), "application/rss+xml", Encoding.UTF8);
    }

    // GET /Feed/Atom
    public async Task<IActionResult> Atom()
    {
        var entries = await BuildFeedEntriesAsync();
        var siteUrl = $"{Request.Scheme}://{Request.Host}";
        XNamespace atom = "http://www.w3.org/2005/Atom";

        var feed = new XElement(atom + "feed",
            new XElement(atom + "title", "APW - Contenido guardado"),
            new XElement(atom + "link", new XAttribute("href", siteUrl)),
            new XElement(atom + "id", siteUrl + "/"),
            new XElement(atom + "updated", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")),
            entries.Select(e => new XElement(atom + "entry",
                new XElement(atom + "title", e.Title ?? "Sin titulo"),
                new XElement(atom + "link", new XAttribute("href", e.Link)),
                new XElement(atom + "id", e.PermaLink),
                new XElement(atom + "updated", e.CreatedAt.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ")),
                new XElement(atom + "summary", e.Description ?? string.Empty),
                new XElement(atom + "author", new XElement(atom + "name", e.SourceName))
            ))
        );

        var xml = new XDocument(new XDeclaration("1.0", "utf-8", null), feed);
        return Content(xml.Declaration!.ToString() + Environment.NewLine + xml.ToString(), "application/atom+xml", Encoding.UTF8);
    }

    // Arma las entradas del feed
    private async Task<List<FeedEntry>> BuildFeedEntriesAsync()
    {
        var savedItems = await _sourceItemService.GetSourceItemsAsync();
        var sources = await _sourceService.GetSourcesAsync();
        var sourceNames = sources.ToDictionary(s => s.Id, s => s.Name);
        var siteUrl = $"{Request.Scheme}://{Request.Host}";

        return savedItems
            .Select(item => new
            {
                item.Id,
                item.CreatedAt,
                item.SourceId,
                Parsed = System.Text.Json.JsonSerializer.Deserialize<ParsedSourceItemViewModel>(item.Json)
            })
            .Where(x => x.Parsed is not null)
            .Select(x => new FeedEntry
            {
                Title = x.Parsed!.Title,
                Description = x.Parsed.Description,
                Link = string.IsNullOrWhiteSpace(x.Parsed.Link) ? $"{siteUrl}/Home/DownloadItem/{x.Id}" : x.Parsed.Link!,
                PermaLink = $"{siteUrl}/Home/DownloadItem/{x.Id}",
                SourceName = sourceNames.TryGetValue(x.SourceId, out var name) ? name : "Desconocida",
                CreatedAt = x.CreatedAt
            })
            .OrderByDescending(e => e.CreatedAt)
            .Take(50)
            .ToList();
    }

    private class FeedEntry
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string Link { get; set; } = string.Empty;
        public string PermaLink { get; set; } = string.Empty;
        public string SourceName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}