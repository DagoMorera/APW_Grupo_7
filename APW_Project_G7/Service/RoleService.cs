using APW.Architecture.Providers;
using APW.Mvc.Models;


namespace APW.Mvc.Service;

public interface IRoleService
{
    Task<IEnumerable<RoleViewModel>> GetRolesAsync();
    Task<RoleViewModel> GetRoleByIdAsync(int id);
    Task CreateRoleAsync(RoleViewModel role);
    Task UpdateRoleAsync(int id, RoleViewModel role);
    Task DeleteRoleAsync(int id);
}

// Consume el endpoint RoleApi para las operaciones de Role
public class RoleService : IRoleService
{
    private readonly IRestProvider _restProvider;
    private readonly string _endpoint;

    public RoleService(IRestProvider restProvider, IConfiguration configuration)
    {
        _restProvider = restProvider;
        _endpoint = configuration.GetValue<string>("ApiEndpoints:RoleApi")
            ?? throw new InvalidOperationException("ApiEndpoints:RoleApi is not configured.");
    }

    public async Task<IEnumerable<RoleViewModel>> GetRolesAsync()
    {
        var content = await _restProvider.GetAsync(_endpoint, null);
        return JsonProvider.DeserializeSimple<IEnumerable<RoleViewModel>>(content);
    }

    public async Task<RoleViewModel> GetRoleByIdAsync(int id)
    {
        var content = await _restProvider.GetAsync(_endpoint, id.ToString());
        return JsonProvider.DeserializeSimple<RoleViewModel>(content);
    }

    public async Task CreateRoleAsync(RoleViewModel role)
    {
        var json = JsonProvider.Serialize(role);
        await _restProvider.PostAsync(_endpoint, json);
    }

    public async Task UpdateRoleAsync(int id, RoleViewModel role)
    {
        var json = JsonProvider.Serialize(role);
        await _restProvider.PutAsync(_endpoint, id.ToString(), json);
    }

    public async Task DeleteRoleAsync(int id)
    {
        await _restProvider.DeleteAsync(_endpoint, id.ToString());
    }
}