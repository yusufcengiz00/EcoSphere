using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EcoSphere.Core.Models;
using EcoSphere.Core.Repositories;

namespace EcoSphere.Data.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(EcoSphereDbContext context) : base(context)
    {
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());
    }
}
