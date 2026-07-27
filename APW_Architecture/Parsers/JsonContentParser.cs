using System.Text.Json;

namespace APW.Architecture.Parsers;

// Parsea contenido JSON, buscando el arreglo de items y mapeando campos comunes
public class JsonContentParser : IContentParser
{
    // Nombres de campo mas comunes que suelen representar cada dato
    private static readonly string[] TitleFields = { "title", "name", "headline" };
    private static readonly string[] DescriptionFields = { "description", "summary", "content", "body" };
    private static readonly string[] LinkFields = { "link", "url", "href" };
    private static readonly string[] ImageFields = { "image", "imageUrl", "thumbnail", "urlToImage" };

    public IEnumerable<ParsedSourceItem> Parse(string rawContent)
    {
        using var document = JsonDocument.Parse(rawContent);
        var array = FindItemsArray(document.RootElement);

        var items = new List<ParsedSourceItem>();
        foreach (var element in array)
        {
            items.Add(new ParsedSourceItem
            {
                Title = FindField(element, TitleFields),
                Description = FindField(element, DescriptionFields),
                Link = FindField(element, LinkFields),
                ImageUrl = FindField(element, ImageFields),
                RawJson = element.GetRawText()
            });
        }

        return items;
    }

    // Busca el arreglo de items: si la raiz ya es un arreglo, lo usa; si no, busca la primera propiedad que sea un arreglo
    private static JsonElement.ArrayEnumerator FindItemsArray(JsonElement root)
    {
        if (root.ValueKind == JsonValueKind.Array)
            return root.EnumerateArray();

        if (root.ValueKind == JsonValueKind.Object)
        {
            foreach (var property in root.EnumerateObject())
            {
                if (property.Value.ValueKind == JsonValueKind.Array)
                    return property.Value.EnumerateArray();
            }
        }

        // No se encontro un arreglo, se devuelve vacio
        return default;
    }

    // Busca el primer campo cuyo nombre coincida (sin importar mayusculas) con alguno de los candidatos
    private static string? FindField(JsonElement element, string[] candidates)
    {
        if (element.ValueKind != JsonValueKind.Object) return null;

        foreach (var property in element.EnumerateObject())
        {
            if (candidates.Any(c => string.Equals(c, property.Name, StringComparison.OrdinalIgnoreCase)))
            {
                return property.Value.ValueKind == JsonValueKind.String
                    ? property.Value.GetString()
                    : property.Value.GetRawText();
            }
        }

        return null;
    }
}