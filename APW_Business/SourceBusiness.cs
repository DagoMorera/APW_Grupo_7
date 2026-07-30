using APW.Architecture.Parsers;
using APW.Architecture.Providers;
using APW.Models;
using APW.Repositories;

namespace APW.Business;

public interface ISourceBusiness
{
    Task<IEnumerable<Source>> ReadSourcesAsync();
    Task<Source> FindSourceAsync(int id);
    Task<bool> CreateSourceAsync(Source source);
    Task<bool> UpdateSourceAsync(Source source);
    Task<bool> DeleteSourceAsync(Source source);
    Task<IEnumerable<ParsedSourceItem>> GetParsedItemsAsync(int sourceId);
}

// Logica de negocio de Sources
public class SourceBusiness(ISourceRepository sourceRepository, ISettingRepository settingRepository, IRestProvider restProvider) : ISourceBusiness
{
    private readonly ISourceRepository _sourceRepository = sourceRepository;
    private readonly ISettingRepository _settingRepository = settingRepository;
    private readonly IRestProvider _restProvider = restProvider;

    public async Task<IEnumerable<Source>> ReadSourcesAsync()
    {
        return await _sourceRepository.ReadAsync();
    }

    public async Task<Source> FindSourceAsync(int id)
    {
        return await _sourceRepository.FindAsync(id);
    }

    public async Task<bool> CreateSourceAsync(Source source)
    {
        return await _sourceRepository.CreateAsync(source);
    }

    public async Task<bool> UpdateSourceAsync(Source source)
    {
        return await _sourceRepository.UpdateAsync(source);
    }

    public async Task<bool> DeleteSourceAsync(Source source)
    {
        return await _sourceRepository.DeleteAsync(source);
    }

    // Trae el contenido crudo de la Source y lo parsea segun su tipo (JSON/XML/HTML)
    public async Task<IEnumerable<ParsedSourceItem>> GetParsedItemsAsync(int sourceId)
    {
        var source = await _sourceRepository.FindAsync(sourceId);
        if (source is null) return Enumerable.Empty<ParsedSourceItem>();

        string? apiKey = null;
        if (source.RequiresSecret)
        {
            var settings = await _settingRepository.ReadAsync();
            apiKey = settings.FirstOrDefault(s => s.SourceId == sourceId && s.KeyName == "ApiKey")?.KeyValue;
        }

        var rawContent = await _restProvider.GetAsync(source.Url, null, "X-Api-Key", apiKey);
        var parser = ContentParserFactory.Create(source.ComponentType);

        return parser.Parse(rawContent);
    }
}