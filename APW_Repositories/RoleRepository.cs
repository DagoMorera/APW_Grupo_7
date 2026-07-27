using APW.Data.MSSQLEF;
using APW.Models;
using System.Data;

namespace APW.Repositories;

public interface IRoleRepository : IRepositoryBase<Role>
{
}

// Repositorio de Roles, hereda las operaciones basicas
public class RoleRepository : RepositoryBase<Role>, IRoleRepository
{
    public RoleRepository(ApwDbContext context) : base(context)
    {
    }
}