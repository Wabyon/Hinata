using System.Collections.Generic;
using System.Threading.Tasks;
using Hinata.Models;

namespace Hinata.Repositories
{
    public interface IDraftRepository
    {
        Task<Draft> FindByIdAsync(string id);

        Task<IEnumerable<Draft>> GetByUserIdAsync(string userId);

        Task SaveAsync(Draft draft);

        Task DeleteAsync(Draft draft);
    }
}