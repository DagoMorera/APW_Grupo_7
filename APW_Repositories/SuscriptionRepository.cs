using APW.Data.MSSQLEF;
using APW.Models;

namespace APW.Repositories;

public interface ISubscriptionRepository : IRepositoryBase<Subscription>
{
}

// Repositorio de Subscriptions
public class SubscriptionRepository : RepositoryBase<Subscription>, ISubscriptionRepository
{
    public SubscriptionRepository(ApwDbContext context) : base(context)
    {
    }
}