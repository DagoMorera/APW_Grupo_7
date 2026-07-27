using System.Xml.Linq;

namespace APW.Architecture.Parsers;

// Parsea contenido XML, buscando los nodos repetidos (items) y mapeando campos comunes
public class XmlContentParser : IContentParser
{
    private static readonly string[] TitleFields = { "title", "name", "headline" };
    private static readonly string[] DescriptionFields = { "description", "summary", "content", "body" };
    private static readonly string[] LinkFields = { "link", "url", "href" };
    private static readonly string[] ImageFields = { "image", "imageUrl", "thumbnail" };

    public IEnumerable<ParsedSourceItem> Parse(string rawContent)
    {
        var document = XDocument.Parse(rawContent);
        var itemNodes = FindItemNodes(document);

        var items = new List<ParsedSourceItem>();
        foreach (var node in itemNodes)
        {
            items.Add(new ParsedSourceItem
            {
                Title = FindField(node, TitleFields),
                Description = FindField(node, DescriptionFields),
                Link = FindField(node, LinkFields),
                ImageUrl = FindField(node, ImageFields),
                RawJson = ConvertNodeToJson(node)
            });
        }

        return items;
    }

    // Busca los nodos que se repiten (ej. <item>, <entry>), asumiendo que son los items de contenido
    private static IEnumerable<XElement> FindItemNodes(XDocument document)
    {
        var root = document.Root;
        if (root is null) return Enumerable.Empty<XElement>();

        // Agrupa los elementos hijos (a cualquier profundidad) por nombre y toma el grupo mas repetido
        var grouped = root.Descendants()
            .GroupBy(e => e.Name.LocalName)
            .Where(g => g.Count() > 1)
            .OrderByDescending(g => g.Count())
            .FirstOrDefault();

        return grouped ?? Enumerable.Empty<XElement>();
    }

    private static string? FindField(XElement node, string[] candidates)
    {
        foreach (var child in node.Elements())
        {
            if (candidates.Any(c => string.Equals(c, child.Name.LocalName, StringComparison.OrdinalIgnoreCase)))
                return child.Value;
        }

        // Tambien revisa atributos, algunos feeds usan atributos en vez de nodos hijos
        foreach (var attribute in node.Attributes())
        {
            if (candidates.Any(c => string.Equals(c, attribute.Name.LocalName, StringComparison.OrdinalIgnoreCase)))
                return attribute.Value;
        }

        return null;
    }

    // Convierte el nodo XML a un JSON simple, para guardarlo en RawJson
    private static string ConvertNodeToJson(XElement node)
    {
        var dict = node.Elements()
            .GroupBy(e => e.Name.LocalName)
            .ToDictionary(g => g.Key, g => g.First().Value as object);

        return System.Text.Json.JsonSerializer.Serialize(dict);
    }
}