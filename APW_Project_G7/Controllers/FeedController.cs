using System.Text;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc;
using APW.Mvc.Models;
using APW.Mvc.Service;

namespace APW.Mvc.Controllers;

// Publica el contenido ya guardado.
public class FeedController : Controller
{
    private const int MaxItems = 50;

    private readonly ISourceItemService _sourceItemService;
    private readonly ISourceService _sourceService;

    public FeedController(ISourceItemService sourceItemService, ISourceService sourceService)
    {
        _sourceItemService = sourceItemService;
        _sourceService = sourceService;
    }

    // GET /feed.xml
    [Route("feed.xml")]
    [Route("Feed/Rss")]
    public async Task<IActionResult> Rss()
    {
        var savedItems = await _sourceItemService.GetSourceItemsAsync();
        var sources = await _sourceService.GetSourcesAsync();
        var sourceNames = sources.ToDictionary(s => s.Id, s => s.Name);

        var baseUrl = $"{Request.Scheme}://{Request.Host}";

        var channelItems = savedItems
            .Select(item => new
            {
                item.Id,
                item.CreatedAt,
                item.SourceId,
                Parsed = System.Text.Json.JsonSerializer.Deserialize<ParsedSourceItemViewModel>(item.Json)
            })
            .Where(x => x.Parsed is not null)
            .OrderByDescending(x => x.CreatedAt)
            .Take(MaxItems)
            .Select(x => BuildItemElement(x.Id, x.CreatedAt, x.SourceId, x.Parsed!, sourceNames, baseUrl));

        var rss = new XDocument(
            new XDeclaration("1.0", "utf-8", null),
            new XElement("rss",
                new XAttribute("version", "2.0"),
                new XElement("channel",
                    new XElement("title", "APW - Feed de contenido guardado"),
                    new XElement("link", baseUrl),
                    new XElement("description", "Items curados y guardados desde las fuentes configuradas en APW"),
                    new XElement("language", "es"),
                    new XElement("lastBuildDate", DateTime.UtcNow.ToString("r")),
                    channelItems
                )
            )
        );

        var xml = rss.Declaration + Environment.NewLine + rss.ToString();
        return Content(xml, "application/rss+xml", Encoding.UTF8);
    }

    private static XElement BuildItemElement(
        int id,
        DateTime createdAt,
        int sourceId,
        ParsedSourceItemViewModel parsed,
        Dictionary<int, string> sourceNames,
        string baseUrl)
    {
        var link = string.IsNullOrWhiteSpace(parsed.Link)
            ? $"{baseUrl}/Home/DownloadItem/{id}"
            : parsed.Link;

        var element = new XElement("item",
            new XElement("title", parsed.Title ?? "Sin titulo"),
            new XElement("description", parsed.Description ?? string.Empty),
            new XElement("link", link),
            new XElement("guid", new XAttribute("isPermaLink", "false"), $"apw-item-{id}"),
            new XElement("pubDate", createdAt.ToUniversalTime().ToString("r")),
            new XElement("source",
                new XAttribute("url", $"{baseUrl}/Explore"),
                sourceNames.TryGetValue(sourceId, out var name) ? name : "Desconocida")
        );

        if (!string.IsNullOrWhiteSpace(parsed.ImageUrl))
        {
            element.Add(new XElement("enclosure",
                new XAttribute("url", parsed.ImageUrl),
                new XAttribute("type", "image/jpeg")));
        }

        return element;
    }
}
