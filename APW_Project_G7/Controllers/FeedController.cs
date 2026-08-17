using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using APW.Mvc.Models;
using APW.Mvc.Service;

namespace APW.Mvc.Controllers;

public class FeedController : Controller
{
    private const int MaxItems = 50;

    private readonly ISourceItemService _sourceItemService;
    private readonly ISourceService _sourceService;
    private readonly ISubscriptionService _subscriptionService;
    private readonly IUserService _userService;

    public FeedController(
        ISourceItemService sourceItemService,
        ISourceService sourceService,
        ISubscriptionService subscriptionService,
        IUserService userService)
    {
        _sourceItemService = sourceItemService;
        _sourceService = sourceService;
        _subscriptionService = subscriptionService;
        _userService = userService;
    }

    // GET /feed.xml
    [Route("feed.xml")]
    [Route("Feed/Rss")]
    public async Task<IActionResult> Rss()
    {
        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        var entries = await GetFeedEntriesAsync(baseUrl);
        return BuildRssResult(entries, "APW - Feed de contenido guardado",
            "Items curados y guardados desde las fuentes configuradas en APW", baseUrl);
    }

    // GET /Feed
    [Route("Feed")]
    [Route("Feed/Preview")]
    public async Task<IActionResult> Preview()
    {
        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        var entries = await GetFeedEntriesAsync(baseUrl);
        return View(entries);
    }

    // GET /Feed/Mine
    [Authorize]
    [Route("Feed/Mine")]
    public async Task<IActionResult> MyFeed()
    {
        var userId = GetUserId();
        if (userId == 0) return Forbid();

        var user = await _userService.GetUserByIdAsync(userId);
        var baseUrl = $"{Request.Scheme}://{Request.Host}";

        var subscribedSourceIds = (await _subscriptionService.GetSubscribedSourceIdsAsync(userId)).ToHashSet();

        var allSources = await _sourceService.GetSourcesAsync();
        var subscribedSources = allSources
            .Where(s => subscribedSourceIds.Contains(s.Id))
            .OrderBy(s => s.Name)
            .ToList();

        var allEntries = await GetFeedEntriesAsync(baseUrl);
        var myEntries = allEntries.Where(e => subscribedSourceIds.Contains(e.SourceId)).ToList();

        ViewBag.PersonalFeedUrl = $"{baseUrl}/feed/{user.FeedToken}.xml";

        var model = new MyFeedViewModel
        {
            SubscribedSources = subscribedSources,
            Entries = myEntries
        };

        return View(model);
    }

    // GET /feed/{token}.xml
    [Route("feed/{token:guid}.xml")]
    public async Task<IActionResult> Personal(Guid token)
    {
        var user = await _userService.GetUserByTokenAsync(token);
        if (user is null) return NotFound();

        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        var subscribedSourceIds = (await _subscriptionService.GetSubscribedSourceIdsAsync(user.Id)).ToHashSet();

        var allEntries = await GetFeedEntriesAsync(baseUrl);
        var myEntries = allEntries.Where(e => subscribedSourceIds.Contains(e.SourceId));

        return BuildRssResult(myEntries, $"APW - Feed personal de {user.Username}",
            "Items de las fuentes a las que este usuario esta suscrito", baseUrl);
    }

    // Arma el XML final de RSS 2.0 a partir de una lista de entradas ya normalizadas
    private IActionResult BuildRssResult(IEnumerable<FeedEntryViewModel> entries, string title, string description, string baseUrl)
    {
        var channelItems = entries.Select(entry => BuildItemElement(entry, baseUrl));

        var rss = new XDocument(
            new XDeclaration("1.0", "utf-8", null),
            new XElement("rss",
                new XAttribute("version", "2.0"),
                new XElement("channel",
                    new XElement("title", title),
                    new XElement("link", baseUrl),
                    new XElement("description", description),
                    new XElement("language", "es"),
                    new XElement("lastBuildDate", DateTime.UtcNow.ToString("r")),
                    channelItems
                )
            )
        );

        var xml = rss.Declaration + Environment.NewLine + rss.ToString();
        return Content(xml, "application/rss+xml", Encoding.UTF8);
    }

    // Trae los SourceItem guardados
    private async Task<List<FeedEntryViewModel>> GetFeedEntriesAsync(string baseUrl)
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

    private static XElement BuildItemElement(FeedEntryViewModel entry, string baseUrl)
    {
        var element = new XElement("item",
            new XElement("title", entry.Title),
            new XElement("description", entry.Description),
            new XElement("link", entry.Link),
            new XElement("guid", new XAttribute("isPermaLink", "false"), $"apw-item-{entry.Id}"),
            new XElement("pubDate", entry.CreatedAt.ToUniversalTime().ToString("r")),
            new XElement("source",
                new XAttribute("url", $"{baseUrl}/Explore"),
                entry.SourceName)
        );

        if (!string.IsNullOrWhiteSpace(entry.ImageUrl))
        {
            element.Add(new XElement("enclosure",
                new XAttribute("url", entry.ImageUrl),
                new XAttribute("type", "image/jpeg")));
        }

        return element;
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

    private int GetUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(claim, out var id) ? id : 0;
    }
}