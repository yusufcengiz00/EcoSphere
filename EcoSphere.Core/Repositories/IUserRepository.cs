using System.Threading.Tasks;
using EcoSphere.Core.Models;

namespace EcoSphere.Core.Repositories;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByUsernameAsync(string username);
}
