using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    private readonly StoreContext _context;
    public UserRepository(StoreContext context) : base(context)
    {
        _context = context;
    }

    public async Task<User> GetByRefreshTokenAsync(string refreshToken)
    {
        return await _context.Users
                            .Include(u => u.Roles)
                            .Include(u => u.RefreshTokens)
                            .FirstOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token.Equals(refreshToken)));
    }

    public async Task<User> GetByUsernameAsync(string username)
    {
        return await _context.Users
                            .Include(u => u.Roles)
                            .Include(u => u.RefreshTokens)
                            .FirstOrDefaultAsync(u => u.UserName.ToLower().Equals(username.ToLower()));
    }
}
