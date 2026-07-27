namespace APW.Architecture.Parsers;

// Contrato del Strategy Pattern: cada formato de contenido implementa su propia forma de parsear
public interface IContentParser
{
    IEnumerable<ParsedSourceItem> Parse(string rawContent);
}