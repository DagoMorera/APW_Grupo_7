using APW.Architecture.Providers;
using APW.Mvc.Models;

namespace APW.Mvc.Service;

public interface IAuthService
{
    Task<LoginResultViewModel?> LoginAsync(string username, string password);
}

// Consume el endpoint AuthApi para validar credenciales
public class AuthService : IAuthService
{
    private readonly IRestProvider _restProvider;
    private readonly string _endpoint;

    public AuthService(IRestProvider restProvider, IConfiguration configuration)
    {
        _restProvider = restProvider;
        _endpoint = configuration.GetValue<string>("ApiEndpoints:AuthApi")
            ?? throw new InvalidOperationException("ApiEndpoints:AuthApi is not configured.");
    }

    public async Task<LoginResultViewModel?> LoginAsync(string username, string password)
    {
        var body = new { Username = username, Password = password };
        var json = JsonProvider.Serialize(body);

        try
        {
            var content = await _restProvider.PostAsync(_endpoint, json);
            return JsonProvider.DeserializeSimple<LoginResultViewModel>(content);
        }
        catch
        {
            // Credenciales invalidas (401) u otro error de la Api
            return null;
        }
    }
}