using APW.Models;
using APW.Repositories;

namespace APW.Business;

public interface ISubscriptionBusiness
{
    Task<IEnumerable<int>> GetSubscribedSourceIdsAsync(int userId);
    Task<bool> ToggleSubscriptionAsync(int userId, int sourceId);
}

// Logica de Subscriptions
public class SubscriptionBusiness(ISubscriptionRepository subscriptionRepository) : ISubscriptionBusiness
{
    private readonly ISubscriptionRepository _subscriptionRepository = subscriptionRepository;

    // Ids de las Sources a las que esta suscrito un usuario
    public async Task<IEnumerable<int>> GetSubscribedSourceIdsAsync(int userId)
    {
        var all = await _subscriptionRepository.ReadAsync();
        return all.Where(s => s.UserId == userId).Select(s => s.SourceId);
    }

    // Si ya estaba suscrito, se desuscribe
    public async Task<bool> ToggleSubscriptionAsync(int userId, int sourceId)
    {
        var all = await _subscriptionRepository.ReadAsync();
        var existing = all.FirstOrDefault(s => s.UserId == userId && s.SourceId == sourceId);

        if (existing is not null)
        {
            await _subscriptionRepository.DeleteAsync(existing);
            return false;
        }

        var subscription = new Subscription
        {
            UserId = userId,
            SourceId = sourceId,
            CreatedAt = DateTime.UtcNow
        };

        await _subscriptionRepository.CreateAsync(subscription);
        return true;
    }
}