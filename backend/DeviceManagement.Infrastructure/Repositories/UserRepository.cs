using DeviceManagement.Application.Interfaces.Repositories;
using DeviceManagement.Domain.Entities;
using DeviceManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DeviceManagement.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<User>> GetAllAsync()
    {
        return await _context.Users.AsNoTracking()
            .OrderBy(u => u.FullName)
            .ToListAsync();
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User?> GetByEmailAsync(string normalizedEmail)
    {
        return await _context.Users.AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == normalizedEmail);
    }

    public async Task<bool> ExistsByEmailAsync(string normalizedEmail, int? exceptUserId = null)
    {
        var query = _context.Users.AsNoTracking().Where(u => u.Email == normalizedEmail);
        if (exceptUserId.HasValue)
            query = query.Where(u => u.Id != exceptUserId.Value);

        return await query.AnyAsync();
    }

    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
    }

    public Task UpdateAsync(User user)
    {
        _context.Users.Update(user);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(User user)
    {
        _context.Users.Remove(user);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
