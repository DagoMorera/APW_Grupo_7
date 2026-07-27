using APW.Architecture.Providers;
using APW.Mvc.Models;
using APW.Architecture.Providers;
namespace APW.Mvc.Service;

public interface IUserService
{
    Task<IEnumerable<UserViewModel>> GetUsersAsync();
    Task<UserViewModel> GetUserByIdAsync(int id);
    Task CreateUserAsync(UserViewModel user);
    Task UpdateUserAsync(int id, UserViewModel user);
    Task DeleteUserAsync(int id);
}

// Consume el endpoint UserApi para las operaciones de User
public class UserService : IUserService
{
    private readonly IRestProvider _restProvider;
    private readonly string _endpoint;

    public UserService(IRestProvider restProvider, IConfiguration configuration)
    {
        _restProvider = restProvider;
        _endpoint = configuration.GetValue<string>("ApiEndpoints:UserApi")
            ?? throw new InvalidOperationException("ApiEndpoints:UserApi is not configured.");
    }

    public async Task<IEnumerable<UserViewModel>> GetUsersAsync()
    {
        var content = await _restProvider.GetAsync(_endpoint, null);
        return JsonProvider.DeserializeSimple<IEnumerable<UserViewModel>>(content);
    }

    public async Task<UserViewModel> GetUserByIdAsync(int id)
    {
        var content = await _restProvider.GetAsync(_endpoint, id.ToString());
        return JsonProvider.DeserializeSimple<UserViewModel>(content);
    }

    public async Task CreateUserAsync(UserViewModel user)
    {
        var json = JsonProvider.Serialize(user);
        await _restProvider.PostAsync(_endpoint, json);
    }

    public async Task UpdateUserAsync(int id, UserViewModel user)
    {
        var json = JsonProvider.Serialize(user);
        await _restProvider.PutAsync(_endpoint, id.ToString(), json);
    }

    public async Task DeleteUserAsync(int id)
    {
        await _restProvider.DeleteAsync(_endpoint, id.ToString());
    }
}