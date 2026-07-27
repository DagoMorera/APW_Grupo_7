using APW.Data.MSSQLEF;
using APW.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace APW.Repositories;

public interface ISettingRepository : IRepositoryBase<Setting>
{
}

// Repositorio de Settings, hereda las operaciones basicas
public class SettingRepository : RepositoryBase<Setting>, ISettingRepository
{
    public SettingRepository(ApwDbContext context) : base(context)
    {
    }
}