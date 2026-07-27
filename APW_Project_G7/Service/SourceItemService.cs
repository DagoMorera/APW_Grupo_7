using APW.Architecture.Providers;
using APW.Mvc.Models;
using APW.Architecture.Providers;
namespace APW.Mvc.Service;

public interface ISourceItemService
{
    Task<IEnumerable<SourceItemViewModel>> GetSourceItemsAsync();
    Task<SourceItemViewModel> GetSourceItemByIdAsync(int id);
    Task CreateSourceItemAsync(SourceItemViewModel sourceItem);
    Task UpdateSourceItemAsync(int id, SourceItemViewModel sourceItem);
    Task DeleteSourceItemAsync(int id);
}

// Consume el endpoint SourceItemApi para las operaciones de SourceItem
public class SourceItemService : ISourceItemService
{
    private readonly IRestProvider _restProvider;
    private readonly string _endpoint;

    public SourceItemService(IRestProvider restProvider, IConfiguration configuration)
    {
        _restProvider = restProvider;
        _endpoint = configuration.GetValue<string>("ApiEndpoints:SourceItemApi")
            ?? throw new InvalidOperationException("ApiEndpoints:SourceItemApi is not configured.");
    }

    public async Task<IEnumerable<SourceItemViewModel>> GetSourceItemsAsync()
    {
        var content = await _restProvider.GetAsync(_endpoint, null);
        return JsonProvider.DeserializeSimple<IEnumerable<SourceItemViewModel>>(content);
    }

    public async Task<SourceItemViewModel> GetSourceItemByIdAsync(int id)
    {
        var content = await _restProvider.GetAsync(_endpoint, id.ToString());
        return JsonProvider.DeserializeSimple<SourceItemViewModel>(content);
    }

    public async Task CreateSourceItemAsync(SourceItemViewModel sourceItem)
    {
        var json = JsonProvider.Serialize(sourceItem);
        await _restProvider.PostAsync(_endpoint, json);
    }

    public async Task UpdateSourceItemAsync(int id, SourceItemViewModel sourceItem)
    {
        var json = JsonProvider.Serialize(sourceItem);
        await _restProvider.PutAsync(_endpoint, id.ToString(), json);
    }

    public async Task DeleteSourceItemAsync(int id)
    {
        await _restProvider.DeleteAsync(_endpoint, id.ToString());
    }
}