using APW.Data.MSSQLEF;
using APW.Models;

namespace APW.Repositories;

public interface ISourceItemRepository : IRepositoryBase<SourceItem>
{
}

// Repositorio de SourceItems, hereda las operaciones basicas
public class SourceItemRepository : RepositoryBase<SourceItem>, ISourceItemRepository
{
    public SourceItemRepository(ApwDbContext context) : base(context)
    {
    }
}