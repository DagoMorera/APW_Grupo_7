using APW.Models;
using APW.Repositories;
using BCrypt.Net;

namespace APW.Business;

public interface IUserBusiness
{
    Task<IEnumerable<User>> ReadUsersAsync();
    Task<User> FindUserAsync(int id);
    Task<bool> CreateUserAsync(User user);
    Task<bool> UpdateUserAsync(User user);
    Task<bool> DeleteUserAsync(User user);
    Task<User?> ValidateCredentialsAsync(string username, string password);
    Task<User?> FindByTokenAsync(Guid token);
}

// Logica de negocio de Usuarios
public class UserBusiness(IUserRepository userRepository) : IUserBusiness
{
    private readonly IUserRepository _userRepository = userRepository;

    public async Task<IEnumerable<User>> ReadUsersAsync()
    {
        return await _userRepository.ReadAsync();
    }

    public async Task<User> FindUserAsync(int id)
    {
        return await _userRepository.FindAsync(id);
    }

    public async Task<bool> CreateUserAsync(User user)
    {
        // Se hashea la password antes de guardarla, nunca en texto plano
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);

        // Token unico para su feed personal
        user.FeedToken = Guid.NewGuid();

        return await _userRepository.CreateAsync(user);
    }

    public async Task<bool> UpdateUserAsync(User user)
    {
        return await _userRepository.UpdateAsync(user);
    }

    public async Task<bool> DeleteUserAsync(User user)
    {
        return await _userRepository.DeleteAsync(user);
    }

    // Valida username y password, devuelve el usuario si son correctos
    public async Task<User?> ValidateCredentialsAsync(string username, string password)
    {
        var users = await _userRepository.ReadAsync();
        var user = users.FirstOrDefault(u => u.Username == username && u.IsActive);

        if (user is null) return null;

        var isValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
        return isValid ? user : null;
    }

    public async Task<User?> FindByTokenAsync(Guid token)
    {
        return await _userRepository.FindByTokenAsync(token);
    }
}