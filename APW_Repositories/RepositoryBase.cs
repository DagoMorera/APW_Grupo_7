using Microsoft.EntityFrameworkCore;
using APW.Data.MSSQLEF;

namespace APW.Repositories;

// Operaciones basicas de acceso a datos, comunes a todas las entidades
public interface IRepositoryBase<T>
{
    Task<bool> UpsertAsync(T entity, bool isUpdating);
    Task<bool> CreateAsync(T entity);
    Task<bool> UpdateAsync(T entity);
    Task<bool> DeleteAsync(T entity);
    Task<IEnumerable<T>> ReadAsync();
    Task<T> FindAsync(int id);
    Task<bool> ExistsAsync(T entity);
}

// Implementacion generica del Repository Pattern usando EF Core
public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : class
{
    private readonly ApwDbContext _context;
    protected DbSet<T> DbSet;

    public RepositoryBase(ApwDbContext context)
    {
        _context = context;
        DbSet = _context.Set<T>();
    }

    // Actualiza si existe, si no crea el registro
    public async Task<bool> UpsertAsync(T entity, bool isUpdating)
    {
        return isUpdating ? await UpdateAsync(entity) : await CreateAsync(entity);
    }

    public async Task<bool> CreateAsync(T entity)
    {
        await _context.AddAsync(entity);
        return await SaveAsync();
    }

    public async Task<bool> UpdateAsync(T entity)
    {
        _context.Update(entity);
        return await SaveAsync();
    }

    public async Task<bool> DeleteAsync(T entity)
    {
        _context.Remove(entity);
        return await SaveAsync();
    }

    public async Task<IEnumerable<T>> ReadAsync()
    {
        return await DbSet.ToListAsync();
    }

    public async Task<T> FindAsync(int id)
    {
        return await DbSet.FindAsync(id);
    }

    public async Task<bool> ExistsAsync(T entity)
    {
        var items = await ReadAsync();
        return items.Any(x => x.Equals(entity));
    }

    // Guarda los cambios pendientes en el contexto
    protected async Task<bool> SaveAsync()
    {
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }
}