using HtmlAgilityPack;

namespace APW.Architecture.Parsers;

// Parsea contenido HTML, extrayendo elementos tipo articulo/tarjeta con titulo y enlace
public class HtmlContentParser : IContentParser
{
    // Selectores comunes donde suelen vivir las "noticias" o items en una pagina
    private static readonly string[] ItemSelectors =
    {
        "//article",
        "//*[contains(@class, 'item')]",
        "//*[contains(@class, 'card')]",
        "//li"
    };

    public IEnumerable<ParsedSourceItem> Parse(string rawContent)
    {
        var document = new HtmlDocument();
        document.LoadHtml(rawContent);

        var nodes = FindItemNodes(document);

        var items = new List<ParsedSourceItem>();
        foreach (var node in nodes)
        {
            var titleNode = node.SelectSingleNode(".//h1|.//h2|.//h3|.//a");
            var linkNode = node.SelectSingleNode(".//a[@href]");
            var imageNode = node.SelectSingleNode(".//img[@src]");

            var title = titleNode?.InnerText.Trim();
            if (string.IsNullOrWhiteSpace(title)) continue; // sin titulo, no sirve como item

            items.Add(new ParsedSourceItem
            {
                Title = title,
                Description = node.InnerText.Trim(),
                Link = linkNode?.GetAttributeValue("href", null),
                ImageUrl = imageNode?.GetAttributeValue("src", null),
                RawJson = System.Text.Json.JsonSerializer.Serialize(new
                {
                    title,
                    link = linkNode?.GetAttributeValue("href", null),
                    image = imageNode?.GetAttributeValue("src", null)
                })
            });
        }

        return items;
    }

    // Prueba los selectores en orden, usa el primero que encuentre resultados
    private static IEnumerable<HtmlNode> FindItemNodes(HtmlDocument document)
    {
        foreach (var selector in ItemSelectors)
        {
            var nodes = document.DocumentNode.SelectNodes(selector);
            if (nodes is not null && nodes.Count > 0)
                return nodes;
        }

        return Enumerable.Empty<HtmlNode>();
    }
}