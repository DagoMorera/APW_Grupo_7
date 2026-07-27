using APW.Data.MSSQLEF;
using APW.Models;

namespace APW.Repositories;

public interface IUserRepository : IRepositoryBase<User>
{
}

// Repositorio de Usuarios, hereda las operaciones basicas
public class UserRepository : RepositoryBase<User>, IUserRepository
{
    public UserRepository(ApwDbContext context) : base(context)
    {
    }
}