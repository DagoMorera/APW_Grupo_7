using APW.Architecture.Providers;
using APW.Mvc.Models;
using APW.Architecture.Providers;
namespace APW.Mvc.Service;

public interface ISourceService
{
    Task<IEnumerable<SourceViewModel>> GetSourcesAsync();
    Task<SourceViewModel> GetSourceByIdAsync(int id);
    Task CreateSourceAsync(SourceViewModel source);
    Task UpdateSourceAsync(int id, SourceViewModel source);
    Task DeleteSourceAsync(int id);
}

// Consume el endpoint SourceApi para las operaciones de Source
public class SourceService : ISourceService
{
    private readonly IRestProvider _restProvider;
    private readonly string _endpoint;

    public SourceService(IRestProvider restProvider, IConfiguration configuration)
    {
        _restProvider = restProvider;
        _endpoint = configuration.GetValue<string>("ApiEndpoints:SourceApi")
            ?? throw new InvalidOperationException("ApiEndpoints:SourceApi is not configured.");
    }

    public async Task<IEnumerable<SourceViewModel>> GetSourcesAsync()
    {
        var content = await _restProvider.GetAsync(_endpoint, null);
        return JsonProvider.DeserializeSimple<IEnumerable<SourceViewModel>>(content);
    }

    public async Task<SourceViewModel> GetSourceByIdAsync(int id)
    {
        var content = await _restProvider.GetAsync(_endpoint, id.ToString());
        return JsonProvider.DeserializeSimple<SourceViewModel>(content);
    }

    public async Task CreateSourceAsync(SourceViewModel source)
    {
        var json = JsonProvider.Serialize(source);
        await _restProvider.PostAsync(_endpoint, json);
    }

    public async Task UpdateSourceAsync(int id, SourceViewModel source)
    {
        var json = JsonProvider.Serialize(source);
        await _restProvider.PutAsync(_endpoint, id.ToString(), json);
    }

    public async Task DeleteSourceAsync(int id)
    {
        await _restProvider.DeleteAsync(_endpoint, id.ToString());
    }
}