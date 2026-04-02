using DeviceManagement.Domain.Entities;

namespace DeviceManagement.Application.Interfaces.Repositories;

public interface IUserRepository
{
    Task<List<User>> GetAllAsync();
    Task<User?> GetByIdAsync(int id);
    Task<User?> GetByEmailAsync(string normalizedEmail);
    Task<bool> ExistsByEmailAsync(string normalizedEmail, int? exceptUserId = null);
    Task AddAsync(User user);
    Task UpdateAsync(User user);
    Task DeleteAsync(User user);
    Task SaveChangesAsync();
}
