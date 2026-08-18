using System.Security.Claims;
using System.Text;
using System.Xml.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using APW.Mvc.Models;
using APW.Mvc.Service;

namespace APW.Mvc.Controllers;

// Publica el RSS 2.0 global, el feed personal por usuario y gestiona /Feed/Mine.
public class FeedController : Controller
{
    private readonly ISourceService _sourceService;
    private readonly ISubscriptionService _subscriptionService;
    private readonly IUserService _userService;
    private readonly IFeedEntryProvider _feedEntryProvider;

    public FeedController(
        ISourceService sourceService,
        ISubscriptionService subscriptionService,
        IUserService userService,
        IFeedEntryProvider feedEntryProvider)
    {
        _sourceService = sourceService;
        _subscriptionService = subscriptionService;
        _userService = userService;
        _feedEntryProvider = feedEntryProvider;
    }

    // GET /feed.xml
    [Route("feed.xml")]
    [Route("Feed/Rss")]
    public async Task<IActionResult> Rss()
    {
        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        var entries = await _feedEntryProvider.GetEntriesAsync(baseUrl);
        return BuildRssResult(entries, "APW - Feed de contenido guardado",
            "Items curados y guardados desde las fuentes configuradas en APW", baseUrl);
    }

    // GET /Feed
    [Route("Feed")]
    [Route("Feed/Preview")]
    public IActionResult Preview()
    {
        return RedirectToAction("Index", "Home");
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

        var allEntries = await _feedEntryProvider.GetEntriesAsync(baseUrl);
        var myEntries = allEntries.Where(e => subscribedSourceIds.Contains(e.SourceId)).ToList();

        ViewBag.PersonalFeedUrl = $"{baseUrl}/feed/{user.FeedToken}.xml";
        ViewBag.HasSubscriptions = subscribedSourceIds.Count > 0;

        return View(myEntries);
    }

    // GET /feed/{token}.xml
    [Route("feed/{token:guid}.xml")]
    public async Task<IActionResult> Personal(Guid token)
    {
        var user = await _userService.GetUserByTokenAsync(token);
        if (user is null) return NotFound();

        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        var subscribedSourceIds = (await _subscriptionService.GetSubscribedSourceIdsAsync(user.Id)).ToHashSet();

        var allEntries = await _feedEntryProvider.GetEntriesAsync(baseUrl);
        var myEntries = allEntries.Where(e => subscribedSourceIds.Contains(e.SourceId));

        return BuildRssResult(myEntries, $"APW - Feed personal de {user.Username}",
            "Items de las fuentes a las que este usuario esta suscrito", baseUrl);
    }

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

    private int GetUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(claim, out var id) ? id : 0;
    }
}