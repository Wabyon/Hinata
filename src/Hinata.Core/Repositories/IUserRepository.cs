using System.Threading.Tasks;
using Hinata.Models;

namespace Hinata.Repositories
{
    public interface IUserRepository
    {
        Task<User> FindByIdAsync(string id);

        Task<User> FindByNameAsync(string name);
    }
}