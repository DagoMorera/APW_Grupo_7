using APW.Data.MSSQLEF;
using APW.Models;
using Microsoft.EntityFrameworkCore;

namespace APW.Repositories;

public interface IUserRepository : IRepositoryBase<User>
{
    // Busca al usuario dueno de un FeedToken
    Task<User?> FindByTokenAsync(Guid token);
}

// Repositorio de Usuarios, hereda las operaciones basicas
public class UserRepository : RepositoryBase<User>, IUserRepository
{
    private readonly ApwDbContext _context;

    public UserRepository(ApwDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<User?> FindByTokenAsync(Guid token)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.FeedToken == token);
    }
}