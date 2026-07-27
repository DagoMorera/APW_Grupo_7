using APW.Models;
using APW.Repositories;
using System.Data;

namespace APW.Business;

public interface IRoleBusiness
{
    Task<IEnumerable<Role>> ReadRolesAsync();
    Task<Role> FindRoleAsync(int id);
    Task<bool> CreateRoleAsync(Role role);
    Task<bool> UpdateRoleAsync(Role role);
    Task<bool> DeleteRoleAsync(Role role);
}

// Logica de negocio de Roles
public class RoleBusiness(IRoleRepository roleRepository) : IRoleBusiness
{
    private readonly IRoleRepository _roleRepository = roleRepository;

    public async Task<IEnumerable<Role>> ReadRolesAsync()
    {
        return await _roleRepository.ReadAsync();
    }

    public async Task<Role> FindRoleAsync(int id)
    {
        return await _roleRepository.FindAsync(id);
    }

    public async Task<bool> CreateRoleAsync(Role role)
    {
        return await _roleRepository.CreateAsync(role);
    }

    public async Task<bool> UpdateRoleAsync(Role role)
    {
        return await _roleRepository.UpdateAsync(role);
    }

    public async Task<bool> DeleteRoleAsync(Role role)
    {
        return await _roleRepository.DeleteAsync(role);
    }
}