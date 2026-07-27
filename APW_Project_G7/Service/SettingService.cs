using APW.Architecture.Providers;
using APW.Mvc.Models;
using APW.Architecture.Providers;
namespace APW.Mvc.Service;

public interface ISettingService
{
    Task<IEnumerable<SettingViewModel>> GetSettingsAsync();
    Task<SettingViewModel> GetSettingByIdAsync(int id);
    Task CreateSettingAsync(SettingViewModel setting);
    Task UpdateSettingAsync(int id, SettingViewModel setting);
    Task DeleteSettingAsync(int id);
}

// Consume el endpoint SettingApi para las operaciones de Setting
public class SettingService : ISettingService
{
    private readonly IRestProvider _restProvider;
    private readonly string _endpoint;

    public SettingService(IRestProvider restProvider, IConfiguration configuration)
    {
        _restProvider = restProvider;
        _endpoint = configuration.GetValue<string>("ApiEndpoints:SettingApi")
            ?? throw new InvalidOperationException("ApiEndpoints:SettingApi is not configured.");
    }

    public async Task<IEnumerable<SettingViewModel>> GetSettingsAsync()
    {
        var content = await _restProvider.GetAsync(_endpoint, null);
        return JsonProvider.DeserializeSimple<IEnumerable<SettingViewModel>>(content);
    }

    public async Task<SettingViewModel> GetSettingByIdAsync(int id)
    {
        var content = await _restProvider.GetAsync(_endpoint, id.ToString());
        return JsonProvider.DeserializeSimple<SettingViewModel>(content);
    }

    public async Task CreateSettingAsync(SettingViewModel setting)
    {
        var json = JsonProvider.Serialize(setting);
        await _restProvider.PostAsync(_endpoint, json);
    }

    public async Task UpdateSettingAsync(int id, SettingViewModel setting)
    {
        var json = JsonProvider.Serialize(setting);
        await _restProvider.PutAsync(_endpoint, id.ToString(), json);
    }

    public async Task DeleteSettingAsync(int id)
    {
        await _restProvider.DeleteAsync(_endpoint, id.ToString());
    }
}