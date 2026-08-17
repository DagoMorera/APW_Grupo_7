using APW.Architecture.Providers;

namespace APW.Mvc.Service;

public interface ISubscriptionService
{
    Task<IEnumerable<int>> GetSubscribedSourceIdsAsync(int userId);
    Task<bool> ToggleSubscriptionAsync(int userId, int sourceId);
}

// Consume el endpoint SubscriptionApi para suscribirse/desuscribirse
public class SubscriptionService : ISubscriptionService
{
    private readonly IRestProvider _restProvider;
    private readonly string _endpoint;

    public SubscriptionService(IRestProvider restProvider, IConfiguration configuration)
    {
        _restProvider = restProvider;
        _endpoint = configuration.GetValue<string>("ApiEndpoints:SubscriptionApi")
            ?? throw new InvalidOperationException("ApiEndpoints:SubscriptionApi is not configured.");
    }

    // Ids de las Sources a las que el usuario esta suscrito
    public async Task<IEnumerable<int>> GetSubscribedSourceIdsAsync(int userId)
    {
        var content = await _restProvider.GetAsync(_endpoint, $"mine/{userId}");
        return JsonProvider.DeserializeSimple<IEnumerable<int>>(content) ?? Enumerable.Empty<int>();
    }

    // Activa/desactiva la suscripcion
    public async Task<bool> ToggleSubscriptionAsync(int userId, int sourceId)
    {
        var body = new { UserId = userId, SourceId = sourceId };
        var json = JsonProvider.Serialize(body);
        var content = await _restProvider.PostAsync(_endpoint, json);

        var result = JsonProvider.DeserializeSimple<ToggleResult>(content);
        return result?.Subscribed ?? false;
    }

    private class ToggleResult
    {
        public bool Subscribed { get; set; }
    }
}