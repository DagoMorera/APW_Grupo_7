namespace APW.Architecture.Parsers;

// Factory Pattern: entrega el parser correcto segun el tipo de contenido de la Source
public static class ContentParserFactory
{
    public static IContentParser Create(string componentType)
    {
        return componentType.ToLowerInvariant() switch
        {
            "json" => new JsonContentParser(),
            "xml" => new XmlContentParser(),
            "html" => new HtmlContentParser(),
            _ => throw new APWException($"Tipo de fuente no soportado: {componentType}")
        };
    }
}