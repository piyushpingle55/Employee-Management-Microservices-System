using Auth.Domain.Entities;
using Auth.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Auth.Infrastructure.Repositories;

public class UserRepository
{
    private readonly AuthDbContext _context;

    public UserRepository(AuthDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByEmailAsync(string email) =>
        await _context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Email == email);

    public async Task AddAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
    }
}
