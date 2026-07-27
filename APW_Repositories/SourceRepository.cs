using APW.Data.MSSQLEF;
using APW.Models;

namespace APW.Repositories;

public interface ISourceRepository : IRepositoryBase<Source>
{
}

// Repositorio de Sources, hereda las operaciones basicas
public class SourceRepository : RepositoryBase<Source>, ISourceRepository
{
    public SourceRepository(ApwDbContext context) : base(context)
    {
    }
}